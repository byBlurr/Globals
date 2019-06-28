using Data;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Globals.Channels
{
    class ChannelData
    {
        public static List<GlobalChannel> Channels = new List<GlobalChannel>();

        public static void AddChannel(string name, string id, int indexToggle, int indexId)
        {
            GlobalChannel chan = new GlobalChannel(name, id, indexToggle, indexId);
            Channels.Add(chan);
        }

        public static void PopulateChannels()
        {
            // Clear it first so that we can use this method to refresh the list too.
            Channels.Clear();

            var dbCon = DBConnection.Instance();
            dbCon.DatabaseName = BotConfig.Load().DatabaseName;
            if (dbCon.IsConnect())
            {
                string query = "SELECT * FROM global_channels";
                var cmd = new MySqlCommand(query, dbCon.Connection);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    AddChannel(reader.GetString(0), reader.GetString(1), reader.GetInt32(2), reader.GetInt32(3));
                }

                dbCon.Close();
            }

            Console.WriteLine("Populated the channel list.");
        }
    }
}
