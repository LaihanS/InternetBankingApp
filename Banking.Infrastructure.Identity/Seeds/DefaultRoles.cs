using Banking.Core.Application.Enums;
using Banking.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banking.Infrastructure.Identity.Seeds
{
    public static class DefaultRoles
    {
        public static async Task SeedAsync(UserManager<User> userManager, RoleManager<IdentityRole> role)
        {
           await role.CreateAsync(new IdentityRole(EnumRoles.SuperAdmin.ToString()));
           await role.CreateAsync(new IdentityRole(EnumRoles.Admin.ToString()));
           await role.CreateAsync(new IdentityRole(EnumRoles.Basic.ToString()));
        }

    }
}
