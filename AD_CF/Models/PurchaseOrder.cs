using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AD_CF.Models
{
    public class PurchaseOrder
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int orderNumber { get; set; } 
        public int quantity { get; set; }
        public string supplierName { get; set; }
        public double price { get; set; }
        public double totalPrice { get; set; }
        public string orderDate { get; set; }

    }
}