using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KOSM.Interfaces;

namespace KOSM.Common
{
    public class Log : ILog
    {
        List<object> messages = new List<object>();

        public List<object> Messages
        {
            get { return messages; }
        }

        public void Add(object message)
        {
            message = message == null ? "null" : message;

            messages.Add(message);

            if (MessageAdded != null)
                MessageAdded(message);
        }

        public void Clear()
        {
            messages.Clear();
        }

        public Action<object> MessageAdded { private get; set; }
    }
}
