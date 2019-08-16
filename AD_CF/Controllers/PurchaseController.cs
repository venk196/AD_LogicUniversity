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
    public class PurchaseController : Controller
    {
        // GET: Purchase
        UserServices userServices = new UserServices();
        public ActionResult CreatePurchase(string sessionId)
        {
            if (sessionId != null && userServices.getUserCountBySessionId(sessionId) == true)
            {

                using (var db = new InventoryDbContext())
                {
                    List<ProductCatalogue> pc = new List<ProductCatalogue>();
                    //                List<Inventory> inv = new List<Inventory>();
                    List<ProductSupplier> ps = new List<ProductSupplier>();
                    //                inv = db.inventories.ToList();
                    pc = db.productCatalogues.ToList();
                    ps = db.productSuppliers.ToList();
                    foreach (var pcs in pc)
                    {
                        //                    foreach (var invs in inv)
                        //                    {
                        //                        if (pcs.itemNumber == invs.itemNumber)
                        //                        {
                        //                            pcs.quantity = invs.quantity;
                        //                        }
                        //                    }
                        foreach (var pss in ps)
                        {
                            if (pss.itemNumber == pcs.itemNumber)
                            {
                                if (pss.preference == 1)
                                {
                                    pcs.supplier1 = pss.id;
                                }
                                else if (pss.preference == 2)
                                {
                                    pcs.supplier2 = pss.id;
                                }
                                else if (pss.preference == 3)
                                {
                                    pcs.supplier3 = pss.id;
                                }
                            }
                        }
                    }
                    db.SaveChanges();
                    ViewData["pc"] = pc;
                    //ViewData["inv"] = inv;
                    ViewData["ps"] = ps;
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

        public ActionResult SaveOrders(FormCollection collection,string sessionId)
        {
            if (sessionId != null && userServices.getUserCountBySessionId(sessionId) == true)
            {

                using (var db = new InventoryDbContext())
                {
                    if (collection["select"] == null)
                    {
                        return Content("<script>alert('Please select at least one item.');history.go(-1);</script>");

                    }
                    string[] select = collection["select"].Split(',');
                    for (int i = 0; i < select.Length; i++)
                    {
                        int itemqty = int.Parse(collection[select[i]]);
                        string suppliertag = select[i] + "-supplier";
                        string supplier = collection[suppliertag];
                        string sId = sessionId;
                        if (supplier == "blank")
                        {
                            return Content("<script>alert('Please select the correct supplier.');history.go(-1);</script>");
                        }
                        else
                        {
                            PurchaseOrder purchaseOrder = new PurchaseOrder();
                            purchaseOrder.quantity = itemqty;
                            purchaseOrder.supplierName = supplier;
                            ProductSupplier productSupplier = db.productSuppliers.Where(x => x.id == supplier).FirstOrDefault();
                            purchaseOrder.price = productSupplier.price;
                            purchaseOrder.totalPrice = purchaseOrder.price * purchaseOrder.quantity;
                            purchaseOrder.orderDate = DateTime.Now.ToShortDateString().ToString();
                            db.purchaseOrders.AddOrUpdate(purchaseOrder);
                            //db.SaveChanges();
                        }
                    }
                    db.SaveChanges();
                    return RedirectToAction("CreatePurchase");
                }
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }

        }
    }
}