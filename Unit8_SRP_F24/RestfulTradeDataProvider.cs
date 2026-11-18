using System.Text.Json;
using Unit8_SRP_F24.Contracts;

namespace Unit8_SRP_F24;

public class RestfulTradeDataProvider : ITradeDataProvider
{
    string url;
    ILogger logger;

    public RestfulTradeDataProvider(string url, ILogger logger)
    {
        this.url = url;
        this.logger = logger;
    }

    public IEnumerable<string> ReadTradeData()
    {
        logger.LogMessage("Reading trade data from Restful API: " + url);

        using (HttpClient client = new HttpClient())
        {
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (!response.IsSuccessStatusCode)
            {
                logger.LogMessage("Failed to read trades from Restful API: " + url);
                throw new Exception("Failed to read trades from Restful API: " + url);
            }

            string json = response.Content.ReadAsStringAsync().Result;
            List<string> tradeData = JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
            return tradeData;
        }
    }
}
