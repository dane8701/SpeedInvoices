using SpeedInvoices.Service.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SpeedInvoices.Service.Controllers
{
    public class BaseController : ApiController
    {
        protected SpeedInvoicesEntities db = new SpeedInvoicesEntities();
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
