using System;

namespace Unit8_SRP_F24.Contracts;

public interface ITradeDataProvider
{
    public IEnumerable<string> ReadTradeData();

    public Task<IEnumerable<string>> ReadTradeDataAsync()
    {
        return Task.FromResult(ReadTradeData());
    }
}
