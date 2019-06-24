using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Globals
{
    class BotConfig
    {
        [JsonIgnore]
        public static readonly string appdir = AppContext.BaseDirectory;

        public string BotStatus { get; set; }
        public string BotToken { get; set; }
        public string BotPrefix { get; set; }

        public string DatabaseName { get; set; }
        public string DatabasePassword { get; set; }

        public BotConfig()
        {
            BotStatus = "";
            BotToken = "";
            BotPrefix = "!";

            DatabaseName = "";
            DatabasePassword = "";
        }

        public void Save(string dir = "config/bot.json")
        {
            string file = Path.Combine(appdir, dir);
            File.WriteAllText(file, ToJson());
        }
        public static BotConfig Load(string dir = "config/bot.json")
        {
            string file = Path.Combine(appdir, dir);
            return JsonConvert.DeserializeObject<BotConfig>(File.ReadAllText(file));
        }
        public string ToJson()
            => JsonConvert.SerializeObject(this, Formatting.Indented);
    }
}
