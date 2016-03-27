using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TeamspeakWebAdmin.Models
{
    public class ClientModel
    {
        public int ClientId { get; set; }
        public int ChannelId { get; set; }
        public int ClientDatabaseId { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }

        public ClientModel() { }
        public ClientModel(string txt)
        {
            var r = new Regex(@"clid=([0-9]+) cid=([0-9]+) client_database_id=([0-9]+) client_nickname=([^ ]*) client_type=([0-9]+)");
            var g = r.Match(txt).Groups;
            ClientId = int.Parse(g[1].Value); ChannelId = int.Parse(g[2].Value);
            ClientDatabaseId = int.Parse(g[3].Value); Name = g[4].Value.Replace("\\s", " ");
            Type = int.Parse(g[5].Value);
        }
    }
}
