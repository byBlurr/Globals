using Data;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Globals.Data
{
    class ServerConfig
    {
        public static bool CheckServer(ulong serverid)
        {
            var dbCon = DBConnection.Instance();
            dbCon.DatabaseName = BotConfig.Load().DatabaseName;
            bool isFound = false;

            if (dbCon.IsConnect())
            {
                string query = "SELECT * FROM server_configs WHERE server_id = @serverid";
                var cmd = new MySqlCommand(query, dbCon.Connection);
                cmd.Parameters.Add("@serverid", MySqlDbType.UInt64).Value = serverid;

                var reader = cmd.ExecuteReader();
                isFound = reader.HasRows;
                dbCon.Close();
            }

            return isFound;
        }

        public static async Task ToggleChannel(ulong Serverid, string ToToggle, bool Enabled, DBConnection dbCon)
        {

            string query = "UPDATE server_configs SET setting_" + ToToggle + " = @toggle WHERE server_id = @serverid";
            var cmd = new MySqlCommand(query, dbCon.Connection);
            cmd.Parameters.Add("@toggle", MySqlDbType.UInt64).Value = Enabled;
            cmd.Parameters.Add("@serverid", MySqlDbType.UInt64).Value = Serverid;

            try
            {
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static async Task RegisterServerAsync(ulong serverid)
        {
            var dbCon = DBConnection.Instance();
            dbCon.DatabaseName = BotConfig.Load().DatabaseName;

            if (dbCon.IsConnect())
            {
                string query = "SELECT * FROM server_configs WHERE server_id = @serverid";
                var cmd = new MySqlCommand(query, dbCon.Connection);
                cmd.Parameters.Add("@serverid", MySqlDbType.UInt64).Value = serverid;
                var reader = await cmd.ExecuteReaderAsync();

                if (reader.HasRows.Equals(false))
                {
                    cmd.Dispose();
                    query = "INSERT INTO server_configs (server_id) VALUES(@serverid);";
                    cmd = new MySqlCommand(query, dbCon.Connection);
                    cmd.Parameters.Add("@serverid", MySqlDbType.Text).Value = serverid;

                    try
                    {
                        await cmd.ExecuteNonQueryAsync();
                        Console.WriteLine("New server added to the database. YAY!");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }

                dbCon.Close();
            }
        }

        public static async Task UnregisterServerAsync(ulong serverid)
        {
            var dbCon = DBConnection.Instance();
            dbCon.DatabaseName = BotConfig.Load().DatabaseName;

            if (dbCon.IsConnect())
            {
                string query = "DELETE FROM server_configs WHERE server_id = @serverid;";
                MySqlCommand cmd = new MySqlCommand(query, dbCon.Connection);
                cmd.Parameters.Add("@serverid", MySqlDbType.Text).Value = serverid;

                try
                {
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                dbCon.Close();
            }
        }

        public static bool GetChannelState(ulong serverid, int indexToggle, DBConnection dbCon)
        {
            string query = "SELECT * FROM server_configs WHERE server_id = @serverid";
            var cmd = new MySqlCommand(query, dbCon.Connection);
            cmd.Parameters.Add("@serverid", MySqlDbType.UInt64).Value = serverid;
            var reader = cmd.ExecuteReader();

            bool toggle = false;

            while (reader.Read())
            {
                if (reader.GetInt32(indexToggle) == 1) toggle = true;
            }

            reader.Close();
            cmd.Dispose();

            return toggle;
        }

        public static ulong GetChannelId(ulong serverid, int indexId, DBConnection dbCon)
        {
            string query = "SELECT * FROM server_configs WHERE server_id = @serverid";
            var cmd = new MySqlCommand(query, dbCon.Connection);
            cmd.Parameters.Add("@serverid", MySqlDbType.UInt64).Value = serverid;
            var reader = cmd.ExecuteReader();

            ulong id = 0;

            while (reader.Read())
            {
                id = (ulong) reader.GetInt64(indexId);
            }

            reader.Close();
            cmd.Dispose();

            return id;
        }

        public static async Task SetupChannel(ulong serverid, ulong channelid, string channel, DBConnection dbCon)
        {
            string query = "SELECT * FROM server_configs WHERE server_id = @serverid";
            var cmd = new MySqlCommand(query, dbCon.Connection);
            cmd.Parameters.Add("@serverid", MySqlDbType.UInt64).Value = serverid;
            var reader = await cmd.ExecuteReaderAsync();

            if (reader.HasRows)
            {
                cmd.Dispose();
                query = "UPDATE server_configs SET channel_" + channel + " = @channelid WHERE server_id = @serverid";
                //query = "UPDATE server_configs SET channel_gaming = @channel_gaming, channel_music = @channel_music WHERE server_id = @serverid";
                cmd = new MySqlCommand(query, dbCon.Connection);
                cmd.Parameters.Add("@serverid", MySqlDbType.UInt64).Value = serverid;
                cmd.Parameters.Add("@channelid", MySqlDbType.UInt64).Value = channelid;

                try
                {
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            reader.Close();
            cmd.Dispose();
        }
    }
}
