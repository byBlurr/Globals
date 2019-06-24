using System.Threading.Tasks;
using System.Reflection;
using Discord.Commands;
using Discord.WebSocket;
using Discord;
using System;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Globals.Data;

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

            bot.JoinedGuild += HandleJoinGuildAsync;
            bot.LeftGuild += HandleLeftGuildAsync;

            bot.Ready += ReadyAsync;
            bot.MessageReceived += HandleGlobalMessageAsync;
            bot.MessageReceived += HandleCommandAsync;

            commands = map.GetService<CommandService>();
        }

        private async Task HandleJoinGuildAsync(SocketGuild guild)
        {
            ServerConfig.RegisterServer(guild.Id);
        }

        private async Task HandleLeftGuildAsync(SocketGuild guild)
        {
            ServerConfig.UnregisterServer(guild.Id);
        }

        private async Task HandleGlobalMessageAsync(SocketMessage pMsg)
        {
            SocketUserMessage message = pMsg as SocketUserMessage;
            if (message == null) return;
            var context = new SocketCommandContext(bot, message);
            if (message.Author.IsBot) return;

            
        }

        private async Task HandleCommandAsync(SocketMessage pMsg)
        {
            SocketUserMessage message = pMsg as SocketUserMessage;
            if (message == null) return;
            var context = new SocketCommandContext(bot, message);
            if (message.Author.IsBot) return;

            int argPos = 0;
            if (message.HasStringPrefix(BotConfig.Load().BotPrefix, ref argPos))
            {
                var result = await commands.ExecuteAsync(context, argPos, map);
                if (!result.IsSuccess && result.ErrorReason != "Unknown command.") Console.WriteLine(result.ErrorReason);
            }
        }

        private async Task ReadyAsync()
        {
            await bot.SetGameAsync(BotConfig.Load().BotStatus);
            await bot.SetStatusAsync(UserStatus.Idle);
        }

        public async Task ConfigureAsync() { await commands.AddModulesAsync(Assembly.GetEntryAssembly(), map); }
    }
}