using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TeamspeakWebAdmin.Models
{
    public class ServerGroupModel
    {
        public int ServerGroupId { get; set; }
        public string Name { get; set; }
        public ServerGroupModel() { }
        public ServerGroupModel(string txt)
        {
            var g = new Regex("sgid=([0-9]+) name=([^ ]*)").Match(txt).Groups;
            ServerGroupId = int.Parse(g[1].Value);
            Name = g[2].Value.Replace("\\s", " ");
        }
    }
}
