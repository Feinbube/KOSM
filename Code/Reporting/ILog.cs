using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KOSM.Reporting
{
    public interface ILog
    {
        List<string> Messages { get; }

        void Add(string message);

        void Clear();
    }
}
