using Data;
using Discord;
using Discord.Commands;
using Globals.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Globals.CommandModules
{
    public class GlobalModeratorModule : ModuleBase
    {
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
                        await UserProfile.AddWarningAsync(User, dbCon);
                        await Context.User.SendMessageAsync("You warned " + User.Username + ".");
                    }
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
                        await UserProfile.AddWarningAsync(User, dbCon, true);
                        await Context.User.SendMessageAsync("You blacklisted " + User.Username + ".");
                    }
                }
            }
        }
    }
}
