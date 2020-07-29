using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using ClientPortal.ViewModels;
using Streemline3_1Poco;
namespace ClientPortal.Controllers
{
    public class TaskController : Controller
    {
        private db pocoDb = new db();
        // GET: Task
        public ActionResult displayTasks()
        {
            string userid = "";
            if (Session["userId"] != null)
            {
                userid = Session["userId"].ToString();
            }
            int userID = int.Parse(userid);
            var user = pocoDb.Fetch<tblUser>("where userID=@0", userID).FirstOrDefault();
            var personID = user.PersonID;
            var person = pocoDb.Fetch<tblPerson>("where PersonID=@0", personID).FirstOrDefault();
            var companyID = person.CompanyID;
            List<int> activeStatusID = new List<int>() { 1, 2, 3, 6, 17, 19, 20, 21 };
            var tasksActive = pocoDb.Fetch<tblTask>("Where taskStatusID IN(@0) and CompanyID=@1  and isnull(parentID,0)=0", activeStatusID, companyID).OrderByDescending(m => m.CreatedDate);

            List<int> closedStatusID = new List<int>() { 7, 16 };
            var closedTasks = pocoDb.Fetch<tblTask>("Where closedDate IS NOT NULL and taskStatusID IN(@0)  and isnull(parentID,0)=0", closedStatusID);
            var topTenTasks = closedTasks.OrderByDescending(m => m.ClosedDate).Take(10);
            var VM = new ClientPortal.ViewModels.TasksViewModel
            {
                Tasks = tasksActive,
                closedTasks = topTenTasks
            };
            return View(VM);


        }
        public ActionResult detailTask(int id)
        {
         
            var task = pocoDb.Fetch<tblTask>("where TaskID=@0", id).FirstOrDefault();
            var taskStatus = pocoDb.Fetch<tblTaskStatus>("Where taskStatusID=@0",task.taskStatusID).FirstOrDefault();
            var notes = pocoDb.Fetch<tblNote>("where objectID=@0", id);
            var pids = notes.Select(x => x.createdByID).ToList();
            var person = pocoDb.Fetch<tblPerson>("where PersonID=@0", task.createdByID).FirstOrDefault();
            var company = pocoDb.Fetch<tblCompany>("Where CompanyID=@0", task.CompanyID).FirstOrDefault();
            var client = pocoDb.Fetch<tblPerson>("Where PersonID=@0",company.ContactID).FirstOrDefault();
            var estimate = pocoDb.FirstOrDefault<tblEstimate>("Where EstimateID IS NOT NULL and estimateID=@0",task.estimateID);                         
            List<tblPerson> personlist = null;
            if (pids.Any() == true)
            {
                personlist = pocoDb.Fetch<tblPerson>("Where PersonID IN (@0)", pids).ToList();
            }
            var VM = new ClientPortal.ViewModels.TasksViewModel
            {
                Task = task,
                Notes=notes,
                People= personlist,
                Status=taskStatus,
                Person=person,
                Company=company,
                Client= client,
                Estimate= estimate
            };
            return View(VM);
        }
        public ActionResult noteList(int taskID)
        {
            var notes = pocoDb.Fetch<tblNote>("where objectID=@0", taskID);
            var VM = new ClientPortal.ViewModels.TasksViewModel
            {
                Notes = notes              
            };
            return View(VM);
        }
        public ActionResult AddNote(TasksViewModel model)
        {
            string userid = "";
            if (Session["userId"] != null)
            {
                userid = Session["userId"].ToString();
            }
            int userID = int.Parse(userid);
            var person = pocoDb.Fetch<tblUser>(" WHERE userID = @0", userID).FirstOrDefault();
            var personId = person.PersonID;

            if (ModelState.IsValid)
            {

                tblNote Note = new tblNote();
                Note.noteText = model.Note.noteText;
                Note.objectTypeID = 11;
                Note.createdDate = DateTime.Now;
                Note.objectID = model.Note.objectID;
                Note.PersonID = personId;
                Note.createdByID = personId;
                pocoDb.Insert(Note);
            }
            return RedirectToAction("detailTask", "Task", new { id = model.Note.objectID });
        }
        public ActionResult UploadFiles(IssueDetailViewModel model, HttpPostedFileBase postedFile)
        {
            if (postedFile != null)
            {

                linkToTblattachment(model.Attachment.itemID, postedFile);
                return RedirectToAction("detailTask", "Task", new { id = model.Attachment.itemID });
            }
            else
            {
                return RedirectToAction("detailTask", "Task", new { id = model.Attachment.itemID });
            }
        }

        public void linkToTblattachment(int issueID, HttpPostedFileBase postedFile)
        {
            string fname;
            fname = postedFile.FileName;

            string folderName = WebConfigurationManager.AppSettings["taskItUploads"].ToString();

            var path = Path.Combine(folderName, fname);
            postedFile.SaveAs(path);
            string sItemid = issueID.ToString();
            string sDateTime = DateTime.Now.ToString();
            string sIdDateTime = string.Concat(sItemid, sDateTime);
            if (ModelState.IsValid)
            {
                tblFileAttachment fileAttachment = new tblFileAttachment();
                fileAttachment.originalFileName = fname;
                fileAttachment.itemID = issueID;
                fileAttachment.createdDate = DateTime.Now;
                fileAttachment.objectTypeID = 11;
                fileAttachment.savedAsFileName = string.Concat(sIdDateTime, fname);
                pocoDb.Insert(fileAttachment);
            }
        }
       
    }
}