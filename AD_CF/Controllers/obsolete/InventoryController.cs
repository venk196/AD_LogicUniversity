using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AD_CF.Context;
using AD_CF.Models;

namespace AD_CF.Controllers
{
    public class InventoryController : Controller
    {
        // GET: Inventory
        public ActionResult Stocklist()
        {
            using (var db = new InventoryDbContext())
            {
                List<ProductCatalogue> pc = new List<ProductCatalogue>();
                List<ProductSupplier> ps = new List<ProductSupplier>();
                List<Inventory> inv = new List<Inventory>();
                pc = db.productCatalogues.ToList();
                ps = db.productSuppliers.ToList();
                inv = db.inventories.ToList();
                foreach (var pcs in pc)
                {
                    foreach (var invs in inv)
                    {
                        if (pcs.itemNumber == invs.itemNumber)
                        {
                            pcs.quantity = invs.quantity;
                        }
                    }
                    foreach (var pss in ps)
                    {
                        if (pss.itemNumber == pcs.itemNumber)
                        {
//                            if (pss.preference == "1")
//                            {
//                                pcs.supplier1 = pss.supplierName;
//                            }
//                            else if (pss.preference == "2")
//                            {
//                                pcs.supplier2 = pss.supplierName;
//                            }
//                            else if (pss.preference == "3")
//                            {
//                                pcs.supplier3 = pss.supplierName;
//                            }
                        }
                    }
                }
                db.SaveChanges();
                ViewData["pc"] = pc;
                ViewData["inv"] = inv;
                return View();
            }

        }
        public ActionResult Viewmovement(string itemNumber)
        {
            using (var db = new InventoryDbContext())
            {
                List<StockMovement> sm = new List<StockMovement>();
                sm = db.stockmovements.ToList();
                List<StockMovement> smt = new List<StockMovement>();
                foreach (var sms in sm)
                {
                    if (sms.itemNumber == itemNumber)
                    {
                        smt.Add(sms);
                    }
                }
                ViewData["smt"] = smt;
                return View();
            }
        }

        public ActionResult EditStock(string itemNumber)
        {
            using (var db = new InventoryDbContext())
            {
                List<ProductCatalogue> pc = new List<ProductCatalogue>();
                pc = db.productCatalogues.ToList();
                List<ProductCatalogue> pcl = new List<ProductCatalogue>();
                foreach(var pcs in pc)
                {
                    if (pcs.itemNumber== itemNumber)
                    {
                        pcl.Add(pcs);
                    }
                }
                ViewData["pcl"] = pcl;
                return View();
            }
        }

        public ActionResult UpdateMovement(string itemNumber, int newqty)
        {
            using (var db = new InventoryDbContext())
            {
                Inventory a = db.inventories.Where(x => x.itemNumber == itemNumber).FirstOrDefault();
                ProductCatalogue pc = db.productCatalogues.Where(x => x.itemNumber == itemNumber).FirstOrDefault();
                StockMovement sm = new StockMovement();
                sm.movementDate = DateTime.Now;
                //sm.movementDescription = pc.supplier1;
                sm.movementQuantity = newqty-a.quantity;
                sm.movementBalance = newqty;
                sm.itemNumber = itemNumber;
                db.stockmovements.Add(sm);
                a.quantity = newqty;
                pc.quantity = newqty;
                db.SaveChanges();
                ViewData["pc"] = pc;
                return RedirectToAction("Stocklist");
            }
        }
    }
}