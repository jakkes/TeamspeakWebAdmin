using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TeamspeakWebAdmin.Models
{
    public class ChannelModel
    {
        public int ChannelId { get; set; }
        public int ParentId { get; set; }
        public int ChannelOrder { get; set; }
        public string Name { get; set; }
        public int TotalClients { get; set; }
        public int NeededSubscribePower { get; set; }

        public ChannelModel() { }
        public ChannelModel(string txt)
        {
            var g = new Regex("cid=([0-9]+) pid=([0-9]+) channel_order=([0-9]+) channel_name=([^ ]*) total_clients=([0-9]+) channel_needed_subscribe_power=([0-9]+)").Match(txt).Groups;
            ChannelId = int.Parse(g[1].Value); ParentId = int.Parse(g[2].Value);
            ChannelOrder = int.Parse(g[3].Value); Name = g[4].Value.Replace("\\s"," ");
            TotalClients = int.Parse(g[5].Value); NeededSubscribePower = int.Parse(g[6].Value);
        }
    }
}
