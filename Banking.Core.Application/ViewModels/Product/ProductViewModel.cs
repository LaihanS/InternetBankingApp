using Banking.Core.Application.ViewModels.UserVMS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banking.Core.Application.ViewModels.Product
{
    public class ProductViewModel
    {
        public int id { get; set; }
        public string UnicDigitSequence { get; set; }
        public string ProductType { get; set; }
        public bool UserHasDebt { get; set; }
        public double? DebtAmount { get; set; } 
        public double? ProductAmount { get; set; }
        public bool IsPrincipalAccount { get; set; }
        public string UserID { get; set; }

        public string? BeneficiarioID { get; set; }

        public UserViewModel Usuario { get; set; }
    }
}
