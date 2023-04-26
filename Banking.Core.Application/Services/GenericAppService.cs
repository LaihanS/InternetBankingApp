
using AutoMapper;
using Banking.Core.Application.Dtos.ImportantDto;
using Banking.Core.Application.IRepositories;
using Banking.Core.Application.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banking.Core.Application.Services
{
    public class GenericAppService<ViewModel, SaveViewModel, Entity> : IGenericAppService<ViewModel, SaveViewModel, Entity> where ViewModel : class
          where SaveViewModel : class
        where Entity : class
    {
        private readonly IGenericAppRepository<Entity> repository;
        private readonly IMapper automapper;



        public GenericAppService(IMapper automapper, IGenericAppRepository<Entity> apprepository)
        {

            this.repository = apprepository;
            this.automapper = automapper;
        }


        public virtual async Task<SaveViewModel> AddAsync(SaveViewModel vm)
        {
            Entity usuario = automapper.Map<Entity>(vm);

            Entity result = await repository.AddAsync(usuario);

            SaveViewModel saveUser = automapper.Map<SaveViewModel>(result);

            return saveUser;
        }


        public virtual async Task<List<ViewModel>> GetAsync()
        {
            var usuarioslist = await repository.GetAsync();

            List<ViewModel> listvm = automapper.Map<List<ViewModel>>(usuarioslist);

            return listvm;

        }

        public virtual async Task<SaveViewModel> GetEditAsync(int id)
        {

            var usuario = await repository.GetByidAsync(id);

            SaveViewModel saveUser = automapper.Map<SaveViewModel>(usuario);

            return saveUser;

        }

        public virtual async Task Delete(SaveViewModel vm, int id)
        {
            Entity usuario = await repository.GetByidAsync(id);
            await repository.DeleteAsync(usuario);
        }

        public virtual async Task EditAsync(SaveViewModel vm, int id)
        {
            Entity usuario = await repository.GetByidAsync(id);
            //string encryptedPassword = PasswordEncrypter.PassHasher(vm.Contraseña);

            usuario = automapper.Map<Entity>(vm);

            await repository.EditAsync(usuario, id);
        }
    }
}
