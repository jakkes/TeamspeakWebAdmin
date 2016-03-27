using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;
using TeamspeakWebAdmin.Models;

namespace TeamspeakWebAdmin.Core
{
    public class Connection
    {

        public string Guid { get; private set; }
        public IPAddress LocalEndPoint { get; private set; }

        private TcpClient client;
        private NetworkStream stream;
        private int timeout = 1000;

        public Connection(IPAddress IP, int Port, IPAddress Local)
        {
            Guid = System.Guid.NewGuid().ToString();
            LocalEndPoint = Local;

            client = new TcpClient();
            client.Connect(new IPEndPoint(IP, Port));
            stream = client.GetStream();

            Read(timeout);
        }

        public void Login(string Username, string Password)
        {
            Send(string.Format("login {0} {1}", Username, Password));
        }
        
        public void Logout()
        {
            Send("logout");
        }

        public ServerModel[] ServerList()
        {
            var r = Send("serverlist").Split('|');
            var re = new ServerModel[r.Length];
            for (int i = 0; i < re.Length; i++)
                re[i] = new ServerModel(r[i]);
            return re;
        }

        public ClientModel[] ClientList()
        {
            var r = Send("clientlist").Split(new char[] { '|'}, StringSplitOptions.RemoveEmptyEntries);
            var re = new ClientModel[r.Length];
            for (int i = 0; i < re.Length; i++)
                re[i] = new ClientModel(r[i]);
            return re;
        }

        public ChannelModel[] ChannelList()
        {
            var r = Send("channellist").Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            var re = new ChannelModel[r.Length];
            for (int i = 0; i < re.Length; i++)
                re[i] = new ChannelModel(r[i]);
            return re;
        }

        public void SelectServer(int Id)
        {
            Send("use sid=" + Id);
        }

        public DetailedClientModel ClientInfo(int id)
        {
            return new DetailedClientModel(Send(string.Format("clientinfo clid={0}", id)));
        }

        public void Poke(string text, int id)
        {
            Send(string.Format("clientpoke msg={0} clid={1}", text.Replace(" ", "\\s"), id));
        }

        public void Kick(string text, int id, int reasonid)
        {
            Send(string.Format("clientkick reasonid={0} msg={1} clid={2}", reasonid, text.Replace(" ", "\\s"), id));
        }

        public void Move(int clid, int cid)
        {
            Send(string.Format("clientmove clid={0} cid={1}", clid, cid));
        }

        public void Ban(string text, int id, int time)
        {
            Send(string.Format("banclient clid={0} time={1} banreason={2}", id, time * 3600, text.Replace(" ", "\\s")));
        }

        private string Send(string msg)
        {
            if (!client.Connected)
                throw new Exception("Connection expired");
            var b = ASCIIEncoding.ASCII.GetBytes(msg + "\n\r");
            stream.Write(b, 0, b.Length);
            return Read(timeout);
        }

        internal string Read()
        {
            byte[] buffer = new byte[1024];
            string message = string.Empty;

            while (stream.DataAvailable)
            {
                int r = stream.Read(buffer, 0, buffer.Length);
                message += ASCIIEncoding.ASCII.GetString(buffer, 0, r);
            }

            var temp = message.Split(new string[] { "\n\r" }, StringSplitOptions.RemoveEmptyEntries);
            if (temp.Last().StartsWith("error"))
            {
                var e = CheckErrorLine(temp.Last());
                if (e.Id != "0")
                    throw new Exception(e.Message);

                string res = string.Empty;
                for (int i = 0; i < temp.Length - 1; i++)
                    res += temp[i] + "\n\r";

                return res;
            }
            else
                return message;
        }

        internal string Read(int timeout)
        {
            int w = 0;
            while(w < timeout)
            {
                if (stream.DataAvailable)
                    return Read();
                Thread.Sleep(100);
                w += timeout / 50;
            }
            throw new Exception("Timeout");
        }

        internal Error CheckErrorLine(string ErrorLine)
        {
            var r = new Regex(@"error\sid=([0-9]+)\smsg=([^ ]+)");
            var p = r.Match(ErrorLine).Groups;
            return new Error(p[1].Value, p[2].Value);
        }
    }

    public class Error
    {
        public string Id { get; set; }
        public string Message { get; set; }

        public Error() { }
        public Error(string Id, string Message)
        {
            this.Id = Id;
            this.Message = Message.Replace("\\s", " ");
        }
    }
}
