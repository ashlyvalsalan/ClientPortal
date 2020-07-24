//using ClientPortal.Models;
using ClientPortal.ViewModels;
using Microsoft.Ajax.Utilities;
using Streemline3_1Poco;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
namespace ClientPortal.Controllers
{
    [Authorize]

    public class IssuesController : Controller
    {
        private db pocoDb = new db();
     

        public ActionResult Index()
        {
            int id = 0;
            string userPerson="";
            if (Session["userId"] != null)
            {
               userPerson = Session["userId"].ToString();
            }
            id = int.Parse(userPerson);
            var userInfo = pocoDb.Fetch<tblUser>(" WHERE userID=@0",id).FirstOrDefault();
            var UserName = userInfo.userName;
            var UserPassword = userInfo.userPassword;
            if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(UserPassword))
            {
                return RedirectToAction("Login", "Home");
            }
            if (ModelState.IsValid)
            {
                var user = pocoDb.Fetch<tblUser>(" WHERE userName = @0 and userPassword=@1", UserName, UserPassword).FirstOrDefault();
                if (user != null)
                {
                    Session["username"] = UserName;
                    Session["userId"] = user.userID;
                    var solutions = pocoDb.Fetch<tblSolution>(" WHERE isActive=1");

                    var VM = new ClientPortal.ViewModels.LoginViewModel
                    {
                        Solutions = solutions
                    };

                    return View(VM);
                }
                else
                {
                    Session["LoginErrorMessage"] = "Invalid Username or password";
                    return View("Login", Session["LoginErrorMessage"]);
                }
            }
            return RedirectToAction("Login", "Home");

        }


        public ActionResult Search(string searchString)
        {

            if (string.IsNullOrEmpty(searchString))
            {
                var tblSolutions = pocoDb.Fetch<tblSolution>("Where isActive=1").ToList();
                var titleList = tblSolutions.Select(x => x.solutionTitle).ToList();
                var Solution = new ClientPortal.ViewModels.LoginViewModel
                {
                    Solutions = tblSolutions

                };

                return View(Solution);
            }
            else
            {
                var selectedTitle = pocoDb.Fetch<tblSolution>("Where solutionTitle LIKE @0", "%" + searchString + "%").ToList();
                var VMSolution = new ClientPortal.ViewModels.LoginViewModel
                {
                    Solutions = selectedTitle

                };

                return PartialView(VMSolution);
            }
        }

        public ActionResult Details(int? id)
        {

            // Get information for this issue
            var issue = pocoDb.Fetch<tblIssue>(" WHERE issueId = @0", id).FirstOrDefault();
            var issueStatus = pocoDb.Fetch<tblIssueStatus>("Where statusID=@0", issue.issueStatusID).FirstOrDefault();
            var attachments = pocoDb.Fetch<tblFileAttachment>("Where objectTypeID=@0 and itemID=@1", 11, id);
            var notes = pocoDb.Fetch<tblNote>(" Where  objectTypeID = @0  and objectID = @1", 11, id).ToList();

            //person created the note
            var pids = notes.Select(x => x.createdByID).ToList();
            List<tblPerson> personlist = null;
            if (pids.Any() == true)
            {
                personlist = pocoDb.Fetch<tblPerson>("Where PersonID IN (@0)", pids).ToList();
            }

            var VM = new IssueDetailViewModel
            {
                Issue = issue,
                Notes = notes,
                Attachments = attachments,
                IssueStatus = issueStatus,
                People = personlist
            };

            return View(VM);

        }

