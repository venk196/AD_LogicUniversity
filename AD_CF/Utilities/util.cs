using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using AD_CF.Models;
using AD_CF.Context;

namespace AD_CF.Utilities
{
    public class util
    {
        public static ProductCatalogue GetProductByItemNumber(String itemNumber)
        {
            using (var db = new InventoryDbContext ())
            {
                ProductCatalogue product = db.productCatalogues.Where(x => x.itemNumber == itemNumber).FirstOrDefault();
                return product;
            }
        }

        public static int addquan(string cmd, String Quantity)
        {
            int quan = 0;
            quan = Convert.ToInt32(Quantity);
            if (cmd != "minus")
            {
                quan = quan + 1;
            }

            if (cmd == "minus")
            {
                quan = quan - 1;
            }
            return quan;

        }

        public static void ValidateDelegateStatus(string sessionId)
        {
            using (var db = new InventoryDbContext())
            {
                var user = db.users.Where(x => x.sessionId == sessionId).FirstOrDefault();
                var list = db.delegations.Where(x => x.employee.departmentId == user.employee.departmentId).ToList(); //have all the delegations for a particular department

                foreach (Delegation x in list)
                {
                    if (x.endDate < DateTime.Now)
                    {
                        x.status = "INACTIVE";                        
                        db.delegations.AddOrUpdate(x);
                        db.SaveChanges();
                    }
                }
            }
        }

        public static bool IsDateValid(DateTime start, DateTime end)
        {
            if (start > end)
                return false; //not valid
            if (start.Date < DateTime.Now.Date)
                return false;//past date
            return true;
        }
       
    }
}