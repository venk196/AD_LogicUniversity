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
    public class ReportController : Controller
    {
        // GET
        public ActionResult Trend()
        {
            return View();
        }

        public ActionResult Others2(string sessionId)
        {
            using (var db = new InventoryDbContext())
            {
                List<Requisition> req = new List<Requisition>();
                req = db.requisitions.ToList();
                List<Requisition> requ = new List<Requisition>();
                List<Department> dp = new List<Department>();
                dp = db.departments.ToList();
                ProductCatalogue pc = new ProductCatalogue();
                List<double> total = new List<double>();
                List<string> department = new List<string>();
                ProductSupplier ps = new ProductSupplier();
                double totalprice = 0;
                foreach (var dps in dp)
                {
                    foreach (var reqs in req)
                    {
                        if (dps.id == reqs.department)
                        {
                            if (reqs.status == "approved" || reqs.status == "partial")
                            {
                                if (reqs.datecompleted != null && reqs.datecompleted.Value.Month == DateTime.Now.AddMonths(-1).Month && reqs.datecompleted.Value.Year == DateTime.Now.Year)
                                {
                                    requ.Add(reqs);
                                }
                            }
                        }
                    }
                    foreach (var requs in requ)
                    {
                        foreach (var productReq in requs.productReqs)
                        {
                            pc = db.productCatalogues.Where(x => x.itemNumber == productReq.productitemnumber).FirstOrDefault();
                            ps = db.productSuppliers.Where(x => x.itemNumber == productReq.productitemnumber && x.preference == 1).FirstOrDefault();
                            totalprice = totalprice + pc.quantity * ps.price;//I can't find the supplier in Requisition and ProductReq model, so i can't get the price, just add one temperory¡£
                        }
                    }
                    total.Add(totalprice);
                    department.Add(dps.departmentName);
                }
                ViewData["total"] = total;
                ViewData["department"] = department;
                return View();
            }
        }

        public ActionResult Others(string sessionId)
        {
            using (var db = new InventoryDbContext())
            {
                List<Requisition> rt = new List<Requisition>();
                ItemQuantity iq = new ItemQuantity();
                rt = db.requisitions.ToList();
                foreach (var rts in rt)
                {
                    foreach (var proReqs in rts.productReqs)
                    {
                        iq.add(proReqs.productitemnumber, proReqs.productDesc, proReqs.unitOfMeasure, proReqs.deliveredQuantity);
                    }

                }
                return View(iq);
            }

        }

    }
}
