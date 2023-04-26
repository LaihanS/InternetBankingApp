using AutoMapper;
using Banking.Core.Application.Dtos.ImportantDto;
using Banking.Core.Application.IRepositories;
using Banking.Core.Application.ViewModels.UserVMS;
using Banking.Core.Domain.Entities;
using Banking.Infrastructure.Identity.Contexts;
using Banking.Infrastructure.Identity.Entities;
using Banking.Infrastructure.Persistence.Contexts;
using Banking.Infrastructure.Persistence.Mappings;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using User = Banking.Infrastructure.Identity.Entities.User;

namespace Banking.Infrastructure.Persistence.Repositories
{
    public class UserRepository : GenericRepository<UserDto, User>, IUserRepository
    {
        public readonly IdentityContext identityccontext;

        private readonly IMapper mapper;

        public UserRepository(IMapper mapper, IdentityContext context) : base(mapper, context)
        {
            identityccontext = context;
            this.mapper = mapper;
        }

        public async Task<UserDto> GetUserAccount(string iduser)
        {
            User user = await identityccontext.
                Set<User>().FirstOrDefaultAsync(p => p.Id == iduser);

            if (user == null)
            {
                return new UserDto();
            }
            else
            {
                return mapper.Map<UserDto>(user);
            }
        }

        //public override async Task<Usuario> AddAsync(Usuario entity)
        //{
        //    entity.Contraseña = PasswordEncrypter.PassHasher(entity.Contraseña);
        //    await base.AddAsync(entity);

        //    return entity;
        //}


        //public async Task<Usuario> LoginAsync(LoginViewModel logvm)
        //{
        //    string passEncripted = PasswordEncrypter.PassHasher(logvm.Password);

        //    Usuario usuario = await applicationcon.Set<Usuario>().
        //     FirstOrDefaultAsync(user => user.UserName == logvm.UserName && user.Contraseña == passEncripted);


        //    return usuario;
        //}

        //public async Task<bool> ValidateifExists(SaveUserViewModel uservm)
        //{

        //    //string passEncripted = PasswordEncrypter.PassHasher(uservm.Contraseña);
        //    Usuario usuario = await applicationcon.Set<Usuario>().
        //     FirstOrDefaultAsync(user => user.UserName == uservm.UserName /*|| user.Contraseña == passEncripted*/);


        //    return usuario == null ? false : true;
        //}

        //public async Task<UserViewModel> GetUserByNameAsync(string username)
        //{
        //    Usuario usuario = await applicationcon.Set<Usuario>().
        //     FirstOrDefaultAsync(user => user.UserName == username);

        //    UserViewModel user = mapper.Map<UserViewModel>(usuario);

        //    return user;

        //}



    }

}
