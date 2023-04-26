using Banking.Core.Application.Dtos.Account;
using Banking.Core.Application.Dtos.ImportantDto;
using Banking.Core.Application.Services;
using Banking.Core.Application.ViewModels.Product;
using Banking.Core.Application.ViewModels.UserVMS;
using Banking.Core.Domain.Entities;

namespace Banking.Core.Application.IServices
{
    public interface IProductService: IGenericAppService<ProductViewModel, SaveProductViewModel, Products>
    {
        Task<SaveProductViewModel> AddProductAsync(SaveProductViewModel saveproductvm);
        Task<UserViewModel> GetUser(string userid);
        Task<ProductViewModel> AddMember(string userid, string account);
        Task<List<ProductViewModel>> ProductAsigned();
    }
}