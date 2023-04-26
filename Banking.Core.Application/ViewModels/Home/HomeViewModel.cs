using Banking.Core.Application.ViewModels.Pago;
using Banking.Core.Application.ViewModels.Product;
using Banking.Core.Application.ViewModels.UserVMS;
using Banking.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banking.Core.Application.ViewModels.Home
{
    public class HomeViewModel
    {
        public ICollection<PagoViewModel>? Pagos { get; set; }
        public ICollection<PagoViewModel>? PagosToday { get; set; }
        public ICollection<UserViewModel>? ClientesActivos { get; set; }
        public ICollection<UserViewModel>? ClientesInactivos { get; set; }
        public ICollection<ProductViewModel>? Productos { get; set; }
        public ICollection<TransactionViewModel>? Transacciones { get; set; }
        public ICollection<TransactionViewModel>? TransaccionesToday { get; set; }
    }

}
