using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AD_CF.Models
{
    public class ProductReq
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string productitemnumber { get; set; }
        public string unitOfMeasure { get; set; }
        public string productDesc { get; set; }

        [Required]
        public int quantity { get; set; }
        public int retrievedQuantity { get; set; }
        public int deliveredQuantity { get; set; }
        


        public ProductReq()
        {

        }
        public ProductReq(string productitemnumber, string productDesc, string unit,int quantity)
        {
            this.productitemnumber = productitemnumber;
            this.productDesc = productDesc;
            this.quantity = quantity;
            this.unitOfMeasure = unit;
        }

        //public int requisitionId { get; set; }
        public Requisition req { get; set; }
        
        
    }
}