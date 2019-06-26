using Data;
using Discord;
using Discord.Commands;
using Globals.Data;
using Globals.Global;
using Globals.Util;
using MySql.Data.MySqlClient;
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
                    string ChannelInUse = await Message.GetGlobalChannelInUseAsync(Context, dbCon);

                    if (!ChannelInUse.Equals(""))
                    {
                        await Context.Message.DeleteAsync();

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

        [Command("profile")]
        public async Task ProfileIdAsync(ulong ID = 0)
        {
            if (ID != 0)
            {
                var dbCon = DBConnection.Instance();
                dbCon.DatabaseName = BotConfig.Load().DatabaseName;
                if (dbCon.IsConnect())
                {
                    string ChannelInUse = await Message.GetGlobalChannelInUseAsync(Context, dbCon);

                    if (!ChannelInUse.Equals(""))
                    {
                        await Context.Message.DeleteAsync();

                        ulong userid = ID;
                        int scount = await UserProfile.GetServerCountAsync(userid, dbCon);
                        int mcount = await UserProfile.GetMessageCountAsync(userid, dbCon);
                        int wcount = await UserProfile.GetWarningCountAsync(userid, dbCon);
                        string group = await UserProfile.GetGroupAsync(userid, dbCon);

                        var embed = new EmbedBuilder() { Color = new Color(114, 137, 218) };
                        embed.WithAuthor(Context.User.Username + " from " + Context.Guild.Name, Context.User.GetAvatarUrl());
                        embed.WithDescription(ID + "'s Global Profile");
                        embed.AddField(new EmbedFieldBuilder() { Name = "User ID", Value = userid, IsInline = true });
                        embed.AddField(new EmbedFieldBuilder() { Name = "Servers", Value = scount + " servers", IsInline = true });
                        embed.AddField(new EmbedFieldBuilder() { Name = "Messages", Value = mcount + " messages", IsInline = true });
                        embed.AddField(new EmbedFieldBuilder() { Name = "Warnings", Value = wcount + "/3", IsInline = true });
                        embed.AddField(new EmbedFieldBuilder() { Name = "Group", Value = group, IsInline = true });
                        embed.WithCurrentTimestamp();

                        string query = "SELECT * FROM server_configs;";
                        var cmd = new MySqlCommand(query, dbCon.Connection);
                        var reader = await cmd.ExecuteReaderAsync();
                        while (await reader.ReadAsync())
                        {
                            await Message.PostMessageAsync(ChannelInUse, reader, embed);
                        }
                    }
                    dbCon.Close();
                }
            }
        }

        [Command("profile")]
        public async Task ProfileAsync(IUser User = null)
        {
            if (User != null)
            {
                var dbCon = DBConnection.Instance();
                dbCon.DatabaseName = BotConfig.Load().DatabaseName;
                if (dbCon.IsConnect())
                {
                    string ChannelInUse = await Message.GetGlobalChannelInUseAsync(Context, dbCon);

                    if (!ChannelInUse.Equals(""))
                    {
                        await Context.Message.DeleteAsync();

                        ulong userid = User.Id;
                        int scount = await UserProfile.GetServerCountAsync(userid, dbCon);
                        int mcount = await UserProfile.GetMessageCountAsync(userid, dbCon);
                        int wcount = await UserProfile.GetWarningCountAsync(userid, dbCon);
                        string group = await UserProfile.GetGroupAsync(userid, dbCon);

                        var embed = new EmbedBuilder() { Color = new Color(114, 137, 218) };
                        embed.WithAuthor(Context.User.Username + " from " + Context.Guild.Name, Context.User.GetAvatarUrl());
                        embed.WithDescription(User.Username + "#" + User.DiscriminatorValue + "'s Global Profile");
                        embed.AddField(new EmbedFieldBuilder() { Name = "User ID", Value = userid, IsInline = true });
                        embed.AddField(new EmbedFieldBuilder() { Name = "Servers", Value = scount + " servers", IsInline = true });
                        embed.AddField(new EmbedFieldBuilder() { Name = "Messages", Value = mcount + " messages", IsInline = true });
                        embed.AddField(new EmbedFieldBuilder() { Name = "Warnings", Value = wcount + "/3", IsInline = true });
                        embed.AddField(new EmbedFieldBuilder() { Name = "Group", Value = group, IsInline = true });
                        embed.WithThumbnailUrl(User.GetAvatarUrl());
                        embed.WithCurrentTimestamp();

                        string query = "SELECT * FROM server_configs;";
                        var cmd = new MySqlCommand(query, dbCon.Connection);
                        var reader = await cmd.ExecuteReaderAsync();
                        while (await reader.ReadAsync())
                        {
                            await Message.PostMessageAsync(ChannelInUse, reader, embed);
                        }
                    }
                    dbCon.Close();
                }
            }
        }
    }
}
