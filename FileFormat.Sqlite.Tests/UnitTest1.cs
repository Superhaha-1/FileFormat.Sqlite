using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FileFormat.Sqlite.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        [DeploymentItem("file.db",)]
        public void TestMethod1()
        {
        }
    }
}
