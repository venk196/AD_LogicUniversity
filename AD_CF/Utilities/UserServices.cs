using AD_CF.Context;
using AD_CF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AD_CF.Utilities
{
    public class UserServices
    {
        InventoryDbContext db = new InventoryDbContext();
        
        public User getUserBySessionId(string sessionId)
        {
            return db.users.Where(x => x.sessionId == sessionId).FirstOrDefault();
        }

        public bool getUserCountBySessionId(string sessionId)
        {
            if (db.users.Any(x => x.sessionId == sessionId))
                return true;
            return false;
        }
    }
}