using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AD_CF.Models
{
    public class Department
    {
        [Key]
        //id refers to department code
        public string id { get; set; }
        public string departmentName { get; set; }
        public string collectionName { set; get; }
        public string representative { set; get; }
        
        //public Collection collection { set; get; }
        public virtual ICollection<Employee> employees { get; set; }
        
        //public string departmentCode { get; set; }
        public string contactName { get; set; }
        public string phoneNo { get; set; }
        public string faxNo { get; set; }
        public string departmentHead { get; set; }

    }
}