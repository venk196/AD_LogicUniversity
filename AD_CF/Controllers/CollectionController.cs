using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AD_CF.Context;
using AD_CF.Models;
using AD_CF.Utilities;

namespace AD_CF.Controllers
{
    public class CollectionController : Controller
    {
        UserServices UserServices = new UserServices();
        // GET: Collection
        public ActionResult DHcollectionpoint(string sessionId)
        {
            if (sessionId != null && UserServices.getUserCountBySessionId(sessionId)==true )            
            {
                using (var db = new InventoryDbContext())
                {
                    User user = db.users.Where(x => x.sessionId == sessionId).FirstOrDefault();
                    Department dp = new Department();
                    List<Collection> cp = new List<Collection>();
                    List<Employee> emps = new List<Employee>();
                    //List<Representative> rp = new List<Representative>();
                    dp = db.departments.Where(x => x.id == user.employee.departmentId).FirstOrDefault();
                    cp = db.collections.ToList();
                    emps = db.employees.Where(x => x.role == "deptstaff" && x.departmentId == user.employee.departmentId).ToList();
                    ViewData["dp"] = dp;
                    ViewData["cp"] = cp;
                    ViewData["emps"] = emps;
                    ViewData["sessionId"] = sessionId;
                    ViewData["staffname"] = user.employee.empName;
                    return View();
                }
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        public ActionResult UpdateCollection(string collectionpoint, string representative, string sessionId)
        {
            if (sessionId != null && UserServices.getUserCountBySessionId(sessionId) == true)
            {
                using (var db = new InventoryDbContext())
                {
                    string session = (string)Session["sessionId"];
                    User user = db.users.Where(x => x.sessionId == session).FirstOrDefault();
                    String department = user.employee.departmentId;
                    Department dep = db.departments.Where(x => x.id == department).FirstOrDefault();
                    dep.collectionName = collectionpoint;
                    dep.representative = representative;
                    db.SaveChanges();
                    return RedirectToAction("DHcollectionpoint", new { sessionId });
                }
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }
    }


}