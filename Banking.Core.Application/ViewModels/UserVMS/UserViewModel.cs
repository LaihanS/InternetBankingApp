using Banking.Core.Application.ViewModels.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banking.Core.Application.ViewModels.UserVMS
{
    public class UserViewModel
    {
        public string? Id { get; set; }
        public string UserName { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Cedula { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public bool EmailConfirmed { get; set; }
        public ICollection<string> Roles { get; set; }
        public ICollection<ProductViewModel> Productos { get; set; }

        public ICollection<ProductViewModel> Beneficiarios { get; set; }
    }
}
