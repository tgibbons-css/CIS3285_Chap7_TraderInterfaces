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
        List<string> tradeData = new List<string>();
        using (HttpClient client = new HttpClient())
        {
            try
            {
                using (HttpResponseMessage response = client.GetAsync(url).GetAwaiter().GetResult())
                {
                    response.EnsureSuccessStatusCode();
                    using (Stream stream = response.Content.ReadAsStreamAsync().GetAwaiter().GetResult())
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string? line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            tradeData.Add(line);
                        }
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                logger.LogMessage("Error reading trade data from URL: " + url + ". " + ex.Message);
                throw;
            }
        }

        return tradeData;
    }
}
