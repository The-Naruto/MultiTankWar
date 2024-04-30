// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

internal class MyFistHostedService : IHostedService
{
    readonly ILogger<MyFistHostedService> _logger;
    /// <summary>
    /// 用于在服务的生命周期关键节点插入逻辑
    /// </summary>
    readonly IHostApplicationLifetime _applicationLifetime;


    public MyFistHostedService(ILogger<MyFistHostedService> logger, IHostApplicationLifetime applicationLifetime)
    {
        _logger = logger;
        _applicationLifetime = applicationLifetime;
        _logger.LogDebug("i'm init now");
        _applicationLifetime.ApplicationStarted.Register(OnStart);
        _applicationLifetime.ApplicationStopping.Register(OnStop);
        _applicationLifetime.ApplicationStopped.Register(OnStopComplete);

    }

    private void OnStopComplete()
    {
        _logger.LogDebug("i'm StopComplete");
    }

    private void OnStop()
    {
        _logger.LogDebug("i'm ready to Stop");
    }

    private void OnStart()
    {
        _logger.LogDebug("i'm ready to Start");
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogDebug("i'm ready to start now !");
        _logger.LogDebug("test i'll not print!");
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogDebug("i'm ready to stop now !");
        return Task.CompletedTask;
    }
}