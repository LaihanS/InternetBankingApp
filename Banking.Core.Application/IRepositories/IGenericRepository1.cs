namespace Banking.Core.Application.IRepositories
{
    public interface IGenericRepository1<Dto, Entity> where Dto : class
         where Entity : class
    {

        Task DeleteAsync(Dto entity, string id);
        Task EditAsync(Dto entity, string id);
        Task<List<Dto>> GetAsync();
        Task<List<Dto>> GetAsyncWithJoin(List<string> navProperties);
        Task<Dto> GetByidAsync(string id);
        Task<Dto> AddAsync(Dto entity);
    }
}