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

namespace Banking.Infrastructure.Persistence.Repositories
{
    public class TransactionRepository : GenericAppRepository<Transacciones>, ITransactionRepository
    {
        public readonly ApplicationContext applicationContext;

        private readonly IMapper mapper;

        public TransactionRepository(IMapper mapper, ApplicationContext applicationContext) : base(applicationContext)
        {
            this.applicationContext = applicationContext;
            this.mapper = mapper;
        }
    }
}
