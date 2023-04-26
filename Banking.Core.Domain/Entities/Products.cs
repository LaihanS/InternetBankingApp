using Banking.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banking.Core.Domain.Entities
{
    public class Products: AuditableBaseEntity
    {
        public string UnicDigitSequence { get; set; }
        public string ProductType { get; set; }

        public double? DebtAmount { get; set; }
        public double? ProductAmount { get; set; }
        public bool IsPrincipalAccount { get; set; }

        public bool UserHasDebt { get; set; }
        public string UserID { get; set; }

        public string? BeneficiarioID { get; set; }

        public ICollection<Pagos> Pagos { get; set; }

    }
}
   