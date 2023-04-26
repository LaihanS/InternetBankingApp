
using AutoMapper;
using MyMapper = AutoMapper.IMapper;
using Banking.Core.Application.IRepositories;
using Banking.Core.Application.IServices;
using Banking.Core.Application.Services;
using Banking.Infrastructure.Persistence.Mappings;
using Banking.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Banking.Infrastructure.Persistence.Contexts;

namespace Banking.Infrastructure.Persistence
{
    public static class ServiceRegistration
    {
        public static void AddPersistenceInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {

            #region contexts
            if (configuration.GetValue<bool>("UseInMemoryDatabase"))
            {
                services.AddDbContext<ApplicationContext>(o => o.UseInMemoryDatabase("CItasDB"));
            }
            else
            {
                var connectionString = configuration.GetConnectionString("ConnectionDefault");
                services.AddDbContext<ApplicationContext>(options =>
                {

                    options.UseSqlServer(connectionString, m => m.MigrationsAssembly(typeof(ApplicationContext).Assembly.FullName));
                    options.EnableSensitiveDataLogging();
                });
            }
            #endregion

            services.AddAutoMapper(typeof(PersistenceProfile).Assembly);


            #region repositories
            services.AddTransient(typeof(IGenericRepository1<,>), typeof(GenericRepository<,>));
            services.AddTransient(typeof(IGenericAppRepository<>), typeof(GenericAppRepository<>));
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IProductRepository, ProductRepository>();
            services.AddTransient<IPagoRepository, PagoRepository>();
            services.AddTransient<ITransactionRepository, TransactionRepository>();
            #endregion
        }
    }

}
