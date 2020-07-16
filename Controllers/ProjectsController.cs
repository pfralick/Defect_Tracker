using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Falcon_Bug_Tracker.Helpers;
using Falcon_Bug_Tracker.Models;
using Falcon_Bug_Tracker.ViewModel;

namespace Falcon_Bug_Tracker.Controllers
{
    public class ProjectsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ProjectsHelper projHelper = new ProjectsHelper();

        //[Authorize(Roles ="PM")]
        public ActionResult ManageProjectAssignments()
        {
            var emptyCustomUserDataList = new List<CustomUserData>();

            // I have decided that it should work this way..
            var users = db.Users.ToList();

            //Load up a Mulit Select List of Users
            ViewBag.UserIds = new MultiSelectList(db.Users, "Id", "Email");

            //Load up a Mulit Select List of Projects
            ViewBag.ProjectIds = new MultiSelectList(db.Projects, "Id", "Name");

            //Load up the View Model
            foreach (var user in users)
            {
                emptyCustomUserDataList.Add(new CustomUserData
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    ProjectNames = projHelper.ListUserProjects(user.Id).Select( p => p.Name).ToList()
                });
            }

            return View(emptyCustomUserDataList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ManageProjectAssignments(List<string> userIds, List<int> projectIds)
        {
            //if and only if I have chosen at least one person will I do the dollowing operations
            if (userIds == null || projectIds == null)
                return RedirectToAction("ManageProjectAssignments");

            // I can simply add each ot the selected users to each of the selected projects
            foreach(var userId in userIds)
            {
                foreach(var projectId in projectIds)
                {
                    projHelper.AddUserToProject(userId, projectId);
                }
            }
            return RedirectToAction("ManageProjectAssignments");
        }


        public ActionResult ManageProjectLevelUsers(int id)
        {
            var userIds = projHelper.UsersOnProject(id).Select(u => u.Id).ToList();
            ViewBag.UserIds = new MultiSelectList(db.Users, "Id", "Email", userIds);
            return View(db.Projects.Find(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ManageProjectLevelUsers(List<string> userIds, int projectId)
        {
            if(userIds != null)
                {
                    var projMemberIds = projHelper.UsersOnProject(projectId).Select(u => u.Id).ToList();
                    foreach (var memberId in projMemberIds)
                    {
                        projHelper.RemoveUserFromProject(memberId, projectId);
                    }

                    if (userIds != null)
                    { 
                        foreach (var userId in userIds)
                        {
                            projHelper.AddUserToProject(userId, projectId);
                        }

                    }
                    return RedirectToAction("ManageProjectLevelUsers", new { id = projectId });
                }
            return RedirectToAction("ManageProjectLevelUsers", new { id = projectId });
        }



        // GET: Projects
        public ActionResult Index()
        {      
            return View(db.Projects.ToList());
        }

        // GET: Projects/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // GET: Projects/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description,ProjectManagerId,Created,Updated,IsArchived")] Project project)
        {
            if (ModelState.IsValid)
            {
                project.Created = DateTime.Now;
                db.Projects.Add(project);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(project);
        }

        // GET: Projects/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,ProjectManagerId,Created,Updated,IsArchived")] Project project)
        {
            if (ModelState.IsValid)
            {
                project.Updated = DateTime.Now;
                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(project);
        }

        // GET: Projects/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Project project = db.Projects.Find(id);
            db.Projects.Remove(project);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
