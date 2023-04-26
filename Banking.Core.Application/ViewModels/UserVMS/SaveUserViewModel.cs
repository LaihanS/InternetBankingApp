using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banking.Core.Application.ViewModels.UserVMS
{
    public class SaveUserViewModel
    {
        public string? Id { get; set; }

        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Escriba el username...")]
        public string UserName { get; set; }

        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Escriba el apellido...")]
        public string LastName { get; set; }

        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Escriba el nombre...")]
        public string FirstName { get; set; }

        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Escriba la cedula...")]
        public string Cedula { get; set; }

        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Escriba el email...")]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Escriba la contraseña...")]
        public string Password { get; set; }

        [Compare(nameof(Password), ErrorMessage = "Las contraseñas no coinciden")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Escriba la confirmación")]
        public string ConfirmPassword { get; set; }

        public ICollection<string>? Roles { get; set; }
        public float? Monto { get; set; } = default; 
        public bool IsAdmin { get; set; }
        public string? ErrorDetails { get; set; }
        public bool HasError { get; set; }
    }
}
