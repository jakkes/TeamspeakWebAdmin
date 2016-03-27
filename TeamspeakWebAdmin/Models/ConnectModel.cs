using System;

namespace TeamspeakWebAdmin.Models
{
    public class ConnectModel
    {
        public string IP { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Guid { get; set; }
        public bool Error { get; set; } = false;
        public string ErrorMessage { get; set; }
    }
}
