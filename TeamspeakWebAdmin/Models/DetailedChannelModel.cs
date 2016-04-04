using System.Text.RegularExpressions;

namespace TeamspeakWebAdmin.Models
{
    public class DetailedChannelModel
    {
        public int ParentId { get; set; }
        public string Name { get; set; }
        public string Topic { get; set; }
        public string Description { get; set; }
        public string Password { get; set; }
        public int Codec { get; set; }
        public int CodecQuality { get; set; }
        public int MaxClients { get; set; }
        public int MaxFamilyClients { get; set; }
        public int Order { get; set; }
        public int FlagPassword { get; set; }

        public DetailedChannelModel() { }
        public DetailedChannelModel(string txt)
        {
            var g = new Regex(@"pid=([0-9]+) channel_name=?([^ ]*) channel_topic=?([^ ]*) channel_description=?([^ ]*) channel_password=?([^ ]*) channel_codec=([0-9]+) channel_codec_quality=([0-9]+) channel_maxclients=([0-9-]+) channel_maxfamilyclients=([0-9-]+) channel_order=([0-9]+).*channel_flag_password=([0-9]+)").Match(txt).Groups;  

            ParentId = int.Parse(g[1].Value); Name = g[2].Value.Replace("\\s", " ");
            Topic = g[3].Value.Replace("\\s", " ");
            Description = g[4].Value.Replace("\\s", " "); Password = g[5].Value.Replace("\\s", " ");
            Codec = int.Parse(g[6].Value); CodecQuality = int.Parse(g[7].Value);
            MaxClients = int.Parse(g[8].Value); MaxFamilyClients = int.Parse(g[9].Value);
            Order = int.Parse(g[10].Value); FlagPassword = int.Parse(g[11].Value);
        }
    }
}
