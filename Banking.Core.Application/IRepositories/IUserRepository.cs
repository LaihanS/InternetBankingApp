using Banking.Core.Application.Dtos.ImportantDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banking.Core.Application.IRepositories
{
    public interface IUserRepository : IGenericRepository1<UserDto, User>
    {
        Task<UserDto> GetUserAccount(string iduser);
    }
}
