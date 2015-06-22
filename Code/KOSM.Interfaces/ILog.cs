using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KOSM.Interfaces
{
    public interface ILog
    {
        List<object> Messages { get; }

        void Add(object message);

        Action<object> MessageAdded { set; }        

        void Clear();
    }
}
