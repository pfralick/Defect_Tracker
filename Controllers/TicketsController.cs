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
using Microsoft.AspNet.Identity;

namespace Falcon_Bug_Tracker.Controllers
{
    public class TicketsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ProjectsHelper projHelper = new ProjectsHelper();
        private HistoryHelper historyHelper = new HistoryHelper();
        private NotificationHelper notificationHelper = new NotificationHelper();

        //Include Ticket attachment functionality in my ticket dashboard
        public ActionResult Dashboard(int id)
        {

            return View(db.Tickets.Find(id));
        }






        // GET: Tickets
        public ActionResult Index()
        {
            var tickets = db.Tickets.Include(t => t.Developer).Include(t => t.Project).Include(t => t.Submitter);
            return View(tickets.ToList());
        }

        // GET: Tickets/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // GET: Tickets/Create
        [Authorize(Roles = "SUB")]
        //public ActionResult Create(int? projectId)
        public ActionResult Create()
        {
            //ViewBag.DeveloperId = new SelectList(db.Users, "Id", "FirstName");

            //Produce a list of only my Projects and then put that list into the SelectList
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name");
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name");
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name");
            ViewBag.TicketStatusId = new SelectList(db.TicketStatus, "Id", "Name");

            //ViewBag.SubmitterId = new SelectList(db.Users, "Id", "FirstName");
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ProjectId,TicketTypeId,TicketStatusId,TicektPriorityId,SubmitterId,DeveloperId,Title,Description,Created,Updated,IsArchived")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                     
                ticket.Created = DateTime.Now;
                ticket.SubmitterId = User.Identity.GetUserId();
                db.Tickets.Add(ticket);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.DeveloperId = new SelectList(db.Users, "Id", "FirstName", ticket.DeveloperId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            ViewBag.SubmitterId = new SelectList(db.Users, "Id", "FirstName", ticket.SubmitterId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatus, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        //[Authorize(Roles = "DEV, SUB, PM")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Ticket ticket = db.Tickets.Find(id);

            var currentUserId = User.Identity.GetUserId();

            //I need some additional, more granular security to determine if this is my ticket
            //This is my ticket depends on your Role
            //If I am a Developer:

            if (User.IsInRole("DEV") && ticket.DeveloperId != currentUserId)
            {
                TempData["UnAuthorizedTicketAccess"] = $"You're not authorized to edit ticket {id}";
                return View(ticket);
                //return RedirectToAction("Dashboard", "Home");
            }

            //if (User.IsInRole("PM"))
            //{
            //    var myTicketIds = db.Projects.Where(p => p.ProjectManagerId == currentUserId).SelectMany(p => p.Tickets).Select(ticket => ticket.Id);
            //}

            
            if (ticket == null)
            {
                return HttpNotFound();
            }
            ViewBag.DeveloperId = new SelectList(db.Users, "Id", "FirstName", ticket.DeveloperId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            ViewBag.SubmitterId = new SelectList(db.Users, "Id", "FirstName", ticket.SubmitterId);
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ProjectId,TicketTypeId,TicketStatusId,TicektPriorityId,SubmitterId,DeveloperId,Title,Description,Created,Updated,IsArchived")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                //I want to use AsNoTracking() to get a Memento Ticket object
                var oldTicket = db.Tickets.AsNoTracking().FirstOrDefault(t => t.Id == ticket.Id);

                ticket.Updated = DateTime.Now;
                ticket.SubmitterId = User.Identity.GetUserId();
                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();

                var newTicket = db.Tickets.AsNoTracking().FirstOrDefault(t => t.Id == ticket.Id);

                //Use the History Helper to create the appropriate records
                historyHelper.ManageHistoryRecordCreation(oldTicket, ticket);

                //Use the Notification Helper to create the appropriate records
                notificationHelper.ManageNotifications(oldTicket, newTicket);

                //Now I can compare the new Ticket, "ticket" to the old Ticket, "oldTicket"
                //for changes that need to be recorded in the History table
                if (oldTicket.Title != ticket.Title)
                {
                    var newHistoryRecord = new TicketHistory();

                    newHistoryRecord.Created = (DateTime)ticket.Updated;
                    newHistoryRecord.UserId = User.Identity.GetUserId();
                    newHistoryRecord.Property = "Title";
                    newHistoryRecord.OldValue = oldTicket.Title;
                    newHistoryRecord.NewValue = ticket.Title;
                    newHistoryRecord.TicketId = ticket.Id;

                    db.TicketHistories.Add(newHistoryRecord);
                    db.SaveChanges();

                    //Use the Notification Helper to create the appropriate records
                    notificationHelper.ManageNotifications(oldTicket, newTicket);

                }

                return RedirectToAction("Index","TicketHistories");
            }
            ViewBag.DeveloperId = new SelectList(db.Users, "Id", "Email", ticket.DeveloperId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            ViewBag.SubmitterId = new SelectList(db.Users, "Id", "FirstName", ticket.SubmitterId);
            return View(ticket);
        }

        // GET: Tickets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ticket ticket = db.Tickets.Find(id);
            db.Tickets.Remove(ticket);
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
