﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OOP_Community.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult PageNotFound()
        {
            return View();
        }
    }
}