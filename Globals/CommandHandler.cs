using System.Threading.Tasks;
using System.Reflection;
using Discord.Commands;
using Discord.WebSocket;
using Discord;
using System;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace Globals
{
    class CommandHandler
    {
        private CommandService commands;
        private static DiscordSocketClient bot;
        private IServiceProvider map;

        public CommandHandler(IServiceProvider provider)
        {
            map = provider;
            bot = map.GetService<DiscordSocketClient>();

            bot.Ready += ReadyAsync;

            commands = map.GetService<CommandService>();
        }

        private async Task ReadyAsync()
        {
            await bot.SetGameAsync(BotConfig.Load().BotStatus);
            await bot.SetStatusAsync(UserStatus.Idle);
        }

        public async Task ConfigureAsync() { await commands.AddModulesAsync(Assembly.GetEntryAssembly(), map); }
    }
}