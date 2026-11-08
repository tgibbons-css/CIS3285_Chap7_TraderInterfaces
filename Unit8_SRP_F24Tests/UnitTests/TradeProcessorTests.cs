using Unit8_SRP_F24;
using Unit8_SRP_F24.Contracts;

namespace Unit8_SRP_F24Tests.UnitTests;

[TestClass]
public class TradeProcessorTests
{
    [TestMethod]
    public void ProcessTrades_ReadsParsesAndStoresTradesInOrder()
    {
        // Arrange
        var calls = new List<string>();
        IEnumerable<string> lines = new[] { "AAABBB,1000,1.25", "CCCDDD,2000,2.50" };
        IEnumerable<TradeRecord> trades = new[]
        {
                new TradeRecord { SourceCurrency = "AAA", DestinationCurrency = "BBB", Lots = 0.01f, Price = 1.25m },
                new TradeRecord { SourceCurrency = "CCC", DestinationCurrency = "DDD", Lots = 0.02f, Price = 2.50m }
            };
        var dataProvider = new StubTradeDataProvider(lines, calls);
        var parser = new StubTradeParser(trades, calls);
        var storage = new StubTradeStorage(calls);
        var processor = new TradeProcessor(dataProvider, parser, storage);

        // Act
        processor.ProcessTrades();

        // Assert
        CollectionAssert.AreEqual(new[] { "read", "parse", "store" }, calls);
        Assert.AreSame(lines, parser.ReceivedLines);
        Assert.AreSame(trades, storage.StoredTrades);
    }

    [TestMethod]
    public async Task ProcessTradesAsync_ReadsParsesAndStoresTradesInOrder()
    {
        var calls = new List<string>();
        IEnumerable<string> lines = new[] { "AAABBB,1000,1.25" };
        IEnumerable<TradeRecord> trades = new[]
        {
            new TradeRecord { SourceCurrency = "AAA", DestinationCurrency = "BBB", Lots = 0.01f, Price = 1.25m }
        };
        var dataProvider = new StubTradeDataProvider(lines, calls);
        var parser = new StubTradeParser(trades, calls);
        var storage = new StubTradeStorage(calls);
        var processor = new TradeProcessor(dataProvider, parser, storage);

        await processor.ProcessTradesAsync();

        CollectionAssert.AreEqual(new[] { "read", "parse", "store" }, calls);
        Assert.AreSame(lines, parser.ReceivedLines);
        Assert.AreSame(trades, storage.StoredTrades);
    }

    [TestMethod]
    public void ProcessTrades_WithNoLines_StillParsesAndStoresEmptyResults()
    {
        // Arrange
        var calls = new List<string>();
        IEnumerable<string> lines = Array.Empty<string>();
        IEnumerable<TradeRecord> trades = Array.Empty<TradeRecord>();
        var dataProvider = new StubTradeDataProvider(lines, calls);
        var parser = new StubTradeParser(trades, calls);
        var storage = new StubTradeStorage(calls);
        var processor = new TradeProcessor(dataProvider, parser, storage);

        // Act
        processor.ProcessTrades();

        // Assert
        CollectionAssert.AreEqual(new[] { "read", "parse", "store" }, calls);
        Assert.AreSame(lines, parser.ReceivedLines);
        Assert.AreSame(trades, storage.StoredTrades);
    }

    [TestMethod]
    public void ProcessTrades_WhenDataProviderFails_PropagatesExceptionAndDoesNotContinue()
    {
        // Arrange
        var calls = new List<string>();
        var expectedException = new InvalidOperationException("Could not read trade data.");
        var dataProvider = new StubTradeDataProvider(calls, expectedException);
        var parser = new StubTradeParser(Array.Empty<TradeRecord>(), calls);
        var storage = new StubTradeStorage(calls);
        var processor = new TradeProcessor(dataProvider, parser, storage);

        // Act
        var actualException = Assert.ThrowsException<InvalidOperationException>(
            () => processor.ProcessTrades());

        // Assert
        Assert.AreSame(expectedException, actualException);
        CollectionAssert.AreEqual(new[] { "read" }, calls);
    }

    [TestMethod]
    public void ProcessTrades_WhenParserFails_PropagatesExceptionAndDoesNotStore()
    {
        // Arrange
        var calls = new List<string>();
        var expectedException = new InvalidOperationException("Could not parse trade data.");
        var dataProvider = new StubTradeDataProvider(Array.Empty<string>(), calls);
        var parser = new StubTradeParser(calls, expectedException);
        var storage = new StubTradeStorage(calls);
        var processor = new TradeProcessor(dataProvider, parser, storage);

        // Act
        var actualException = Assert.ThrowsException<InvalidOperationException>(
            () => processor.ProcessTrades());

        // Assert
        Assert.AreSame(expectedException, actualException);
        CollectionAssert.AreEqual(new[] { "read", "parse" }, calls);
    }

    private sealed class StubTradeDataProvider : ITradeDataProvider
    {
        private readonly IEnumerable<string> lines;
        private readonly List<string> calls;
        private readonly Exception? exception;

        public StubTradeDataProvider(IEnumerable<string> lines, List<string> calls)
        {
            this.lines = lines;
            this.calls = calls;
        }

        public StubTradeDataProvider(List<string> calls, Exception exception)
        {
            this.calls = calls;
            this.exception = exception;
            lines = Array.Empty<string>();
        }

        public IEnumerable<string> ReadTradeData()
        {
            calls.Add("read");
            if (exception is not null)
            {
                throw exception;
            }

            return lines;
        }
    }

    private sealed class StubTradeParser : ITradeParser
    {
        private readonly IEnumerable<TradeRecord> trades;
        private readonly List<string> calls;
        private readonly Exception? exception;

        public StubTradeParser(IEnumerable<TradeRecord> trades, List<string> calls)
        {
            this.trades = trades;
            this.calls = calls;
        }

        public StubTradeParser(List<string> calls, Exception exception)
        {
            this.calls = calls;
            this.exception = exception;
            trades = Array.Empty<TradeRecord>();
        }

        public IEnumerable<TradeRecord> ParseTrades(IEnumerable<string> lines)
        {
            calls.Add("parse");
            ReceivedLines = lines;
            if (exception is not null)
            {
                throw exception;
            }

            return trades;
        }

        public IEnumerable<string>? ReceivedLines { get; private set; }
    }

    private sealed class StubTradeStorage : ITradeStorage
    {
        private readonly List<string> calls;

        public StubTradeStorage(List<string> calls)
        {
            this.calls = calls;
        }

        public IEnumerable<TradeRecord>? StoredTrades { get; private set; }

        public void StoreTrades(IEnumerable<TradeRecord> trades)
        {
            calls.Add("store");
            StoredTrades = trades;
        }
    }
}
