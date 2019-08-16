using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AD_CF.Context;
using AD_CF.Models;

namespace AD_CF.Controllers
{
    public class ItemController : Controller
    {
        // GET: Item
        public ActionResult Index(string supplierName,string itemNo)
        {
            using (var db = new InventoryDbContext())
            {
                ProductSupplier ps = new ProductSupplier();
                if (supplierName == "blank")
                {
                    return null;
                }
                else
                {
                    ps = db.productSuppliers.Where(x => x.id == supplierName && x.itemNumber == itemNo).FirstOrDefault();
                    return Json(new { supplierName = supplierName, itemNo = itemNo, price = ps.price, }, JsonRequestBehavior.AllowGet);
                }
            }
        }
    }
}