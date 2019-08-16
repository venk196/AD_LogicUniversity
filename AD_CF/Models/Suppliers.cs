using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AD_CF.Models
{
    public class Suppliers
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int supplierId { get; set; }
        public string supplierCode { get; set; }
        public string supplierName { get; set; }
        public string gstNo { get; set; }
        public string contactName { get; set; }
        public string phoneNo { get; set; }
        public string faxNo { get; set; }
        public string address { get; set; }
    }
}