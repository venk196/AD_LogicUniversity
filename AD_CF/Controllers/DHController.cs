using AD_CF.Context;
using AD_CF.Models;
using AD_CF.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AD_CF.Controllers
{
    public class DHController : Controller
    {


        RequestServices requestServices = new RequestServices();
        UserServices userServices = new UserServices();
        EmailServices EmailServices = new EmailServices();

        public ActionResult DHdelegation(string sessionId, string msg)
        {
            if (sessionId != null && userServices.getUserCountBySessionId(sessionId) == true)
            {
                List<Employee> emps = new List<Employee>();
                Delegation d = new Delegation();


                using (var db = new InventoryDbContext())
                {
                    User user = db.users.Where(x => x.sessionId == sessionId).FirstOrDefault();
                    string userDeptId = user.employee.departmentId;
                    emps = db.employees.Where(x => x.role == "deptstaff" && x.departmentId == userDeptId).ToList();
                }

                ViewData["emps"] = emps;
                ViewData["d"] = d;
                ViewData["sessionId"] = sessionId;
                ViewData["staffname"] = userServices.getUserBySessionId(sessionId).employee.empName;

                if (msg != null)
                {
                    ViewData["msg"] = msg;
                }
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }


        public ActionResult CreateDelegation(string employeeName, Delegation d, string sessionId)
        {
            if (sessionId != null && userServices.getUserCountBySessionId(sessionId) == true)
            {

                bool overlap = false;
                Delegation del = new Delegation();
                del.delegateToName = employeeName;
                del.startDate = d.startDate;
                del.endDate = d.endDate;

                if ((util.IsDateValid(del.startDate, del.endDate)) == true)
                {


                    //check for delegation period overlap between the new delegation and those in database
                    using (var db = new InventoryDbContext())
                    {
                        User u = db.users.Where(x => x.sessionId == sessionId).FirstOrDefault();
                        List<Delegation> dels = new List<Delegation>();
                        dels = db.delegations.Where(x => x.Department.id == u.employee.Department.id).ToList();

                        foreach (var c in dels)
                        {
                            if (DateTime.Compare(del.endDate, c.startDate) < 0)
                            {
                                overlap = false;
                            }
                            else if (DateTime.Compare(del.startDate, c.endDate) > 0)
                            {
                                overlap = false;
                            }
                            else
                            {
                                overlap = true;
                                break;
                            }
                        }
                    }
                    if (overlap == false)
                    {
                        using (var db = new InventoryDbContext())
                        {
                            User user = db.users.Where(x => x.employee.empName == employeeName).FirstOrDefault();
                            List<Delegation> deles = new List<Delegation>();
                            deles = db.delegations.Where(x => x.Department.id == user.employee.Department.id).ToList();

                            //link the employee to the delagation obj usng employee id
                            del.employeeId = user.employeeId;
                            del.employee = user.employee;
                            del.Department = user.employee.Department;
                            del.status = "ACTIVE";
                            db.delegations.Add(del);
                            db.SaveChanges();
                            return RedirectToAction("Existdelegation", new { sessionId });
                        }
                    }
                    //if fall to here, there is overlap with current delegation. as such, send msg
                    return RedirectToAction("DHdelegation", new { sessionId, msg = "*Delegation overlaps with another existing delegation. Please check.*" });
                }
                else
                {
                    return RedirectToAction("DHdelegation", new { sessionId, msg = "*Please enter Valid date*" });
                }
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }


        public ActionResult DHapprove(string sessionId)
        {
            if (sessionId != null && userServices.getUserCountBySessionId(sessionId) == true)
            {

                User user = userServices.getUserBySessionId(sessionId);
                String department = user.employee.departmentId;
                ViewData["sessionId"] = sessionId;
                ViewData["staffname"] = user.employee.empName;
                return View(requestServices.getAllPendingApprovalRequisitions(department));
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }



        public ActionResult RequestApprove(string requisitionid, string cmd, string sessionId, FormCollection formCollection)
        {
            if (sessionId != null && userServices.getUserCountBySessionId(sessionId) == true)
            {
                using (var db = new InventoryDbContext())
                {
                    int id = int.Parse(requisitionid);
                    Requisition requistion = requestServices.GetRequistion(id);
                    User user = db.users.Where(x => x.sessionId == sessionId).FirstOrDefault();
                    string emp = user.employeeId;
                    if (cmd == "Approve")
                    {
                        requistion.status = "Approved";
                        requistion.approvedBy = emp;
                        EmailServices.sendEmail(requistion.Employee.Email, cmd, requistion.reqformNumber);


                    }
                    else if (cmd == "Reject")
                    {
                        requistion.status = "Rejected";
                        requistion.approvedBy = emp;
                        EmailServices.sendEmail(requistion.Employee.Email, cmd, requistion.reqformNumber);
                    }
                    string comments = formCollection["messagetext"];
                    if (comments != null || comments != "")
                    {
                        requistion.comment = comments;
                    }
                    //setdate approved
                    requistion.dateapproved = DateTime.Now;
                    db.requisitions.AddOrUpdate(requistion);
                    db.SaveChanges();
                }
                return RedirectToAction("DHapprove", "DH", new { sessionId });
            }

            else
            {
                return RedirectToAction("Login", "Login");
            }
        }


        public ActionResult Existdelegation(string sessionId)
        {
            if (sessionId != null && userServices.getUserCountBySessionId(sessionId) == true)
            {
                using (var db = new InventoryDbContext())
                {
                    User user = db.users.Where(x => x.sessionId == sessionId).FirstOrDefault();
                    List<Delegation> dels = new List<Delegation>();
                    dels = db.delegations.Where(x => x.Department.id == user.employee.Department.id).ToList();
                    ViewData["delegations"] = dels;
                    ViewData["sessionid"] = sessionId;
                    ViewData["staffname"] = user.employee.empName;
                    return View();
                }
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }
        public ActionResult Editdelegation(int DN, string sessionId, string msg)
        {
            if (sessionId != null && userServices.getUserCountBySessionId(sessionId) == true)
            {

                using (var db = new InventoryDbContext())
                {
                    Delegation a = db.delegations.Where(d => d.id == DN).FirstOrDefault();
                    User user = db.users.Where(x => x.sessionId == sessionId).FirstOrDefault();
                    ViewData["d"] = a;
                    ViewData["sessionId"] = sessionId;
                    ViewData["staffname"] = user.employee.empName;
                    ViewData["msg"] = msg;
                    return View(a);
                }
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }
        public ActionResult UpdateDelegation(Delegation d, string sessionId)
        {
            if (sessionId != null && userServices.getUserCountBySessionId(sessionId) == true)
            {
                if ((util.IsDateValid(d.startDate, d.endDate)) == true)
                {
                    using (var db = new InventoryDbContext())
                    {
                        User user = db.users.Where(x => x.employeeId == d.employeeId).FirstOrDefault();
                        Delegation a = db.delegations.Where(x => x.Department.id == user.employee.Department.id && x.id == d.id).FirstOrDefault();
                        a.id = d.id;
                        //a.delegateToName = d.delegateToName;
                        a.endDate = d.endDate;
                        a.startDate = d.startDate;
                        db.SaveChanges();
                        return RedirectToAction("Existdelegation", new { sessionId });
                    }
                }

                else
                {
                    ViewData["d"] = d;
                    return RedirectToAction("Editdelegation", new { DN = d.id, sessionId, msg = "*Please enter Valid date*" });
                }
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }


        public ActionResult RemoveDelegation(int DN, string sessionId)
        {
            if (sessionId != null && userServices.getUserCountBySessionId(sessionId) == true)
            {

                using (var db = new InventoryDbContext())
                {

                    Delegation a = db.delegations.Where(x => x.id == DN).FirstOrDefault();

                    db.delegations.Remove(a);
                    db.SaveChanges();

                    return RedirectToAction("Existdelegation", new { sessionId });
                }
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        public ActionResult DHViewHistory(string sessionId)
        {
            if (sessionId != null && userServices.getUserCountBySessionId(sessionId) == true)
            {

                User user = userServices.getUserBySessionId(sessionId);
                String department = user.employee.departmentId;
                List<Requisition> requisitions = requestServices.GetRequistions(department);
                //don't need to remove as DH can see all requisitions by the dept
                //requisitions.RemoveAll(x => x.Employee.name != user.employee.name);
                requisitions.Sort();
                ViewData["sessionId"] = sessionId;
                ViewData["staffname"] = user.employee.empName;
                return View(requisitions);
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

    }
}