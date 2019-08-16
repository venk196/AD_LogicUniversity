
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using AD_CF.Models;
using AD_Project_Iteration_1_Main.Context;

namespace AD_CF.Context
{
    public class InventoryDbContext : DbContext
    {
       public InventoryDbContext() : base("Server=LAPTOP-E7E5F6HB; Database=InventoryDB; Integrated Security = True")
        //public InventoryDbContext() : base("Server=LAPTOP-P9D7CRN0; Database=InventoryDB; Integrated Security = True")
        //public InventoryDbContext() : base("Server=localhost,1433; Database=InventoryDB; User=sa;Password=Password#123;Trusted_Connection=False;")
        {
            Database.SetInitializer(new InventoryDbInitializer<InventoryDbContext>());
        }
        public DbSet<ProductCatalogue> productCatalogues { get; set; }
        public DbSet<Requisition> requisitions { get; set; }
        public DbSet<ProductReq> productReqs { get; set; }
        public DbSet<User> users { get; set; }
        public DbSet<Employee> employees { get; set; }
        public DbSet<Delegation> delegations { get; set; }
        public DbSet<Department> departments { get; set; }
        public DbSet<Collection> collections { get; set; }
        public DbSet<Suppliers> supplier { get; set; }
        
        public DbSet<Inventory> inventories { get; set; }
        public DbSet<Voucher> vouchers { get; set; }
        public DbSet<ProductSupplier> productSuppliers { get; set; }

        public DbSet<StockMovement> stockmovements { get; set; }
        
        public DbSet<PurchaseOrder> purchaseOrders { get; set; }

        public class InventoryDbInitializer<T>:
            DropCreateDatabaseAlways<InventoryDbContext>
        {
            protected override void Seed(InventoryDbContext context)
            {
                base.Seed(DataLoader.LoadData(context));
            }
        }
    }
}