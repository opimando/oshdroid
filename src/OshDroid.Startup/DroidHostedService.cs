#region Copyright

/*
 * File: DroidHostedService.cs
 * Author: denisosipenko
 * Created: 2022-10-06
 * Copyright © 2022 Denis Osipenko
 */

#endregion Copyright

using OshDroid.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace OshDroid.Startup;

public class DroidHostedService : IHostedService
{
    private readonly ILogger<DroidHostedService> _logger;
    private readonly DroidBot _bot;

    public DroidHostedService(ILogger<DroidHostedService> logger, DroidBot bot)
    {
        _logger = logger;
        _bot = bot;
    }

    /// <inheritdoc />
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _bot.Start();
        _logger.LogInformation("Приложение запущено");
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task StopAsync(CancellationToken cancellationToken)
    {
        _bot.Stop();
        _logger.LogInformation("Приложение остановлено");
        return Task.CompletedTask;
    }
}