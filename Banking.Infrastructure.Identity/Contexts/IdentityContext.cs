using Banking.Core.Application.Dtos.Account;
using Banking.Core.Application.Helpers;
using Banking.Core.Domain.Common;
using Banking.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banking.Infrastructure.Identity.Contexts
{
    public class IdentityContext: IdentityDbContext<User>
    { 
        
        //private readonly IHttpContextAccessor httpContextAccessor;
        //private readonly AuthenticationResponse user = new();   

        public IdentityContext(DbContextOptions<IdentityContext> options) : base(options) {
        }


        public override Task<int> SaveChangesAsync(CancellationToken cancellation = new CancellationToken())
        {
            foreach (var item in ChangeTracker.Entries<AuditableBaseEntity>())
            {
                switch (item.State)
                {
                    case EntityState.Added:
                        item.Entity.created = DateTime.Now;
                        item.Entity.createdBy = "Laihus";
                        break;

                    case EntityState.Modified:
                        item.Entity.modifiedAt = DateTime.Now;
                        item.Entity.modifiedBy = "Laihus";
                        break;
                }
            }
            return base.SaveChangesAsync(cancellation);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //FLUENT API
            #region TableNames
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("Identity");

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable(name: "Users");
            });

            modelBuilder.Entity<IdentityRole>(entity =>
            {
                entity.ToTable(name: "Roles");
            });

            modelBuilder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.ToTable(name: "UserRoles");
            });

            modelBuilder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.ToTable(name: "UserLogins");
            });
            #endregion

            #region "Primary Keys"
           
            #endregion

            #region Relaciones


            #endregion

            #region Config

            #region Post
            //modelBuilder.Entity<Publicacion>().Property(post => post.id).IsRequired().HasMaxLength(150);
            #endregion

            #endregion
        }

    }
 }
