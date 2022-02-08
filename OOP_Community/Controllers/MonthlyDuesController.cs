using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OOP_Community.Models;

namespace OOP_Community.Controllers
{
    public class MonthlyDuesController : DBController
    {
        // GET: MonthlyDue
        public ActionResult CreateMonthlyDues()
        {
            if (Session["idUser"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            MyCommunity_DBEntities db = getDB();

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
        public ActionResult ManageMonthlyDues()
        {
            if (Session["idUser"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            MyCommunity_DBEntities db = getDB();
            List<Resident> residentList = db.Residents.ToList();
            List<User> userList = db.Users.ToList();
            List<Transaction> transactionList = db.Transactions.ToList();
            List<Monthly_Due> monthlyDueList = db.Monthly_Due.ToList();
            try
            {
                ViewData["monthlyDues"] = (from transaction in transactionList
                                           join user in userList on transaction.transactedBy equals user.id into ul
                                           from subuser in ul.DefaultIfEmpty()

                                           join monthlydue in monthlyDueList on transaction.monthly_Due equals monthlydue.id into mdl
                                           from submonthlydue in mdl.DefaultIfEmpty()

                                           join resident in residentList on transaction.resident equals resident.id into rl
                                           from subresident in rl.DefaultIfEmpty()

                                           select new MonthlyDuesJoin
                                           {
                                               TransactionList = transaction,
                                               UserList = subuser,
                                               MonthlyDueList = submonthlydue,
                                               ResidentList = subresident
                                           }).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return View();
        }
        public ActionResult CreatingMonthlyDues(FormCollection fc)
        {
            MyCommunity_DBEntities db = getDB();
            Monthly_Due m = new Monthly_Due();
            try
            {
                var residentList = (from resident in db.Residents
                                    select resident).ToList();

                m.monthlyFee = Convert.ToDecimal(fc["monthlyFee"]);
                m.month = fc["month"];
                m.dueDate = Convert.ToDateTime(fc["dueDate"]);
                m.createdBy = Convert.ToInt16(Session["idUser"]);
                db.Monthly_Due.Add(m);
                int monthly_Due = m.id;

                foreach (var resident in residentList)
                {
                    Transaction t = new Transaction();
                    t.monthly_Due = monthly_Due;
                    t.resident = resident.id;
                    t.paidStatus = false;
                    db.Transactions.Add(t);
                }
                int res = db.SaveChanges();
                if (res > 0)
                    Session["Message"] = "Monthly Dues has been created successfully.";
                else
                    Session["Message"] = "Failed to create monthly dues, please try again.";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return RedirectToAction("CreateMonthlyDues");
        }
        public ActionResult UpdateMonthlyDues(FormCollection fc)
        {
            MyCommunity_DBEntities db = getDB();

            try
            {
                int transactionID = Convert.ToInt16(fc["transactionID"]);

                var transactions = (from transaction in db.Transactions
                                    where transaction.id == transactionID
                                    select transaction);

                foreach (Transaction transaction in transactions)
                {
                    transaction.transactedBy = Convert.ToInt16(Session["idUser"]);
                    transaction.paidStatus = Convert.ToBoolean(fc["paidStatus"]);
                    if (fc["paidDate"] != " " || fc["paidDate"] != "" || fc["paidDate"] != null)
                        transaction.paidDate = Convert.ToDateTime(fc["paidDate"]);
                }

                int res = db.SaveChanges();
                if (res > 0)
                    Session["Message"] = "A transaction has been updated successfully.";
                else
                    Session["Message"] = "Failed to update transaction, please try again.";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return RedirectToAction("ManageMonthlyDues");
        }
        public ActionResult DeleteTransactions()
        {
            MyCommunity_DBEntities db = getDB();
            try
            {
                int transactionID = Convert.ToInt16(Request["transactionID"]);
                var deleteTransaction = (from transaction in db.Transactions
                                         where transaction.id == transactionID
                                         select transaction).FirstOrDefault();
                db.Transactions.Remove(deleteTransaction);

                int res = db.SaveChanges();
                if (res > 0)
                    Session["Message"] = "A transaction has been deleted successfully.";
                else
                    Session["Message"] = "Failed to delete transaction, please try again.";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return RedirectToAction("ManageMonthlyDues");
        }
    }
}