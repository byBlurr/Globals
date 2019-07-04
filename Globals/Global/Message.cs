using Data;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Globals.Channels;
using Globals.Data;
using Globals.Util;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace Globals.Global
{
    class Message
    {
        public static async Task PostGlobalMessageAsync(SocketCommandContext Context)
        {
            ulong user_id = Context.User.Id;
            string user_name = Context.User.Username;
            string user_rank = "";
            string user_server = Context.Guild.Name;
            string user_image = Context.User.GetAvatarUrl();

            string message_text = Context.Message.Content;
            IReadOnlyCollection<SocketUser> message_mentions = Context.Message.MentionedUsers;
            string message_channel = "";
            string message_footer =  Context.Message.Timestamp.ToString();

            IReadOnlyCollection<Attachment> message_attachments = Context.Message.Attachments;
            List<string> message_images = new List<string>();
            foreach (Attachment attachment in message_attachments)
            {
                if (attachment.Filename.EndsWith(".png") || attachment.Filename.EndsWith(".jpg") || attachment.Filename.EndsWith(".jpeg") || attachment.Filename.EndsWith(".gif"))
                {
                    message_images.Add(Image.SaveImage(attachment.Filename, attachment.Url));
                    Console.WriteLine(Image.SaveImage(attachment.Filename, attachment.Url));
                }
            }

            if (message_text.Length == 0) message_text = "_Find attachment above._";

            string globals_id = "";

            var dbCon = DBConnection.Instance();
            dbCon.DatabaseName = BotConfig.Load().DatabaseName;
            if (dbCon.IsConnect())
            {
                message_channel = await GetGlobalChannelInUseAsync(Context, dbCon);

                if (!message_channel.Equals(""))
                {
                    // Check the user exists
                    await UserProfile.CheckUserAsync(Context.User.Id, dbCon);

                    // Check user rank
                    user_rank = await UserProfile.GetGroupAsync(Context.User.Id, dbCon);

                    // Get their globals id
                    globals_id = await UserProfile.GetGlobalsIdAsync(Context.User.Id, dbCon);

                    // Save the message in the db
                    string query = "INSERT INTO global_messages (user_id, user_name, user_server, user_image, message_text, message_channel, message_footer) " +
                        "VALUES(@user_id, @user_name, @user_server, @user_image, @message_text, @message_channel, @message_footer);";
                    var cmd = new MySqlCommand(query, dbCon.Connection);
                    cmd.Parameters.Add("@user_id", MySqlDbType.UInt64).Value = user_id;
                    cmd.Parameters.Add("@user_name", MySqlDbType.String).Value = user_name;
                    cmd.Parameters.Add("@user_server", MySqlDbType.String).Value = user_server;
                    cmd.Parameters.Add("@user_image", MySqlDbType.String).Value = user_image;
                    cmd.Parameters.Add("@message_text", MySqlDbType.String).Value = message_text;
                    cmd.Parameters.Add("@message_channel", MySqlDbType.String).Value = message_channel;
                    cmd.Parameters.Add("@message_footer", MySqlDbType.String).Value = globals_id + " | " + message_footer;

                    try
                    {
                        await cmd.ExecuteNonQueryAsync();
                        await Context.Message.DeleteAsync();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }

                // Post the messages everywhere
                if (!message_channel.Equals(""))
                {
                    var embed = new EmbedBuilder() { Color = new Color(114, 137, 218) };
                    embed.WithAuthor(user_name + " from " + user_server, user_image);
                    embed.WithDescription(message_text);
                    embed.WithFooter(user_rank + " - " + globals_id + " | " + message_footer);

                    string query = "SELECT * FROM server_configs;";
                    var cmd = new MySqlCommand(query, dbCon.Connection);
                    var reader = await cmd.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {
                        if (!user_rank.ToLower().Equals("blacklisted"))
                        {
                            await PostToChannelAsync(message_channel, reader, embed, message_images, message_mentions);
                        }
                    }

                    reader.Close();
                    cmd.Dispose();
                }

                dbCon.Close();
            }
            else Console.WriteLine("Couldnt connect...");

        }

        public static async Task DeleteAsync(IUser User, DBConnection dbCon)
        {
            List<IMessage> MessagesToRemove = await GetMessagesByUserAsync(User, dbCon);

            var remove = Task.Run(async () =>
            {
                if (MessagesToRemove.Count > 0)
                {
                    for (int i = 0; i <= MessagesToRemove.Count; i++)
                    {
                        if (MessagesToRemove[i] != null)
                        {
                            foreach (IEmbed embed in MessagesToRemove[i].Embeds)
                            {
                                if (embed != null)
                                {
                                    /*if (MessagesToRemove[i].Attachments.Count > 0)
                                    {
                                        Console.WriteLine("DOES THIS EVEN WORK BRUV?!?");
                                        var chan = MessagesToRemove[i].Channel;

                                        await MessagesToRemove[i].DeleteAsync();
                                        var emb = embed.ToEmbedBuilder().WithDescription("").WithFooter("Removed by moderator at " + DateTime.UtcNow.ToString()).Build();
                                        await chan.SendMessageAsync(null, false, emb);
                                    }
                                    else
                                    {*/
                                        var emb = embed.ToEmbedBuilder().WithDescription("").WithFooter("Removed by moderator at " + DateTime.UtcNow.ToString()).Build();
                                        await (MessagesToRemove[i] as IUserMessage).ModifyAsync(x => x.Embed = emb);
                                    //}

                                    await Task.Delay(1100);
                                }
                                else
                                {
                                    Console.WriteLine("What happened?");
                                }
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Looks like theres nothing to remove?");
                }
            });
        }

        public static async Task<string> GetGlobalChannelInUseAsync(ICommandContext Context, DBConnection dbCon)
        {
            string message_channel = "";
            string query = "SELECT * FROM server_configs WHERE server_id = @serverid";
            var cmd = new MySqlCommand(query, dbCon.Connection);
            cmd.Parameters.Add("@serverid", MySqlDbType.UInt64).Value = Context.Guild.Id;

            DbDataReader reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                if (message_channel.Equals("")) CheckChannel(ref message_channel, Context, reader);
            }

            cmd.Dispose();
            reader.Close();
            return message_channel;
        }

        public static async Task PostToChannelAsync(string message_channel, DbDataReader reader, EmbedBuilder embed, List<string> message_images = null, IReadOnlyCollection<SocketUser> message_mentions = null)
        {

            for (int i = 0; i < ChannelData.Channels.Count; i++)
            {
                if(message_channel.Equals(ChannelData.Channels[i].Id))
                {
                    var Enabled = reader.GetInt32(ChannelData.Channels[i].IndexToggle);
                    var Id = (ulong)reader.GetInt64(ChannelData.Channels[i].IndexId);

                    if (Enabled == 1)
                    {
                        await PostMessageAsync(message_channel, reader, embed, message_images, message_mentions, Id);
                    }
                }
            }
        }

        private static async Task PostMessageAsync(string message_channel, DbDataReader reader, EmbedBuilder embed, List<string> message_images, IReadOnlyCollection<SocketUser> message_mentions, ulong Id)
        {
            var guild = CommandHandler.GetBot().GetGuild((ulong)reader.GetInt64(1));
            if (guild != null)
            {
                var channel = guild.GetChannel(Id);
                if (channel != null)
                {
                    var send = Task.Run(async () =>
                    {
                        if (message_images != null)
                        {
                            if (message_images.Count > 0)
                            {
                                foreach (string attachment in message_images)
                                {
                                    var img = await (channel as IMessageChannel).SendFileAsync(attachment, null, false, embed.Build());
                                    Image.DeleteImage(attachment);
                                }
                            }
                            else
                            {
                                await SendMessageAsync(guild, embed, message_mentions, channel);
                            }
                        }
                        else
                        {
                            await SendMessageAsync(guild, embed, message_mentions, channel);
                        }
                        await Task.Delay(10);
                    });
                }
                else
                {
                    var message = await guild.Owner.SendMessageAsync("It appears there is an issue with finding the '" + message_channel + "` channel on your server. Try deleting the servers and then running the create command again.");
                    await Delete.DeleteMessage(message);
                }
            }
        }

        private static async Task SendMessageAsync(SocketGuild guild, EmbedBuilder embed, IReadOnlyCollection<SocketUser> message_mentions, SocketGuildChannel channel)
        {
            IUserMessage message = await (channel as IMessageChannel).SendMessageAsync(null, false, embed.Build());
            if (message_mentions.Count > 0)
            {
                var sendMentions = Task.Run(async () =>
                {
                    string mentions = "";
                    foreach (var user in message_mentions)
                    {
                        if (guild.GetUser(user.Id) != null)
                        {
                            mentions = mentions + " " + user.Mention;
                        }
                    }
                    if (mentions.Length > 0)
                    {
                        var mention = await (channel as IMessageChannel).SendMessageAsync(mentions);
                        await Task.Delay(100);
                        await mention.DeleteAsync();
                    }
                });
            }
        }

        private static void CheckChannel(ref string message_channel, ICommandContext Context, DbDataReader reader)
        {
            for (int i = 0; i < ChannelData.Channels.Count; i++)
            {
                var Enabled = reader.GetInt32(ChannelData.Channels[i].IndexToggle);
                var Id = (ulong) reader.GetInt64(ChannelData.Channels[i].IndexId);

                if (Enabled == 1)
                {
                    if (Id == Context.Channel.Id)
                    {
                        message_channel = ChannelData.Channels[i].Id;
                    }
                }
            }
        }

        public static async Task<List<IMessage>> GetMessagesByUserAsync(IUser User, DBConnection dbCon, int amount = 10)
        {
            string GId = await UserProfile.GetGlobalsIdAsync(User.Id, dbCon);

            List<IMessage> MessagesByUser = new List<IMessage>();
            string query = "SELECT * FROM server_configs;";
            var cmd = new MySqlCommand(query, dbCon.Connection);
            var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                for (int i = 0; i < ChannelData.Channels.Count; i++)
                {
                    if (reader.GetInt32(ChannelData.Channels[i].IndexToggle) == 1)
                    {
                        var Channel = CommandHandler.GetBot().GetGuild((ulong)reader.GetInt64(1)).GetTextChannel((ulong)reader.GetInt64(ChannelData.Channels[i].IndexId));
                        if (Channel != null)
                        {
                            var Messages = await Channel.GetMessagesAsync(amount).FlattenAsync();
                            AddMessagesToListAsync(Messages, GId, ref MessagesByUser);
                        }
                    }
                }
            }
            reader.Close();
            cmd.Dispose();
            return MessagesByUser;
        }

        public static async Task<List<IMessage>> GetMessageByUserAsync(IUser User, DBConnection dbCon, string channel)
        {
            List<IMessage> MessagesByUser = new List<IMessage>();
            string GId = await UserProfile.GetGlobalsIdAsync(User.Id, dbCon);

            string query = "SELECT * FROM server_configs;";
            var cmd = new MySqlCommand(query, dbCon.Connection);
            var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                for (int i = 0; i < ChannelData.Channels.Count; i++)
                {
                    if (channel == ChannelData.Channels[i].Id)
                    {
                        var Channel = CommandHandler.GetBot().GetGuild((ulong)reader.GetInt64(1)).GetTextChannel((ulong)reader.GetInt64(ChannelData.Channels[i].IndexId));
                        if (Channel != null)
                        {
                            var Messages = await Channel.GetMessagesAsync(10).FlattenAsync();
                            AddMessageToListAsync(Messages, GId, ref MessagesByUser);
                        }
                    }
                }
            }

            return MessagesByUser;
        }

        private static void AddMessagesToListAsync(IEnumerable<IMessage> Messages, string GId, ref List<IMessage> MessagesByUser)
        {
            foreach (IMessage message in Messages)
            {
                if (message != null)
                {
                    foreach (IEmbed embed in message.Embeds)
                    {
                        if (embed.Footer.HasValue)
                        {
                            if (embed.Footer.Value.Text.Contains(GId))
                            {
                                MessagesByUser.Add(message);
                            }
                        }
                    }
                }
            }
        }

        private static void AddMessageToListAsync(IEnumerable<IMessage> Messages, string GId, ref List<IMessage> MessagesByUser)
        {
            foreach (IMessage message in Messages)
            {
                if (message != null)
                {
                    foreach (IEmbed embed in message.Embeds)
                    {
                        if (embed.Footer.HasValue)
                        {
                            if (embed.Footer.Value.Text.Contains(GId))
                            {
                                MessagesByUser.Add(message);
                                // Return because we only want the first one...
                                return;
                            }
                        }
                    }
                }
            }
        }
    }
}
