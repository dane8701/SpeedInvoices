using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SpeedInvoices.Web.Controllers
{
    public class AccueilController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
    }
}