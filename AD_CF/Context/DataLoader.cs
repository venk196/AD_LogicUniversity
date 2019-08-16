using AD_CF.Context;
using AD_CF.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace AD_Project_Iteration_1_Main.Context
{
    public class DataLoader
    {
        public static void LoadProducts()
        {
            string query;
            var fileStream = new FileStream(@"C:\Users\KRISH\OneDrive - National University of Singapore\AD\FINAL AD\AD_CF 14 Aug v1\AD_CF\AD_CF\Context\Products.sql", FileMode.Open, FileAccess.Read);
            //var fileStream = new FileStream("//Users/annietay/Downloads/AD_CF 14 Aug v1/AD_CF/AD_CF/Context/Products.sql", FileMode.Open, FileAccess.Read);       
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                query = streamReader.ReadToEnd();
            }

            using (SqlConnection connection = new SqlConnection("Server=LAPTOP-E7E5F6HB; Database=InventoryDB; Integrated Security = True"))
            //using (SqlConnection connection = new SqlConnection("Server=localhost,1433; Database=InventoryDB; User=sa;Password=Password#123;Trusted_Connection=False;"))            
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    int result = command.ExecuteNonQuery();

                    // Check Error
                    if (result < 0)
                        Console.WriteLine("Error inserting data into Database!");
                }

            }
        }
        public static InventoryDbContext LoadData(InventoryDbContext context)
        {

            DataLoader.LoadProducts();

            User u = new User();
            u.id = "1";
            u.userName = "venkat";
            MD5 md5 = new MD5CryptoServiceProvider();
            Byte[] originalBytes = ASCIIEncoding.Default.GetBytes("venkat");
            Byte[] encodedBytes = md5.ComputeHash(originalBytes);
            u.password = BitConverter.ToString(encodedBytes);
            u.employeeId = "10017";

            User u2 = new User();
            u2.id = "2";
            u2.userName = "diwei";
            MD5 md5_1 = new MD5CryptoServiceProvider();
            Byte[] originalBytes_1 = ASCIIEncoding.Default.GetBytes("diwei");
            Byte[] encodedBytes_1 = md5_1.ComputeHash(originalBytes_1);
            u2.password = BitConverter.ToString(encodedBytes_1);
            u2.employeeId = "10016";

            User u3 = new User();
            u3.id = "3";
            u3.userName = "zhongbao";
            MD5 md5_2 = new MD5CryptoServiceProvider();
            Byte[] originalBytes_2 = ASCIIEncoding.Default.GetBytes("zhongbao");
            Byte[] encodedBytes_2 = md5_2.ComputeHash(originalBytes_2);
            u3.password = BitConverter.ToString(encodedBytes_2);
            u3.employeeId = "10022";

            User u4 = new User();
            u4.id = "4";
            u4.userName = "siqi";
            MD5 md5_3 = new MD5CryptoServiceProvider();
            Byte[] originalBytes_3 = ASCIIEncoding.Default.GetBytes("siqi");
            Byte[] encodedBytes_3 = md5_3.ComputeHash(originalBytes_3);
            u4.password = BitConverter.ToString(encodedBytes_3);
            u4.employeeId = "10020";
            
            User u5 = new User();
            u5.id = "5";
            u5.userName = "soonleng";
            MD5 md5_4 = new MD5CryptoServiceProvider();
            Byte[] originalBytes_4 = ASCIIEncoding.Default.GetBytes("soonleng");
            Byte[] encodedBytes_4 = md5_4.ComputeHash(originalBytes_4);
            u5.password = BitConverter.ToString(encodedBytes_4);
            u5.employeeId = "10021";
            
            User u6 = new User();
            u6.id = "6";
            u6.userName = "denghan";
            MD5 md5_5 = new MD5CryptoServiceProvider();
            Byte[] originalBytes_5 = ASCIIEncoding.Default.GetBytes("denghan");
            Byte[] encodedBytes_5 = md5_5.ComputeHash(originalBytes_5);
            u6.password = BitConverter.ToString(encodedBytes_5);
            u6.employeeId = "10018";
            
            User u7 = new User();
            u7.id = "7";
            u7.userName = "victoria";
            MD5 md5_6 = new MD5CryptoServiceProvider();
            Byte[] originalBytes_6 = ASCIIEncoding.Default.GetBytes("victoria");
            Byte[] encodedBytes_6 = md5_6.ComputeHash(originalBytes_6);
            u7.password = BitConverter.ToString(encodedBytes_6);
            u7.employeeId = "10023";
            
            User u8 = new User();
            u8.id = "8";
            u8.userName = "annie";
            MD5 md5_7 = new MD5CryptoServiceProvider();
            Byte[] originalBytes_7 = ASCIIEncoding.Default.GetBytes("annie");
            Byte[] encodedBytes_7 = md5_7.ComputeHash(originalBytes_7);
            u8.password = BitConverter.ToString(encodedBytes_7);
            u8.employeeId = "10024";

 
            context.users.Add(u);
            context.users.Add(u2);
            context.users.Add(u3);
            context.users.Add(u4);
            context.users.Add(u5);
            context.users.Add(u6);
            context.users.Add(u7);
            context.users.Add(u8);
            context.SaveChanges();


//            Voucher v = new Voucher();
//            v.date = new DateTime(2018, 11, 11, 15, 00, 08);
//            v.itemNumber = "I1001";
//            v.itemName = "test";
//            v.price = 390;
//            v.quantityAdjusted = 13;
//            v.status = "Pending";
//            v.reason = "Missing";
//            context.vouchers.Add(v);
//
//            Voucher v1 = new Voucher();
//            v1.date = new DateTime(2019, 10, 11, 15, 00, 48);
//            v1.itemNumber = "I1002";
//            v.itemName = "test";
//            v1.price = 200;
//            v1.quantityAdjusted = 10;
//            v1.status = "Pending";
//            v1.reason = "i dont know";
//            context.vouchers.Add(v1);

            
            return context;
        }
    }
}