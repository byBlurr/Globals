using Discord;
using Discord.Commands;
using Globals.Data;
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
            await Context.Channel.SendMessageAsync("Setup started, use the command `!enable` next.");
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
            ServerConfig.GetChannelSettings(Context.Guild.Id, ref GamingState, ref MusicState, ref MoviesState, ref R6State, ref LeagueState, ref RustState, ref GtaState, ref PubgState, ref FortniteState, ref ApexState);

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

            await Context.Channel.SendMessageAsync(null, false, embed.Build());
        }

        [Command("create")]
        [RequireBotPermission(GuildPermission.ManageChannels)]
        public async Task CreateAsync()
        {
            await Context.Message.DeleteAsync();

            // Create globals category
            // Create all channels
        }
    }
}
