using Microsoft.Extensions.Hosting;
using Redis.OM;
using Services.Abstractions;
using Services.Contracts;


namespace Services.Implementation.HostedServices;

public class IndexCreationService : IHostedService
{
    private readonly RedisConnectionProvider _provider;
    public IndexCreationService(RedisConnectionProvider provider)
    {
        _provider = provider;

    }

    /// <summary>
    /// Checks redis to see if the index already exists, if it doesn't create a new index
    /// </summary>
    /// <param name="cancellationToken"></param>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var info = (await _provider.Connection.ExecuteAsync("FT._LIST")).ToArray().Select(x => x.ToString());
        if (info.All(x => x != "logisticoffer-idx"))
        {
          //  await _provider.Connection.CreateIndexAsync(typeof(LogisticOffer));
        }
        if (info.All(x => x != "contragentdto-idx"))
        {
          //  await _provider.Connection.CreateIndexAsync(typeof(ContragentDto));
        }



    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}