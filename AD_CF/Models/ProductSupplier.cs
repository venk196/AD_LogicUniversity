using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AD_CF.Models
{
    public class ProductSupplier
    {
        [Key, Column(Order = 0)]
        public string id { get; set; }
        [Key, Column(Order = 1)]
        public string itemNumber { get; set; }
        public double price { get; set; }
        public int preference { get; set; }
    }
}