using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OOP_Community.Models;

namespace OOP_Community.Controllers
{
    public class ProjectController : DBController
    {
        public ActionResult CreateProject()
        {
            if (Session["idUser"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            return View();
        }
        public ActionResult AddingProjects(FormCollection fc)
        {
            MyCommunity_DBEntities db = getDB();
            Project p = new Project();

            try
            {
                p.name = fc["name"];
                p.description = fc["description"];
                p.cost = Convert.ToDecimal(fc["cost"]);
                p.status = fc["status"];
                p.dateStarted = Convert.ToDateTime(fc["dateStarted"]);
                p.dateFinished = Convert.ToDateTime(fc["dateFinished"]);
                p.issuedBy = Convert.ToInt16(Session["idUser"]);
                db.Projects.Add(p);

                int res = db.SaveChanges();
                if (res > 0)
                    Session["Message"] = "A project has been created successfully.";
                else
                    Session["Message"] = "Failed to create project, please try again.";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return RedirectToAction("CreateProject");
        }
        public ActionResult ManageProject()
        {
            if (Session["idUser"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            MyCommunity_DBEntities db = getDB();

            try
            {
                var projectList = (from project in db.Projects
                                   join user in db.Users on project.issuedBy equals user.id
                                   select project).ToList();

                var issuedByList = (from user in db.Users
                                    join project in db.Projects on user.id equals project.issuedBy
                                    select user).ToList();
                ViewData["projectList"] = projectList;
                ViewData["issuedByList"] = issuedByList;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return View();
        }
        public ActionResult UpdateProjects(FormCollection fc)
        {
            MyCommunity_DBEntities db = getDB();

            try
            {
                int projectID = Convert.ToInt16(fc["projectID"]);

                var projects = (from project in db.Projects
                                where project.id == projectID
                                select project);

                foreach (Project project in projects)
                {
                    project.name = fc["name"];
                    project.description = fc["description"];
                    project.cost = Convert.ToDecimal(fc["cost"]);
                    project.status = fc["status"];
                    project.dateStarted = Convert.ToDateTime(fc["dateStarted"]);
                    project.dateFinished = Convert.ToDateTime(fc["dateFinished"]);
                }
                int res = db.SaveChanges();
                if (res > 0)
                    Session["Message"] = "A project has been updated successfully.";
                else
                    Session["Message"] = "Failed to update project, please try again.";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return RedirectToAction("ManageProject");
        }
        public ActionResult DeleteProjects()
        {
            MyCommunity_DBEntities db = getDB();

            try
            {
                int projectID = Convert.ToInt16(Request["projectID"]);
                var deleteProject = (from project in db.Projects
                                     where project.id == projectID
                                     select project).FirstOrDefault();

                db.Projects.Remove(deleteProject);
                int res = db.SaveChanges();
                if (res > 0)
                    Session["Message"] = "A project has been deleted successfully.";
                else
                    Session["Message"] = "Failed to delete project, please try again.";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return RedirectToAction("ManageProject");
        }
    }
}