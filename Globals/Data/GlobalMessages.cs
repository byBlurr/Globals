using Data;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Globals.Data
{
    class GlobalMessages
    {
        public static async Task ClearData()
        {
            var dbCon = DBConnection.Instance();
            dbCon.DatabaseName = BotConfig.Load().DatabaseName;

            if (dbCon.IsConnect())
            {
                string query = "DELETE FROM global_messages";
                var cmd = new MySqlCommand(query, dbCon.Connection);
                await cmd.ExecuteNonQueryAsync();
                dbCon.Close();
            }
        }
    }
}
