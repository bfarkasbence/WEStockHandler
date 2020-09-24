using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WEStockHandler.Models;

namespace WEStockHandler.Data
{
    public class ApplicationContext : IdentityDbContext
    {
        public ApplicationContext (DbContextOptions<ApplicationContext> options)
            : base(options)
        {
        }

        public DbSet<WEStockHandler.Models.AttendanceModel> AttendanceModel { get; set; }

        

        public DbSet<WEStockHandler.Models.ProductModel> ProductModel { get; set; }

        

        public DbSet<WEStockHandler.Models.StockChangeModel> StockChangeModel { get; set; }

        

        public DbSet<WEStockHandler.Models.ConsultantModel> ConsultantModel { get; set; }


        public DbSet<WEStockHandler.Models.BtbSentProductsModel> BtnSentProductModel { get; set; }
    }
}
