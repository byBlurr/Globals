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

        public static void GetChannelSettingsAsText(ulong serverid, ref string gamingState, ref string musicState, ref string moviesState, ref string r6State, ref string leagueState, ref string rustState, ref string gtaState, ref string pubgState, ref string fortniteState, ref string apexState)
        {
            var dbCon = DBConnection.Instance();
            dbCon.DatabaseName = BotConfig.Load().DatabaseName;

            if (dbCon.IsConnect())
            {
                string query = "SELECT * FROM server_configs WHERE server_id = @serverid";
                var cmd = new MySqlCommand(query, dbCon.Connection);
                cmd.Parameters.Add("@serverid", MySqlDbType.UInt64).Value = serverid;
                var reader = cmd.ExecuteReader();

                while (reader.Read())
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

        public static void GetChannelSettingsAsBool(ulong serverid, ref bool gamingState, ref bool musicState, ref bool moviesState, ref bool r6State, ref bool leagueState, ref bool rustState, ref bool gtaState, ref bool pubgState, ref bool fortniteState, ref bool apexState)
        {
            var dbCon = DBConnection.Instance();
            dbCon.DatabaseName = BotConfig.Load().DatabaseName;

            if (dbCon.IsConnect())
            {
                string query = "SELECT * FROM server_configs WHERE server_id = @serverid";
                var cmd = new MySqlCommand(query, dbCon.Connection);
                cmd.Parameters.Add("@serverid", MySqlDbType.UInt64).Value = serverid;
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    if (reader.GetInt32(12) == 1) gamingState = true;
                    if (reader.GetInt32(13) == 1) musicState = true;
                    if (reader.GetInt32(14) == 1) moviesState = true;
                    if (reader.GetInt32(15) == 1) r6State = true;
                    if (reader.GetInt32(16) == 1) leagueState = true;
                    if (reader.GetInt32(17) == 1) rustState = true;
                    if (reader.GetInt32(18) == 1) gtaState = true;
                    if (reader.GetInt32(19) == 1) pubgState = true;
                    if (reader.GetInt32(20) == 1) fortniteState = true;
                    if (reader.GetInt32(21) == 1) apexState = true;
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

        public static void GetChannelIds(ulong serverid, ref ulong gamingId, ref ulong musicId, ref ulong moviesId, ref ulong r6Id, ref ulong leagueId, ref ulong rustId, ref ulong gtaId, ref ulong pubgId, ref ulong fortniteId, ref ulong apexId)
        {
            var dbCon = DBConnection.Instance();
            dbCon.DatabaseName = BotConfig.Load().DatabaseName;

            if (dbCon.IsConnect())
            {
                string query = "SELECT * FROM server_configs WHERE server_id = @serverid";
                var cmd = new MySqlCommand(query, dbCon.Connection);
                cmd.Parameters.Add("@serverid", MySqlDbType.UInt64).Value = serverid;
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    if (reader.GetInt64(2) != 0) gamingId = (ulong)reader.GetInt64(2);
                    if (reader.GetInt64(3) != 0) musicId = (ulong)reader.GetInt64(3);
                    if (reader.GetInt64(4) != 0) moviesId = (ulong)reader.GetInt64(4);
                    if (reader.GetInt64(5) != 0) r6Id = (ulong)reader.GetInt64(5);
                    if (reader.GetInt64(6) != 0) leagueId = (ulong)reader.GetInt64(6);
                    if (reader.GetInt64(7) != 0) rustId = (ulong)reader.GetInt64(7);
                    if (reader.GetInt64(8) != 0) gtaId = (ulong)reader.GetInt64(8);
                    if (reader.GetInt64(9) != 0) pubgId = (ulong)reader.GetInt64(9);
                    if (reader.GetInt64(10) != 0) fortniteId = (ulong)reader.GetInt64(10);
                    if (reader.GetInt64(11) != 0) apexId = (ulong)reader.GetInt64(11);
                }

                dbCon.Close();
            }
        }

        public static async Task SetupChannels(ulong serverid, ulong gamingId, ulong musicId, ulong moviesId, ulong r6Id, ulong leagueId, ulong rustId, ulong gtaId, ulong pubgId, ulong fortniteId, ulong apexId)
        {
            var dbCon = DBConnection.Instance();
            dbCon.DatabaseName = BotConfig.Load().DatabaseName;

            if (dbCon.IsConnect())
            {
                string query = "SELECT * FROM server_configs WHERE server_id = @serverid";
                var cmd = new MySqlCommand(query, dbCon.Connection);
                cmd.Parameters.Add("@serverid", MySqlDbType.UInt64).Value = serverid;
                var reader = await cmd.ExecuteReaderAsync();

                if (reader.HasRows)
                {
                    cmd.Dispose();
                    query = "UPDATE server_configs SET channel_gaming = @channel_gaming, channel_music = @channel_music, channel_movies = @channel_movies, channel_rainbowsix = @channel_rainbowsix, channel_league = @channel_league, channel_rust = @channel_rust, channel_gta = @channel_gta, channel_pubg = @channel_pubg, channel_fortnite = @channel_fortnite, channel_apex = @channel_apex WHERE server_id = @serverid";
                    //query = "UPDATE server_configs SET channel_gaming = @channel_gaming, channel_music = @channel_music WHERE server_id = @serverid";
                    cmd = new MySqlCommand(query, dbCon.Connection);
                    cmd.Parameters.Add("@serverid", MySqlDbType.UInt64).Value = serverid;
                    cmd.Parameters.Add("@channel_gaming", MySqlDbType.Text).Value = gamingId;
                    cmd.Parameters.Add("@channel_music", MySqlDbType.Text).Value = musicId;
                    cmd.Parameters.Add("@channel_movies", MySqlDbType.Text).Value = moviesId;
                    cmd.Parameters.Add("@channel_rainbowsix", MySqlDbType.Text).Value = r6Id;
                    cmd.Parameters.Add("@channel_league", MySqlDbType.Text).Value = leagueId;
                    cmd.Parameters.Add("@channel_rust", MySqlDbType.Text).Value = rustId;
                    cmd.Parameters.Add("@channel_gta", MySqlDbType.Text).Value = gtaId;
                    cmd.Parameters.Add("@channel_pubg", MySqlDbType.Text).Value = pubgId;
                    cmd.Parameters.Add("@channel_fortnite", MySqlDbType.Text).Value = fortniteId;
                    cmd.Parameters.Add("@channel_apex", MySqlDbType.Text).Value = apexId;

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