        public ActionResult UploadFiles(int issueID)
        {


            if (Request.Files.Count > 0)
            { 
                HttpFileCollectionBase files = Request.Files;
                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFileBase file = files[i];
                    string fname;
                    fname = file.FileName;
                    var path = Path.Combine(Server.MapPath("~/taskItUploads/"), fname);
                    file.SaveAs(path);
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
                        var tblAttachment = pocoDb.Fetch<tblFileAttachment>("Where objectTypeID=@0 and itemID=@1", 11, issueID);
                        var VM = new ClientPortal.ViewModels.IssueListViewModel
                        {
                            Attachments = tblAttachment
                        };
                        return View(VM);
                    }
                }
                return View("UploadFiles", "Issues");
            }
            return View();
        }
        [HttpPost]
        public JsonResult UploadImage(int itemId, int objectTypeId, string imageData)
        {

            int userPersonID = 0;
            try
            {
                var fileName = String.Format("screenshot-{0:d-MM-yy}.png", DateTime.Today);

                string userPerson = "";
                if (Session["userId"] != null)
                {
                    userPerson = Session["userId"].ToString();
                }
                userPersonID = int.Parse(userPerson);


                tblFileAttachment fileAttachment = new tblFileAttachment();
                fileAttachment.originalFileName = fileName;
                fileAttachment.savedAsFileName = itemId.ToString() + " " + DateTime.Now.ToString("yyyyMMdd hhmmss ") + fileName;
                fileAttachment.itemID = itemId;
                fileAttachment.objectTypeID = objectTypeId;
                fileAttachment.createdDate = DateTime.Now;
                fileAttachment.createdByID = userPersonID;
                fileAttachment.Save();
                string folderName = WebConfigurationManager.AppSettings["taskItUploads"].ToString();
                var path = Path.Combine(folderName, fileAttachment.savedAsFileName);
                using (FileStream fs = new FileStream(path, FileMode.Create))
                {
                    using (BinaryWriter bw = new BinaryWriter(fs))
                    {
                        byte[] data = Convert.FromBase64String(imageData);
                        bw.Write(data);
                        bw.Close();
                    }
                }

            }
            catch (UnauthorizedAccessException ex)
            {

                return Json(ex.Message + "Permission to upload file denied");

            }
            catch (Exception ex)
            {
                return Json("Upload failed: " + ex.Message);
            }
            var lastAttachment = pocoDb.Fetch<tblFileAttachment>("where itemID=@0 and objectTypeID=@1 and createdByID=@2", 0, 11, userPersonID).LastOrDefault();
            var screenshotID = lastAttachment.fileID;
            Session["screenshotid"] = screenshotID;
            return Json("File uploaded successfully");
        }

        public ActionResult AddNote(IssueListViewModel model)
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
            return RedirectToAction("Details", "Issues", new { id = model.Note.objectID });
        }

        public ActionResult statusChangeOnReopenButtonClick(int? issueID)
        { 
            if (ModelState.IsValid)
            {
                using (db db = new db())
                {
                    tblIssue updateIssue = pocoDb.Fetch<tblIssue>("WHERE issueID = @0", issueID).FirstOrDefault();
                    updateIssue.issueStatusID = 15;
                    pocoDb.Update(updateIssue);
                }

            }
            return RedirectToAction("Details", "Issues", new { id = issueID });

        }

        public ActionResult statusChangeOnReturnLink(int? issueID)
        {

            if (ModelState.IsValid)
            {
                using (db db = new db())
                {
                    tblIssue updateIssue = pocoDb.Fetch<tblIssue>(" WHERE issueID = @0", issueID).FirstOrDefault();
                    updateIssue.issueStatusID = 15;
                    pocoDb.Update(updateIssue);
                }

            }
            return RedirectToAction("Details", "Issues", new { id = issueID });

        }

        public ActionResult statusChangeOnMarkComplete(int? issueID)
        {
            if (ModelState.IsValid)
            {
                tblIssue updateIssue = pocoDb.Fetch<tblIssue>(" WHERE issueID = @0", issueID).FirstOrDefault();
                updateIssue.issueStatusID = 11;
                pocoDb.Update(updateIssue);
            }
            return RedirectToAction("Details", "Issues", new { id = issueID });

        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                pocoDb.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult filterStatus(int componentID, int solutionID, Boolean checkbox)
        {

            if (componentID == 0 && checkbox == false)
            {
                List<int> clientStatusExceptClosed = new List<int>();

                clientStatusExceptClosed.Add(1);
                clientStatusExceptClosed.Add(4);
                clientStatusExceptClosed.Add(10);
                clientStatusExceptClosed.Add(12);
                clientStatusExceptClosed.Add(13);
                clientStatusExceptClosed.Add(14);
                clientStatusExceptClosed.Add(15);
                clientStatusExceptClosed.Add(16);
                clientStatusExceptClosed.Add(17);
                var clientIssuesExceptClosed = pocoDb.Fetch<tblIssue>("Where issueStatusID IN(@0) and solutionId=@1", clientStatusExceptClosed, solutionID);
                var tblType = pocoDb.Fetch<tblIssueType>("where 1=1").ToList();
                var tblstatus = pocoDb.Fetch<tblIssueStatus>("where 1=1").ToList();
                var VMComponent = new ClientPortal.ViewModels.IssueListViewModel
                {
                    Issues = clientIssuesExceptClosed,
                    Statuses = tblstatus,
                    Types = tblType

                };

                return PartialView(VMComponent);

            }

            else
            {
                if (checkbox == false)
                {
                    List<int> clientStatusExceptClosed = new List<int>();

                    clientStatusExceptClosed.Add(1);
                    clientStatusExceptClosed.Add(4);
                    clientStatusExceptClosed.Add(10);
                    clientStatusExceptClosed.Add(12);
                    clientStatusExceptClosed.Add(13);
                    clientStatusExceptClosed.Add(14);
                    clientStatusExceptClosed.Add(15);
                    clientStatusExceptClosed.Add(16);
                    clientStatusExceptClosed.Add(17);
                    var clientIssuesExceptClosed = pocoDb.Fetch<tblIssue>("Where issueStatusID IN(@0) and solutionId=@1 and solutionComponentID=@2", clientStatusExceptClosed, solutionID, componentID);
                    var tblType = pocoDb.Fetch<tblIssueType>("where 1=1").ToList();
                    var tblstatus = pocoDb.Fetch<tblIssueStatus>("where 1=1").ToList();
                    var VMComponent = new ClientPortal.ViewModels.IssueListViewModel
                    {
                        Issues = clientIssuesExceptClosed,
                        Statuses = tblstatus,
                        Types = tblType

                    };

                    return PartialView(VMComponent);
                }
                else
                {

                    List<int> clientStatusExceptClosed = new List<int>();

                    clientStatusExceptClosed.Add(1);
                    clientStatusExceptClosed.Add(4);
                    clientStatusExceptClosed.Add(9);
                    clientStatusExceptClosed.Add(10);
                    clientStatusExceptClosed.Add(11);
                    clientStatusExceptClosed.Add(12);
                    clientStatusExceptClosed.Add(13);
                    clientStatusExceptClosed.Add(14);
                    clientStatusExceptClosed.Add(15);
                    clientStatusExceptClosed.Add(16);
                    clientStatusExceptClosed.Add(17);
                    var clientIssuesExceptClosed = pocoDb.Fetch<tblIssue>("Where issueStatusID IN(@0) and solutionId=@1 and solutionComponentID=@2", clientStatusExceptClosed, solutionID, componentID);
                    var tblType = pocoDb.Fetch<tblIssueType>("where 1=1").ToList();
                    var tblstatus = pocoDb.Fetch<tblIssueStatus>("where 1=1").ToList();
                    var VMComponent = new ClientPortal.ViewModels.IssueListViewModel
                    {
                        Issues = clientIssuesExceptClosed,
                        Statuses = tblstatus,
                        Types = tblType

                    };

                    return PartialView(VMComponent);
                }
            }
        }

        public ActionResult cancelledIssues(int solutionID)
        {

            List<int> cancelledStatusIDs = new List<int>();
            cancelledStatusIDs.Add(9);
            cancelledStatusIDs.Add(11);
            var tblIssues = pocoDb.Fetch<tblIssue>("Where solutionId=@0 and issueStatusID IN(@1)", solutionID, cancelledStatusIDs).ToList();
            var topTenIssues = tblIssues.OrderByDescending(m => m.ModifiedDate).Take(10);
            var tblType = pocoDb.Fetch<tblIssueType>("where 1=1").ToList();
            var tblstatus = pocoDb.Fetch<tblIssueStatus>("where 1=1").ToList();
            var VMComponent = new ClientPortal.ViewModels.IssueListViewModel
            {
                Issues = topTenIssues,
                Statuses = tblstatus,
                Types = tblType

            };
            return View(VMComponent);
        }

        public ActionResult solutionIssues(int ID)
        {
            string userid = "";
            if (Session["userId"] != null)
            {
                userid = Session["userId"].ToString();
            }
            int userID = int.Parse(userid);
            Session["solutionId"] = ID;
            var solution = pocoDb.Fetch<tblSolution>("where solutionID = @0", ID).FirstOrDefault();
            var Types = pocoDb.Fetch<tblIssueType>("where 1=1").ToList();
            var statuses = pocoDb.Fetch<tblIssueStatus>("where 1=1");
            var tblComponent = pocoDb.Fetch<tblSolutionComponent>("where solutionID=@0", ID).ToList();
            List<int> clientStatusId = new List<int>();
            clientStatusId.Add(13);
            clientStatusId.Add(14);
            var clientIssues = pocoDb.Fetch<tblIssue>("Where issueStatusID IN(@0) and solutionId=@1", clientStatusId, ID);
            List<int> clientStatusExceptClosed = new List<int>();
            clientStatusExceptClosed.Add(1);
            clientStatusExceptClosed.Add(4);
            clientStatusExceptClosed.Add(10);
            clientStatusExceptClosed.Add(12);
            clientStatusExceptClosed.Add(13);
            clientStatusExceptClosed.Add(14);
            clientStatusExceptClosed.Add(15);
            clientStatusExceptClosed.Add(16);
            clientStatusExceptClosed.Add(17);
            var clientIssuesExceptClosed = pocoDb.Fetch<tblIssue>("Where issueStatusID IN(@0) and solutionId=@1", clientStatusExceptClosed, ID);
            var tblstatusClientTitle = pocoDb.Fetch<tblIssueStatus>("where 1=1").DistinctBy(m => m.ClientTitle);
            ViewBag.ClientTitle = new SelectList(tblstatusClientTitle, "ClientTitle", "ClientTitle");
            var VM = new ClientPortal.ViewModels.IssueListViewModel
            {
                Solutions= solution,
                Statuses = statuses,
                Components = tblComponent,
                Types = Types,
                clientIssues = clientIssues,
                clientIssuesExceptClosed = clientIssuesExceptClosed
            };
            return View(VM);
        }

        public ActionResult statusChangePushToClient(int solutionID)
        {
            if (ModelState.IsValid)
            {
                using (db db = new db())
                {
                    tblSolution updateSolution = pocoDb.Fetch<tblSolution>("WHERE solutionID=@0", solutionID).FirstOrDefault();
                    updateSolution.statusId = 1;
                    pocoDb.Update(updateSolution);
             }

            }
            return RedirectToAction("solutionIssues", "Issues", new { ID = solutionID });
        }
        public ActionResult reviewComplete()
        {
            return View();
        }
        public ActionResult statusChangeReviewComplete()
        {
            string solid = "";
            if (Session["solutionId"] != null)
            {
                solid = Session["solutionId"].ToString();
            }
         
            if (ModelState.IsValid)
            {
                using (db db = new db())
                {
                    tblSolution updateSolution = pocoDb.Fetch<tblSolution>("WHERE solutionID=@0", int.Parse(solid)).FirstOrDefault();
                    updateSolution.statusId = 2;
                    pocoDb.Update(updateSolution);

                }
            }
            return RedirectToAction("solutionIssues", "Issues", new { ID = int.Parse(solid)});
        }

        public ActionResult CreateIssue(int solutionID)
        {

            var solution = pocoDb.Fetch<tblSolution>("WHERE solutionID = @0", solutionID).FirstOrDefault();
            var tblComponent = pocoDb.Fetch<tblSolutionComponent>("WHERE solutionID = @0", solutionID);
            var tblstatus = pocoDb.Fetch<tblIssueStatus>("").ToList();
            var VM = new ClientPortal.ViewModels.IssueCreateViewModel
            {
                solution= solution,
                Components = tblComponent,
                Statuses = tblstatus
            };
            return View(VM);

        }


        // GET: Issues/Create
        [HttpGet]

        public ActionResult Create(string h, string w, string b, int componentid, string url, string solutionid)
        {
            Session["solutionID"] = solutionid;
            int solutionID = Convert.ToInt32(solutionid);   
            var solution = pocoDb.Fetch<tblSolution>("WHERE solutionID = @0", solutionID).FirstOrDefault();
            var tblComponent = pocoDb.Fetch<tblSolutionComponent>("WHERE solutionID = @0", solutionID);
            var tblstatus = pocoDb.Fetch<tblIssueStatus>("").ToList();
         
            Session["height"] = h;
            Session["width"] = w;
            Session["browser"] = b;
            ViewBag.Components = new SelectList(tblComponent, "componentID", "componentTitle", componentid);
            var VM = new ClientPortal.ViewModels.IssueCreateViewModel
            {

                Statuses = tblstatus,
                solution = solution


            };

            return View(VM);


        }

        [HttpPost]
        public ActionResult Create(IssueCreateViewModel model, HttpPostedFileBase postedFile, string button)
        {

            //getting the userID
            string userid = "";
            if (Session["userId"] != null)
            {
                userid = Session["userId"].ToString();
            }
            int userID = int.Parse(userid);
            int screenID=0;
            //getting the screenshotID
            string screenShotId = "";
            if (Session["screenshotid"] != null)
            {
                screenShotId = Session["screenshotid"].ToString();
            }
            if(screenShotId!="")
            {
                screenID = int.Parse(screenShotId);
            }
            
            
            var person = pocoDb.Fetch<tblUser>(" WHERE userID = @0", userID).FirstOrDefault();
            var createdbyId = person.userID;
            var personid = person.PersonID.ToString();
            int personId = Int32.Parse(personid);
            try
            {

                if (ModelState.IsValid)
                {

                    tblIssue issue = new tblIssue();
                    issue.issueTitle = model.Issue.issueTitle;
                    issue.issueDescription = model.Issue.issueDescription;
                    issue.solutionComponentID = model.Issue.solutionComponentID;
                    issue.issueTypeID = model.Issue.issueTypeID;
                    issue.issueStatusID = model.Issue.issueStatusID;
                    issue.IssueURL = model.Issue.IssueURL;
                    issue.CreatedByID = personId;

                    issue.IsCritical = model.Issue.IsCritical;
                    issue.browserHeight = model.Issue.browserHeight;
                    issue.browserWidth = model.Issue.browserWidth;
                    issue.userAgent = model.Issue.userAgent;
                    issue.CreatedDate = DateTime.Now;
                    issue.issueCategoryID = 1;
                    issue.issuePriorityID = 1;
                    if (issue.issueDescription == null)
                    {
                        issue.issueDescription = "";
                    }
                    issue.solutionId = model.Issue.solutionId;
                    pocoDb.Insert(issue);
                    var issueList = pocoDb.Fetch<tblIssue>(" WHERE issueTitle=@0 and issueDescription=@1 and solutionId=@2", model.Issue.issueTitle, model.Issue.issueDescription, model.Issue.solutionId).FirstOrDefault();
                    var issueId = issueList.issueID;
                    if (postedFile != null)
                    {
                        linkToTblattachment(issueId, postedFile);
                    }
                   if( screenShotId != "")
                    {
                        var screenattachments = pocoDb.Fetch<tblFileAttachment>(" WHERE fileID=@0", screenID).FirstOrDefault();
                        screenattachments.itemID = issueId;
                        pocoDb.Update(screenattachments);
                    }
                    

                    if (button == "Save")
                    {
                        return RedirectToAction("solutionIssues", "Issues", new { ID = model.Issue.solutionId });
                    }
                    else if (button == "Save and Add another")
                    {
                        return RedirectToAction("create", "Issues", new { h = model.Issue.browserHeight, w = model.Issue.browserWidth, b = model.Issue.userAgent, componentid = model.Issue.solutionComponentID, url = model.Issue.IssueURL, solutionid = model.Issue.solutionId });
                    }
                    else if (button == "Save&Close")
                    {

                        return RedirectToAction("Index", "Issues");

                    }


                }
            }
            catch (Exception ex)
            {

            }
            return RedirectToAction("Issues/solutionIssues/ new { id = issue.solutionId }");

        }



        public void linkToTblattachment(int issueID, HttpPostedFileBase postedFile)
        {
            string fname;
            fname = postedFile.FileName;
            var path = Path.Combine(Server.MapPath("~/taskItUploads/"), fname);
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
                fileAttachment.savedAsFileName = string.Concat(sIdDateTime, fname);;
                pocoDb.Insert(fileAttachment);
            }
        }
        public void Go()
        {
            string width = "0";
            string height = "0";
            string browser = "0";
            if (Request.QueryString["url"] == null)
            {
                Response.Redirect("/Issue/solutionIssues");
                
            }


            String projectUrl = Convert.ToString(Request.QueryString["projectUrl"]);
            String url = Convert.ToString(Request.QueryString["url"]);
            if (String.IsNullOrEmpty(projectUrl)) { projectUrl = url; }

            string baseUrl = getBaseUrl(projectUrl);
            String pageurl = getPageUrl(baseUrl, url);
            pageurl = pageurl.Replace("http://", "");
            pageurl = pageurl.Replace("#", "#.aspx");
            Int32 solutionID = getSolutionID(baseUrl);

            if (solutionID == 0)
            { Response.Redirect(String.Format("/error?msg=Solution not found for {0}.", baseUrl)); }
            Session["SolutionID"] = solutionID;

            string componentTitle = Request.QueryString["ct"].ToString();
            Int32 componentId = getComponentID(pageurl, solutionID);
            String redirectPage = "/Issues/Create";
            if (Request.QueryString["w"] != null)
            {
                width = Request.QueryString["w"].ToString();
            }
            if (Request.QueryString["h"] != null)
            {
                height = Request.QueryString["h"].ToString();
            }
            if (Request.QueryString["b"] != null)
            {
                browser = Request.QueryString["b"].ToString();
            }

            if (Request.QueryString["ct"] == null)
            {
                Response.Redirect(redirectPage);
            }

            Response.Redirect(redirectPage + "?h=" + height + "&w=" + width + "&b=" + browser + "&componentid=" + componentId.ToString() + "&solutionid=" + solutionID.ToString() + "&url=" + Convert.ToString(Request.QueryString["url"]));
        }
        public static string getPageUrl(string BaseUrl, string url)
        {
            return url.Replace(BaseUrl, "");
        }

        public static string getBaseUrl(string fullUrl)
        {
            string baseUrl = fullUrl.Replace("http://", "");
            baseUrl = baseUrl.Replace("https://", "");

            baseUrl = baseUrl.Replace(":24242", "");
            if (baseUrl.Contains("/"))
            {
                Int32 index = baseUrl.IndexOf("/");
                baseUrl = baseUrl.Substring(0, index);

            }

            return baseUrl;

        }

        public int getSolutionID(String baseurl)
        {
            var solution = pocoDb.Fetch<tblSolution>(" where developmentURL = @0", baseurl).FirstOrDefault();
            int solutionID = solution.solutionID;

            return solutionID;
        }

        public int getComponentID(String Url, int solutionId)
        {
            int compId;
            var components = pocoDb.Fetch<tblSolutionComponent>(" where solutionID = @0", solutionId);
            var component = pocoDb.Fetch<tblSolutionComponent>("where componentUrl = @0 and solutionID =@1", Url, solutionId).FirstOrDefault();
            if (component == null)
            {
                addNewComponent(Url, solutionId);
                var componentNew = pocoDb.Fetch<tblSolutionComponent>("where componentUrl= @0 and solutionID =@1", Url, solutionId).FirstOrDefault();
                compId = componentNew.componentID;
                return compId;
            }
            else
            {
                compId = component.componentID;
                return compId;
            }
        }

        public void addNewComponent(String Url, int solutionId)
        {
            if (ModelState.IsValid)
            {

                tblSolutionComponent solutionComponent = new tblSolutionComponent();
                solutionComponent.componentTitle = Request.QueryString["ct"].ToString();
                solutionComponent.componentUrl = Url;
                solutionComponent.solutionID = solutionId;
                solutionComponent.CreatedDate = DateTime.Now;
                pocoDb.Insert(solutionComponent);


            }
        }
    }

}
