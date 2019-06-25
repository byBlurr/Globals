using Data;
using Discord;
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
        public static async Task CheckUserAsync(ulong userid, DBConnection dbCon)
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

        public static async Task AddWarningAsync(IUser User, DBConnection dbCon, bool ban = false)
        {
            ulong userid = User.Id;

            string query = "SELECT * FROM user_profiles WHERE user_id = @userid;";
            var cmd = new MySqlCommand(query, dbCon.Connection);
            cmd.Parameters.Add("@userid", MySqlDbType.UInt64).Value = userid;
            var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                int warnings = reader.GetInt32(6);
                if (warnings <= 2 && ban == false)
                {
                    // Warn them
                    cmd.Dispose();
                    reader.Close();
                    await User.SendMessageAsync("A global chat moderator has warned you for your actions. Please keep the chat mature!");
                    query = "UPDATE user_profiles SET count_warnings = @count_warnings WHERE user_id = @userid";
                    cmd = new MySqlCommand(query, dbCon.Connection);
                    cmd.Parameters.Add("@userid", MySqlDbType.UInt64).Value = userid;
                    cmd.Parameters.Add("@count_warnings", MySqlDbType.UInt32).Value = (warnings + 1);

                    try
                    {
                        await cmd.ExecuteNonQueryAsync();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
                else
                {
                    // Blacklist them
                    cmd.Dispose();
                    reader.Close();
                    await User.SendMessageAsync("A global chat moderator has warned you for your actions. You have been blacklisted!");
                    query = "UPDATE user_profiles SET count_warnings = @count_warnings, user_blacklisted = @blacklisted WHERE user_id = @userid";
                    cmd = new MySqlCommand(query, dbCon.Connection);
                    cmd.Parameters.Add("@userid", MySqlDbType.UInt64).Value = userid;
                    cmd.Parameters.Add("@count_warnings", MySqlDbType.UInt32).Value = 3;
                    cmd.Parameters.Add("@blacklisted", MySqlDbType.UInt32).Value = 1;

                    try
                    {
                        await cmd.ExecuteNonQueryAsync();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }
            cmd.Dispose();
            reader.Close();
        }

        public static async Task UnBanUserAsync(IUser User, DBConnection dbCon)
        {
            ulong userid = User.Id;

            string query = "SELECT * FROM user_profiles WHERE user_id = @userid;";
            var cmd = new MySqlCommand(query, dbCon.Connection);
            cmd.Parameters.Add("@userid", MySqlDbType.UInt64).Value = userid;
            var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                // Blacklist them
                cmd.Dispose();
                reader.Close();
                await User.SendMessageAsync("A global chat moderator has removed a warning from you.");
                query = "UPDATE user_profiles SET count_warnings = @count_warnings, user_blacklisted = @blacklisted WHERE user_id = @userid";
                cmd = new MySqlCommand(query, dbCon.Connection);
                cmd.Parameters.Add("@userid", MySqlDbType.UInt64).Value = userid;
                cmd.Parameters.Add("@count_warnings", MySqlDbType.UInt32).Value = 0;
                cmd.Parameters.Add("@blacklisted", MySqlDbType.UInt32).Value = 0;

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

        public static async Task RemoveWarningAsync(IUser User, DBConnection dbCon, int warnToRemove)
        {
            ulong userid = User.Id;

            string query = "SELECT * FROM user_profiles WHERE user_id = @userid;";
            var cmd = new MySqlCommand(query, dbCon.Connection);
            cmd.Parameters.Add("@userid", MySqlDbType.UInt64).Value = userid;
            var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                    // Blacklist them
                    cmd.Dispose();
                    reader.Close();
                    await User.SendMessageAsync("A global chat moderator has removed a warning from you.");
                    query = "UPDATE user_profiles SET count_warnings = @count_warnings, user_blacklisted = @blacklisted WHERE user_id = @userid";
                    cmd = new MySqlCommand(query, dbCon.Connection);
                    cmd.Parameters.Add("@userid", MySqlDbType.UInt64).Value = userid;
                    cmd.Parameters.Add("@count_warnings", MySqlDbType.UInt32).Value = warnToRemove;
                    cmd.Parameters.Add("@blacklisted", MySqlDbType.UInt32).Value = 0;

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

        public static bool CanModerate(ulong userid, DBConnection dbCon)
        {
            bool CanMod = false;

            string query = "SELECT * FROM user_profiles WHERE user_id = @userid;";
            var cmd = new MySqlCommand(query, dbCon.Connection);
            cmd.Parameters.Add("@userid", MySqlDbType.UInt64).Value = userid;
            var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                if (reader.GetInt32(1) == 1) CanMod = true;
                else if (reader.GetInt32(2) == 1) CanMod = true;
            }
            cmd.Dispose();
            reader.Close();

            return CanMod;
        }

        public static async Task<string> GetGroupAsync(ulong userid, DBConnection dbCon)
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

        public static async Task<int> GetMessageCountAsync(ulong userid, DBConnection dbCon)
        {
            int count = 0;

            string query = "SELECT * FROM user_profiles WHERE user_id = @userid;";
            var cmd = new MySqlCommand(query, dbCon.Connection);
            cmd.Parameters.Add("@userid", MySqlDbType.UInt64).Value = userid;
            var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                count = reader.GetInt32(5);
            }
            cmd.Dispose();
            reader.Close();
            return count;
        }

        public static async Task<int> GetServerCountAsync(ulong userid, DBConnection dbCon)
        {
            int count = 0;

            string query = "SELECT * FROM user_profiles WHERE user_id = @userid;";
            var cmd = new MySqlCommand(query, dbCon.Connection);
            cmd.Parameters.Add("@userid", MySqlDbType.UInt64).Value = userid;
            var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                count = reader.GetInt32(4);
            }
            cmd.Dispose();
            reader.Close();
            return count;
        }

        public static async Task<int> GetWarningCountAsync(ulong userid, DBConnection dbCon)
        {
            int count = 0;

            string query = "SELECT * FROM user_profiles WHERE user_id = @userid;";
            var cmd = new MySqlCommand(query, dbCon.Connection);
            cmd.Parameters.Add("@userid", MySqlDbType.UInt64).Value = userid;
            var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                count = reader.GetInt32(6);
            }
            cmd.Dispose();
            reader.Close();
            return count;
        }
    }
}
