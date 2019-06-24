using Data;
using Discord.Commands;
using Globals.Util;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
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
            string user_server = Context.Guild.Name;
            string user_image = Context.User.GetAvatarUrl();

            string message_text = Context.Message.Content;
            string message_channel = "";
            string message_footer = Context.Message.Timestamp.ToString();

            var dbCon = DBConnection.Instance();
            dbCon.DatabaseName = BotConfig.Load().DatabaseName;
            if (dbCon.IsConnect())
            {
                string query = "SELECT * FROM server_configs WHERE server_id = @serverid";
                var cmd = new MySqlCommand(query, dbCon.Connection);
                cmd.Parameters.Add("@serverid", MySqlDbType.UInt64).Value = Context.Guild.Id;

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    // TODO: Add all other channels

                    var GamingEnabled = reader.GetInt32(12);
                    var GamingId = (ulong)reader.GetInt64(2);

                    var MusicEnabled = reader.GetInt32(13);
                    var MusicId = (ulong)reader.GetInt64(4);

                    var MovieEnabled = reader.GetInt32(14);
                    var MovieId = (ulong)reader.GetInt64(3);


                    if (GamingEnabled == 1)
                    {
                        if (GamingId == Context.Channel.Id)
                        {
                            message_channel = References.GamingChannel;
                            break;
                        }
                    }

                    if (MusicEnabled == 1)
                    {
                        if (MusicId.Equals(Context.Channel.Id))
                        {
                            message_channel = References.MusicChannel;
                            break;
                        }
                    }

                    if (MovieEnabled == 1)
                    {
                        if (MovieId.Equals(Context.Channel.Id))
                        {
                            message_channel = References.MovieChannel;
                            break;
                        }
                    }
                }

                cmd.Dispose();
                if (!message_channel.Equals(""))
                {
                    query = "INSERT INTO global_messages (user_id, user_name, user_server, user_image, message_text, message_channel, message_footer) " +
                        "VALUES(@user_id, @user_name, @user_server, @user_image, @message_text, @message_channel, @message_footer);";
                    cmd = new MySqlCommand(query, dbCon.Connection);
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
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }

                    // TODO: Post the message on all servers

                }

                dbCon.Close();
            }
            else Console.WriteLine("Couldnt connect...");
        }
    }
}
