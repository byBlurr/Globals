using System;
using System.Collections.Generic;
using System.Text;

namespace Globals.Util
{
    class ProfanityFilter
    {
        private static string[] FileteredWords =
        {
            "nigger",
            "n i g g e r",
            "nigga",
            "n i g g a",
        };

        public static bool HasProfanity(string text)
        {
            text = text.ToLower();
            foreach (string filteredWord in FileteredWords)
            {
                if (text.Contains(filteredWord))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
