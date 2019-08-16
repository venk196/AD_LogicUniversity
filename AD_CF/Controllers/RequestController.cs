using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AD_CF.Context;
using AD_CF.Models;
using AD_CF.Utilities;

namespace AD_CF.Controllers
{
    public class RequestController : Controller
    {
        RequestServices requestServices = new RequestServices();
        UserServices userServices = new UserServices();


        [HttpGet]
        public ActionResult Products(string sessionId, string msg)
        {
            if (sessionId != null && userServices.getUserCountBySessionId(sessionId) == true)
            {

                if (msg != null)
                {
                    ViewData["msg"] = msg;
                }
                ViewData["sessionId"] = sessionId;
                return View(requestServices.getAllProducts());
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }
        
        [HttpPost]
        public ActionResult Products(FormCollection formCollection, string sessionId)
        {
            if (sessionId != null && userServices.getUserCountBySessionId(sessionId) == true)
            {

                string selectedProductsString = formCollection["selectedProducts"];
                if (selectedProductsString == null)
                {
                    return RedirectToAction("Products", "Request", new { sessionId, msg = "*No items selected.*" });
                }
                return RedirectToAction("SelectQuantity", "Request", new { sessionId, selectedProductsString });
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        [HttpGet]
        public ActionResult SelectQuantity(string sessionId, string selectedProductsString, string msg)
        {
            if (sessionId != null && userServices.getUserCountBySessionId(sessionId) == true)
            {

                string[] selectedProducts = selectedProductsString.Split(',');
                //use itemcode to create string for requesteditems
                string requeststring = requestServices.getRequestedProductAndQuantityString(selectedProducts);
                List<ProductReq> requests = requestServices.returnProductRequestsasList(requeststring);
                ViewData["selectedProductsString"] = selectedProductsString;
                ViewData["sessionId"] = sessionId;
                if (msg != null)
                {
                    ViewData["msg"] = msg;
                }
                return View(requests);
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }
        
        [HttpPost]
        public ActionResult SelectQuantity(FormCollection formCollection, string sessionId, string selectedProductsString)
        {
            if (sessionId != null && userServices.getUserCountBySessionId(sessionId) == true)
            {
                string[] selectedProducts = selectedProductsString.Split(',');
                string requeststring = requestServices.getRequestedProductAndQuantityString(selectedProducts);
                List<ProductReq> requests = requestServices.returnProductRequestsasList(requeststring);
                //in formcollection, qty is value, where the key is item number + '-qty'. retreive to create list of ProdReq for request
                foreach (ProductReq req in requests)
                {
                    req.quantity = int.Parse(formCollection[(req.productitemnumber + "-qty")]);
                }
                //remove if qty is zero as it is not necessary to keep it in the request
                requests.RemoveAll(x => x.quantity == 0);

                //add
                if (requests.Count == 0)
                {
                    return RedirectToAction("SelectQuantity", "Request", new { sessionId, selectedProductsString, msg = "*No quantity selected.*" });
                }
                else
                {
                    requestServices.addNewRequisition("Pending Approval", DateTime.Now, requests, sessionId);
                }
                return RedirectToAction("DBDeptStaff", "Dashboard", new { sessionId });
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }

        }

       

        public ActionResult DSViewHistory(string sessionId)
        {
            if (sessionId != null && userServices.getUserCountBySessionId(sessionId) == true)
            {

                using (var db = new InventoryDbContext())
                {
                    User user = db.users.Where(x => x.sessionId == sessionId).FirstOrDefault();
                    String department = user.employee.departmentId;
                    List<Requisition> requisitions = requestServices.GetRequistions(department);
                    requisitions.RemoveAll(x => x.Employee.empName != user.employee.empName);
                    requisitions.Sort();
                    ViewData["sessionId"] = sessionId;
                    return View(requisitions);
                }
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }


        public ActionResult CancelRequest(int reqformnumber, string sessionId)
        {
            if (sessionId != null && userServices.getUserCountBySessionId(sessionId) == true)
            {

                using (var db = new InventoryDbContext())
                {
                    Requisition requistion = requestServices.GetRequistion(reqformnumber);
                    requistion.status = "Cancelled";
                    db.requisitions.AddOrUpdate(requistion);
                    db.SaveChanges();
                }

                return RedirectToAction("DSViewHistory", "Request", new { sessionId });
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }

        }


    }
}