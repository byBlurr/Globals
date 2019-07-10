using System;
using System.Collections.Generic;
using System.Text;

namespace Globals.Util
{
    class References
    {
        public static readonly ulong SERVERID = 591710248360738863;
        public static readonly ulong REQUESTCHANNELID = 593087682771419146;

        public static readonly string NAME = "Global Chat";
        public static readonly string VERSION = "v0.8a";
        public static readonly string DATE = "July, 10th 2019";
        public static readonly string TITLE = ""; // Leave blank if it isn't a update worthy of a title.
        public static readonly string CHANGELOG =    "- See when someone is typing in a global channel.\n" +
                                            "- Global Founders can now make announcements to the general channel.\n" +
                                            "- Images will now be within the embed!\n" +
                                            "- Force delete has been added for moderators.\n" +
                                            "- Use the `!changelog` command to view the latest changelog.\n" +
                                            "";
        public static readonly string FOOTER = "Make requests for future updates with `!request <idea>`";

        public static readonly string DELETEDURL = "https://cdn.discordapp.com/attachments/376842648847384586/598544396778209301/goneforever.png";
    }
}
