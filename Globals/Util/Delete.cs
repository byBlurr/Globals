using Discord;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Globals.Util
{
    class Delete
    {
        public static async Task DeleteMessage(IUserMessage Message, int Time = 10000)
        {
            var delete = Task.Run(async () =>
            {
                await Task.Delay(Time);
                if (Message != null) await Message.DeleteAsync();
            });
        }
    }
}
