using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using KOSM.Common;

namespace KOSM.Tests
{
    [TestClass]
    public class KOSM_Common_Reporting_Format_Test
    {
        [TestMethod]
        public void KerbalTimespanTest()
        {
            Assert.AreEqual("1s 0ms", Format.KerbalTimespan(1));
            Assert.AreEqual("1m 40s", Format.KerbalTimespan(100));
            Assert.AreEqual("1h 0m", Format.KerbalTimespan(3600));
            Assert.AreEqual("2h 46m", Format.KerbalTimespan(10000));
            Assert.AreEqual("1d 0h", Format.KerbalTimespan(21600));
            Assert.AreEqual("4d 3h", Format.KerbalTimespan(100000));
            Assert.AreEqual("1y 0d", Format.KerbalTimespan(9203400));
        }
    }
}
