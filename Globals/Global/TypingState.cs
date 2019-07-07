using System;
using System.Collections.Generic;
using System.Text;

namespace Globals.Global
{
    class TypingState
    {
        public string Channel { get; set; }
        public bool State { get; set; }

        public TypingState(string channel, bool state)
        {
            Channel = channel;
            State = state;
        }
    }
}
