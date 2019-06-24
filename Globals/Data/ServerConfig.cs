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
        /// <summary>
        /// Check if the server already has a config.
        /// </summary>
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

        public static async Task ToggleChannel(ulong Serverid, string ToToggle, bool Enabled)
        {
            var dbCon = DBConnection.Instance();
            dbCon.DatabaseName = BotConfig.Load().DatabaseName;

            if (dbCon.IsConnect())
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

                dbCon.Close();
            }
        }

        public static void GetChannelSettings(ulong serverid, ref string gamingState, ref string musicState, ref string moviesState, ref string r6State, ref string leagueState, ref string rustState, ref string gtaState, ref string pubgState, ref string fortniteState, ref string apexState)
        {
            var dbCon = DBConnection.Instance();
            dbCon.DatabaseName = BotConfig.Load().DatabaseName;

            if (dbCon.IsConnect())
            {
                string query = "SELECT * FROM server_configs WHERE server_id = @serverid";
                var cmd = new MySqlCommand(query, dbCon.Connection);
                cmd.Parameters.Add("@serverid", MySqlDbType.UInt64).Value = serverid;
                var reader = cmd.ExecuteReader();

                while(reader.Read())
                {
                    if (reader.GetInt32(12) == 1) gamingState = "Enabled";
                    if (reader.GetInt32(13) == 1) musicState = "Enabled";
                    if (reader.GetInt32(14) == 1) moviesState = "Enabled";
                    if (reader.GetInt32(15) == 1) r6State = "Enabled";
                    if (reader.GetInt32(16) == 1) leagueState = "Enabled";
                    if (reader.GetInt32(17) == 1) rustState = "Enabled";
                    if (reader.GetInt32(18) == 1) gtaState = "Enabled";
                    if (reader.GetInt32(19) == 1) pubgState = "Enabled";
                    if (reader.GetInt32(20) == 1) fortniteState = "Enabled";
                    if (reader.GetInt32(21) == 1) apexState = "Enabled";
                }

                dbCon.Close();
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
                cmd.Dispose();

                if (!reader.HasRows)
                {
                    query = "INSERT INTO server_configs (server_id) VALUES(@serverid);";
                    cmd = new MySqlCommand(query, dbCon.Connection);
                    cmd.Parameters.Add("@serverid", MySqlDbType.Text).Value = serverid;

                    try
                    {
                        await cmd.ExecuteNonQueryAsync();
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
    }
}
