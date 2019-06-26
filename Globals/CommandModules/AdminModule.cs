using Discord;
using Discord.Commands;
using Globals.Data;
using Globals.Util;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Globals.CommandModules
{
    public class AdminModule : ModuleBase
    {
        [Command("setup")]
        [RequireBotPermission(GuildPermission.ManageChannels)]
        public async Task RegisterAsync()
        {
            await Context.Message.DeleteAsync();

            await ServerConfig.RegisterServerAsync(Context.Guild.Id);
            var Message = await Context.Channel.SendMessageAsync("Setup started, use the command `!enable` next.");
            await Delete.DeleteMessage(Message);
        }

        [Command("enable")]
        [RequireBotPermission(GuildPermission.ManageChannels)]
        public async Task EnableAsync(string channel = "", bool toggle = true)
        {
            await Context.Message.DeleteAsync();

            if (channel.ToLower().Equals("gaming")) await ServerConfig.ToggleChannel(Context.Guild.Id, "gaming", toggle);
            else if (channel.ToLower().Equals("music")) await ServerConfig.ToggleChannel(Context.Guild.Id, "music", toggle);
            else if (channel.ToLower().Equals("movies")) await ServerConfig.ToggleChannel(Context.Guild.Id, "movies", toggle);
            else if (channel.ToLower().Equals("rainbow6")) await ServerConfig.ToggleChannel(Context.Guild.Id, "rainbowsix", toggle);
            else if (channel.ToLower().Equals("league")) await ServerConfig.ToggleChannel(Context.Guild.Id, "league", toggle);
            else if (channel.ToLower().Equals("rust")) await ServerConfig.ToggleChannel(Context.Guild.Id, "rust", toggle);
            else if (channel.ToLower().Equals("gta")) await ServerConfig.ToggleChannel(Context.Guild.Id, "gta", toggle);
            else if (channel.ToLower().Equals("pubg")) await ServerConfig.ToggleChannel(Context.Guild.Id, "pubg", toggle);
            else if (channel.ToLower().Equals("fortnite")) await ServerConfig.ToggleChannel(Context.Guild.Id, "fortnite", toggle);
            else if (channel.ToLower().Equals("apex")) await ServerConfig.ToggleChannel(Context.Guild.Id, "apex", toggle);

            string GamingState = "Disabled", MusicState = "Disabled", MoviesState = "Disabled", R6State = "Disabled", LeagueState = "Disabled", RustState = "Disabled", GtaState = "Disabled", PubgState = "Disabled", FortniteState = "Disabled", ApexState = "Disabled";
            ServerConfig.GetChannelSettingsAsText(Context.Guild.Id, ref GamingState, ref MusicState, ref MoviesState, ref R6State, ref LeagueState, ref RustState, ref GtaState, ref PubgState, ref FortniteState, ref ApexState);

            var embed = new EmbedBuilder() { Color = new Color(114, 137, 218) };
            embed.WithDescription("Enable/Disable channels using `!enable <channel> <true/false>`, for example `!enable rainbow6 true`.\nOnce the correct channels are set, use the command `!create` next.");
            embed.AddField(new EmbedFieldBuilder() { Name = "Gaming", Value = GamingState });
            embed.AddField(new EmbedFieldBuilder() { Name = "Music", Value = MusicState });
            embed.AddField(new EmbedFieldBuilder() { Name = "Movies", Value = MoviesState });
            embed.AddField(new EmbedFieldBuilder() { Name = "Rainbow6", Value = R6State });
            embed.AddField(new EmbedFieldBuilder() { Name = "League", Value = LeagueState });
            embed.AddField(new EmbedFieldBuilder() { Name = "Rust", Value = RustState });
            embed.AddField(new EmbedFieldBuilder() { Name = "GTA", Value = GtaState });
            embed.AddField(new EmbedFieldBuilder() { Name = "PUBG", Value = PubgState });
            embed.AddField(new EmbedFieldBuilder() { Name = "Fortnite", Value = FortniteState });
            embed.AddField(new EmbedFieldBuilder() { Name = "Apex", Value = ApexState });
            embed.WithCurrentTimestamp();

            var Message = await Context.Channel.SendMessageAsync(null, false, embed.Build());
            await Delete.DeleteMessage(Message);
        }

        [Command("create")]
        [RequireBotPermission(GuildPermission.ManageChannels)]
        public async Task CreateAsync()
        {
            await Context.Message.DeleteAsync();

            // Create globals category
            var cat = await Context.Guild.CreateCategoryAsync("Globals");

            // Create all channels
            bool GamingState = false, MusicState = false, MoviesState = false, R6State = false, LeagueState = false, RustState = false, GtaState = false, PubgState = false, FortniteState = false, ApexState = false;
            ulong GamingId = 0, MusicId = 0, MoviesId = 0, R6Id = 0, LeagueId = 0, RustId = 0, GtaId = 0, PubgId = 0, FortniteId = 0, ApexId = 0;
            ServerConfig.GetChannelSettingsAsBool(Context.Guild.Id, ref GamingState, ref MusicState, ref MoviesState, ref R6State, ref LeagueState, ref RustState, ref GtaState, ref PubgState, ref FortniteState, ref ApexState);

            if (GamingState == true)
            {
                var chan = await Context.Guild.CreateTextChannelAsync("Gaming");
                await chan.ModifyAsync(x => x.CategoryId = cat.Id);
                GamingId = chan.Id;
            }

            if (MusicState == true)
            {
                var chan = await Context.Guild.CreateTextChannelAsync("Music");
                await chan.ModifyAsync(x => x.CategoryId = cat.Id);
                MusicId = chan.Id;
            }

            if (MoviesState == true)
            {
                var chan = await Context.Guild.CreateTextChannelAsync("Movies");
                await chan.ModifyAsync(x => x.CategoryId = cat.Id);
                MoviesId = chan.Id;
            }

            if (R6State == true)
            {
                var chan = await Context.Guild.CreateTextChannelAsync("Rainbow Six");
                await chan.ModifyAsync(x => x.CategoryId = cat.Id);
                R6Id = chan.Id;
            }

            if (LeagueState == true)
            {
                var chan = await Context.Guild.CreateTextChannelAsync("League of Legends");
                await chan.ModifyAsync(x => x.CategoryId = cat.Id);
                LeagueId = chan.Id;
            }

            if (RustState == true)
            {
                var chan = await Context.Guild.CreateTextChannelAsync("Rust");
                await chan.ModifyAsync(x => x.CategoryId = cat.Id);
                RustId = chan.Id;
            }

            if (GtaState == true)
            {
                var chan = await Context.Guild.CreateTextChannelAsync("GTA");
                await chan.ModifyAsync(x => x.CategoryId = cat.Id);
                GtaId = chan.Id;
            }

            if (PubgState == true)
            {
                var chan = await Context.Guild.CreateTextChannelAsync("PUBG");
                await chan.ModifyAsync(x => x.CategoryId = cat.Id);
                PubgId = chan.Id;
            }

            if (FortniteState == true)
            {
                var chan = await Context.Guild.CreateTextChannelAsync("Fortnite");
                await chan.ModifyAsync(x => x.CategoryId = cat.Id);
                FortniteId = chan.Id;
            }

            if (ApexState == true)
            {
                var chan = await Context.Guild.CreateTextChannelAsync("Apex");
                await chan.ModifyAsync(x => x.CategoryId = cat.Id);
                ApexId = chan.Id;
            }

            await ServerConfig.SetupChannels(Context.Guild.Id, GamingId, MusicId, MoviesId, R6Id, LeagueId, RustId, GtaId, PubgId, FortniteId, ApexId);

            var Message = await Context.Channel.SendMessageAsync("Setup completed, use the command `!help` to modify your settings in the future.");
            await Delete.DeleteMessage(Message);
        }
    }
}
