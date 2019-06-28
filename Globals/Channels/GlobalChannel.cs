using System;
using System.Collections.Generic;
using System.Text;

namespace Globals.Channels
{
    class GlobalChannel
    {
        string Name { get; set; }
        string Id { get; set; }
        int IndexToggle { get; set; }
        int IndexId { get; set; }

        public GlobalChannel(string name, string id, int indexToggle, int indexId)
        {
            Name = name;
            Id = id;
            IndexToggle = indexToggle;
            IndexId = indexId;
        }
    }
}
