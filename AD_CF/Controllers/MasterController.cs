using AD_CF.Context;
using AD_CF.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AD_CF.Utilities;
using Newtonsoft.Json;

namespace AD_CF.Controllers
{
    public class MasterController : Controller
    {

        
        UserServices userServices = new UserServices();
        
        [HttpGet]
        public ActionResult SupplierList(string sessionId)
        {
            if (sessionId != null && userServices.getUserCountBySessionId(sessionId) == true)
            {
                using (var db = new InventoryDbContext())
                {
                    List<Suppliers> supplier = db.supplier.ToList();
                    ViewData["sessionId"] = sessionId;
                    ViewData["Suppliers"] = supplier;
                    ViewData["staffname"] = userServices.getUserBySessionId(sessionId).employee.empName;
                    return View(supplier);
                }
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        public ActionResult SupplierListNew(string sessionId)
        {
            if (sessionId != null && userServices.getUserCountBySessionId(sessionId) == true)
            {

                Suppliers s = new Suppliers();

                using (var db = new InventoryDbContext())
                {
                    List<Suppliers> supplier = db.supplier.ToList();
                    ViewData["sessionId"] = sessionId;
                    ViewData["Suppliers"] = supplier;
                    ViewData["staffname"] = userServices.getUserBySessionId(sessionId).employee.empName;
                    ViewData["s"] = s;
                    return View();
                }
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }


        [HttpPost]
        public ActionResult InsertSupplier(Suppliers s, string sessionId)
        {
            if (sessionId != null && userServices.getUserCountBySessionId(sessionId) == true)
            {


                using (var db = new InventoryDbContext())
                {

                    db.supplier.Add(s);
                    db.SaveChanges();
                }


                return RedirectToAction("SupplierList", new { sessionId });
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }

        }

        public ActionResult SupplierListEdit(int id, string sessionId)
        {
            if (sessionId != null && userServices.getUserCountBySessionId(sessionId) == true)
            {


                using (var db = new InventoryDbContext())
                {
                    Suppliers s = db.supplier.Where(x => x.supplierId == id).FirstOrDefault();
                    ViewData["s"] = s;
                    ViewData["sessionId"] = sessionId;
                    ViewData["staffname"] = userServices.getUserBySessionId(sessionId).employee.empName;
                    return View();
                }
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

   //         [HttpPost]
        public ActionResult UpdateSupplier(Suppliers s, string sessionId)
        {
            if (sessionId != null && userServices.getUserCountBySessionId(sessionId) == true)
            {

                using (var db = new InventoryDbContext())
                {

                    Suppliers updatedSupplier = db.supplier.Where(x => x.supplierId == s.supplierId).FirstOrDefault();

                    updatedSupplier.supplierCode = s.supplierCode;
                    updatedSupplier.supplierName = s.supplierName;
                    updatedSupplier.gstNo = s.gstNo;
                    updatedSupplier.contactName = s.contactName;
                    updatedSupplier.phoneNo = s.phoneNo;
                    updatedSupplier.faxNo = s.faxNo;
                    updatedSupplier.address = s.address;
                    db.supplier.AddOrUpdate(updatedSupplier);
                    db.SaveChanges();
                }
                ViewData["staffname"] = userServices.getUserBySessionId(sessionId).employee.empName;
                return RedirectToAction("SupplierList", new { sessionId });
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

       
        public ActionResult DeleteSupplier(int id, string sessionId)
        {
            if (sessionId != null && userServices.getUserCountBySessionId(sessionId) == true)
            {

                using (var db = new InventoryDbContext())
                {
                    Suppliers ss = db.supplier.Where(x => x.supplierId == id).FirstOrDefault();
                    db.supplier.Remove(ss);
                    db.SaveChanges();
                }
                ViewData["staffname"] = userServices.getUserBySessionId(sessionId).employee.empName;
                return RedirectToAction("SupplierList", new { sessionId });
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        [HttpGet]
        public ActionResult DeptList(string sessionId)
        {
            if (sessionId != null && userServices.getUserCountBySessionId(sessionId) == true)
            {


                using (var db = new InventoryDbContext())
                {
                    List<Department> dept = db.departments.ToList();
                    ViewData["Department"] = dept;
                    ViewData["sessionId"] = sessionId;
                    ViewData["staffname"] = userServices.getUserBySessionId(sessionId).employee.empName;
                    return View(dept);
                }
               
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }


        [HttpGet]
        public ActionResult Catalogue(string sessionId)
        {
            if (sessionId != null && userServices.getUserCountBySessionId(sessionId) == true)
            {

                using (var db = new InventoryDbContext())
                {
                    List<ProductCatalogue> catalogue = db.productCatalogues.ToList();
                    catalogue.Sort();
                    ViewData["sessionId"] = sessionId;
                    ViewData["Catalogue"] = catalogue;
                    ViewData["staffname"] = userServices.getUserBySessionId(sessionId).employee.empName;
                    return View(catalogue);
                }
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }
        
        public ActionResult CatalogueNew(string sessionId)
        {
            if (sessionId != null && userServices.getUserCountBySessionId(sessionId) == true)
            {

                ProductCatalogue p = new ProductCatalogue();

                using (var db = new InventoryDbContext())
                {
                    List<ProductCatalogue> catalogue = db.productCatalogues.ToList();

                    ViewData["Catalogue"] = catalogue;
                    ViewData["sessionId"] = sessionId;
                    ViewData["staffname"] = userServices.getUserBySessionId(sessionId).employee.empName;
                    ViewData["p"] = p;
                    return View();
                }
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

   
        public ActionResult InsertCatalogue(ProductCatalogue p, string sessionId)
        {
            if (sessionId != null && userServices.getUserCountBySessionId(sessionId) == true)
            {

                using (var db = new InventoryDbContext())
                {
                    //initiate quantity at zero
                    p.quantity = 0;
                    db.productCatalogues.Add(p);

                    db.SaveChanges();

                }
                return RedirectToAction("Catalogue", new { sessionId });
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        
        public ActionResult CatalogueEdit(int id, string sessionId)
        {
            if (sessionId != null && userServices.getUserCountBySessionId(sessionId) == true)
            {

                using (var db = new InventoryDbContext())
                {
                    ProductCatalogue p = db.productCatalogues.Where(d => d.id == id).FirstOrDefault();
                    ViewData["p"] = p;
                    ViewData["sessionId"] = sessionId;
                    return View();
                }
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }
        
        [HttpPost]
        public ActionResult UpdateCatalogue(ProductCatalogue p, string sessionId)
        {
            if (sessionId != null && userServices.getUserCountBySessionId(sessionId) == true)
            {

                using (var db = new InventoryDbContext())
                {

                    ProductCatalogue updatedCatalogue = db.productCatalogues.Where(x => x.id == p.id).FirstOrDefault();
                    //updatedCatalogue.id = pcat.id;
                    updatedCatalogue.itemNumber = p.itemNumber;
                    updatedCatalogue.category = p.category;
                    updatedCatalogue.description = p.description;
                    updatedCatalogue.reorderlevel = p.reorderlevel;
                    updatedCatalogue.reorderquantity = p.reorderquantity;
                    updatedCatalogue.unitofmeasure = p.unitofmeasure;
                    db.productCatalogues.AddOrUpdate(updatedCatalogue);
                    db.SaveChanges();
                    return RedirectToAction("Catalogue", new { sessionId });
                }
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }

        }



        public ActionResult DeleteCatalogue(int id, string sessionid)
        {
            if (sessionid != null && userServices.getUserCountBySessionId(sessionid) == true)
            {
                using (var db = new InventoryDbContext())
                {
                    ProductCatalogue pdtcat = db.productCatalogues.Where(x => x.id == id).FirstOrDefault();
                    db.users.Where(x => x.sessionId == sessionid).FirstOrDefault();                    
                    Session["sessionid"] = sessionid;
                    db.productCatalogues.Remove(pdtcat);
                    db.SaveChanges();
                }

                return RedirectToAction("Catalogue", new { sessionid });
            }

            else
            {
                return RedirectToAction("Login", "Login");
            }
        }
        
    }

}

