using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OOP_Community.Models;

namespace OOP_Community.Controllers
{
    public class HomeController : DBController
    {
        // GET: Home
        [HandleError]
        public ActionResult Index()
        {
            if (Session["idUser"] == null)
            {
                return RedirectToAction("Login");
            }
            MyCommunity_DBEntities db = getDB();
            GetNewResidents();
            GetTotalMD();
            GetCompletedProj();

            try
            {
                var residentList = (from resident in db.Residents
                                    select resident).ToList();
                ViewData["residents"] = residentList;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return View();
        }
        public ActionResult Login()
        {
            if (Session["idUser"] != null)
            {
                return RedirectToAction("Index");
            }

            return View();
        }
        public ActionResult Settings()
        {
            if (Session["idUser"] == null)
            {
                return RedirectToAction("Login");
            }

            return View();
        }
        public ActionResult LogingIn(FormCollection fc)
        {
            MyCommunity_DBEntities db = getDB();
            try
            {
                if (fc["email"] != null && fc["password"] != null)
                {
                    var email = fc["email"];
                    var password = fc["password"];
                    var data = db.Users.Where(u => u.email.Equals(email) && u.password.Equals(password)).ToList();
                    if (data.Count() > 0)
                    {
                        //add session
                        Session["Fname"] = data.FirstOrDefault().fname;
                        Session["Lname"] = data.FirstOrDefault().lname;
                        Session["FullName"] = data.FirstOrDefault().fname + " " + data.FirstOrDefault().lname;
                        Session["Email"] = data.FirstOrDefault().email;
                        Session["idUser"] = data.FirstOrDefault().id;
                        Session["contactNo"] = data.FirstOrDefault().contactNo;
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        Session["Message"] = "Incorrect Email or Password, please try again.";
                        return RedirectToAction("Login");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return View();
        }
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }
        public void GetNewResidents()
        {
            MyCommunity_DBEntities db = getDB();
            DateTime now = DateTime.Now;
            DateTime startDate = new DateTime(now.Year, now.Month, 1);
            DateTime endDate = startDate.AddMonths(1).AddDays(-1);

            try
            {
                var oldResidents = (from resident in db.Residents
                                    where resident.dateJoined < startDate
                                    select resident).Count();
                var newResidents = (from resident in db.Residents
                                    where resident.dateJoined >= startDate && resident.dateJoined <= endDate
                                    select resident).Count();

                var initialR = oldResidents;
                var finalR = initialR + newResidents;

                ViewData["NewResidents"] = newResidents;
                float NewResidentsPer = ((float)100 * (finalR - initialR) / initialR);
                ViewData["NewResidentsPer"] = Math.Round(NewResidentsPer, 2);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public void GetTotalMD()
        {
            MyCommunity_DBEntities db = getDB();
            DateTime now = DateTime.Now;
            String currentMonth = DateTime.Now.ToString("MMMM");
            DateTime startDate = new DateTime(now.Year, now.Month, 1);

            try
            {

                var totalMD = (from t in db.Transactions
                               join md in db.Monthly_Due on t.monthly_Due equals md.id
                               where md.month == currentMonth
                               select md.monthlyFee).Sum();

                var paid = (from t in db.Transactions
                            join md in db.Monthly_Due on t.monthly_Due equals md.id
                            where md.month == currentMonth && t.paidStatus == true
                            select md.monthlyFee).Sum();

                ViewData["Paid"] = paid;
                ViewData["TotalMD"] = totalMD;
                float paidPer = (float)paid / (float)totalMD * 100;
                ViewData["PaidPer"] = Math.Round(paidPer, 2);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public void GetCompletedProj()
        {
            MyCommunity_DBEntities db = getDB();
            DateTime now = DateTime.Now;
            DateTime startDate = new DateTime(now.Year, now.Month, 1);
            DateTime endDate = startDate.AddMonths(1).AddDays(-1);

            try
            {

                var totalProj = (from p in db.Projects
                                 where p.dateFinished >= startDate && p.dateFinished <= endDate
                                 select p).Count();

                var completed = (from p in db.Projects
                                 where p.status == "Completed" && p.dateFinished >= startDate && p.dateFinished <= endDate
                                 select p).Count();

                ViewData["Completed"] = completed;
                ViewData["TotalProj"] = totalProj;
                float comPer = (float)completed / (float)totalProj * 100;
                ViewData["comPer"] = Math.Round(comPer, 2);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public ActionResult UpdatingProfile(FormCollection fc)
        {
            MyCommunity_DBEntities db = getDB();

            try
            {
                int userID = Convert.ToInt32(Session["idUser"]);

                var users = (from user in db.Users
                             where user.id == userID
                             select user);

                foreach (User user in users)
                {
                    Session["Fname"] = user.fname = fc["fname"];
                    Session["Lname"] = user.lname = fc["lname"];
                    Session["FullName"] = fc["fname"] + " " + fc["lname"];
                    Session["Email"] = user.email = fc["email"];
                    Session["contactNo"] = user.contactNo = fc["contactNo"];
                }
                int res = db.SaveChanges();
                if (res > 0)
                    Session["Message"] = "Account Information has been updated successfully.";
                else
                    Session["Message"] = "Failed to update account information, please try again.";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return RedirectToAction("Settings");
        }
        public ActionResult UpdatingPassword(FormCollection fc)
        {
            MyCommunity_DBEntities db = getDB();

            try
            {
                int userID = Convert.ToInt16(Session["idUser"]);
                String password = fc["currentPassword"];
                String newPassword = fc["newPassword1"];
                System.Diagnostics.Debug.WriteLine(userID);
                System.Diagnostics.Debug.WriteLine(password);
                System.Diagnostics.Debug.WriteLine(newPassword);

                var users = (from user in db.Users
                             where user.id == userID && user.password == password
                             select user).ToList();

                if (users.Count() > 0)
                {
                    System.Diagnostics.Debug.WriteLine("Passsword Exists");
                    foreach (var user in users)
                    {
                        user.password = newPassword;
                    }
                }
                else
                {
                    Session["Message"] = "Incorrect Password, please try again.";
                }

                int res = db.SaveChanges();
                if (res > 0)
                    Session["Message"] = "Password has been changed successfully.";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return RedirectToAction("Settings");
        }
    }
}