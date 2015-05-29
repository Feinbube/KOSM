using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KOSM.Reporting
{
    public class Log : ILog
    {
        List<string> messages = new List<string>();

        public List<string> Messages
        {
            get { return messages; }
        }

        public void Add(string message)
        {
            messages.Add(message);
        }

        public void Clear()
        {
            messages.Clear();
        }
    }
}
