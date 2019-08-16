using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AD_CF.Models
{
    public class AndroidUserCallback
    {
        public AndroidUserCallback(string employeeId, string department, string sessionId, string status)
        {
            this.employeeId = employeeId;
            this.department = department;
            this.sessionId = sessionId;
            this.status = status;
        }
        public string employeeId { get; set; }
        public string department { get; set; }
        public string sessionId { get; set; }
        public string status { get; set; }
    }
}