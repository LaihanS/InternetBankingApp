using Banking.Core.Application.IRepositories;
using Banking.Core.Application.IServices;
using Banking.Core.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Banking.Core.Application
{
    public static class ServiceRegistration
    {
        public static void AddSharedInfrastructure(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());


            #region ServicesDependency
            services.AddTransient(typeof(IGenericService<,,,>), typeof(GenericService<,,,>));
            services.AddTransient(typeof(IGenericAppService<,,>), typeof(GenericAppService<,,>));
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IProductService, ProductService>();
            services.AddTransient<IPagoService, PagoService>();
            services.AddTransient<ITransactionService, TransactionService>();
            #endregion
        }
    }
}
