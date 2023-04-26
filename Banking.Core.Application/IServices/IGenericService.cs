namespace Banking.Core.Application.IServices
{
    public interface IGenericService<ViewModel, SaveViewModel, Dto, Entity>
        where ViewModel : class
        where SaveViewModel : class
    {
        Task<SaveViewModel> AddAsync(SaveViewModel vm);
        Task Delete(SaveViewModel vm, string id);
        Task EditAsync(SaveViewModel vm, string id);
        Task<List<ViewModel>> GetAsync();
        Task<SaveViewModel> GetEditAsync(string id);
    }
}