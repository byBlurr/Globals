using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Globals
{
    class Program
    {
        static void Main(string[] args) => new Program().StartAsync().GetAwaiter().GetResult();
        private DiscordSocketClient client;
        private CommandHandler handler;

        public async Task StartAsync()
        {
            client = new DiscordSocketClient(new DiscordSocketConfig()
            {
                LogLevel = LogSeverity.Verbose,
            });

            client.Log += Logger;

            EnsureConfigExists();

            await client.LoginAsync(TokenType.Bot, BotConfig.Load().BotToken);
            await client.StartAsync();

            var serviceProvider = ConfigureServices();
            handler = new CommandHandler(serviceProvider);
            await handler.ConfigureAsync();

            await Task.Delay(-1);
        }

        public static Task Logger(LogMessage lmsg)
        {
            var cc = Console.ForegroundColor;
            switch (lmsg.Severity)
            {
                case LogSeverity.Critical:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    break;
                case LogSeverity.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogSeverity.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogSeverity.Info:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
                case LogSeverity.Verbose:
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    break;
                case LogSeverity.Debug:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
            }
            Console.WriteLine($"{DateTime.Now} [{lmsg.Severity,8}] {lmsg.Source}: {lmsg.Message}");
            if (lmsg.Source.ToLower().Contains("gateway") && lmsg.Message.ToLower().Contains("ready")) return Task.CompletedTask;
            if (lmsg.Source.ToLower().Contains("rest") && lmsg.Message.Contains("POST")) return Task.CompletedTask;
            if (lmsg.Source.ToLower().Contains("rest") && lmsg.Message.Contains("DELETE")) return Task.CompletedTask;
            Console.ForegroundColor = cc;
            return Task.CompletedTask;
        }

        public IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection().AddSingleton(client).AddSingleton(new CommandService(new CommandServiceConfig { CaseSensitiveCommands = false }));
            var provider = new DefaultServiceProviderFactory().CreateServiceProvider(services);

            return provider;
        }

        public static void EnsureConfigExists()
        {
            if (!Directory.Exists(Path.Combine(AppContext.BaseDirectory, "config")))
                Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "config"));

            string ConfigFile = Path.Combine(AppContext.BaseDirectory, "config/bot.json");

            if (!File.Exists(ConfigFile))
            {
                var config = new BotConfig();
                config.Save();
            }
        }
    }
}
