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
    public class SaveTransactionViewModel
    {
        public int id { get; set; }
        public DateTime? created { get; set; }
        public string PaymentFor { get; set; }
        public string PaymentFrom { get; set; }
        public double PaymentAmount { get; set; }
        public string userID { get; set; }
        public int ProductID { get; set; }
    }

}
