using System;

namespace Messages
{
    public class GenericMessage
    {

        protected  GenericMessage(string messageType)
        {
            MessageDate = DateTime.Now;
            MessageType = messageType;
        }

        public DateTime MessageDate { get; set; }

        public string MessageType { get; set; }
        
    }
}
