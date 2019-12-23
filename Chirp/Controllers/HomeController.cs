using Chirp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Chirp.Controllers
{
    public class HomeController : Controller
    {
        private static char[] tagDelimeters = { ' ', '.' };

        public ActionResult Index()
        {
            MvcApplication app = HttpContext.ApplicationInstance as MvcApplication;
            ViewBag.Chirps = app != null ? app.ChirpStorage.GetAllMessages() : new List<Message>();
            return View();
        }

        public ActionResult Search()
        {
            string user = Request.Params["user"];
            string tag = Request.Params["tag"];

            MvcApplication app = HttpContext.ApplicationInstance as MvcApplication;
            if (user != null)
            {
                ViewBag.ResultsDescriptor = string.Format("Search results for user <span style='background-color:yellow;'>@{0}</span>", user);
                ViewBag.Chirps = app != null ? app.ChirpStorage.SearchMessagesByUser(user) : new List<Message>();
            }
            else if(tag!=null)
            {
                ViewBag.ResultsDescriptor = string.Format("Search results for tag <span style='background-color:yellow;'>#{0}</span>", tag);
                ViewBag.Chirps = app != null ? app.ChirpStorage.SearchMessagesByTag(tag) : new List<Message>();
            }
            else
            {
                ViewBag.ResultsDescriptor = string.Format("No search results", user);
                ViewBag.Chirps = new List<Message>();
            }

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "About Chirp.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Contact IT Mentors.";

            return View();
        }

        public ActionResult Chirp()
        {
            string text = Request.Params["messageText"];

            var chirp = new Message()
            {
                User = string.IsNullOrWhiteSpace(User.Identity.Name) ? "anonymous" : User.Identity.Name,
                Text = text,
                Timestamp = DateTime.UtcNow // this is temporary - actual timestamp will be storage timestamp
            };

            MvcApplication app = HttpContext.ApplicationInstance as MvcApplication;
            app.ChirpStorage.PostMessage(chirp);

            return RedirectToAction("Index");
        }
    }
}