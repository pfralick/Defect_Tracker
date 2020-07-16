using Falcon_Bug_Tracker.Helpers;
using Falcon_Bug_Tracker.Models;
using Falcon_Bug_Tracker.ViewModel;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace Falcon_Bug_Tracker.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index()
        {
            return View();
        }

        //public ActionResult Index()
        //{
        //    return View(db.Projects.ToList());
        //}

        private UserRolesHelper roleHelper = new UserRolesHelper();

        [Authorize(Roles = "ADMIN, PM, DEV, SUB")]
        public ActionResult Dashboard()
        {
            var allTickets = db.Tickets.ToList();

            //Load up a dashboardViewModel to feed to the view
            var dashboardVm = new DashboardViewModel()
            {
                TicketCount = allTickets.Count,
                //HighPriorityTicketCount = allTickets.Where(t => t.TicketPriority.Name == "High").Count(),
                //NewTicketCount = allTickets.Where(t => t.TicketStatus.Name == "New").Count(),
                HighPriorityTicketCount = 3,
                NewTicketCount = 4,
                TotalComments = db.TicketComments.Count(),

                AllTickets = allTickets,
            };

            dashboardVm.ProjectVM.ProjectCount = db.Projects.Count();
            dashboardVm.ProjectVM.AllProjects = db.Projects.ToList();
            dashboardVm.ProjectVM.AllPMs = roleHelper.UsersInRole("PM").ToList();

            
            return View(dashboardVm);
        }

        [Authorize(Roles = "ADMIN, PM, DEV, SUB")]
        public ActionResult Projects()
        {
            var model = db.Projects.ToList();
            return View(model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        //public ActionResult UserProfile()
        //{
        //    var model = new UserProfilesVM();
        //    var userId = User.Identity.GetUserId();
        //    var user = db.Users.Find(userId);
        //    model.AvatarPath = user.AvatarPath;
        //    model.Email = user.Email;
        //    model.FullName = user.FullName;
        //    model.Id = user.FullName;
        //    model.TicketsIn = ticketHelper.ListMyTickets();
        //    model.Role = UserRolesHelper.ListUserRoles(userId).FirstOrDefault();

        //    return View(model);

        //}

        public ActionResult Contact()
        {

            return View();
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




        //Adding Email to MVC
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Contact(EmailModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var body = "<p>Email From: <bold>{0}</bold>({1})</p><p>Message:</p><p>{2}</p>";
                    var from = "Peter's Blog<example@email.com>";

                    //model.Body = "This is a message from your bug tracker site.  The name and the email of the contacting person is above.";

                    var email = new MailMessage(from, WebConfigurationManager.AppSettings["emailto"])
                    {
                        Subject = "Bug Tracker Contact Email",
                        Body = string.Format(body, model.FromName, model.FromEmail, model.Body),
                        IsBodyHtml = true
                    };

                    //var svc = new PersonalEmail();
                    //await svc.SendAsync(email);

                    return View(new EmailModel());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    await Task.FromResult(0);
                }
            }
            //return View(model);
            return View();
        }
        //    public IQueryable<BlogPost> IndexSearch(string searchStr)
        //    {
        //        IQueryable<BlogPost> result = null;
        //        if (searchStr != null)
        //        {
        //            result = db.BlogPosts.AsQueryable();
        //            result = result.Where(p => p.Title.Contains(searchStr) ||
        //                                p.Body.Contains(searchStr) ||
        //                                p.Comments.Any(c => c.Body.Contains(searchStr) ||
        //                                                c.Author.FirstName.Contains(searchStr) ||
        //                                                c.Author.LastName.Contains(searchStr) ||
        //                                                c.Author.DisplayName.Contains(searchStr) ||
        //                                                c.Author.Email.Contains(searchStr)));
        //        }
        //        else
        //        {
        //            result = db.BlogPosts.AsQueryable();
        //        }
        //        return result.OrderByDescending(p => p.Created);
        //    }
        //}

    }
}