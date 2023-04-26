using AutoMapper;
using Banking.Core.Application.Enums;
using Banking.Core.Application.Helpers;
using Banking.Core.Application.IRepositories;
using Banking.Core.Application.IServices;
using Banking.Core.Application.ViewModels.Home;
using Banking.Core.Application.ViewModels.Pago;
using Banking.Core.Application.ViewModels.Product;
using Banking.Core.Application.ViewModels.UserVMS;
using Banking.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banking.Core.Application.Services
{
    public class TransactionService : GenericAppService<TransactionViewModel, SaveTransactionViewModel, Transacciones>, ITransactionService
    {
        private readonly IPagoRepository pagoRepository;
        private readonly IUserRepository userRepository;
        private readonly ITransactionRepository transactionRepository;
        private readonly IMapper imapper;
        private readonly Sequence9Digit sequence9 = new();
        public TransactionService(IUserRepository userRepository, ITransactionRepository transactionRepository, IPagoRepository pagoRepository, IMapper imapper) : base(imapper, transactionRepository)
        {
            this.pagoRepository = pagoRepository;
            this.imapper = imapper;
            this.userRepository = userRepository;
            this.transactionRepository = transactionRepository;
        }


        public async Task<List<TransactionViewModel>> GetTransactionsToday()
        {
            List<TransactionViewModel> viewModels = imapper.Map<List<TransactionViewModel>>(await transactionRepository.GetAsync());
            List<TransactionViewModel> todaytransaction = new();

            if (viewModels != null)
            {
                todaytransaction = viewModels.Where(p => p.created.Value.Day == DateTime.Now.Day).ToList();
            }

            return todaytransaction;
        }

    }
}
