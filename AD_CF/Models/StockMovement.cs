using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AD_CF.Models
{
    public class StockMovement
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int movementId { get; set; }
        public DateTime movementDate { get; set; }
        public string movementDescription { get; set; }
        public int movementQuantity { get; set; }
        public int movementBalance { get; set; }
        public string itemNumber { get; set; }
    }
}