using Banking.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banking.Core.Domain.Entities
{
    public class Pagos: AuditableBaseEntity
    {
        public string PaymentFor { get; set; }
        public string PaymentFrom { get; set; }
        public double PaymentAmount { get; set; }

        public string? userID { get; set; }
        public int? ProductID { get; set; }

        public Products? Product { get; set; }
    }
}
