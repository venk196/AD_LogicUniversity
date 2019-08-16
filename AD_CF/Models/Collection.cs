using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AD_CF.Models
{
    public class Collection
    {
        [Key]
        public string collectionPt { get; set; }  //collectionpoint name
    }
}