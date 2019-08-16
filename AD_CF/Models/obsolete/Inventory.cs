using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace AD_CF.Models
{
    public class Inventory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { set; get; }
        public String itemNumber { get; set; }

        //[Remote("Disbursement","Store",HttpMethod="POST",
        //    ErrorMessage ="The quantity that you have retrieved exceeds the number present in the inventory")]
        public int quantity { get; set; }
        public string location { get; set; }
        
        internal string Where(bool v)
        {
            throw new NotImplementedException();
        }
    }
}