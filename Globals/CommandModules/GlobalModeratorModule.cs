using Data;
using Discord;
using Discord.Commands;
using Globals.Channels;
using Globals.Data;
using Globals.Global;
using Globals.Util;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Globals.CommandModules
{
    public class GlobalModeratorModule : ModuleBase
    {
        [Command("announce")]
        public async Task AnnounceAsync([Remainder] string Announcement = null)
        {
            if (Announcement != null && Announcement.Length > 0)
            {
                var dbCon = DBConnection.Instance();
                dbCon.DatabaseName = BotConfig.Load().DatabaseName;
                if (dbCon.IsConnect())
                {
                    if (UserProfile.CanAdministrate(Context.User.Id, dbCon))
                    {
                        await Context.Message.DeleteAsync();

                        GlobalChannel GeneralChannel = ChannelData.FindChannelById("general");
                        if (GeneralChannel != null)
                        {
                            var embed = new EmbedBuilder() { Color = new Color() };
                            embed.WithTitle("Global Announcement");
                            embed.WithDescription(Announcement);
                            embed.WithFooter("Global announcement by " + Context.User.Username + " at " + DateTime.UtcNow.ToShortTimeString());
                            await Message.PostGlobalAnnouncementAsync(embed, GeneralChannel, dbCon);
                        }
                        else
                        {
                            var Message = await Context.Channel.SendMessageAsync("Announcement failed. General chat could not be found?");
                            await Delete.DeleteMessage(Message);
                        }
                    }
                    dbCon.Close();
                }
            }
        }

        [Command("warn")]
        public async Task WarnUserAsync(IUser User = null)
        {
            if (User != null)
            {
                var dbCon = DBConnection.Instance();
                dbCon.DatabaseName = BotConfig.Load().DatabaseName;
                if (dbCon.IsConnect())
                {
                    if (UserProfile.CanModerate(Context.User.Id, dbCon))
                    {
                        await Context.Message.DeleteAsync();
                        await Message.DeleteAsync(User, dbCon);
                        await UserProfile.AddWarningAsync(User, dbCon);
                        await Context.User.SendMessageAsync("You warned " + User.Username + ".");
                    }
                    dbCon.Close();
                }
            }
        }

        [Command("ban")]
        [Alias("bl", "blacklist")]
        public async Task BanUserAsync(IUser User = null)
        {
            if (User != null)
            {
                var dbCon = DBConnection.Instance();
                dbCon.DatabaseName = BotConfig.Load().DatabaseName;
                if (dbCon.IsConnect())
                {
                    if (UserProfile.CanModerate(Context.User.Id, dbCon))
                    {
                        await Context.Message.DeleteAsync();
                        await Message.DeleteAsync(User, dbCon);
                        await UserProfile.AddWarningAsync(User, dbCon, true);
                        await Context.User.SendMessageAsync("You blacklisted " + User.Username + ".");
                    }
                    dbCon.Close();
                }
            }
        }

        [Command("unban")]
        [Alias("unbl", "unblacklist")]
        public async Task UnBanUserAsync(IUser User = null)
        {
            if (User != null)
            {
                var dbCon = DBConnection.Instance();
                dbCon.DatabaseName = BotConfig.Load().DatabaseName;
                if (dbCon.IsConnect())
                {
                    if (UserProfile.CanModerate(Context.User.Id, dbCon))
                    {
                        await Context.Message.DeleteAsync();
                        await UserProfile.UnBanUserAsync(User, dbCon);
                        await Context.User.SendMessageAsync("You unblacklisted " + User.Username + ".");
                    }
                    dbCon.Close();
                }
            }
        }

        [Command("removewarning")]
        [Alias("rw", "remwarn")]
        public async Task RemoveWarningAsync(IUser User = null, int warnAmt = 0)
        {
            if (User != null)
            {
                var dbCon = DBConnection.Instance();
                dbCon.DatabaseName = BotConfig.Load().DatabaseName;
                if (dbCon.IsConnect())
                {
                    if (UserProfile.CanModerate(Context.User.Id, dbCon))
                    {
                        await Context.Message.DeleteAsync();
                        await UserProfile.RemoveWarningAsync(User, dbCon, warnAmt);
                        await Context.User.SendMessageAsync("You removed " + warnAmt.ToString() + " warning(s) from " + User.Username + ".");
                    }
                    dbCon.Close();
                }
            }
        }

    }
}
