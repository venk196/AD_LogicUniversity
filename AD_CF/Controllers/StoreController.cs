using AD_CF.Context;
using AD_CF.Models;
using AD_CF.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Schema;

namespace AD_CF.Controllers
{
    public class StoreController : Controller
    {
        RequestServices requestServices = new RequestServices();
        UserServices userServices = new UserServices();
        
        
        public ActionResult Requisitions(string sessionId, string msg)
        {
            if (sessionId != null && userServices.getUserCountBySessionId(sessionId) == true)
            {

                PendingItemsModel retrieval = new PendingItemsModel();
                List<int> matchingItemInventory = new List<int>();
                string requisitionnumbers = "";
                using (var db = new InventoryDbContext())
                {
                    List<Requisition> pendingReqs = db.requisitions.Where(x => x.status == "Approved").ToList();
                    //sort earliest first
                    pendingReqs.Sort();
                    pendingReqs.Reverse();
                    //use requisitions to create a list of all requisitions to find sum of all items and their quantities
                    foreach (Requisition requisition in pendingReqs)
                    {
                        //add requisition to string of requisition numbers
                        if (requisitionnumbers == "")
                        {
                            requisitionnumbers = requisitionnumbers + requisition.reqformNumber;
                        }
                        else
                        {
                            requisitionnumbers = requisitionnumbers + "," + requisition.reqformNumber;
                        }
                        //add product request from requisition to pending product model
                        List<ProductCatalogue> productCatalogues = db.productCatalogues.ToList();
                        foreach (ProductReq productReq in requisition.productReqs)
                        {
                            ProductCatalogue pc = productCatalogues.Where(x => x.itemNumber == productReq.productitemnumber)
                                .FirstOrDefault();
                            string location = pc.location;
                            string unit = pc.unitofmeasure;
                            retrieval.add(productReq.productitemnumber, productReq.productDesc, unit,
                                productReq.quantity, location);
                        }
                    }
                    for (int i = 0; i < retrieval.itemName.Count(); i++)
                    {
                        //for each item, find its corresponding inventory stock amount
                        string itemNumber = retrieval.itemNumber[i];
                        ProductCatalogue p = db.productCatalogues.Where(x => x.itemNumber == itemNumber).FirstOrDefault();
                        matchingItemInventory.Add(p.quantity);
                    }
                }
                ViewData["itemInventory"] = matchingItemInventory;
                ViewData["sessionId"] = sessionId;
                ViewData["requisitions"] = requisitionnumbers;
                ViewData["msg"] = msg;
                ViewData["staffname"] = userServices.getUserBySessionId(sessionId).employee.empName;
                return View(retrieval);
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }
       

        
        public ActionResult DoDisbursement(FormCollection formCollection, string sessionId, string requisitions)
        {
            if (sessionId != null && userServices.getUserCountBySessionId(sessionId) == true)
            {

                string[] items = formCollection["disburseitems"].Split(',');
                int[] retrievedQty = new int[items.Length];
                for (int i = 0; i < items.Length; i++)
                {
                    retrievedQty[i] = int.Parse(formCollection[items[i] + "-qty"]);
                    //check if retrievedQty is errorenous
                    if (retrievedQty[i] < 0)
                    {
                        return RedirectToAction("Requisitions", new { sessionId, msg = "*Retrieved quantity cannot be negative.*" });
                    }
                }

                string[] requisitionsInvolved = requisitions.Split(',');
                //pull requisitions from db and update them on amount disbursing to them.
                //note: system allocates and fulfills earliest records first. Reverse the sort (sorts by latest)
                List<Requisition> involvedRequisitions = new List<Requisition>();
                using (var db = new InventoryDbContext())
                {
                    // after getting all retrieved itemqty deduct from inventory first before allocation
                    for (int i = 0; i < items.Length; i++)
                    {
                        string updateItemNumber = items[i];
                        ProductCatalogue pc = db.productCatalogues.Where(x => x.itemNumber == updateItemNumber).FirstOrDefault();

                        //add inventory changes to stockmovt model
                        StockMovement sm = new StockMovement();
                        sm.movementDate = DateTime.Now;
                        sm.movementDescription = "Disbursement";
                        sm.movementQuantity = -retrievedQty[i];
                        sm.movementBalance = -retrievedQty[i] + pc.quantity;
                        sm.itemNumber = updateItemNumber;
                        db.stockmovements.AddOrUpdate(sm);
                        db.SaveChanges();


                        pc.quantity -= retrievedQty[i];
                        //update StockList (voucher)
                        Voucher change = new Voucher();
                        change.voucherDate = DateTime.Now;
                        change.itemNumber = updateItemNumber;
                        change.price = 0;
                        change.quantityAdjusted = -(retrievedQty[i]);
                        change.reason = "Retrieval from stock for disbursement.";
                        change.status = "Automated";
                        db.vouchers.Add(change);
                    }

                    //now, logically fill up requests if possible, if not, max filling as much as possible
                    for (int i = 0; i < requisitionsInvolved.Length; i++)
                    {
                        int reqformNo = int.Parse(requisitionsInvolved[i]);
                        Requisition r = db.requisitions.Where(x => x.reqformNumber == reqformNo).FirstOrDefault();
                        //loop product request in requisition and try to fill up
                        foreach (ProductReq pr in r.productReqs)
                        {
                            //find corresponding item position, then deduct from qty if available.
                            //if deduction of requested qty is negative, means not enough quantity, then add whatever left
                            //remember to reduce amount of qty from retrievedQty
                            for (int j = 0; j < items.Length; j++)
                            {
                                if (pr.productitemnumber == items[j])
                                {
                                    if ((retrievedQty[j] - pr.quantity) <= 0)
                                    {
                                        pr.retrievedQuantity = retrievedQty[j];
                                        retrievedQty[j] = 0;
                                        //no more. Note that then from here on, other request with item will get zero
                                    }
                                    else
                                    {
                                        pr.retrievedQuantity = pr.quantity;
                                        retrievedQty[j] = retrievedQty[j] - pr.quantity;
                                        //reduce from retrievedQty and add to amount retrieved for requisition
                                    }
                                }
                            }
                        }
                        //after doing so for all product request, save the disbursement for r
                        r.status = "Disbursing";
                    }

                    db.SaveChanges();
                }
                //direct to actual Disbursement view
                return RedirectToAction("Disbursement", new { sessionId });
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        public ActionResult Disbursement(string sessionId)
        {
            if (sessionId != null && userServices.getUserCountBySessionId(sessionId) == true)
            {

                using (var db = new InventoryDbContext())
                {
                    //show disbursement of the day after retrieval and setting quantities for disbursement(automated)
                    List<Requisition> disbursements = requestServices.getDisbursementRequisitions();
                    List<Department> departments = requestServices.GetDepartments();
                    List<ProductCatalogue> pc = requestServices.getAllProducts().ToList();
                    User user = db.users.Where(x => x.sessionId == sessionId).First();
                    //for disbursements, show earliest first
                    disbursements.Sort();
                    disbursements.Reverse();
                    ViewData["sessionId"] = sessionId;
                    ViewData["disbursements"] = disbursements;
                    ViewData["departments"] = departments;
                    ViewData["staffname"] = user.employee.empName;



                }
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }

        }
        public ActionResult Acknowledge(FormCollection formCollection, string command, int reqNumber, string sessionId)
        {
            if (sessionId != null && userServices.getUserCountBySessionId(sessionId) == true)
            {

                if (command == "acknowledge")
                {
                    //formcollection has all actual quantities associated with requisition, collected by itemnumber-confirmqty.
                    //if all meet, status changed to confirmed, add delivered qty, end.
                    //add confirmqty to requisition, if does not match retrieved qty, have to add back to inventory.
                    //if fall short of requested qty, status close as partial. automatically add new requisition with same infos but only for items fallen short
                    bool complete = true;
                    using (var db = new InventoryDbContext())
                    {
                        Requisition req = db.requisitions.Where(x => x.reqformNumber == reqNumber).FirstOrDefault();
                        foreach (ProductReq item in req.productReqs)
                        {
                            int confirmedQty = int.Parse(formCollection[item.productitemnumber + "-confirmqty"]);
                            item.deliveredQuantity = confirmedQty;
                            if (item.retrievedQuantity != item.deliveredQuantity)
                            {
                                //add back to inventory (delivered is always equal or less than retrieved)
                                ProductCatalogue pc = db.productCatalogues.Where(x => x.itemNumber == item.productitemnumber).FirstOrDefault();
                                pc.quantity = pc.quantity + (item.retrievedQuantity - confirmedQty);
                                //update StockList (voucher)
                                Voucher change = new Voucher();
                                change.voucherDate = DateTime.Now;
                                change.itemNumber = item.productitemnumber;
                                change.price = 0;
                                change.quantityAdjusted = (item.retrievedQuantity - confirmedQty);
                                change.reason = "Discrepency between retrieved qty and confirmed qty.";
                                change.status = "Automated";
                                db.vouchers.Add(change);
                            }
                            if (item.deliveredQuantity < item.quantity)
                            {
                                complete = false;
                            }
                        }
                        //if not complete, set current req as partial and create new requisition for remaining items
                        if (complete == false)
                        {
                            req.status = "Partial";
                            //new requisition is alreay approved, same dates and personnel, awaiting disbursement
                            Requisition newReq = new Requisition();
                            newReq.approvedBy = req.approvedBy;
                            newReq.comment = "Auto-generated request for undelivered requests.";
                            newReq.dateapproved = req.dateapproved;
                            newReq.datecreated = req.datecreated;
                            newReq.department = req.department;
                            newReq.Employee = req.Employee;
                            newReq.status = "Approved";
                            //add ProductRequests
                            List<ProductReq> newProductReqs = new List<ProductReq>();
                            foreach (ProductReq pr in req.productReqs)
                            {
                                if (pr.deliveredQuantity < pr.quantity)
                                {
                                    ProductReq newPr = new ProductReq();
                                    newPr.productDesc = pr.productDesc;
                                    newPr.productitemnumber = pr.productitemnumber;
                                    newPr.quantity = pr.quantity - pr.deliveredQuantity;
                                    newPr.req = newReq;
                                    newPr.unitOfMeasure = pr.unitOfMeasure;
                                    newProductReqs.Add(newPr);
                                }
                            }
                            newReq.productReqs = newProductReqs;
                            db.requisitions.Add(newReq);
                        }
                        else
                        {
                            //if complete, set to complete. Nothing else to do
                            req.status = "Complete";
                        }
                        db.SaveChanges();
                    }

                    return RedirectToAction("Disbursement", new { sessionId });
                }
                else if (command == "cancel")
                {
                    Requisition r = new Requisition();

                    using (var db = new InventoryDbContext())
                    {
                        //Cancel is cancel disbursement, not the request. Set it back to Approved
                        r = db.requisitions.Where(x => x.reqformNumber == reqNumber).FirstOrDefault();
                        r.status = "Approved";
                        //add item qty back to inventory, remove retrievedqty from requisition
                        foreach (ProductReq item in r.productReqs)
                        {
                            ProductCatalogue pc = db.productCatalogues.Where(x => x.itemNumber == item.productitemnumber).FirstOrDefault();

                            //add inventory changes to stockmovt model
                            StockMovement sm = new StockMovement();
                            sm.movementDate = DateTime.Now;
                            sm.movementDescription = "Disbursement Cancelled";
                            sm.movementQuantity = item.retrievedQuantity;
                            sm.movementBalance = item.retrievedQuantity + pc.quantity;
                            sm.itemNumber = item.productitemnumber;
                            db.stockmovements.AddOrUpdate(sm);
                            db.SaveChanges();

                            //update inventory change in pc
                            pc.quantity += item.retrievedQuantity;
                            item.retrievedQuantity = 0;
                        }
                        db.SaveChanges();
                    }
                    return RedirectToAction("Disbursement", new { sessionId });
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }



        public ActionResult StockList(string sessionId, string msg)
        {
            if (sessionId != null && userServices.getUserCountBySessionId(sessionId) == true)
            {

                List<ProductCatalogue> inventories = new List<ProductCatalogue>();

                using (var db = new InventoryDbContext())
                {
                    inventories = db.productCatalogues.ToList();
                }
                inventories.Sort();
                if (msg != null)
                {
                    ViewData["msg"] = msg;

                }

                ViewData["sessionId"] = sessionId;
                ViewData["inventories"] = inventories;
                ViewData["staffname"] = userServices.getUserBySessionId(sessionId).employee.empName;


                return View();
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }



        public ActionResult updateInventory(FormCollection formcollection, string itemNumber, string reason, string sessionId)
        {
            if (sessionId != null && userServices.getUserCountBySessionId(sessionId) == true)
            {

                using (var db = new InventoryDbContext())
                {


                    ProductCatalogue a = db.productCatalogues.Where(x => x.itemNumber == itemNumber).FirstOrDefault();
                    ProductSupplier ps = db.productSuppliers.Where(x => x.itemNumber == a.itemNumber).FirstOrDefault();
                    int adjqty = int.Parse(formcollection[itemNumber + "-adjqty"]);
                    if (adjqty < 0)
                    {
                        if (System.Math.Abs(adjqty) > a.quantity)
                        {
                            return RedirectToAction("StockList", "Store", new { sessionId, msg = "*Quantity cannot diminish to less than 0.*" });
                        }
                        else
                        {
                            Voucher v = new Voucher();
                            v.quantityAdjusted = adjqty;
                            v.status = "Pending";
                            v.voucherDate = DateTime.Now;
                            v.itemNumber = itemNumber;
                            v.itemName = a.description;
                            v.price = ps.price * System.Math.Abs(v.quantityAdjusted);
                            v.reason = reason;
                            db.vouchers.Add(v);
                        }


                    }


                    ProductCatalogue pc = db.productCatalogues.Where(x => x.itemNumber == itemNumber).FirstOrDefault();
                    StockMovement sm = new StockMovement();
                    if (adjqty < 0)
                    {
                        sm.movementDescription = "Manual Stock Adjustment";
                    }
                    else
                    {
                        sm.movementDescription = "Delivery from supplier";
                    }
                    sm.movementDate = DateTime.Now;
                    sm.movementQuantity = adjqty;
                    sm.movementBalance = adjqty + pc.quantity;
                    sm.itemNumber = itemNumber;
                    db.stockmovements.AddOrUpdate(sm);
                    db.SaveChanges();


                    a.quantity = a.quantity + adjqty;
                    db.SaveChanges();
                    return RedirectToAction("StockList", new { sessionId });
                }
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        public ActionResult StockListMovt(string itemNumber, string sessionId)
        {
            if (sessionId != null && userServices.getUserCountBySessionId(sessionId) == true)
            {


                using (var db = new InventoryDbContext())
                {

                    List<StockMovement> itemStockMovement =
                        db.stockmovements.Where(x => x.itemNumber == itemNumber).ToList();
                    ProductCatalogue pc = db.productCatalogues.Where(x => x.itemNumber == itemNumber).FirstOrDefault();

                    ViewData["itemNumber"] = itemNumber;
                    ViewData["itemDesc"] = pc.description;
                    ViewData["uom"] = pc.unitofmeasure;
                    ViewData["smt"] = itemStockMovement;
                    ViewData["staffname"] = userServices.getUserBySessionId(sessionId).employee.empName;
                    ViewData["sessionId"] = sessionId;

                    return View();
                }
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }
        
    }
}