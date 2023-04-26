using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banking.Core.Application.Dtos.Account
{
    public class RegisterResponse
    {
        public bool HasError { get; set; }
        public string ErrorDetails { get; set; }

        public string UserID { get; set; }
    }
}
