using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banking.Core.Domain.Settings
{
    public class EmailSettings
    {
        public string Emailfrom { get; set; }
        public int SmtpPort { get; set; }
        public string SmtpHost { get; set; }
        public string SmtpUser { get; set; }
        public string SmtpPass { get; set; }
        public string DisplayName { get; set; }
    }
}
