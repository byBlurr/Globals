using Data;
using Globals.Global;
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
        public static List<TypingState> TypingStates = new List<TypingState>();

        public static void AddChannel(string name, string id, int indexToggle, int indexId)
        {
            GlobalChannel chan = new GlobalChannel(name, id, indexToggle, indexId);
            Channels.Add(chan);

            TypingState state = new TypingState(id, false);
            TypingStates.Add(state);
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

        public static GlobalChannel FindChannelById(string channel_id)
        {
            foreach (GlobalChannel channel in Channels)
            {
                if (channel.Id == channel_id) return channel;
            }

            return null;
        }

        public static GlobalChannel FindChannelByName(string channel_name)
        {
            foreach (GlobalChannel channel in Channels)
            {
                if (channel.Name == channel_name) return channel;
            }

            return null;
        }

        public static bool GetTypingState(string channel_name)
        {
            foreach (TypingState state in TypingStates)
            {
                if (state.Channel.ToLower().Equals(channel_name.ToLower())) return state.State;
            }
            return false;
        }

        public static void UpdateTypingState(string channel_name, bool triggered)
        {
            foreach (TypingState state in TypingStates)
            {
                if (state.Channel.ToLower().Equals(channel_name.ToLower()))
                {
                    state.State = triggered;
                }
            }
        }
    }
}
