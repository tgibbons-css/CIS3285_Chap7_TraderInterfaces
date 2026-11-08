using Unit8_SRP_F24.Contracts;

namespace Unit8_SRP_F24;

public class URLTradeDataProvider : ITradeDataProvider
{
    private readonly string url;        // URL to fetch trade data from
    private readonly ILogger logger;    //  Logger to log messages

    public URLTradeDataProvider(string url, ILogger logger)
    {
        this.url = url;
        this.logger = logger;
    }

    public IEnumerable<string> ReadTradeData()
    {
        logger.LogMessage("Reading trade data from URL: " + url);
        // -------------- Add your code here to fetch trade data from the URL and return it as an IEnumerable<string> --------------
        List<string> tradeData = new List<string>();
        logger.LogMessage("Reading trades from URL: " + url);
        using (HttpClient client = new HttpClient())
        {
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (!response.IsSuccessStatusCode)
            {
                // log error and throw an exception if the URL fails
                logger.LogMessage($"Failed to fetch trade data from URL: {url}. Status code: {response.StatusCode}");
                throw new Exception($"Failed to fetch trade data from URL: {url}. Status code: {response.StatusCode}");
            }
            // set up a Stream and StreamReader to access the data
            using (Stream stream = response.Content.ReadAsStreamAsync().Result)
            using (StreamReader reader = new StreamReader(stream))
            {
                // Read each line of the text file using reader.ReadLine()
                string line;
                // Read until the reader returns a null line
                while ((line = reader.ReadLine()) != null)
                {
                    tradeData.Add(line);
                }
            }
        }
        return tradeData;
    }
}
