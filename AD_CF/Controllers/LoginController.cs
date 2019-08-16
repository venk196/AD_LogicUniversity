using AD_CF.Context;
using AD_CF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace AD_CF.Controllers
{
    public class LoginController : Controller
    {
    
        public ActionResult Login(string userName, string password)
        {
            //if first time to page, userName is null, password is null, just return view
            if(userName==null && password == null)
            {
                return View();
            }
            //if not, validate user

            List<User> users = new List<User>();

            using (var db = new InventoryDbContext())
            {
                users = db.users.ToList();
            }

            foreach (var u in users)
            {
                if (u.userName == userName && u.password == HashPassword(password))
                {
                    //create sessionid
                    string sessionId = Guid.NewGuid().ToString();

                    //save the sessionid in users database
                    using(var db = new InventoryDbContext())
                    {
                        User user1 = db.users.Where(x => x.userName == userName).FirstOrDefault();
                        user1.sessionId = sessionId;
                        db.SaveChanges();
                    }                   

                    using (var db = new InventoryDbContext())
                    {
                        User user1 = db.users.Where(x => x.userName == userName).FirstOrDefault();
                        if(user1.employee.role == "depthead")
                        {
                            return RedirectToAction("DBDeptHead", "Dashboard", new { sessionId });
                        }

                        if (user1.employee.role == "deptstaff")
                        {
                            return RedirectToAction("DBDeptStaff", "Dashboard", new { sessionId });
                        }
                        
                        if (user1.employee.role == "storeclerk")
                        {
                            return RedirectToAction("DBStoreClerk", "Dashboard", new { sessionId });
                        }
                        
                        if (user1.employee.role == "storemgr")
                        {
                            return RedirectToAction("DBStoreMgr", "Dashboard", new { sessionId });
                        }
                    }

                }
            }
            //if fall through, user and password is invalid. Show error message
            ViewData["msg"] = "*Incorrect username or password. Please try again.*";
            return View();
        }

        public string HashPassword(string password)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            Byte[] originalBytes = ASCIIEncoding.Default.GetBytes(password);
            Byte[] encodedBytes = md5.ComputeHash(originalBytes);
            
            return BitConverter.ToString(encodedBytes);
        }
    }
}