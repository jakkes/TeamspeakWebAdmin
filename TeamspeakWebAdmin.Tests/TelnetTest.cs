using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TeamspeakWebAdmin.Core;
using System.Net;

namespace TeamspeakWebAdmin.Tests
{
    [TestClass]
    public class TelnetTest
    {

        Connection conn = new Connection(IPAddress.Parse("192.168.2.193"), 10011, IPAddress.Parse("192.168.2.68"));

        private void Login()
        {
            conn.Login("serveradmin", "0AZCP8OR");
        }

        [TestMethod]
        public void LoginLogout()
        {
            conn.Login("serveradmin", "0AZCP8OR");
            conn.Logout();
            try
            {
                conn.Login("serveradmin", "abcabc");
                Assert.Fail();
            }
            catch (Exception)
            {
                
            }
        }

        [TestMethod]
        public void ServerList()
        {
            Login();
            var s = conn.ServerList();
            Assert.IsNotNull(s);
            Assert.IsTrue(s.Length > 0);
        }

        [TestMethod]
        public void ChannelList()
        {
            Login();
            conn.SelectServer(conn.ServerList()[0].Id);
            var s = conn.ChannelList();
            Assert.IsNotNull(s);
            Assert.IsTrue(s.Length > 0);
        }

        [TestMethod]
        public void ClientList()
        {
            Login();
            conn.SelectServer(conn.ServerList()[0].Id);
            var s = conn.ClientList();
            Assert.IsNotNull(s);
            Assert.IsTrue(s.Length > 0);
        }
    }
}
