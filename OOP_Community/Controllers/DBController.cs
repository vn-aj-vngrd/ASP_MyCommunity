using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OOP_Community.Models;

namespace OOP_Community.Controllers
{
    public class DBController : Controller
    {
        private MyCommunity_DBEntities db;

        public DBController()
        {
            db = new MyCommunity_DBEntities();
        }

        public MyCommunity_DBEntities getDB()
        {
            return db;
        }

    }
}