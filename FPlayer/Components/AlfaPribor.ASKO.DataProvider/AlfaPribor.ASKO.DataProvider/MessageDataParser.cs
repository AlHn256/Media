using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlfaPribor.ASKO.Data;

namespace AlfaPribor.ASKO.DataProvider
{

    public class MessageDataParser
    {
        private Dictionary<int,string> _messageTextDictionary = new Dictionary<int,string>();
        private object _lockObject = new object();
        
        public MessageDataParser(IList<MessageData> messages)
        {
            lock (_lockObject)
            {
                FillDictionary(messages);
            }
        }

        private void FillDictionary(IList<MessageData> messages)
        {
            foreach (var mess in messages)
            {
                try
                {
                    _messageTextDictionary.Add(mess.Id, mess.Text);
                }
                catch { }
            }
        }

        public string GetMessageText(int messageID)
        {
            string text = string.Empty;
            if (!_messageTextDictionary.ContainsKey(messageID))
                return string.Empty;
            return _messageTextDictionary[messageID];
        }

        public string GetMessageDebugText(EventData data)
        {
            string text = string.Empty;
            try
            {
                string msgData = string.Empty;
                lock (_lockObject)
                {
                    msgData = GetMessageText(data.Sn);
                }
                text = string.Format("{0} {1} : {2} {3}", data.EventTime.ToString("YYYY.DD.MM HH:mm:ss"), data.Source, msgData, data.Data);
            }
            catch { text = string.Empty; }
            return text;
        }
    }
}
