using Unit8_SRP_F24.Contracts;

namespace Unit8_SRP_F24;


class Program
{
    static async Task Main(string[] args)
    {
        const string tradeURL = "http://raw.githubusercontent.com/tgibbons-css/CIS3285_Unit9_F24/refs/heads/master/SingleResponsibilityPrinciple/trades.txt";
        const string restfulURL = "https://unit9trader.azurewebsites.net/api/TradeData";
        ILogger logger = new ConsoleLogger();
        ITradeValidator tradeValidator = new SimpleTradeValidator(logger);
        ITradeDataProvider tradeDataProvider = args.Contains("--url", StringComparer.OrdinalIgnoreCase)
            ? new URLTradeDataProvider(tradeURL, logger)
            : new RestfulTradeDataProvider(restfulURL, logger);
        ITradeMapper tradeMapper = new SimpleTradeMapper();
        ITradeParser tradeParser = new SimpleTradeParser(tradeValidator, tradeMapper);
        ITradeStorage tradeStorage = new AdoNetTradeStorage(logger);

        var tradeProcessor = new TradeProcessor(tradeDataProvider, tradeParser, tradeStorage);
        await tradeProcessor.ProcessTradesAsync();

        //Console.ReadKey();
    }
}
