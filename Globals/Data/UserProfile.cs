using Data;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace Globals.Data
{
    class UserProfile
    {
        /// <summary>
        /// Check if the user already has a profile.
        /// </summary>
        public static bool CheckUser(ulong userid)
        {
            var dbCon = DBConnection.Instance();
            dbCon.DatabaseName = BotConfig.Load().DatabaseName;
            bool isFound = false;

            if (dbCon.IsConnect())
            {
                string query = "SELECT * FROM user_profiles WHERE user_id = @userid";
                var cmd = new MySqlCommand(query, dbCon.Connection);
                cmd.Parameters.Add("@userid", MySqlDbType.UInt64).Value = userid;

                var reader = cmd.ExecuteReader();
                isFound = reader.HasRows;
                dbCon.Close();
            }

            return isFound;
        }
    }
}
