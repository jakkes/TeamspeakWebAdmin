using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TeamspeakWebAdmin.Models
{
    public class ChannelGroupModel
    {
        public int ChannelGroupId { get; set; }
        public string Name { get; set; }
        
        public ChannelGroupModel() { }
        public ChannelGroupModel(string txt)
        {
            var g = new Regex("cgid=([0-9]+) name=([^ ]*)").Match(txt).Groups;
            ChannelGroupId = int.Parse(g[1].Value);
            Name = g[2].Value.Replace("\\s", " ");
        }
    }
}
