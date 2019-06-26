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
            for (int i = 0; i <= FileteredWords.Length; i++)
            {
                if (text.Contains(FileteredWords[i]))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
