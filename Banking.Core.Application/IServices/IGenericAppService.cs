namespace Banking.Core.Application.IServices
{
    public interface IGenericAppService<ViewModel, SaveViewModel, Entity>
        where ViewModel : class
        where SaveViewModel : class
         where Entity : class
    {
        Task<SaveViewModel> AddAsync(SaveViewModel vm);
        Task Delete(SaveViewModel vm, int id);
        Task EditAsync(SaveViewModel vm, int id);
        Task<List<ViewModel>> GetAsync();
        Task<SaveViewModel> GetEditAsync(int id);
    }
}