using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AD_CF.Models
{
    public class Employee
    {
        [Key]
        public string id { get; set; }
        public string empName { get; set; }           
        public string role { get; set; }
        public virtual ICollection<Delegation> delegations { get; set; }
        public string departmentId { get; set; }
        public virtual Department Department { get; set; }

        public String Email { get; set; }
        public virtual ICollection<Requisition> Requistions { get; set; }

    }
}