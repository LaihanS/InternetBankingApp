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
    public class PagoRepository: GenericAppRepository<Pagos>, IPagoRepository
    {
        public readonly ApplicationContext applicationContext;

        private readonly IMapper mapper;

        public PagoRepository(IMapper mapper, ApplicationContext applicationContext) : base(applicationContext)
        {
            this.applicationContext = applicationContext;
            this.mapper = mapper;
        }
    }
}
