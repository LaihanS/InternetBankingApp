using Banking.Core.Application.ViewModels.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banking.Core.Application.Dtos.ImportantDto
{
    public class PagoDto
    {
        public int id { get; set; }

        public string PaymentFor { get; set; }
        public string PaymentFrom { get; set; }
        public double PaymentAmount { get; set; }

        public string userID { get; set; }
        public int ProductID { get; set; }

        public ProductViewModel Product { get; set; }
    }
}
