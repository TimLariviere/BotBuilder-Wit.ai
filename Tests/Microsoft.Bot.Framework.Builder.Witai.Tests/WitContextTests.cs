﻿using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Microsoft.Bot.Framework.Builder.Witai.Tests
{
    [TestClass]
    public class WitContextTests
    {
        [TestMethod]
        public void Is_Case_Insensitive()
        {
            //Arrange
            var witContext = new WitContext();
            witContext["teSt"] = "testData";

            //Action
            //Assert
            if (witContext.TryGetValue("TEsT", out object val))
            {
                Assert.AreEqual(val, "testData");
            }
            else
            {
                Assert.Fail("WitContext should not be case sensitive");
            }
        }
    }
}