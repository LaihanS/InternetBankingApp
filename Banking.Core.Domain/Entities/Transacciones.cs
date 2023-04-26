using Banking.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banking.Core.Domain.Entities
{
    public class Transacciones: AuditableBaseEntity
    {
        public string? TransactFor { get; set; }
        public string? TransactFrom { get; set; }
        public double TransactAmount { get; set; }

        public string? userID { get; set; }

    }
}
