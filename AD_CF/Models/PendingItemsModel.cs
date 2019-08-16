using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AD_CF.Models
{
    public class PendingItemsModel
    {
        public PendingItemsModel()
        {
            itemNumber = new List<string>();
            itemName = new List<string>();
            itemQty = new List<int>();
            itemUnitOfMeasure = new List<string>();
            itemLocation = new List<string>();

        }
        public List<string> itemName { get; set; }
        public List<int> itemQty { get; set; }
        public List<string> itemUnitOfMeasure { get; set; }
        public List<string> itemNumber { get; set; }
        public List<string> itemLocation { get; set; }
 
        public void add(string addItemNumber, string addItemName, string unit, int qty, string location)
        {
            bool found = false;
            for (int i=0; i<itemName.Count; i++)
            {
                if (itemName.ElementAt(i) == addItemName)
                {
                    //change quantity to qty plus new qty
                    itemQty[i] = itemQty.ElementAt(i) + qty;
                    found = true;
                }
            }
            //after loop, if item not found, add it as part of the list
            if (found == false)
            {
                itemName.Add(addItemName);
                itemNumber.Add(addItemNumber);
                itemUnitOfMeasure.Add(unit);
                itemQty.Add(qty);
                itemLocation.Add(location);
            }
            
        }
    }
}