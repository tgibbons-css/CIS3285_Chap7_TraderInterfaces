using System.Net;
using System.Text.Json;
using Unit8_SRP_F24.Contracts;

namespace Unit8_SRP_F24;

public class RestfulTradeDataProvider : ITradeDataProvider
{
    private static readonly HttpClient client = new();
    private readonly string url;
    private readonly ILogger logger;

    public RestfulTradeDataProvider(string url, ILogger logger)
    {
        this.url = url;
        this.logger = logger;
    }

    public IEnumerable<string> ReadTradeData()
    {
        return ReadTradeDataAsync().GetAwaiter().GetResult();
    }

    public async Task<IEnumerable<string>> ReadTradeDataAsync()
    {
        logger.LogMessage("Reading trade data from Restful API: " + url);

        HttpResponseMessage response;
        try
        {
            response = await client.GetAsync(url).ConfigureAwait(false);
        }
        catch (HttpRequestException exception)
        {
            logger.LogMessage("ERROR: Could not read trade data from RESTful API: {0}", exception.Message);
            throw;
        }

        using (response)
        {
            if (response.StatusCode is < HttpStatusCode.OK or >= HttpStatusCode.MultipleChoices)
            {
                logger.LogMessage("ERROR: Could not read trade data from RESTful API. HTTP status: {0}", response.StatusCode);
                throw new HttpRequestException(
                    $"Could not read trade data from RESTful API. HTTP status: {response.StatusCode}",
                    null,
                    response.StatusCode);
            }

            string json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            try
            {
                return JsonSerializer.Deserialize<List<string>>(json)
                    ?? throw new JsonException("The RESTful API returned a null trade list.");
            }
            catch (JsonException exception)
            {
                logger.LogMessage("ERROR: Could not deserialize trade data from RESTful API: {0}", exception.Message);
                throw;
            }
        }
    }
}
