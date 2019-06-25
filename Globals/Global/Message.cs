using Data;
using Discord;
using Discord.Commands;
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
            string message_channel = "";
            string message_footer = Context.Message.Timestamp.ToString();

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
                    cmd.Parameters.Add("@message_footer", MySqlDbType.String).Value = message_footer;

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
                    embed.WithFooter(user_rank + " - " + message_footer);

                    string query = "SELECT * FROM server_configs;";
                    var cmd = new MySqlCommand(query, dbCon.Connection);
                    var reader = await cmd.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {
                        if (!user_rank.ToLower().Equals("blacklisted"))
                        {
                            await PostMessageAsync(message_channel, reader, embed);
                        }
                    }
                }

                dbCon.Close();
            }
            else Console.WriteLine("Couldnt connect...");

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

        public static async Task PostMessageAsync(string message_channel, DbDataReader reader, EmbedBuilder embed)
        {
            if (message_channel.Equals(References.GamingChannel))
            {
                var Enabled = reader.GetInt32(12);
                var Id = (ulong)reader.GetInt64(2);

                if (Enabled == 1)
                {
                    var guild = CommandHandler.GetBot().GetGuild((ulong)reader.GetInt64(1));
                    var channel = guild.GetChannel(Id);
                    await (channel as IMessageChannel).SendMessageAsync(null, false, embed.Build());
                }
            }
            else if (message_channel.Equals(References.MusicChannel))
            {
                var Enabled = reader.GetInt32(13);
                var Id = (ulong)reader.GetInt64(3);

                if (Enabled == 1)
                {
                    var guild = CommandHandler.GetBot().GetGuild((ulong)reader.GetInt64(1));
                    var channel = guild.GetChannel(Id);
                    await (channel as IMessageChannel).SendMessageAsync(null, false, embed.Build());
                }
            }
            else if (message_channel.Equals(References.MovieChannel))
            {
                var Enabled = reader.GetInt32(14);
                var Id = (ulong)reader.GetInt64(4);

                if (Enabled == 1)
                {
                    var guild = CommandHandler.GetBot().GetGuild((ulong)reader.GetInt64(1));
                    var channel = guild.GetChannel(Id);
                    await (channel as IMessageChannel).SendMessageAsync(null, false, embed.Build());
                }
            }
            else if (message_channel.Equals(References.R6Channel))
            {
                var Enabled = reader.GetInt32(15);
                var Id = (ulong)reader.GetInt64(5);

                if (Enabled == 1)
                {
                    var guild = CommandHandler.GetBot().GetGuild((ulong)reader.GetInt64(1));
                    var channel = guild.GetChannel(Id);
                    await (channel as IMessageChannel).SendMessageAsync(null, false, embed.Build());
                }
            }
            else if (message_channel.Equals(References.LeagueChannel))
            {
                var Enabled = reader.GetInt32(16);
                var Id = (ulong)reader.GetInt64(6);

                if (Enabled == 1)
                {
                    var guild = CommandHandler.GetBot().GetGuild((ulong)reader.GetInt64(1));
                    var channel = guild.GetChannel(Id);
                    await (channel as IMessageChannel).SendMessageAsync(null, false, embed.Build());
                }
            }
            else if (message_channel.Equals(References.RustChannel))
            {
                var Enabled = reader.GetInt32(17);
                var Id = (ulong)reader.GetInt64(7);

                if (Enabled == 1)
                {
                    var guild = CommandHandler.GetBot().GetGuild((ulong)reader.GetInt64(1));
                    var channel = guild.GetChannel(Id);
                    await (channel as IMessageChannel).SendMessageAsync(null, false, embed.Build());
                }
            }
            else if (message_channel.Equals(References.GtaChannel))
            {
                var Enabled = reader.GetInt32(18);
                var Id = (ulong)reader.GetInt64(8);

                if (Enabled == 1)
                {
                    var guild = CommandHandler.GetBot().GetGuild((ulong)reader.GetInt64(1));
                    var channel = guild.GetChannel(Id);
                    await (channel as IMessageChannel).SendMessageAsync(null, false, embed.Build());
                }
            }
            else if (message_channel.Equals(References.PubgChannel))
            {
                var Enabled = reader.GetInt32(19);
                var Id = (ulong)reader.GetInt64(9);

                if (Enabled == 1)
                {
                    var guild = CommandHandler.GetBot().GetGuild((ulong)reader.GetInt64(1));
                    var channel = guild.GetChannel(Id);
                    await (channel as IMessageChannel).SendMessageAsync(null, false, embed.Build());
                }
            }
            else if (message_channel.Equals(References.FortniteChannel))
            {
                var Enabled = reader.GetInt32(20);
                var Id = (ulong)reader.GetInt64(10);

                if (Enabled == 1)
                {
                    var guild = CommandHandler.GetBot().GetGuild((ulong)reader.GetInt64(1));
                    var channel = guild.GetChannel(Id);
                    await (channel as IMessageChannel).SendMessageAsync(null, false, embed.Build());
                }
            }
            else if (message_channel.Equals(References.ApexChannel))
            {
                var Enabled = reader.GetInt32(21);
                var Id = (ulong)reader.GetInt64(11);

                if (Enabled == 1)
                {
                    var guild = CommandHandler.GetBot().GetGuild((ulong)reader.GetInt64(1));
                    var channel = guild.GetChannel(Id);
                    await (channel as IMessageChannel).SendMessageAsync(null, false, embed.Build());
                }
            }
        }

        private static void CheckChannel(ref string message_channel, ICommandContext Context, DbDataReader reader)
        {
            var GamingEnabled = reader.GetInt32(12);
            var GamingId = (ulong)reader.GetInt64(2);
            if (GamingEnabled == 1)
            {
                if (GamingId == Context.Channel.Id)
                {
                    message_channel = References.GamingChannel;
                }
            }

            var MusicEnabled = reader.GetInt32(13);
            var MusicId = (ulong)reader.GetInt64(3);
            if (MusicEnabled == 1)
            {
                if (MusicId.Equals(Context.Channel.Id))
                {
                    message_channel = References.MusicChannel;
                }
            }

            var MovieEnabled = reader.GetInt32(14);
            var MovieId = (ulong)reader.GetInt64(4);
            if (MovieEnabled == 1)
            {
                if (MovieId.Equals(Context.Channel.Id))
                {
                    message_channel = References.MovieChannel;
                }
            }

            var R6Enabled = reader.GetInt32(15);
            var R6Id = (ulong)reader.GetInt64(5);
            if (R6Enabled == 1)
            {
                if (R6Id.Equals(Context.Channel.Id))
                {
                    message_channel = References.R6Channel;
                }
            }

            var LeagueEnabled = reader.GetInt32(16);
            var LeagueId = (ulong)reader.GetInt64(6);
            if (LeagueEnabled == 1)
            {
                if (LeagueId.Equals(Context.Channel.Id))
                {
                    message_channel = References.LeagueChannel;
                }
            }

            var RustEnabled = reader.GetInt32(17);
            var RustId = (ulong)reader.GetInt64(7);
            if (RustEnabled == 1)
            {
                if (RustId.Equals(Context.Channel.Id))
                {
                    message_channel = References.RustChannel;
                }
            }

            var GtaEnabled = reader.GetInt32(18);
            var GtaId = (ulong)reader.GetInt64(8);
            if (GtaEnabled == 1)
            {
                if (GtaId.Equals(Context.Channel.Id))
                {
                    message_channel = References.GtaChannel;
                }
            }

            var PubgEnabled = reader.GetInt32(19);
            var PubgId = (ulong)reader.GetInt64(9);
            if (PubgEnabled == 1)
            {
                if (PubgId.Equals(Context.Channel.Id))
                {
                    message_channel = References.PubgChannel;
                }
            }

            var FortniteEnabled = reader.GetInt32(20);
            var FortniteId = (ulong)reader.GetInt64(10);
            if (FortniteEnabled == 1)
            {
                if (FortniteId.Equals(Context.Channel.Id))
                {
                    message_channel = References.FortniteChannel;
                }
            }

            var ApexEnabled = reader.GetInt32(21);
            var ApexId = (ulong)reader.GetInt64(11);
            if (ApexEnabled == 1)
            {
                if (ApexId.Equals(Context.Channel.Id))
                {
                    message_channel = References.R6Channel;
                }
            }
        }
    }
}
