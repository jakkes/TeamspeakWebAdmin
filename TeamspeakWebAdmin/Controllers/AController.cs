using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TeamspeakWebAdmin.Core;

namespace TeamspeakWebAdmin.Controllers
{
    public class AController : Controller
    {
        public JsonResult ServerList(string Guid)
        {
            try
            {
                return Json(Connections.Get(Guid).ServerList());
            }
            catch (Error e) { return Json(e.Args); }
            catch (Exception e) { return Json(e.Message); }
        }

        public JsonResult SelectServer(string Guid, int Id)
        {
            try
            {
                Connections.Get(Guid).SelectServer(Id); return Json("Success");
            }
            catch (Error e) { return Json(e.Args); }
            catch (Exception e) { return Json(e.Message); }
        }

        public JsonResult ServerGroupList(string Guid)
        {
            try
            {
                return Json(Connections.Get(Guid).ServerGroupList());
            }
            catch (Error e) { return Json(e.Args); }
            catch (Exception e) { return Json(e.Message); }
        }

        public JsonResult ChannelGroupList(string Guid)
        {
            try
            {
                return Json(Connections.Get(Guid).ChannelGroupList());
            }
            catch (Error e) { return Json(e.Args); }
            catch (Exception e) { return Json(e.Message); }
        }

        public JsonResult ClientList(string Guid)
        {
            try
            {
                return Json(Connections.Get(Guid).ClientList());
            }
            catch (Error e) { return Json(e.Args); }
            catch (Exception e) { return Json(e.Message); }
        }

        public JsonResult ChannelList(string Guid)
        {
            try
            {
                return Json(Connections.Get(Guid).ChannelList());
            }
            catch (Error e) { return Json(e.Args); }
            catch (Exception e) { return Json(e.Message); }
        }

        public JsonResult ClientInfo(string Guid, int ClientId)
        {
            try
            {
                return Json(Connections.Get(Guid).ClientInfo(ClientId));
            }
            catch (Error e) { return Json(e.Args); }
            catch (Exception e) { return Json(e.Message); }
        }

        public JsonResult ChannelInfo(string Guid, int ChannelId)
        {
            try
            {
                return Json(Connections.Get(Guid).ChannelInfo(ChannelId));
            }
            catch (Error e) { return Json(e.Args); }
            catch (Exception e) { return Json(e.Message); }
        }

        public JsonResult Poke(string Guid, string Text, int ClientId)
        {
            try
            {
                Connections.Get(Guid).Poke(Text, ClientId);
                return Json("Success");
            }
            catch (Error e) { return Json(e.Args); }
            catch (Exception e) { return Json(e.Message); }
        }

        public JsonResult Kick(string Guid, string Text, int ClientId, int Reasonid)
        {
            try
            {
                Connections.Get(Guid).Kick(Text, ClientId, Reasonid);
                return Json("Success");
            }
            catch (Error e) { return Json(e.Args); }
            catch (Exception e) { return Json(e.Message); }
        }

        public JsonResult Move(string Guid, int ClientId, int ChannelId)
        {
            try
            {
                Connections.Get(Guid).Move(ClientId, ChannelId);
                return Json("Success");
            }
            catch (Error e) { return Json(e.Args); }
            catch (Exception e) { return Json(e.Message); }
        }

        public JsonResult Ban(string Guid, string Text, int ClientId, int Time)
        {
            try
            {
                Connections.Get(Guid).Ban(Text, ClientId, Time);
                return Json("Success");
            }
            catch (Error e) { return Json(e.Args); }
            catch (Exception e) { return Json(e.Message); }
        }

        public JsonResult ChannelEdit(string Guid, int ChannelId, string Name, string Topic, string Description)
        {
            try
            {
                Connections.Get(Guid).ChannelEdit(ChannelId, Name, Topic, Description);
                return Json("Success");
            }
            catch (Error e)
            {
                return Json(e.Args);
            }
        }

        public JsonResult ChannelSetPassword(string Guid, int ChannelId, string Password)
        {
            try
            {
                Connections.Get(Guid).ChannelSetPassword(ChannelId, Password);
                return Json("Success");
            } catch(Error e) { return Json(e.Args); }
        }
    }
}