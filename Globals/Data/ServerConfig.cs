﻿using Data;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

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

        public static void RegisterServer(ulong serverid)
        {
            var dbCon = DBConnection.Instance();
            dbCon.DatabaseName = BotConfig.Load().DatabaseName;

            if (dbCon.IsConnect())
            {
                string query = "SELECT * FROM server_configs WHERE server_id = @serverid";
                var cmd = new MySqlCommand(query, dbCon.Connection);
                cmd.Parameters.Add("@serverid", MySqlDbType.UInt64).Value = serverid;
                var reader = cmd.ExecuteReader();
                cmd.Dispose();

                if (!reader.HasRows)
                {
                    query = "INSERT INTO server_configs (server_id) VALUES(@serverid);";
                    cmd = new MySqlCommand(query, dbCon.Connection);
                    cmd.Parameters.Add("@serverid", MySqlDbType.Text).Value = serverid;

                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }

                dbCon.Close();
            }
        }

        public static void UnregisterServer(ulong serverid)
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
                    cmd.ExecuteNonQuery();
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
