using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication2.Controllers
{
    public class DefaultController : Controller
    {
        // GET: Default/Index/98989
        public ActionResult Index(int myid = 0000)
        {
            //var newModel = new Models.PaypalResult { id = myid, name = "dd" };
            return View();
        } 
    }
}