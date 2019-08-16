using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AD_CF.Models
{
    public class Requisition : IComparable
    {
        [Key]
        public int reqformNumber { get; set; }
      
        public string status { get; set; }
        public string approvedBy { get; set; }
        
        public DateTime datecreated { get; set; }
       
        public DateTime? dateapproved { get; set; }
        public string comment { get; set; }
        public string department { get; set; }

        public virtual ICollection<ProductReq> productReqs { get; set; }

        public virtual Employee Employee { get; set; } 

        public Requisition() { }
        public Requisition(string status, DateTime datecreated, string department, 
            List<ProductReq> productReqs, Employee employee)
        {
            this.status = status;
            this.datecreated = datecreated;
            this.department = department;
            this.productReqs = productReqs;
            this.Employee = employee;
        }
        public int CompareTo(Object obj)
        {
            Requisition that = obj as Requisition;
            return -(this.datecreated.CompareTo(that.datecreated));
        }
        
    }
}