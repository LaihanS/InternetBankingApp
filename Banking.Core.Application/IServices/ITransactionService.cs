using Banking.Core.Application.ViewModels.Home;
using Banking.Core.Application.ViewModels.Pago;
using Banking.Core.Domain.Entities;

namespace Banking.Core.Application.IServices
{
    public interface ITransactionService: IGenericAppService<TransactionViewModel, SaveTransactionViewModel, Transacciones>
    {
        Task<List<TransactionViewModel>> GetTransactionsToday();
    }
}