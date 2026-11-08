using System;
using System.Reflection;

using Unit8_SRP_F24;
using Unit8_SRP_F24.Contracts;

namespace Unit8_SRP_F24Tests.UnitTests;

    [TestClass()]
    public class URLTradeDataProviderTests
    {
        private int countStrings(IEnumerable<string> collectionOfStrings)
        {
            // count the trades
            int count = 0;
            foreach (var trade in collectionOfStrings)
            {
                count++;
            }
            return count;
        }


        [TestMethod()]
        public void TestSize4()
        {
            //Arrange
            ILogger logger = new ConsoleLogger();
            string tradeURL = "http://raw.githubusercontent.com/tgibbons-css/CIS3285_Unit9_F24/refs/heads/master/SingleResponsibilityPrinciple/trades.txt";

            ITradeDataProvider tradeProvider = new URLTradeDataProvider(tradeURL, logger);

            //Act
            IEnumerable<string> trades = tradeProvider.ReadTradeData();

            //Assert

            Assert.AreEqual(4, countStrings(trades));
        }
        
        [TestMethod()]
        public void TestTradeDataContainsExpectedFirstTrade()
        {
            //Arrange
            ILogger logger = new ConsoleLogger();
            string tradeURL = "http://raw.githubusercontent.com/tgibbons-css/CIS3285_Unit9_F24/refs/heads/master/SingleResponsibilityPrinciple/trades.txt";

            ITradeDataProvider tradeProvider = new URLTradeDataProvider(tradeURL, logger);

            //Act
            IEnumerable<string> trades = tradeProvider.ReadTradeData();

            //Assert

            Assert.AreEqual("GBPUSD,1000,1.51", trades.First());
        }
    }
