using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TeamspeakWebAdmin.Models
{
    public class DetailedClientModel
    {
        public int ChannelId { get; set; }
        public int IdleTime { get; set; }
        public string UniqueId { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public string Platform { get; set; }
        public int ClientDatabaseId { get; set; }
        public int ChannelGroupId { get; set; }
        public int[] ServerGroupIds { get; set; }
        public string ClientCreated { get; set; }
        public string ClientLastConnected { get; set; }
        public int TotalConnections { get; set; }
        public string IP { get; set; }

        public DetailedClientModel() { }
        public DetailedClientModel(string txt)
        {
            var g = new Regex(@"cid=([0-9]+) client_idle_time=([0-9]+) client_unique_identifier=([^ ]+) client_nickname=([^ ]+) client_version=([^ ]+) client_platform=([^ ]+).*client_database_id=([0-9]+) client_channel_group_id=([0-9]+) client_servergroups=([0-9,]+) client_created=([0-9]+) client_lastconnected=([0-9]+) client_totalconnections=([0-9]+).*connection_client_ip=?([^ ]*)").Match(txt).Groups;
            ChannelId = int.Parse(g[1].Value); IdleTime = int.Parse(g[2].Value); UniqueId = g[3].Value;
            Name = g[4].Value.Replace("\\s"," "); Version = g[5].Value.Replace("\\s", " ");
            Platform = g[6].Value.Replace("\\s", " "); ClientDatabaseId = int.Parse(g[7].Value);
            ChannelGroupId = int.Parse(g[8].Value);
            var sids = g[9].Value.Split(',');
            ServerGroupIds = new int[sids.Length];
            for (int i = 0; i < sids.Length; i++)
                ServerGroupIds[i] = int.Parse(sids[i]);
            ClientCreated = g[10].Value; ClientLastConnected = g[11].Value; TotalConnections = int.Parse(g[12].Value);
            IP = g[13].Value;
        }
    }
}
