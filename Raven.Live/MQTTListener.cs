namespace Raven.Live;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Services;
using System.Threading.Tasks;


internal class MQTTListener : IHostedService
{
    private readonly ILogger<MQTTListener> _logger;
    private readonly IRavenUsersClient _ravenUsersClient;


    public MQTTListener(
        ILogger<MQTTListener> logger,
        IRavenUsersClient ravenUsersClient
    )
    {
        _logger = logger;
        _ravenUsersClient = ravenUsersClient;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("StartAsync has been called.");

        // await _ravenUsersClient.ReportUserOnSite("test");
        _logger.LogInformation(await _ravenUsersClient.GetVersion());
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("StopAsync has been called.");
    }
}
