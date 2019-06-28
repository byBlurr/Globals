using System;
using System.Collections.Generic;
using System.Text;

namespace Globals.Channels
{
    class GlobalChannel
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public int IndexToggle { get; set; }
        public int IndexId { get; set; }

        public GlobalChannel(string name, string id, int indexToggle, int indexId)
        {
            Name = name;
            Id = id;
            IndexToggle = indexToggle;
            IndexId = indexId;
        }
    }
}
