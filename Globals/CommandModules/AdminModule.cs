using Data;
using Discord;
using Discord.Commands;
using Globals.Channels;
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
        [RequireUserPermission(GuildPermission.ManageChannels)]
        public async Task RegisterAsync()
        {
            await Context.Message.DeleteAsync();

            await ServerConfig.RegisterServerAsync(Context.Guild.Id);
            var Message = await Context.Channel.SendMessageAsync("Setup started, use the command `!enable` next.");
            await Delete.DeleteMessage(Message);
        }

        [Command("enable")]
        [RequireUserPermission(GuildPermission.ManageChannels)]
        public async Task EnableAsync(string channel = "", bool toggle = true)
        {
            await Context.Message.DeleteAsync();

            var dbCon = DBConnection.Instance();
            dbCon.DatabaseName = BotConfig.Load().DatabaseName;

            if (dbCon.IsConnect())
            {
                for (int i = 0; i < ChannelData.Channels.Count; i++)
                {
                    if (channel.ToLower().Equals(ChannelData.Channels[i].Id)) await ServerConfig.ToggleChannel(Context.Guild.Id, ChannelData.Channels[i].Id, toggle, dbCon);
                }

                var embed = new EmbedBuilder() { Color = new Color(114, 137, 218) };
                embed.WithDescription("Enable/Disable channels using `!enable <channel> <true/false>`, for example `!enable r6 true`.\nOnce the correct channels are set, use the command `!create`, if this is the initial setup or `!update`, if the bot is already setup on your server.");

                // TODO: Loop through the channels instead of having them hard coded.
                for (int i = 0; i < ChannelData.Channels.Count; i++)
                {
                    embed.AddField(new EmbedFieldBuilder() { Name = ChannelData.Channels[i].Id, Value = ServerConfig.GetChannelState(Context.Guild.Id, ChannelData.Channels[i].IndexToggle, dbCon).ToString() });
                }

                embed.WithCurrentTimestamp();

                var Message = await Context.Channel.SendMessageAsync(null, false, embed.Build());
                await Delete.DeleteMessage(Message);

                dbCon.Close();
            }
        }

        [Command("create")]
        [RequireUserPermission(GuildPermission.ManageChannels)]
        public async Task CreateAsync()
        {
            await Context.Message.DeleteAsync();

            // Create globals category
            var cat = await Context.Guild.CreateCategoryAsync("Globals");

            var dbCon = DBConnection.Instance();
            dbCon.DatabaseName = BotConfig.Load().DatabaseName;

            if (dbCon.IsConnect())
            {
                for (int i = 0; i < ChannelData.Channels.Count; i++)
                {
                    if (ServerConfig.GetChannelState(Context.Guild.Id, ChannelData.Channels[i].IndexToggle, dbCon) == true)
                    {
                        var chan = await Context.Guild.CreateTextChannelAsync(ChannelData.Channels[i].Name);
                        await chan.ModifyAsync(x => x.CategoryId = cat.Id);
                        await ServerConfig.SetupChannel(Context.Guild.Id, chan.Id, ChannelData.Channels[i].Id, dbCon);
                    }
                }

                dbCon.Close();
            }
            
            var Message = await Context.Channel.SendMessageAsync("Setup completed, use the `!update` command again to disable/enable channels in the future, once you disable or enable servers, use the `!update` command.");
            await Delete.DeleteMessage(Message);
        }

        [Command("update")]
        [RequireUserPermission(GuildPermission.ManageChannels)]
        public async Task UpdateAsync()
        {
            await Context.Message.DeleteAsync();

            var cats = Context.Guild.GetCategoriesAsync().Result;
            ICategoryChannel cat = null;
            foreach (var category in cats)
            {
                if (category.Name.ToLower().Equals("globals"))
                {
                    cat = category;
                }
            }

            if (cat != null)
            {
                var dbCon = DBConnection.Instance();
                dbCon.DatabaseName = BotConfig.Load().DatabaseName;

                if (dbCon.IsConnect())
                {
                    for (int i = 0; i < ChannelData.Channels.Count; i++)
                    {
                        var chanId = ServerConfig.GetChannelId(Context.Guild.Id, ChannelData.Channels[i].IndexId, dbCon);
                        if (ServerConfig.GetChannelState(Context.Guild.Id, ChannelData.Channels[i].IndexToggle, dbCon) == false && chanId != 0)
                        {
                            var chan = await Context.Guild.GetChannelAsync(chanId);
                            if (chan != null)
                            {
                                await chan.DeleteAsync();
                                await ServerConfig.SetupChannel(Context.Guild.Id, 0, ChannelData.Channels[i].Id, dbCon);
                            }
                        }
                        else if (ServerConfig.GetChannelState(Context.Guild.Id, ChannelData.Channels[i].IndexToggle, dbCon) == true && chanId == 0)
                        {
                            var chan = await Context.Guild.CreateTextChannelAsync(ChannelData.Channels[i].Name);
                            await chan.ModifyAsync(x => x.CategoryId = cat.Id);
                            await ServerConfig.SetupChannel(Context.Guild.Id, chan.Id, ChannelData.Channels[i].Id, dbCon);
                        }
                    }

                    dbCon.Close();
                }

                var message = await Context.Channel.SendMessageAsync("Your global channels should now be updated. Please use the `!request` command in a global channel, if you have any issues.");
                await Delete.DeleteMessage(message);
            }
            else
            {
                var message = await Context.Channel.SendMessageAsync("We couldn't find the globals category in your server, I suggest deleting all the global channels and category. Then run the `!setup` command again.");
                await Delete.DeleteMessage(message);
            }
        }
    }
}
