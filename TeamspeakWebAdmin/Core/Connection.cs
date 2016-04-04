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
using TeamspeakWebAdmin;

namespace TeamspeakWebAdmin.Core
{
    public class Connection
    {
        public string Guid { get; private set; }
        public IPAddress LocalEndPoint { get; private set; }

        private TcpClient client;
        private NetworkStream stream;
        private int timeout = 5000;
        private bool reading = false;

        public Connection(IPAddress IP, int Port, IPAddress Local)
        {
            Guid = System.Guid.NewGuid().ToString();
            LocalEndPoint = Local;

            client = new TcpClient();
            client.Connect(new IPEndPoint(IP, Port));
            stream = client.GetStream();
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
            {
                re[i] = new ChannelModel(r[i]);
            }
            return re;
        }

        public void ChannelEdit(int id, string Name, string Topic, string Description)
        {
            string s = "channeledit cid="+id;
            if (Name != null)
                s += " channel_name=" + Name.Replace(" ", "\\s");
            if (Topic != null)
                s += " channel_topic=" + Topic.Replace(" ", "\\s");
            if (Description != null)
                s += " channel_description=" + Description.Replace(" ", "\\s");

            if (s.Length == ("channeledit cid="+id).Length)
                return;

            Send(s);
        }

        public void ChannelSetPassword(int id, string password)
        {
            Send(string.Format("channeledit cid={0} channel_password={1}", id, password));
        }

        public void SelectServer(int Id)
        {
            Send("use sid=" + Id);
        }
        
        public ServerGroupModel[] ServerGroupList()
        {
            var r = Send("servergrouplist").Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            var re = new ServerGroupModel[r.Length];
            for (int i = 0; i < re.Length; i++)
                re[i] = new ServerGroupModel(r[i]);
            return re;
        }

        public ChannelGroupModel[] ChannelGroupList()
        {
            var r = Send("channelgrouplist").Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            var re = new ChannelGroupModel[r.Length];
            for (int i = 0; i < re.Length; i++)
                re[i] = new ChannelGroupModel(r[i]);
            return re;
        }

        public DetailedClientModel ClientInfo(int id)
        {
            return new DetailedClientModel(Send(string.Format("clientinfo clid={0}", id)));
        }

        public DetailedChannelModel ChannelInfo(int id)
        {
            return new DetailedChannelModel(Send(string.Format("channelinfo cid={0}", id)));
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
            if (reading)
                throw new Error(new ErrorEventArgs("", "Slow down"));
            if (!client.Connected)
                throw new Error(new ErrorEventArgs("", "Connection expired"));
            reading = true;
            Read();
            var b = ASCIIEncoding.ASCII.GetBytes(msg + "\n\r");
            stream.Write(b, 0, b.Length);
            string re = "";
            int w = 0;
            while (!re.Contains("error"))
            {
                re += Read();
                Thread.Sleep(50);
                if (w > timeout)
                {
                    reading = false;
                    throw new Error(new ErrorEventArgs("", "Slow down"));
                }
                w += 50;
            }
            reading = false;
            CheckErrorLine(re);
            return re;
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

            return message;
        }
        
        internal void CheckErrorLine(string ErrorLine)
        {
            var r = new Regex(@"error\sid=([0-9]+)\smsg=([^ ]+)");
            var p = r.Match(ErrorLine).Groups;
            if (p[1].Value != "0")
                throw new Error(new ErrorEventArgs(p[1].Value, p[2].Value));
        }
    }

    public class ErrorEventArgs
    {
        public string Id { get; set; }
        public string Message { get; set; }

        public ErrorEventArgs() { }
        public ErrorEventArgs(string Id, string Message)
        {
            this.Id = Id;
            this.Message = Message.Replace("\\s", " ");
        }
    }

    public class Error : Exception
    {
        public ErrorEventArgs Args { get; set; }
        public Error(ErrorEventArgs e) : base(e.Message)
        {
            Args = e;
        }
    }
}
