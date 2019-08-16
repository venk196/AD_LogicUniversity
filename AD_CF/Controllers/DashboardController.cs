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
    public class DashboardController : Controller
    {
        RequestServices requestServices = new RequestServices();
        UserServices userServices = new UserServices();
        // GET
        public ActionResult DBStoreMgr(string sessionId)
        {
            if (sessionId != null && userServices.getUserCountBySessionId(sessionId) == true)
            {
                using (var db = new InventoryDbContext())
                {

                    User user = db.users.Where(x => x.sessionId == sessionId).First();
                    ViewData["staffname"] = user.employee.empName;
                    ViewData["sessionId"] = sessionId;
                    return View();
                }
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        public ActionResult DBStoreClerk(string sessionId)
        {
            if (sessionId != null && userServices.getUserCountBySessionId(sessionId) == true)
            {
                using (var db = new InventoryDbContext())
                {

                    User user = db.users.Where(x => x.sessionId == sessionId).First();
                    ViewData["staffname"] = user.employee.empName;
                    ViewData["sessionId"] = sessionId;
                    return View();
                }
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        public ActionResult DBDeptHead(string sessionId)
        {
            if (sessionId != null && userServices.getUserCountBySessionId(sessionId) == true)
            {
                using (var db = new InventoryDbContext())
                {
                    User user = db.users.Where(x => x.sessionId == sessionId).First();
                    String department = user.employee.departmentId;
                    List<Requisition> departmentRequisitions = requestServices.getAllUncompleteRequisitions(department);
                    departmentRequisitions.Sort();

                    //use requisitions to create a list of all requisitions to find sum of all items and their quantities
                    PendingItemsModel pendingItems = new PendingItemsModel();
                    List<ProductCatalogue> productCatalogues = db.productCatalogues.ToList();
                    foreach (Requisition requisition in departmentRequisitions)
                    {
                        foreach (ProductReq productReq in requisition.productReqs)
                        {
                            ProductCatalogue pc = productCatalogues.Where(x => x.itemNumber == productReq.productitemnumber).FirstOrDefault();
                            string location = pc.location;
                            string unit = pc.unitofmeasure;
                            pendingItems.add(productReq.productitemnumber, productReq.productDesc, unit, productReq.quantity, location);
                        }
                    }

                    //sort pendingitems?
                    ViewData["staffname"] = user.employee.empName;
                    ViewData["sessionId"] = sessionId;
                    return View(pendingItems);
                }
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        public ActionResult DBDeptStaff(string sessionId)
        {
            //as staff, main page to show current pending items and qty that are from requests that are not 
            //cancelled, completed, or partialcomplete
            if (sessionId != null && userServices.getUserCountBySessionId(sessionId) == true)
            {
                using (var db = new InventoryDbContext())
                {
                    User user = db.users.Where(x => x.sessionId == sessionId).FirstOrDefault();
                    String department = user.employee.departmentId;
                    List<Requisition> departmentRequisitions = requestServices.getAllUncompleteRequisitions(department);
                    departmentRequisitions.Sort();
                    //use requisitions to create a list of all requisitions to find sum of all items and their quantities
                    PendingItemsModel pendingItems = new PendingItemsModel();
                    List<ProductCatalogue> productCatalogues = db.productCatalogues.ToList();
                    foreach (Requisition requisition in departmentRequisitions)
                    {
                        foreach (ProductReq productReq in requisition.productReqs)
                        {
                            ProductCatalogue pc = productCatalogues.Where(x => x.itemNumber == productReq.productitemnumber).FirstOrDefault();
                            string location = pc.location;
                            string unit = pc.unitofmeasure;
                            pendingItems.add(productReq.productitemnumber, productReq.productDesc, unit, productReq.quantity, location);
                        }
                    }
                    //sort pendingitems?
                    ViewData["staffname"] = user.employee.empName;
                    ViewData["sessionId"] = sessionId;
                    return View(pendingItems);
                }
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }
    }
}