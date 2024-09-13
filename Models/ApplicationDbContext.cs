using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using IdentityServerProject.Models;
using Microsoft.AspNetCore.Identity;
using System.Reflection.Emit;

namespace IdentityServerProject.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
      
        //
       public DbSet<MyDataModel> MyDataModels { get; set; } // MyDataModel sınıfını veritabanına ekliyoruz
        
        //
        public object Messages { get; internal set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);



            builder.Entity<MyDataModel>().HasKey(i => i.Id);
            builder.Entity<MyDataModel>().Property(i => i.CustomerName).IsRequired();
            builder.Entity<MyDataModel>().Property(i => i.BaslangicTarihi).IsRequired();
            builder.Entity<MyDataModel>().Property(i => i.BitisTarihi).IsRequired();
            builder.Entity<MyDataModel>().Property(i => i.DosyaAdi).IsRequired();
            builder.Entity<MyDataModel>().Property(i => i.YüklemeTarihi).IsRequired();



            // Seed roles
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole { Name = "admin", NormalizedName = "ADMIN" },
                new IdentityRole { Name = "user", NormalizedName = "USER" }
            );
        }

    }
}
