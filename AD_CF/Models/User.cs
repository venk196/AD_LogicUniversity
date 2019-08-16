using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AD_CF.Models
{
    public class User
    {
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string id { get; set; }
        public string userName { get; set; }
        public string password { get; set; }
        public string sessionId { get; set; } 
        public string employeeId { get; set; }
        public virtual Employee employee { get; set; }
    }
}