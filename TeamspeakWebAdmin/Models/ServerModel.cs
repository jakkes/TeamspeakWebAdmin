using System.Text.RegularExpressions;

namespace TeamspeakWebAdmin.Models
{
    public class ServerModel
    {
        public int Id { get; set; }
        public int Port { get; set; }
        public string Status { get; set; }
        public int Clients { get; set; }
        public int MaxClients { get; set; }
        public string Uptime { get; set; }
        public string Name { get; set; }

        public ServerModel(string text)
        {
            var r = new Regex(@"virtualserver_id=([0-9]+) virtualserver_port=([0-9]+) virtualserver_status=([^ ]+) virtualserver_clientsonline=([0-9]+) virtualserver_queryclientsonline=([0-9]+) virtualserver_maxclients=([0-9]+) virtualserver_uptime=([0-9]+) virtualserver_name=([^ ]*) virtualserver_autostart=([0-1])");
            var a = r.Match(text).Groups;
            Id = int.Parse(a[1].Value); Port = int.Parse(a[2].Value); Status = a[3].Value;
            Clients = int.Parse(a[4].Value); MaxClients = int.Parse(a[6].Value);
            Uptime = a[7].Value; Name = a[8].Value.Replace("\\s", " ");
        }
    }
}