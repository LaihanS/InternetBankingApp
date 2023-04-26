using Banking.Core.Application.Dtos.ImportantDto;
using Banking.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banking.Core.Application.IRepositories
{
    public interface IProductRepository : IGenericAppRepository<Products>
    {
        Task<Products> GetAccountByDigits(string sequence);
    }
}
