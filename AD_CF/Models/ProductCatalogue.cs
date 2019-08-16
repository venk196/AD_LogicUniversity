using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace AD_CF.Models
{
    public class ProductCatalogue : IComparable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string itemNumber { get; set; }
        public string description { get; set; }
        public string category { get; set; }
        public int reorderlevel { get; set; }
        public int reorderquantity { get; set; }        
        public string unitofmeasure { get; set; }
        public int quantity { get; set; }
        
        public string location {  get; set; }

        //public double price { get; set; }

        public int CompareTo(Object obj)
        {
            ProductCatalogue that = obj as ProductCatalogue;
            return this.itemNumber.CompareTo(that.itemNumber);
        }
        
        public string supplier1 { get; set; }
        public string supplier2 { get; set; }
        public string supplier3 { get; set; }
    }
}