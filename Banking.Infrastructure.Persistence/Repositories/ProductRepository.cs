using AutoMapper;
using Banking.Core.Application.Dtos.ImportantDto;
using Banking.Core.Application.IRepositories;
using Banking.Infrastructure.Identity.Contexts;
using Banking.Core.Domain.Entities;
using Banking.Infrastructure.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Banking.Core.Application.Enums;

namespace Banking.Infrastructure.Persistence.Repositories
{
    public class ProductRepository : GenericAppRepository<Products>, IProductRepository
    {
        public readonly ApplicationContext applicationContext;

        private readonly IMapper mapper;

        public ProductRepository(IMapper mapper, ApplicationContext applicationContext) : base(applicationContext)
        {
            this.applicationContext = applicationContext;
            this.mapper = mapper;
        }

        public async Task<Products> GetAccountByDigits(string sequence)
        {
            Products product = await applicationContext.
                Set<Products>().FirstOrDefaultAsync(p => p.UnicDigitSequence == sequence);

            if (product == null)
            {
                return new Products();
            }
            else
            {
                return product;
            }
        }
    }
}
