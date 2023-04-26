using Banking.Core.Application.IRepositories;
using Banking.Infrastructure.Identity.Contexts;
using Banking.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banking.Infrastructure.Persistence.Repositories
{
    public class GenericAppRepository<Entity> : IGenericAppRepository<Entity> where Entity : class
    {
        private readonly ApplicationContext context;

        public GenericAppRepository(ApplicationContext context)
        {

            this.context = context;
        }

        public virtual async Task<Entity> AddAsync(Entity entity)
        {
            await context.Set<Entity>().AddAsync(entity);
            await context.SaveChangesAsync();

            return entity;
        }

        public virtual async Task EditAsync(Entity entity, int id)
        {
            Entity entry = await context.Set<Entity>().FindAsync(id);
            context.Entry(entry).CurrentValues.SetValues(entity);
            await context.SaveChangesAsync();
        }

        public virtual async Task DeleteAsync(Entity entity)
        {
            context.Set<Entity>().Remove(entity);
            await context.SaveChangesAsync();
        }

        public virtual async Task<List<Entity>> GetAsync()
        {
            return await context.Set<Entity>().ToListAsync();
        }
        public virtual async Task<List<Entity>> GetAsyncWithJoin(List<string> navProperties)
        {
            var query = context.Set<Entity>().AsQueryable();

            foreach (string item in navProperties)
            {
                query = query.Include(item);
            }

            return await query.ToListAsync();
        }

        public virtual async Task<Entity> GetByidAsync(int id)
        {
            return await context.Set<Entity>().FindAsync(id);
        }
    }
}
