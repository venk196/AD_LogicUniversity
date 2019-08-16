using AD_CF.Context;
using AD_CF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AD_CF.Utilities
{
    public class RequestServices
    {

        InventoryDbContext db = new InventoryDbContext();

        public void addNewRequisition(string status, DateTime created, List<ProductReq> requests, string sessionId)
        {
            User user = db.users.Where(x => x.sessionId == sessionId).FirstOrDefault();
            Requisition requisition = new Requisition();
            requisition.status = status;
            requisition.datecreated = created;
            requisition.department = user.employee.departmentId;
            requisition.Employee = user.employee;
            requisition.productReqs = new List<ProductReq>();
            foreach (ProductReq productReq in requests)
            {
                productReq.req = requisition;
                requisition.productReqs.Add(productReq);
            }
            db.requisitions.Add(requisition);
            db.SaveChanges();
        }
        public List<ProductCatalogue> getAllProducts()
        {
            return db.productCatalogues.ToList();
        }
        public string getRequestedProductAndQuantityString(string[] selected)
        {
            string createdString = "";

            //get list of products that were selected to be used for quantity selected
            List<ProductCatalogue> products = db.productCatalogues.ToList();
            List<ProductCatalogue> selectedProducts = new List<ProductCatalogue>();
            foreach (string requested in selected)
            {
                selectedProducts.Add(products.Where(x => x.itemNumber == requested).FirstOrDefault());
            }
            //create string of information of selected items
            foreach (ProductCatalogue product in selectedProducts)
            {
                //if first values, no comma in front
                if (createdString == "")
                {
                    //three items: item code, item name and qty portions
                    createdString = createdString + product.itemNumber + "," + product.description + "," + product.unitofmeasure + "," + 0;
                }
                else
                {
                    createdString = createdString + "," + product.itemNumber + "," + product.description + "," + product.unitofmeasure + "," + 0;
                }
            }
            return createdString;
        }

        public List<ProductReq> returnProductRequestsasList(string requeststring)
        {
            List<ProductReq> requestedItemList = new List<ProductReq>();
            string[] requestinfo = requeststring.Split(',');
            for (int i = 0; i < requestinfo.Length; i++)
            {
                if (i % 4 == 0)
                {
                    //first pos is itemcode, second is description, third is qty
                    ProductReq pr = new ProductReq(requestinfo[i], requestinfo[i + 1], requestinfo[i+2], int.Parse(requestinfo[i + 3]));
                    requestedItemList.Add(pr);
                }
            }
            return requestedItemList;
        }

        public string updateRequestInfo(string current, string id, int cmd)
        {
            string[] currentInfo = current.Split(',');
            for (int i = 0; i < currentInfo.Length; i += 3)
            {
                if (currentInfo[i] == id)
                {
                    currentInfo[i + 3] = (int.Parse(currentInfo[i + 2]) + cmd).ToString();
                    break;
                }
            }
            return returnStringInfo(currentInfo);
        }

        public string returnStringInfo(string[] stringArrayInfo)
        {
            string result = "";
            for (int i = 0; i < stringArrayInfo.Length; i += 4)
            {
                //first instance, no comma
                if (result == "")
                {
                    result = result + stringArrayInfo[i] + "," + stringArrayInfo[i + 1] + "," + stringArrayInfo[i + 2] + "," + stringArrayInfo[i + 3];
                }
                else
                {
                    result = result + "," + stringArrayInfo[i] + "," + stringArrayInfo[i + 1] + "," + stringArrayInfo[i + 2] + "," + stringArrayInfo[i + 3];
                }
            }
            return result;
        }

        public List<Requisition> getAllUncompleteRequisitions(string department)
        {
            List<Requisition> requistions = db.requisitions.ToList();
            List<Requisition> reqdeplist = (requistions.Where(x => x.department == department &&
            (x.status == "Pending Approval" || x.status == "Approved" || x.status == "Disbursing"))).ToList();
            return reqdeplist;
        }

        public List<Requisition> getAllUncompleteRequisitions()
        {
            List<Requisition> requistions = db.requisitions.Where(x=>x.status=="Approved").ToList();
            return requistions;
        }

        public List<Requisition> getAllPendingApprovalRequisitions(String department)
        {
            List<Requisition> requistions = db.requisitions.ToList();
            List<Requisition> reqdeplist = requistions.Where(x => x.department == department &&
            x.status == "Pending Approval").ToList();
            return reqdeplist;
        }

        public List<Requisition> getDisbursementRequisitions()
        {
            return db.requisitions.Where(x => x.status == "Disbursing").ToList();
        }
        public List<Requisition> getrequisitionsByEmployee(Employee emp)
        {
            List<Requisition> requisitions = db.requisitions.ToList();
            List<Requisition> selection = requisitions.Where(x => x.Employee == emp).ToList();
            return selection;
        }

        public Requisition GetRequistion(int reqformnumber)
        {
            Requisition requistion = db.requisitions.Where(x => x.reqformNumber == reqformnumber).FirstOrDefault();

            return requistion;
        }

        public List<Requisition> GetRequistions(String department)
        {
            List<Requisition> requistions = db.requisitions.ToList();

            List<Requisition> reqdeplist = (requistions.Where(x => x.department == department)).ToList();

            return reqdeplist;

        }
        public List<Department> GetDepartments()
        {
            return db.departments.ToList();
        }

    }
}