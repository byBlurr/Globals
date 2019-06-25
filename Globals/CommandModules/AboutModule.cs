using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Globals.CommandModules
{
    public class AboutModule : ModuleBase
    {
        [Command("about")]
        public async Task AboutAsync()
        {
            await Context.Message.DeleteAsync();
            var embed = new EmbedBuilder() { Color = new Color(114, 137, 218) };
            embed.WithTitle("About Globals");
            embed.WithDescription("Global Bot aims to bring different communities closer. Global chats are a good way to find new team mates, people to play against or good to chat with new people.");
            embed.WithUrl("https://discord.gg/kgjZaNt");
            embed.WithImageUrl("https://cdn.discordapp.com/attachments/591710482696634369/593207970234499104/0.jpg");
            embed.WithAuthor("", Context.Guild.IconUrl);
            await Context.Channel.SendMessageAsync("", false, embed.Build());
        }

        [Command("help")]
        public async Task HelpAsync()
        {
            await Context.Message.DeleteAsync();
            await Context.Channel.SendMessageAsync("Coming soon.");
        }

        [Command("developer")]
        [Alias("dev")]
        public async Task DeveloperAsync()
        {
            await Context.Message.DeleteAsync();
            var embed = new EmbedBuilder() { Color = new Color(114, 137, 218) };
            embed.WithTitle("Globals Developer");
            embed.WithDescription("Global Bot was developed by Blurr Development and VerdillianStudios, for Discord Hack Week 2019. Development of Global Bot started on the 24th June 2019.");
            embed.WithUrl("https://github.com/byBlurr");
            embed.WithThumbnailUrl("https://avatars2.githubusercontent.com/u/20552533?s=460&v=4");
            await Context.Channel.SendMessageAsync("", false, embed.Build());
        }

        [Command("donate")]
        [Alias("fund", "support")]
        public async Task DonateAsync()
        {
            await Context.Message.DeleteAsync();
            var embed = new EmbedBuilder() { Color = new Color(114, 137, 218) };
            embed.WithTitle("Donate to Globals");
            embed.WithDescription("A cool way to donate to the developers will be here eventually.");
            embed.WithUrl("https://discord.gg/kgjZaNt");
            embed.WithImageUrl("https://cdn.discordapp.com/attachments/591710482696634369/593207970234499104/0.jpg");
            await Context.Channel.SendMessageAsync("", false, embed.Build());
        }
    }
}
