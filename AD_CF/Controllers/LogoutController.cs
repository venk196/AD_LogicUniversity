using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AD_CF.Models;
using AD_CF.Context;


namespace AD_CF.Controllers
{
    public class LogoutController : Controller
    {
        // GET: Logout

        public ActionResult Logout(string sessionId)
        {
            using (var db = new InventoryDbContext()) {
                User user = db.users.Where(x => x.sessionId == sessionId).FirstOrDefault();
                user.sessionId = null;
                db.SaveChanges();
            }
            return RedirectToAction("Login", "Login");

        }
        public void RemoveSession()
        {
            //Acess database and enter session into database
            using (var db = new InventoryDbContext ())
            {
                string SessionId = (string)Session["SessionId"];
                User user = db.users.Where(x => x.sessionId == SessionId).FirstOrDefault();

                user.sessionId = null;                
                Session["SessionId"] = null;

                //save changes
                db.SaveChanges();

            }

        }
    }
}