using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GoDutch.Controllers
{
//    [RoutePrefix("events")]
    public class EventsWebController : Controller
    {
        [Route("events/{id}")]
        // GET: Event/Details/5
        public ActionResult Details(int id)
        {
            ViewBag.EventId = id;
            return View();
        }

        
    }
}
