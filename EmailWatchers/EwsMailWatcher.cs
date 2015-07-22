using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Microsoft.Exchange.WebServices.Data;
using TrelloManagement.Models.Models;
using TrelloManagement.Models.Settings;

namespace EmailWatchers
{
    public class EwsMailWatcher
    {
        private readonly EwsSettings _settings;
        private readonly string _trelloForwardingEmailAddress;

        private ExchangeService _service;
        private StreamingSubscriptionConnection _connection;

        private bool _continueListening;

        private Action<MessageReceived[]> _callback;

        public EwsMailWatcher(EwsSettings settings, string trelloForwardingEmailAddress)
        {
            _settings = settings;
            _trelloForwardingEmailAddress = trelloForwardingEmailAddress;
        }

        static bool RedirectionCallback(string url)
        {
            return url.ToLower().StartsWith("https://");
        }

        public void BeginListening(Action<MessageReceived[]> callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException("callback", "callback to deal with messages is null");
            }

            //register callback to be able to inform when new messages arrive
            _callback = callback;

            _continueListening = true;

            Connect();
        }

        private void Connect()
        {
            if (_service == null)
            {
                Console.WriteLine("Creating EWS Service");
                _service = new ExchangeService();
                _service.Credentials = new NetworkCredential(_settings.UserEmailAddress, _settings.Password);

                // Look up the user's EWS endpoint by using Autodiscover.
                _service.AutodiscoverUrl(_settings.UserEmailAddress, RedirectionCallback);

                Console.WriteLine("EWS Endpoint: {0}", _service.Url);
            }

            Console.WriteLine("attempting to open connection to {0}", _settings.UserEmailAddress);

            // Subscribe to streaming notifications in the Inbox. 
            var streamingSubscription = _service.SubscribeToStreamingNotifications(
                new FolderId[] {WellKnownFolderName.Inbox},
                EventType.NewMail
                );


            // Create a streaming connection to the service object, over which events are returned to the client.
            // Keep the streaming connection open for 30 minutes.
            _connection = new StreamingSubscriptionConnection(_service, 30);
            _connection.AddSubscription(streamingSubscription);
            _connection.OnNotificationEvent += OnNotificationEvent;
            _connection.OnDisconnect += OnDisconnect;
            _connection.Open();

            Console.WriteLine("Connected to {0}", _settings.UserEmailAddress);
            Console.WriteLine("Listening for new emails");
        }

        public void EndListening()
        {
            Console.WriteLine("Stopping listening for new emails");
            _continueListening = false;

            //dispose all items.
            if (_connection != null && _connection.IsOpen)
            {
                _connection.Close();
            }

            if (_connection != null)
            {
                _connection.Dispose();
                _connection = null;
            }

            Console.WriteLine("Connection closed");
        }

        private void OnDisconnect(object sender, SubscriptionErrorEventArgs args)
        {
            if (!_continueListening)
            {
                return;
            }

            //TODO add failure attempts drop out.

            Console.WriteLine("attempting to reconnect");
            Connect();
        }

        private void OnNotificationEvent(object sender, NotificationEventArgs args)
        {
            var messages = new List<MessageReceived>();

            foreach (var argEvent in args.Events)
            {
                var mailReceivedEvent = argEvent as ItemEvent;
                if (mailReceivedEvent == null) continue;
                if (mailReceivedEvent.EventType != EventType.NewMail) continue;

                ServiceResponseCollection<GetItemResponse> items = _service.BindToItems(new[] { mailReceivedEvent.ItemId }, new PropertySet(BasePropertySet.FirstClassProperties, EmailMessageSchema.From, EmailMessageSchema.ToRecipients));
                foreach (GetItemResponse item in items)
                {
                    var message = new MessageReceived();

                    message.From = ((EmailAddress)item.Item[EmailMessageSchema.From]).Address;
                    message.To = ((EmailAddressCollection)item.Item[EmailMessageSchema.ToRecipients]).Select(recipient => recipient.Address).ToArray();
                    message.Subject = item.Item.Subject;
                    message.Message = item.Item.Body.ToString();
                    if (item.Item.HasAttachments)
                    {
                        var memStreams = new List<MemoryStream>();
                        foreach (var attachment in item.Item.Attachments.Where(a => a is FileAttachment))
                        {
                            var fileAttachment = attachment as FileAttachment;
                            var ms = new MemoryStream();
                            fileAttachment.Load(ms);

                            memStreams.Add(ms);
                        }

                        message.Attachments = memStreams.ToArray();
                    }

                    //cast it to mail mesage to forward it to trello board
                    var msg = (EmailMessage)item.Item;

                    string messageBodyPrefix = "This message was sent by: " + message.From;
                    ResponseMessage responseMessage = msg.CreateForward();
                    responseMessage.ToRecipients.Add(_trelloForwardingEmailAddress);
                    responseMessage.BodyPrefix = messageBodyPrefix;
                    responseMessage.SendAndSaveCopy();

                    messages.Add(message);
                }
            }

            _callback(messages.ToArray());
        }
    }
}
