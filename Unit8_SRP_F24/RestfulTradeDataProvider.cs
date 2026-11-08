using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Unit8_SRP_F24.Contracts;

namespace Unit8_SRP_F24;

public class RestfulTradeDataProvider : ITradeDataProvider
{
    private readonly string url;
    private readonly ILogger logger;

    public RestfulTradeDataProvider(string url, ILogger logger)
    {
        this.url = url;
        this.logger = logger;
    }

    public IEnumerable<string> ReadTradeData()
    {
        logger.LogMessage("Reading trade data from Restful API: " + url);

        // The interface is synchronous, so run the asynchronous web request in a task
        // and wait for its result here.
        return Task.Run(ReadTradeDataAsync).GetAwaiter().GetResult();
    }

    private async Task<IEnumerable<string>> ReadTradeDataAsync()
    {
        // HttpClient sends the request to the API and receives its response.
        using HttpClient client = new HttpClient();
        using HttpResponseMessage response = await client.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            logger.LogMessage($"Failed to fetch trade data from Restful API: {url}. Status code: {response.StatusCode}");
            response.EnsureSuccessStatusCode();
        }

        // Read the response body as text. The API returns JSON containing an array of trade strings.
        string json = await response.Content.ReadAsStringAsync();

        // Convert the JSON array into strings that the trade parser can read.
        return JsonSerializer.Deserialize<List<string>>(json)
            ?? throw new JsonException("The Restful API returned an empty trade-data response.");
    }
}
