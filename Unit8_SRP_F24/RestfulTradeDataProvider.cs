using System;
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
            try
            {
                using (HttpResponseMessage response = client.GetAsync(url).GetAwaiter().GetResult())
                {
                    response.EnsureSuccessStatusCode();
                    using (Stream stream = response.Content.ReadAsStreamAsync().GetAwaiter().GetResult())
                    {
                        List<string>? trades = JsonSerializer.Deserialize<List<string>>(stream);
                        if (trades == null || trades.Any(trade => trade == null))
                        {
                            throw new JsonException("The response must contain an array of trade strings.");
                        }

                        return trades;
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                logger.LogMessage("Error reading trade data from Restful API: " + url + ". " + ex.Message);
                throw;
            }
            catch (JsonException ex)
            {
                logger.LogMessage("Invalid trade data from Restful API: " + url + ". " + ex.Message);
                throw;
            }
        }
    }
}
