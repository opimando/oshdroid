using OshDroid.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OshDroid.Core.Interfaces;
using OshDroid.Core.Models;
using Serilog;
using Telegram;
using TelegramModel;

namespace OshDroid.Startup;

internal class Program
{
    public static async Task Main(string[] args)
    {
        IHostBuilder hostBuilder = new HostBuilder()
            .ConfigureHostConfiguration(config =>
            {
                config.AddJsonFile("settings.json", false, true);
                config.AddJsonFile("logger.json", false, true);
                config.AddEnvironmentVariables();
            })
            .ConfigureServices((hostContext, services) =>
            {
                var options = hostContext.Configuration.Get<Settings>()!;
                if (options == null)
                    throw new ArgumentNullException(nameof(options), "Не удалось получить настройки приложения");

                services.AddSingleton(options);
                services.AddSingleton<IHostedService, DroidHostedService>();
                services.AddSingleton<ITelegramProvider>(
                    sp => new TelegramProvider(options.TgKey, sp.GetRequiredService<ILogger<TelegramProvider>>()));
                services.AddSingleton<IRandomizer, DefaultNetRandomizer>();
                services.AddSingleton<IVoteHolder, SingleVoteHolder>();
                services.AddSingleton(new DroidOptions
                {
                    GroupId = options.TgGroupId,
                    VoteOptions = options.VoteOptions
                });
                services.AddSingleton<DroidBot>();
            });

        hostBuilder
            .UseSerilog((context, loggerConfig) => loggerConfig.ReadFrom.Configuration(context.Configuration))
            .ConfigureLogging((host, config) =>
            {
                if (!host.Configuration.GetChildren().Any(s => s.Key.StartsWith("Serilog")))
                    config.AddConsole();
            });

        IHost build = hostBuilder.Build();
        await build.RunAsync();
    }
}