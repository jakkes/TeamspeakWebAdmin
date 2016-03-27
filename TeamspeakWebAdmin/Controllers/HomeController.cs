using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TeamspeakWebAdmin.Models;
using TeamspeakWebAdmin.Core;
using System.Net;

namespace TeamspeakWebAdmin.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(ConnectModel model)
        {
            if (TempData["model"] != null)
            {
                var m = (ConnectModel)TempData["model"];
                TempData.Remove("model");
                return View(m);
            }
            else
            {
                return View(model);
            }
        }

        public ActionResult Connect(ConnectModel model)
        {
            if (Session["model"] != null)
            {
                model = (ConnectModel)Session["model"];
                if (Connections.Get(model.Guid) != null)
                    return View(model);
            }

            if (!ModelState.IsValid || string.IsNullOrEmpty(model.IP))
            {
                model.Error = true;
                TempData["model"] = model;
                return RedirectToAction("Index");
            }

            try {
                model.IP = Dns.GetHostAddresses(model.IP)[0].ToString();
                model.Guid = Connections.Connect(model, Request.UserHostAddress);
            }
            catch (Exception ex)
            {
                model.Error = true;
                model.ErrorMessage = ex.Message;
                TempData["model"] = model;
                return RedirectToAction("Index");
            }
            Session["model"] = model;
            return View(model);
        }
    }
}