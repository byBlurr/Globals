using Data;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Globals.Data
{
    class UserProfile
    {
        /// <summary>
        /// Check if the user already has a profile.
        /// </summary>
        public static async Task CheckUser(ulong userid, DBConnection dbCon)
        {
            string query = "SELECT * FROM user_profiles WHERE user_id = @userid";
            var cmd = new MySqlCommand(query, dbCon.Connection);
            cmd.Parameters.Add("@userid", MySqlDbType.UInt64).Value = userid;
            var reader = await cmd.ExecuteReaderAsync();

            if (!reader.HasRows)
            {
                cmd.Dispose();
                query = "INSERT INTO user_profiles (user_id, count_servers) VALUES(@userid, @servercount);";
                cmd = new MySqlCommand(query, dbCon.Connection);
                cmd.Parameters.Add("@userid", MySqlDbType.Int64).Value = userid;
                cmd.Parameters.Add("@servercount", MySqlDbType.Int32).Value = 1;

                try
                {
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            cmd.Dispose();
            reader.Close();
        }

        public static async Task<string> GetUserRankAsync(ulong userid, DBConnection dbCon)
        {
            string user_rank = "User";

            string query = "SELECT * FROM user_profiles WHERE user_id = @userid;";
            var cmd = new MySqlCommand(query, dbCon.Connection);
            cmd.Parameters.Add("@userid", MySqlDbType.UInt64).Value = userid;
            var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                if (reader.GetInt32(1) == 1) user_rank = "Globals Founder";
                else if (reader.GetInt32(2) == 1) user_rank = "Global Moderator";
                else if (reader.GetInt32(3) == 1) user_rank = "Blacklisted";
            }
            cmd.Dispose();
            reader.Close();
            return user_rank;
        }
    }
}
