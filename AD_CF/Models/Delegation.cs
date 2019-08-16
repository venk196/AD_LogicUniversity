using AD_CF.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AD_CF.Models
{
    public class Delegation 
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        [DisplayName("Delegate To")]
        public string delegateToName { get; set; }

        [DisplayName("Start Date")]
        public DateTime startDate { get; set; }
        
        //[DataType(DataType.Date)]
        [DisplayName("End Date")]
        public DateTime endDate { get; set; }

        public string employeeId { get; set; }
        public Employee employee { get; set; }
        
        public virtual Department Department { get; set; }

        public string status { get; set; }

    }
}