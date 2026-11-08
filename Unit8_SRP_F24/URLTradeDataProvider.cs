using System.Net;
using Unit8_SRP_F24.Contracts;

namespace Unit8_SRP_F24;

public class URLTradeDataProvider : ITradeDataProvider
{
    private static readonly HttpClient client = new();
    private readonly string url;
    private readonly ILogger logger;

    public URLTradeDataProvider(string url, ILogger logger)
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
        logger.LogMessage("Reading trade data from URL: " + url);

        HttpResponseMessage response;
        try
        {
            response = await client.GetAsync(url).ConfigureAwait(false);
        }
        catch (HttpRequestException exception)
        {
            logger.LogMessage("ERROR: Could not read trade data from URL: {0}", exception.Message);
            throw;
        }

        using (response)
        {
            if (response.StatusCode is < HttpStatusCode.OK or >= HttpStatusCode.MultipleChoices)
            {
                logger.LogMessage("ERROR: Could not read trade data from URL. HTTP status: {0}", response.StatusCode);
                throw new HttpRequestException(
                    $"Could not read trade data from URL. HTTP status: {response.StatusCode}",
                    null,
                    response.StatusCode);
            }

            using Stream stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            using StreamReader reader = new(stream);
            List<string> tradeData = new();
            string? line;
            while ((line = await reader.ReadLineAsync().ConfigureAwait(false)) is not null)
            {
                tradeData.Add(line);
            }

            return tradeData;
        }
    }
}
