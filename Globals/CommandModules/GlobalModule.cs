using Data;
using Discord;
using Discord.Commands;
using Globals.Global;
using Globals.Util;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Globals.CommandModules
{
    public class GlobalModule : ModuleBase
    {
        [Command("request")]
        public async Task RequestAsync([Remainder] string Request = "")
        {
            if (!Request.Equals(""))
            {
                var dbCon = DBConnection.Instance();
                dbCon.DatabaseName = BotConfig.Load().DatabaseName;
                if (dbCon.IsConnect())
                {
                    if (!Message.GetGlobalChannelInUseAsync(Context, dbCon).Equals(""))
                    {
                        var Channel = CommandHandler.GetBot().GetGuild(References.GlobalsServerId).GetChannel(References.RequestChannelId) as IMessageChannel;

                        var embed = new EmbedBuilder() { Color = new Color(114, 137, 218) };
                        embed.WithAuthor(Context.User.Username + "#" + Context.User.DiscriminatorValue + " from " + Context.Guild.Name, Context.User.GetAvatarUrl());
                        embed.WithDescription(Request);
                        embed.WithCurrentTimestamp();
                        await Channel.SendMessageAsync("", false, embed.Build());
                    }
                    dbCon.Close();
                }
            }
        }
    }
}
