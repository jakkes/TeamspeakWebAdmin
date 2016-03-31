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
            catch (Exception e) { return Json(e.Message); }
        }

        public JsonResult SelectServer(string Guid, int Id)
        {
            try
            {
                Connections.Get(Guid).SelectServer(Id); return Json("Success");
            }
            catch (Exception e) { return Json(e.Message); }
        }

        public JsonResult ServerGroupList(string Guid)
        {
            try
            {
                return Json(Connections.Get(Guid).ServerGroupList());
            }
            catch (Exception e) { return Json(e.Message); }
        }

        public JsonResult ChannelGroupList(string Guid)
        {
            try
            {
                return Json(Connections.Get(Guid).ChannelGroupList());
            }
            catch (Exception e) { return Json(e.Message); }
        }

        public JsonResult ClientList(string Guid)
        {
            try
            {
                return Json(Connections.Get(Guid).ClientList());
            }
            catch (Exception e) { return Json(e.Message); }
        }

        public JsonResult ChannelList(string Guid)
        {
            try
            {
                return Json(Connections.Get(Guid).ChannelList());
            }
            catch (Exception e) { return Json(e.Message); }
        }

        public JsonResult ClientInfo(string Guid, int ClientId)
        {
            try
            {
                return Json(Connections.Get(Guid).ClientInfo(ClientId));
            }
            catch (Exception e) { return Json(e.Message); }
        }

        public JsonResult Poke(string Guid, string Text, int ClientId)
        {
            try
            {
                Connections.Get(Guid).Poke(Text, ClientId);
                return Json("Success");
            }
            catch (Exception e) { return Json(e.Message); }
        }

        public JsonResult Kick(string Guid, string Text, int ClientId, int Reasonid)
        {
            try
            {
                Connections.Get(Guid).Kick(Text, ClientId, Reasonid);
                return Json("Success");
            } catch(Exception e) { return Json(e.Message); }
        }

        public JsonResult Move(string Guid, int ClientId, int ChannelId)
        {
            try
            {
                Connections.Get(Guid).Move(ClientId, ChannelId);
                return Json("Success");
            } catch (Exception e) { return Json(e.Message); }
        }

        public JsonResult Ban(string Guid, string Text, int ClientId, int Time)
        {
            try
            {
                Connections.Get(Guid).Ban(Text, ClientId, Time);
                return Json("Success");
            } catch(Exception e) { return Json(e.Message); }
        }
    }
}