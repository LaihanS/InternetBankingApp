using Banking.Core.Domain.Common;
using Banking.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banking.Infrastructure.Persistence.Contexts
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }

        public DbSet<Products> Products { get; set; } 

        public override Task<int> SaveChangesAsync(CancellationToken cancellation = new CancellationToken())
        {
            foreach (var item in ChangeTracker.Entries<AuditableBaseEntity>())
            {
                switch (item.State)
                {
                    case EntityState.Added:
                        item.Entity.created = DateTime.Now;
                        item.Entity.createdBy = "Laihusmanguplus";
                        break;

                    case EntityState.Modified:
                        item.Entity.modifiedAt = DateTime.Now;
                        item.Entity.modifiedBy = "Laihusmanguplus";
                        break;
                }
            }
            return base.SaveChangesAsync(cancellation);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Nombres(tablas)
            modelBuilder.Entity<Products>().ToTable("Productos");
            modelBuilder.Entity<Pagos>().ToTable("Pagos");
            modelBuilder.Entity<Transacciones>().ToTable("Transacciones");
            #endregion

            #region "Primary Keys"

            modelBuilder.Entity<Products>().HasKey(product => product.id);
            modelBuilder.Entity<Pagos>().HasKey(pagos => pagos.id);
            modelBuilder.Entity<Transacciones>().HasKey(transac => transac.id);
            #endregion

            #region Relaciones

            modelBuilder.Entity<Products>().HasMany<Pagos>(product => product.Pagos).
            WithOne(Pago => Pago.Product).
            HasForeignKey(Pago => Pago.ProductID).
            OnDelete(DeleteBehavior.SetNull);

            #endregion

            #region Config 

            #region Post
            //modelBuilder.Entity<Publicacion>().Property(post => post.id).IsRequired().HasMaxLength(150);
            #endregion

            #endregion
        }
    }
}
