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
        [Command("edit")]
        public async Task EditLastAsync([Remainder] string replacement = "")
        {
            var dbCon = DBConnection.Instance();
            dbCon.DatabaseName = BotConfig.Load().DatabaseName;

            if (dbCon.IsConnect())
            {
                string ChannelInUse = await Message.GetGlobalChannelInUseAsync(Context, dbCon);

                if (!ChannelInUse.Equals(""))
                {
                    if (!replacement.Equals("") && !ProfanityFilter.HasProfanity(replacement))
                    {
                        await Context.Message.DeleteAsync();

                        var messages = Message.GetMessageByUserAsync(Context.User, dbCon, ChannelInUse).Result;

                        var remove = Task.Run(async () =>
                        {
                            foreach (var message in messages)
                            {
                                if (message != null)
                                {
                                    foreach (var embed in message.Embeds)
                                    {
                                        await (message as IUserMessage).ModifyAsync(x => x.Embed = embed.ToEmbedBuilder().WithDescription(replacement + " _- edited._").Build());
                                        await Task.Delay(1100);
                                    }
                                }
                            }
                        });
                    }
                    else
                    {
                        var message = await Context.Channel.SendMessageAsync("Correct format of this command is `!edit <new text>`, for example `!edit Pokemon GO is my jam on toast.`.");
                        await Delete.DeleteMessage(message);
                    }
                }
                dbCon.Close();
            }
        }

        [Command("delete")]
        [Alias("del")]
        public async Task DeleteLastAsync()
        {
            var dbCon = DBConnection.Instance();
            dbCon.DatabaseName = BotConfig.Load().DatabaseName;

            if (dbCon.IsConnect())
            {
                string ChannelInUse = await Message.GetGlobalChannelInUseAsync(Context, dbCon);

                if (!ChannelInUse.Equals(""))
                {
                    await Context.Message.DeleteAsync();

                    var messages = Message.GetMessageByUserAsync(Context.User, dbCon, ChannelInUse).Result;

                    var remove = Task.Run(async () =>
                    {
                        foreach (var message in messages)
                        {
                            if (message != null)
                            {
                                foreach (var embed in message.Embeds)
                                {
                                    await (message as IUserMessage).ModifyAsync(x => x.Embed = embed.ToEmbedBuilder().WithDescription("").WithFooter("Removed by user at " + DateTime.UtcNow.ToString()).Build());
                                    await Task.Delay(1100);
                                }
                            }
                        }
                    });
                }
                dbCon.Close();
            }
        }

        [Command("request")]
        public async Task RequestAsync([Remainder] string Request = "")
        {
            var dbCon = DBConnection.Instance();
            dbCon.DatabaseName = BotConfig.Load().DatabaseName;

            if (dbCon.IsConnect())
            {
                string ChannelInUse = await Message.GetGlobalChannelInUseAsync(Context, dbCon);

                if (!ChannelInUse.Equals(""))
                {
                    if (!Request.Equals(""))
                    {
                        await Context.Message.DeleteAsync();

                        var Channel = CommandHandler.GetBot().GetGuild(References.GlobalsServerId).GetChannel(References.RequestChannelId) as IMessageChannel;

                        var embed = new EmbedBuilder() { Color = new Color(114, 137, 218) };
                        embed.WithAuthor(Context.User.Username + "#" + Context.User.DiscriminatorValue + " from " + Context.Guild.Name, Context.User.GetAvatarUrl());
                        embed.WithDescription(Request);
                        embed.WithCurrentTimestamp();
                        await Channel.SendMessageAsync("", false, embed.Build());

                        var message = await Context.Channel.SendMessageAsync("Thank you for your suggestion, out team will now look into it and get back to you!");
                        await Delete.DeleteMessage(message);
                    }
                }
                else
                {
                    var message = await Context.Channel.SendMessageAsync("Correct format of this command is `!request <idea>`, for example `!request Pokemon GO global channel`.");
                    await Delete.DeleteMessage(message);
                }
                dbCon.Close();
            }
        }

        [Command("profile")]
        public async Task ProfileIdAsync(int ID = -1)
        {
            if (ID != -1)
            {
                var dbCon = DBConnection.Instance();
                dbCon.DatabaseName = BotConfig.Load().DatabaseName;
                if (dbCon.IsConnect())
                {
                    string ChannelInUse = await Message.GetGlobalChannelInUseAsync(Context, dbCon);

                    if (!ChannelInUse.Equals(""))
                    {
                        await Context.Message.DeleteAsync();

                        ulong userid = await UserProfile.GetUserIdFromGId(ID, dbCon);
                        int scount = await UserProfile.GetServerCountAsync(userid, dbCon);
                        int mcount = await UserProfile.GetMessageCountAsync(userid, dbCon);
                        int wcount = await UserProfile.GetWarningCountAsync(userid, dbCon);
                        string group = await UserProfile.GetGroupAsync(userid, dbCon);

                        var embed = new EmbedBuilder() { Color = new Color(114, 137, 218) };
                        embed.WithAuthor(Context.User.Username + " from " + Context.Guild.Name, Context.User.GetAvatarUrl());
                        embed.WithDescription(userid + "'s Global Profile");
                        embed.AddField(new EmbedFieldBuilder() { Name = "User ID", Value = userid, IsInline = true });
                        //embed.AddField(new EmbedFieldBuilder() { Name = "Servers", Value = scount + " servers", IsInline = true });
                        //embed.AddField(new EmbedFieldBuilder() { Name = "Messages", Value = mcount + " messages", IsInline = true });
                        embed.AddField(new EmbedFieldBuilder() { Name = "Warnings", Value = wcount + "/3", IsInline = true });
                        embed.AddField(new EmbedFieldBuilder() { Name = "Group", Value = group, IsInline = true });
                        embed.WithCurrentTimestamp();

                        string query = "SELECT * FROM server_configs;";
                        var cmd = new MySqlCommand(query, dbCon.Connection);
                        var reader = await cmd.ExecuteReaderAsync();
                        while (await reader.ReadAsync())
                        {
                            await Message.PostToChannelAsync(ChannelInUse, reader, embed);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Couldn't find the channel or not in a global channel?");
                    }
                    dbCon.Close();
                }
            }
            else
            {
                var message = await Context.Channel.SendMessageAsync("Correct format of this command is `!profile <user>`, for example using mention `!profile @Blurr#3760` or using GId `!profile 2`.\nIf the user is not in the same server as you, you must use the globals id (GId).");
                await Delete.DeleteMessage(message);
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
                        //embed.AddField(new EmbedFieldBuilder() { Name = "Servers", Value = scount + " servers", IsInline = true });
                        //embed.AddField(new EmbedFieldBuilder() { Name = "Messages", Value = mcount + " messages", IsInline = true });
                        embed.AddField(new EmbedFieldBuilder() { Name = "Warnings", Value = wcount + "/3", IsInline = true });
                        embed.AddField(new EmbedFieldBuilder() { Name = "Group", Value = group, IsInline = true });
                        embed.WithThumbnailUrl(User.GetAvatarUrl());
                        embed.WithCurrentTimestamp();

                        string query = "SELECT * FROM server_configs;";
                        var cmd = new MySqlCommand(query, dbCon.Connection);
                        var reader = await cmd.ExecuteReaderAsync();
                        while (await reader.ReadAsync())
                        {
                            await Message.PostToChannelAsync(ChannelInUse, reader, embed);
                        }
                    }
                    dbCon.Close();
                }
            }
            else
            {
                var message = await Context.Channel.SendMessageAsync("Correct format of this command is `!profile <user>`, for example using mention `!profile @Blurr#3760` or using GId `!profile 2`.\nIf the user is not in the same server as you, you must use the globals id (GId).");
                await Delete.DeleteMessage(message);
            }
        }
    }
}
