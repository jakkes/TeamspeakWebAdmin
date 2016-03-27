using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TeamspeakWebAdmin.Models;

namespace TeamspeakWebAdmin.Core
{
    public class Connections
    {
        private static List<Connection> connections = new List<Connection>();

        public static string Connect(ConnectModel model, string Local)
        {
            return Connect(model, IPAddress.Parse(Local));
        }

        /// <summary>
        /// Attempts to connect and login to the Teamspeak Query
        /// </summary>
        /// <param name="model">Model</param>
        /// <param name="Local">Local IP</param>
        /// <exception cref="Exception">Throws a general exception containing a message</exception>
        public static string Connect(ConnectModel model, IPAddress Local)
        {
            var conn = new Connection(IPAddress.Parse(model.IP), model.Port, Local);
            conn.Login(model.Username, model.Password);
            connections.Add(conn);
            return conn.Guid;
        }

        public static Connection Get(string Guid)
        {
            return connections.FirstOrDefault(x => x.Guid == Guid);
        }
    }
}
