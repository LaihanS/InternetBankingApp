using AutoMapper;
using Banking.Core.Application.IRepositories;
using Banking.Infrastructure.Identity.Contexts;
using Banking.Infrastructure.Persistence.Mappings;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banking.Infrastructure.Persistence.Repositories
{
    public class GenericRepository<Dto, Entity> : IGenericRepository1<Dto, Entity> where Entity : class
        where Dto : class
    {
        private readonly IdentityContext context;

        private readonly IMapper _mapper;

        public GenericRepository(IMapper _mapper, IdentityContext context)
        {
        
            this.context = context;
           this._mapper = _mapper;
        }

        public virtual async Task<Dto> AddAsync(Dto entity)
        {
            Entity entidad = _mapper.Map<Entity>(entity);
            var dat = await context.Set<Entity>().AddAsync(entidad);
            await context.SaveChangesAsync();

            Dto result = _mapper.Map<Dto>(dat.Entity);

            return result;
        }

        public virtual async Task EditAsync(Dto entity, string id)
        {
            Entity entry = _mapper.Map<Entity>(entity);
            Entity result = await context.Set<Entity>().FindAsync(id);
            context.Entry(result).CurrentValues.SetValues(entry);
            await context.SaveChangesAsync();

            //Dto entry = await context.Set<Dto>().FindAsync(id);
            //context.Entry(entry).CurrentValues.SetValues(entity);
            //await context.SaveChangesAsync();
        }

        public virtual async Task DeleteAsync(Dto entity, string id)
        {
            Entity entry = _mapper.Map<Entity>(entity);
            Entity result = await context.Set<Entity>().FindAsync(id);
            context.Set<Entity>().Remove(result);
            await context.SaveChangesAsync();
        }

        public virtual async Task<List<Dto>> GetAsync()
        {
            List<Dto> dtos = _mapper.Map<List<Dto>>(await context.Set<Entity>().ToListAsync());

            return dtos;
        }
        public virtual async Task<List<Dto>> GetAsyncWithJoin(List<string> navProperties)
        {
            var query = context.Set<Entity>().AsQueryable();

            foreach (string item in navProperties)
            {
                query = query.Include(item);
            }

            List<Dto> dtos = _mapper.Map<List<Dto>>(await query.ToListAsync());

            return dtos;
        }

        public virtual async Task<Dto> GetByidAsync(string id)
        {

            Dto entry = _mapper.Map<Dto>(await context.Set<Entity>().FindAsync(id));

            return entry;
        }
    }
}
