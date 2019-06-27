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
        [Alias("?")]
        public async Task AboutAsync()
        {
            await Context.Message.DeleteAsync();
            var embed = new EmbedBuilder() { Color = new Color(114, 137, 218) };
            embed.WithTitle("About Globals");
            embed.WithDescription("Global Bot aims to bring different communities closer. Global chats are a good way to find new team mates, people to play against or good to chat with new people.");
            embed.WithUrl("https://discord.gg/kgjZaNt");
            embed.WithImageUrl("https://cdn.discordapp.com/attachments/587411637363802135/593832015065776138/0.jpg");
            await Context.Channel.SendMessageAsync("", false, embed.Build());
        }

        [Command("rules")]
        public async Task RulesAsync()
        {
            await Context.Message.DeleteAsync();

            var embed = new EmbedBuilder() { Color = new Color(114, 137, 218) };
            embed.WithTitle("Global Chat Rules");
            embed.WithDescription("- No racial or sexual slurs, these channels are NOT marked NSFW.\n" +
                "- No bullying of any kind.\n" +
                "- No advertising of any kind.\n" +
                "- No spamming or flooding.\n\n" +
                "Breaking any of the above rules will result in a warning. 3 warnings will automatically blacklist you from global channels. A moderator can choose to blacklist you before 3 warnings however. If you get blacklisted, you will have to appeal your blacklist by messaging a moderator.");
            embed.WithUrl("https://discord.gg/kgjZaNt");
            embed.WithImageUrl("https://cdn.discordapp.com/attachments/587411637363802135/593832015065776138/0.jpg");
            await Context.Channel.SendMessageAsync("", false, embed.Build());
        }

        [Command("help")]
        public async Task HelpAsync()
        {
            await Context.Message.DeleteAsync();

            var embed = new EmbedBuilder() { Color = new Color(114, 137, 218) };
            embed.WithTitle("Globals Help");
            embed.WithDescription("**Admin Commands**\nCommands that the server admin can do.\n" +
                "-    `!setup` - Start the setup process and add your server/guild to the database.\n" +
                "-    `!enable <channel> <true|false>` - Enable or Disable global channels on your server.\n" +
                "-    `!create` - Create the Globals category and channels in your server. Will only create the channels you have enabled.\n" +
                "-    `!update` - Will delete and create the added or removed channels, make sure you do this everytime you add or remove channels.\n" +
                "\n**Global Commands**\nCommands that are only available in the global channels.\n" +
                "-    `!profile <user>` - View the profile of another user. User @user if they're in the same server as you or their globals id (GId) if not. GId is in the footer of their message.\n" +
                "-    `!request <feature>` - Suggest new features for the global chat or Discord server, remember to be descriptive! We may message you for more information.\n" +
                "\n**Other Commands**\nOther commands for you to try out\n" +
                "-    `!help` - Brings up this message derp...\n" +
                "-    `!rules` - Displays the global chat rules.\n" +
                "-    `!developer` - Will give you information on the developers of Globals.\n" +
                "-    `!about` - Will tell you a little about Globals.\n" +
                "-    `!donate` - Will give you information on how you can support Globals!\n" +
                "\n**Moderation Commands**\nCommands for Global Moderators\n" +
                "-    `!war...` - Wait you don't need to know these! You will be told these, if you become a global moderator!\n\n" +
                "\n**More Information**\n" +
                "For more information or help, head over to the Globals server, click the title to this embed.\n");
            embed.WithUrl("https://discord.gg/kgjZaNt");
            embed.WithImageUrl("https://cdn.discordapp.com/attachments/587411637363802135/593832015065776138/0.jpg");
            await Context.Channel.SendMessageAsync("", false, embed.Build());
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
            embed.WithTitle("Support Globals");
            embed.WithDescription("Donating to Globals will help us host, develop and manage the Discord bot. A huge thank you to anyone who donates, we appreciate every penny. We have big plans for Globals and are looking to expand further. Click the title of this embed to go to the donation page!\n\n" +
                "You could also support us by heading over to the official Globals server and boosting the server!");
            embed.WithUrl("https://paypal.me/pools/c/8fZ8flHgt5");
            embed.WithImageUrl("https://cdn.discordapp.com/attachments/587411637363802135/593832015065776138/0.jpg");
            await Context.Channel.SendMessageAsync("", false, embed.Build());
        }
    }
}
