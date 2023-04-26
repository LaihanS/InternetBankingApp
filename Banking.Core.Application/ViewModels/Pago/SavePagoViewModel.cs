using Banking.Core.Application.ViewModels.Product;
using Banking.Core.Application.ViewModels.UserVMS;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banking.Core.Application.ViewModels.Pago
{
    public class SavePagoViewModel
    {
        public int? id { get; set; }

        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Escriba Para Quién es")]
        public string PaymentFor { get; set; }

        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Seleccione De Quién Viene")]
        public string PaymentFrom { get; set; }

        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Escriba el monto")]
        public double PaymentAmount { get; set; }

        public string? userID { get; set; }
        public int? ProductID { get; set; }

        public bool IsTransaction { get; set; }

        public bool HasError { get; set; }
        public string? ErrorDetails { get; set; }

        public ProductViewModel? Cuenta { get; set; }

        public ICollection<ProductViewModel>? Products { get; set; }
    }
}
