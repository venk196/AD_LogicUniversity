using AD_CF.Context;
using AD_CF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AD_CF.Utilities;


namespace AD_CF.Controllers
{
    public class StockAdjmController : Controller
    {
        UserServices userServices = new UserServices();

        public ActionResult IssueVoucher(string sessionId)
        {
            if (sessionId != null && userServices.getUserCountBySessionId(sessionId) == true)
            {
                using (var db = new InventoryDbContext())
                {
                    double totalvalue = 0;
                    List<Voucher> v = db.vouchers.Where(x => x.status == "Pending").ToList();
                    List<DateTime> d = db.vouchers.Where(x => x.status == "Pending").Select(x => x.voucherDate).ToList();
                    List<DateTime> aa = new List<DateTime>();//get unique date
                    foreach (var dd in d)
                    {
                        if (aa.Count == 0)
                            aa.Add(dd.Date);
                        else
                        {
                            Boolean b = false;
                            for (int i = 0; i < aa.Count; i++)
                            {
                                if (dd.Date.Equals(aa[i].Date))
                                    b = true;
                            }
                            if (b == false)
                                aa.Add(dd.Date);
                        }
                    };
                    //sort by date. Group all similiar date to one "single voucher"
                    Dictionary<DateTime, double> dic = new Dictionary<DateTime, double>();
                    foreach (var xx in aa)
                    {
                        totalvalue = 0;
                        foreach (var yy in v)
                        {
                            if (xx.Date.Equals(yy.voucherDate.Date))
                            {
                                totalvalue = totalvalue + yy.price;
                            }
                        }
                        dic.Add(xx, totalvalue);
                    }
                    ViewData["dic"] = dic;
                    ViewData["totalvalue"] = totalvalue;
                    ViewData["date"] = aa;
                    ViewData["vouchers"] = v;
                }
                ViewData["sessionId"] = sessionId;
                ViewData["staffname"] = userServices.getUserBySessionId(sessionId).employee.empName;
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        public ActionResult selectedVoucherS(List<DateTime> select, string sessionId)
        {
            if (sessionId != null && userServices.getUserCountBySessionId(sessionId) == true)
            {

                using (var db = new InventoryDbContext())
                {
                    foreach (DateTime i in select)
                    {
                        List<Voucher> vouchers = db.vouchers.ToList();
                        for (int e = 0; e < vouchers.Count; e++)
                        {
                            if (i.Equals(vouchers[e].voucherDate.Date))
                                vouchers[e].status = "approvedBySup";
                        }
                    }
                    db.SaveChanges();
                }
                return RedirectToAction("VoucherHistoryClerk", new { sessionId });
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        public ActionResult VoucherHistoryClerk(string sessionId)
        {
            if (sessionId != null && userServices.getUserCountBySessionId(sessionId) == true)
            {

                List<Voucher> vouchers = new List<Voucher>();

                using (var db = new InventoryDbContext())
                {
                    vouchers = db.vouchers.ToList();
                }
                vouchers.Sort();
                ViewData["sessionId"] = sessionId;
                ViewData["vouchers"] = vouchers;
                ViewData["staffname"] = userServices.getUserBySessionId(sessionId).employee.empName;
                return View();
            }

            else
            {
                return RedirectToAction("Login", "Login");
            }
        }



        public ActionResult ApproveVoucher(string sessionId)
        {
            if (sessionId != null && userServices.getUserCountBySessionId(sessionId) == true)
            {

                using (var db = new InventoryDbContext())
                {
                    double totalvalue = 0;
                    List<Voucher> v = db.vouchers.Where(x => x.status == "approvedBySup").ToList();
                    List<DateTime> d = db.vouchers.Select(x => x.voucherDate).ToList();
                    List<DateTime> aa = new List<DateTime>();//get unique date
                    foreach (var dd in d)
                    {
                        if (aa.Count == 0)
                            aa.Add(dd.Date);
                        else
                        {
                            Boolean b = false;
                            for (int i = 0; i < aa.Count; i++)
                            {
                                if (dd.Date.Equals(aa[i].Date))
                                    b = true;
                            }
                            if (b == false)
                                aa.Add(dd.Date);
                        }
                    };

                    Dictionary<DateTime, double> dic = new Dictionary<DateTime, double>();
                    foreach (var xx in aa)
                    {
                        totalvalue = 0;
                        foreach (var yy in v)
                        {
                            if (xx.Date.Equals(yy.voucherDate.Date))
                            {
                                totalvalue = totalvalue + yy.price;
                            }
                        }
                        if (totalvalue >= 250)
                            dic.Add(xx, totalvalue);
                    }
                    List<DateTime> bb = new List<DateTime>();//get date which total value >250
                    Dictionary<DateTime, double>.KeyCollection keyCol = dic.Keys;
                    foreach (DateTime n in keyCol)
                    {
                        bb.Add(n);
                    }
                    ViewData["dic"] = dic;
                    ViewData["totalvalue"] = totalvalue;
                    ViewData["date"] = bb;
                    ViewData["vouchers"] = v;
                }
                ViewData["sessionId"] = sessionId;
                ViewData["staffname"] = userServices.getUserBySessionId(sessionId).employee.empName;
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }


        public ActionResult selectedVoucherM(List<DateTime> select, string sessionId)
        {
            if (sessionId != null && userServices.getUserCountBySessionId(sessionId) == true)
            {

                using (var db = new InventoryDbContext())
                {
                    foreach (DateTime i in select)
                    {
                        List<Voucher> vouchers = db.vouchers.ToList();
                        for (int e = 0; e < vouchers.Count; e++)
                        {
                            if (i.Equals(vouchers[e].voucherDate.Date))
                                vouchers[e].status = "approvaledByManager";
                        }
                    }
                    db.SaveChanges();

                }
                return RedirectToAction("VoucherHistoryMgr", new { sessionId });
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }
        public ActionResult RejectVoucher(List<DateTime> select, string sessionId)
        {
            if (sessionId != null && userServices.getUserCountBySessionId(sessionId) == true)
            {
                using (var db = new InventoryDbContext())
                {
                    foreach (DateTime i in select)
                    {
                        List<Voucher> vouchers = db.vouchers.ToList();
                        for (int e = 0; e < vouchers.Count; e++)
                        {
                            if (i.Equals(vouchers[e].voucherDate.Date))
                                vouchers[e].status = "Reject";
                        }
                    }
                    db.SaveChanges();

                }
                return RedirectToAction("VoucherHistoryMgr", new { sessionId });
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        public ActionResult VoucherHistoryMgr(string sessionId)
        {
            if (sessionId != null && userServices.getUserCountBySessionId(sessionId) == true)
            {

                List<Voucher> vouchers = new List<Voucher>();

                using (var db = new InventoryDbContext())
                {
                    vouchers = db.vouchers.ToList();
                }
                vouchers.Sort();
                ViewData["sessionId"] = sessionId;
                ViewData["vouchers"] = vouchers;
                ViewData["staffname"] = userServices.getUserBySessionId(sessionId).employee.empName;
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }

        }
    }
}




