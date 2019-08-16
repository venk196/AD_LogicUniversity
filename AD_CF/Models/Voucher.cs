using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AD_CF.Models
{
    public class Voucher : IComparable
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public DateTime voucherDate { get; set; }

        public string reason { get; set; }

        public string status { get; set; }

        public int quantityAdjusted { get; set; }

        public double price { get; set; }

        public String itemNumber { get; set; }
        public string itemName { get; set; }

        public int CompareTo(Object obj)
        {
            Voucher that = obj as Voucher;
            return -(this.voucherDate.CompareTo(that.voucherDate));
        }
    }
}