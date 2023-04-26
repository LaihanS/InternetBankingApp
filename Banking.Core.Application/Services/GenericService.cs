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
    public class GenericService<ViewModel, SaveViewModel, Dto, Entity> : IGenericService<ViewModel, SaveViewModel, Dto, Entity> where Dto : class
         where ViewModel : class
          where SaveViewModel : class
        where Entity : class
    {
        private readonly IGenericRepository1<Dto, Entity> repository;
        private readonly IGenericAppRepository<Entity> apprepository;
        private readonly IMapper automapper;

        public GenericService(IMapper automapper, IGenericRepository1<Dto, Entity> repository)
        {

            this.repository = repository;
            this.automapper = automapper;
        }

        public GenericService(IMapper automapper, IGenericAppRepository<Entity> apprepository)
        {

            this.apprepository = apprepository;
            this.automapper = automapper;
        }


        public virtual async Task<SaveViewModel> AddAsync(SaveViewModel vm)
        {
            Dto usuario = automapper.Map<Dto>(vm);

           Dto result = await repository.AddAsync(usuario);

            SaveViewModel saveUser = automapper.Map<SaveViewModel>(result);

            return saveUser;
        }


        public virtual async Task<List<ViewModel>> GetAsync()
        {
            var usuarioslist = await repository.GetAsync();

            List<ViewModel> listvm = automapper.Map<List<ViewModel>>(usuarioslist);

            return listvm;

        }

        public virtual async Task<SaveViewModel> GetEditAsync(string id)
        {

            var usuario = await repository.GetByidAsync(id);

            SaveViewModel saveUser = automapper.Map<SaveViewModel>(usuario);

            return saveUser;

        }

        public virtual async Task Delete(SaveViewModel vm, string id)
        {
            Dto usuario = await repository.GetByidAsync(id);
            await repository.DeleteAsync(usuario,id);
        }

        public virtual async Task EditAsync(SaveViewModel vm, string id)
        {
            Dto usuario = await repository.GetByidAsync(id);
            //string encryptedPassword = PasswordEncrypter.PassHasher(vm.Contraseña);

            usuario = automapper.Map<Dto>(vm);

            await repository.EditAsync(usuario, id);
        }
    }
}