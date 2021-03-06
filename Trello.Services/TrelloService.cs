﻿using System;
using System.Net;
using System.Text;
using System.Web;
using TrelloManagement.Models.Models;
using TrelloManagement.Models.Settings;

namespace Trello.Services
{
    public class TrelloService
    {
        //these are just for reference and should be removed once we know it all works.  only used during setup.
        private const string TrelloSecret = "0625145c074b0565985e5fd33973f67ba035a554d18d01c1b6df6ed8faa5175a";

        //this was generated by manually visiting the site... can specify how long it lasts for.
        //see https://trello.com/docs/gettingstarted/authorize.html for more information.
        private readonly string _trelloKey;
        private readonly string _trelloToken;
        private readonly string _thisAppsName;
        private readonly string _boardId;
        private readonly string _boardName;

        private readonly string _listId;
        private readonly string _listName;
        
        public TrelloService(TrelloSettings settings)
        {
            _trelloKey = settings.Key;
            _trelloToken = settings.ApiAppToken;
            _thisAppsName = settings.ApiAppName;
            _boardId = settings.BoardId;
            _boardName = settings.BoardName;

            _listId = settings.ListId;
            _listName = settings.ListName;

            Console.WriteLine("{0} App Has been initialized", _thisAppsName);
        }

        public void Post(MessageReceived message)
        {
            Console.WriteLine("");
            Console.WriteLine("writing to Trello board: " + _boardName);
            Console.WriteLine("");

            var builder = new StringBuilder();
            builder.Append("https://api.trello.com/1/lists/");
            builder.Append(_listId + "/cards?key=");
            builder.Append(_trelloKey + "&token=");
            builder.Append(_trelloToken);
            builder.Append(HttpUtility.UrlEncode(string.Format("&name={0}- {1} {2}", message.Subject, DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString())));
        }

        public void ShowBoards()
        {
            Console.WriteLine("");
            Console.WriteLine("Showing all board can connect to:");
            Console.WriteLine("");

            string url = "https://api.trello.com/1/members/me/boards?fields=name&key=" + _trelloKey + "&token=" + _trelloToken;

            WebRequest webRequest = WebRequest.Create(url);
            WebResponse webResp = webRequest.GetResponse();

            using (var stream = new System.IO.StreamReader(webResp.GetResponseStream()))
            {
                Console.WriteLine(stream.ReadToEnd());
            }
            Console.WriteLine("");
        }

        public void ShowLists()
        {
            Console.WriteLine("");
            Console.WriteLine("Showing all lsits in board: " + _boardName);
            Console.WriteLine("");

            var builder = new StringBuilder();
            builder.Append("https://api.trello.com/1/boards/");
            builder.Append(_boardId + "/lists?key=");
            builder.Append(_trelloKey + "&token=");
            builder.Append(_trelloToken);

            WebRequest webRequest = WebRequest.Create(builder.ToString());
            WebResponse webResp = webRequest.GetResponse();

            using (var stream = new System.IO.StreamReader(webResp.GetResponseStream()))
            {
                Console.WriteLine(stream.ReadToEnd());
            }
            Console.WriteLine("");
        }
    }
}
