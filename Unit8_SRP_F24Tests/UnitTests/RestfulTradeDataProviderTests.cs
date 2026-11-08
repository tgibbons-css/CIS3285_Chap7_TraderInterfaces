using System;
using Unit8_SRP_F24;
using Unit8_SRP_F24.Contracts;

namespace Unit8_SRP_F24Tests.UnitTests;

[TestClass()]
public class RestfulTradeDataProviderTests
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
    public void TestSize3()
    {
        //Arrange
        ILogger logger = new ConsoleLogger();
        string restfulURL = "https://unit9trader.azurewebsites.net/api/TradeData";

        ITradeDataProvider tradeProvider = new RestfulTradeDataProvider(restfulURL, logger);

        //Act
        IEnumerable<string> trades = tradeProvider.ReadTradeData();

        //Assert

        Assert.AreEqual(countStrings(trades), 3);
    }
}
