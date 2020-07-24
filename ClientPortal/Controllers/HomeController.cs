using ClientPortal.ViewModels;
using System.Linq;
using System.Web.Mvc;
using Streemline3_1Poco;
using System.Web.Security;

namespace ClientPortal.Controllers
{
    public class HomeController : Controller
    {
        private db pocoDb = new db();
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
      
        public ActionResult Login(LoginViewModel model)
        {

            string OldHASHValue = string.Empty;
            try
            {
                using (db db = new db())
                {
                    // Ensure we have a valid viewModel to work with  
                    if (!ModelState.IsValid)
                        return View(model);

                    //Retrive Stored HASH Value From Database According To Username (one unique field)  
                    var userInfo = pocoDb.Fetch<tblUser>(" WHERE userName = @0 and userPassword=@1",model.UserName,model.UserPassword).FirstOrDefault();
                    if (userInfo!=null)
                    {
                        //Login Success  
                        //For Set Authentication in Cookie (Remeber ME Option)  
                        SignInRemember(model.UserName);

                        //Set A Unique ID in session  
                        Session["userId"] = userInfo.userID;

                        // If we got this far, something failed, redisplay form  
                        // return RedirectToAction("Index", "Dashboard");  
                        return RedirectToAction("Index", "Issues");
                    }
                    else
                    {
                        //Login Fail  
                        TempData["ErrorMSG"] = "Access Denied! Wrong Credential";
                        return RedirectToAction("Login", "Home");
                    }
                } 
            }
            catch
            {
                throw;
            }
        }
        private void SignInRemember(string userName)
        {
            bool isPersistent = false;
            FormsAuthentication.SignOut();
            FormsAuthentication.SetAuthCookie(userName, isPersistent);
            
        }
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            return RedirectToAction("Login", "Home");
        }





    }
}