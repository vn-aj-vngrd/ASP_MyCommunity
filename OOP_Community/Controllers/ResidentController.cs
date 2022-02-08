using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OOP_Community.Models;

namespace OOP_Community.Controllers
{
    public class ResidentController : DBController
    {
        public ActionResult CreateResident()
        {
            if (Session["idUser"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            return View();
        }
        public ActionResult AddingResidents(FormCollection fc)
        {
            MyCommunity_DBEntities db = getDB();
            Resident r = new Resident();
            Address a = new Address();

            try
            {
                r.fname = fc["fname"];
                r.lname = fc["lname"];
                r.contactNo = fc["contactNo"];
                r.email = fc["email"];
                r.dateofBirth = Convert.ToDateTime(fc["dateofBirth"]);
                r.gender = fc["gender"];
                r.dateJoined = Convert.ToDateTime(fc["dateJoined"]);
                db.Residents.Add(r);
                var residentID = r.id;

                a.resident = residentID;
                a.block = fc["block"];
                a.lot = fc["lot"];
                a.street = fc["street"];
                a.subdivision = fc["subdivision"];
                db.Addresses.Add(a);

                int res = db.SaveChanges();
                if (res > 0)
                    Session["Message"] = "A resident has been created successfully.";
                else
                    Session["Message"] = "Failed to create resident, please try agan.";

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return RedirectToAction("CreateResident");
        }
        public ActionResult ManageResident()
        {
            if (Session["idUser"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            MyCommunity_DBEntities db = getDB();

            try
            {
                var residentList = (from resident in db.Residents
                                    join address in db.Addresses on resident.id equals address.resident
                                    select resident).ToList();

                var addressList = (from address in db.Addresses
                                   join resident in db.Residents on address.resident equals resident.id
                                   select address).ToList();

                ViewData["residentList"] = residentList;
                ViewData["addressList"] = addressList;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return View();
        }
        public ActionResult UpdateResidents(FormCollection fc)
        {
            MyCommunity_DBEntities db = getDB();

            try
            {
                int residentID = Convert.ToInt16(fc["residentID"]);

                var residents = (from resident in db.Residents
                                 where resident.id == residentID
                                 select resident);

                var addresses = (from address in db.Addresses
                                 where address.resident == residentID
                                 select address);

                foreach (Resident resident in residents)
                {
                    resident.fname = fc["fname"];
                    resident.lname = fc["lname"];
                    resident.gender = fc["gender"];
                    resident.dateofBirth = Convert.ToDateTime(fc["dateofBirth"]);
                    resident.email = fc["email"];
                    resident.contactNo = fc["contactNo"];
                    resident.dateJoined = Convert.ToDateTime(fc["dateJoined"]);
                }

                foreach (Address address in addresses)
                {
                    address.lot = fc["lot"];
                    address.block = fc["block"];
                    address.street = fc["street"];
                    address.subdivision = fc["subdivision"];
                }

                int res = db.SaveChanges();
                if (res > 0)
                    Session["Message"] = "Resident Information has been updated successfully.";
                else
                    Session["Message"] = "Failed to update resident information, please try again.";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return RedirectToAction("ManageResident");
        }
        public ActionResult DeleteResidents()
        {
            MyCommunity_DBEntities db = getDB();

            try
            {
                int residentID = Convert.ToInt16(Request["residentID"]);
                var deleteResident = (from resident in db.Residents
                                      where resident.id == residentID
                                      select resident).FirstOrDefault();

                db.Residents.Remove(deleteResident);
                int res = db.SaveChanges();
                if (res > 0)
                    Session["Message"] = "Resident Information has been deleted successfully.";
                else
                    Session["Message"] = "Failed to delete resident information, please try again.";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return RedirectToAction("ManageResident");
        }

    }
}