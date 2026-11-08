using System.Reflection;
using Unit8_SRP_F24.Contracts;

namespace Unit8_SRP_F24;


class Program
{
    static void Main(string[] args)
    {
        var tradeStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Unit8_SRP_F24.trades.txt");
        if (tradeStream is null)
        {
            Console.Error.WriteLine("Could not find the embedded trades.txt file.");
            return;
        }
        ILogger logger = new ConsoleLogger();
        ITradeValidator tradeValidator = new SimpleTradeValidator(logger);

        //original stream reader for trade data
        ITradeDataProvider streamDataProvider = new StreamTradeDataProvider(tradeStream, logger);

        //new Restful reader for trade data
        ITradeDataProvider restfulDataProvider = new RestfulTradeDataProvider("https://unit9trader.azurewebsites.net/api/TradeData", logger);
        
        //new URL reader for trade data
        ITradeDataProvider URLTradeDataProvider = new URLTradeDataProvider("http://raw.githubusercontent.com/tgibbons-css/CIS3285_Unit9_F24/refs/heads/master/SingleResponsibilityPrinciple/trades.txt", logger);

        ITradeMapper tradeMapper = new SimpleTradeMapper();
        ITradeParser tradeParser = new SimpleTradeParser(tradeValidator, tradeMapper);
        ITradeStorage tradeStorage = new AdoNetTradeStorage(logger);


        var tradeProcessor = new TradeProcessor(restfulDataProvider, tradeParser, tradeStorage);
        tradeProcessor.ProcessTrades();

        //Console.ReadKey();
    }
}
