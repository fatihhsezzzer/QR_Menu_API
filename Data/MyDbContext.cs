using Mazina_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Mazina_Backend.Data
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        
        public DbSet<Product> Products { get; set; }
        public DbSet<DailyProductsUpdated> DailyProductsUpdated { get; set; }
        public DbSet<DailyProductsInserted> DailyProductsInserted { get; set; }


        public DbSet<ProductUpdated> ProductsUpdated { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Allergen> Allergens { get; set; }
        public DbSet<SPLog> SPLogs { get; set; }

        public DbSet<ProductAllergen> ProductAllergens { get; set; } // Ara tablo için DbSet


        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<GoogleReviewClick> GoogleReviewClicks { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Ara tablo yapılandırması
            modelBuilder.Entity<ProductAllergen>()
                .HasKey(pa => new { pa.ProductId, pa.AllergenId }); // Birleşik birincil anahtar

            modelBuilder.Entity<ProductAllergen>()
                .HasOne(pa => pa.Product)
                .WithMany(p => p.ProductAllergens)
                .HasForeignKey(pa => pa.ProductId);

            modelBuilder.Entity<ProductAllergen>()
                .HasOne(pa => pa.Allergen)
                .WithMany()
                .HasForeignKey(pa => pa.AllergenId);

           

            modelBuilder.Entity<Product>()
              .Property(p => p.ProductId)
              .ValueGeneratedNever();

            modelBuilder.Entity<ProductUpdated>().HasKey(p => p.ProductId);

              modelBuilder.Entity<ProductUpdated>()
              .Property(p => p.ProductId)
              .ValueGeneratedNever();

            modelBuilder.Entity<DailyProductsUpdated>()
            .Property(p => p.ProductId)
            .ValueGeneratedNever();

            modelBuilder.Entity<DailyProductsInserted>()
            .Property(p => p.ProductId)
            .ValueGeneratedNever();

            modelBuilder.Entity<SPLog>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ProcedureName).HasMaxLength(255);
                entity.Property(e => e.Status).HasMaxLength(50);
                entity.Property(e => e.Message);
                entity.Property(e => e.ExecutionTime).HasDefaultValueSql("GETDATE()");
            });
        }
    }
}
