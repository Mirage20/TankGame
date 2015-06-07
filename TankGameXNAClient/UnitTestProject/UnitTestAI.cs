using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TankGameXNAClient;
using System.Drawing;
namespace UnitTestProject
{
    [TestClass]
    public class UnitTestAI
    {
        [TestMethod]
        public void TestMethodIsCellInRegion()
        {

            PrivateObject aiManager = new PrivateObject(typeof(AIManager));
            bool result = Convert.ToBoolean(aiManager.Invoke("isCellInRegion", new object { new Point(0, 0), new Point(2, 3), 4 }));
            Assert.AreEqual(true, result);
        }

    }
}
