using AutoMapper;
using Banking.Core.Application.Dtos.Account;
using Banking.Core.Application.Enums;
using Banking.Core.Application.Helpers;
using Banking.Core.Application.IServices;
using Banking.Core.Application.ViewModels.Pago;
using Banking.Core.Application.ViewModels.Product;
using Banking.Core.Application.ViewModels.UserVMS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace WebApp.Banking.Controllers
{
    [Authorize(Roles = "Basic")]
    public class ClientUserController : Controller
    {

        private readonly IMapper _mapper;
        private readonly IUserService userService;
        private readonly IProductService productService;
        private readonly IPagoService pagoService;
        private readonly IHttpContextAccessor http;
        AuthenticationResponse user = new();

        public ClientUserController(IProductService productService, IPagoService pagoService, IHttpContextAccessor http, IMapper _mapper, IUserService userService)
        {
            this.pagoService = pagoService;
            this.productService = productService;
            this.http = http;
            this._mapper = _mapper;
            this.userService = userService;
            this.user = http.HttpContext.Session.Get<AuthenticationResponse>("user");
        }

        public async Task<IActionResult> Index()
        {
            List<ProductViewModel> productViewModel = productService.GetAsync().Result.Where(p => p.UserID == user.id).ToList();
            return View(productViewModel);
        }

        public async Task<IActionResult> Beneficiario()
        {
            UserViewModel usuario = await productService.GetUser(user.id);
             return View(usuario);
        }
        
        [HttpPost]
        public async Task<IActionResult> Beneficiario(string accountnumber)
        {
            if (!ModelState.IsValid)
            {
                UserViewModel usuario = await productService.GetUser(user.id);
                return View(usuario);
            }
            ProductViewModel prdoduct = await productService.AddMember(user.id, accountnumber);
            if (prdoduct.UnicDigitSequence == null)
            {
                ModelState.AddModelError("userValidation", "Hubo un error al econtrar esa cuenta: Introduzca un número de cuenta válido");
                UserViewModel usuario = await productService.GetUser(user.id);
                return View(usuario);
            }
            else
            {
                return RedirectToRoute(new { controller = "ClientUser", action = "Beneficiario" });
            }
        }


        public async Task<IActionResult> Pago()
        {
            SavePagoViewModel pago = new();
            pago.Products = productService.GetAsync().Result
              .Where(p => p.ProductType.Equals(ProductTypeEnum.Cuenta_Ahorro.ToString()) && p.UserID == user.id).ToList();
            return View(pago);
        }

        [HttpPost]
        public async Task<IActionResult> Pago(SavePagoViewModel pagoViewModel)
        {
            if (!ModelState.IsValid)
            {
                pagoViewModel.Products = productService.GetAsync().Result
             .Where(p => p.ProductType.Equals(ProductTypeEnum.Cuenta_Ahorro.ToString()) && p.UserID == user.id).ToList();
                return View(pagoViewModel);
            }

            SavePagoViewModel pago = await pagoService.ValidatePayment(pagoViewModel);
            if (pago.HasError)
            {
                pago.Products = productService.GetAsync().Result
              .Where(p => p.ProductType.Equals(ProductTypeEnum.Cuenta_Ahorro.ToString()) && p.UserID == user.id).ToList();
                return View(pago);
            }
            else
            {
                return View("ConfirmPayment", pago);
            }
           
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmPayment(SavePagoViewModel pagoViewModel)
        {
            SavePagoViewModel pago = await pagoService.ConcretePayment(pagoViewModel);
            return RedirectToRoute(new { controller = "ClientUser", action = "Index" });
        }

        public async Task<IActionResult> PagoTarjeta()
        {
            SavePagoViewModel pago = new();
            pago.Products = productService.GetAsync().Result
              .Where(p => p.UserID == user.id).ToList();
            return View(pago);
        }

        [HttpPost]
        public async Task<IActionResult> PagoTarjeta(SavePagoViewModel pagoViewModel)
        {
            if (!ModelState.IsValid)
            {
                pagoViewModel.Products = productService.GetAsync().Result
               .Where(p => p.UserID == user.id).ToList();
                return View(pagoViewModel);
            }

            SavePagoViewModel pago = await pagoService.PaymentTarjeta(pagoViewModel);
            if (pago.HasError)
            {
                pago.Products = productService.GetAsync().Result
              .Where(p => p.UserID == user.id).ToList();
                return View(pago);
            }
            else
            {
                return RedirectToRoute(new { controller = "ClientUser", action = "Index" });
            }

        }

        public async Task<IActionResult> PagoBeneficiario()
        {
            SavePagoViewModel pago = new();
            pago.Products = await productService.GetAsync();
            foreach (ProductViewModel prod in pago.Products)
            {
                prod.Usuario = _mapper.Map<UserViewModel>(await userService.GetEditAsync(prod.UserID));
            }
            return View(pago);
        }

        [HttpPost]
        public async Task<IActionResult> PagoBeneficiario(SavePagoViewModel pagoViewModel)
        {
            if (!ModelState.IsValid)
            {
                pagoViewModel.Products = await productService.GetAsync();
                foreach (ProductViewModel prod in pagoViewModel.Products)
                {
                    prod.Usuario = _mapper.Map<UserViewModel>(await userService.GetEditAsync(prod.UserID));
                }
                return View(pagoViewModel);
            }

            pagoViewModel.IsTransaction = true;
            SavePagoViewModel pago = await pagoService.ValidatePayment(pagoViewModel);

            if (pago.HasError)
            {
                pago.Products = await productService.GetAsync();
                foreach (ProductViewModel prod in pago.Products)
                {
                    prod.Usuario = _mapper.Map<UserViewModel>(await userService.GetEditAsync(prod.UserID));
                }
                return View(pago);
            }
            else
            {
                return View("ConfirmPayment", pago);
            }

        }

        public async Task<IActionResult> PagoPréstamo()
        {
            SavePagoViewModel pago = new();
            pago.Products = productService.GetAsync().Result
              .Where(p => p.UserID == user.id).ToList();
            return View(pago);
        }

        [HttpPost]
        public async Task<IActionResult> PagoPréstamo(SavePagoViewModel pagoViewModel)
        {
            if (!ModelState.IsValid)
            {
                pagoViewModel.Products = productService.GetAsync().Result
             .Where(p => p.UserID == user.id).ToList();
            }

            SavePagoViewModel pago = await pagoService.PaymentPréstamo(pagoViewModel);
            if (pago.HasError)
            {
                pago.Products = productService.GetAsync().Result
              .Where(p => p.UserID == user.id).ToList();
                return View(pago);
            }
            else
            {
                return RedirectToRoute(new { controller = "ClientUser", action = "Index" });
            }

        }

        public async Task<IActionResult> PagoAvance()
        {
            SavePagoViewModel pago = new();
            pago.Products = productService.GetAsync().Result
              .Where(p => p.UserID == user.id).ToList();
            return View(pago);
        }

        [HttpPost]
        public async Task<IActionResult> PagoAvance(SavePagoViewModel pagoViewModel)
        {
            if (!ModelState.IsValid)
            {
                pagoViewModel.Products = productService.GetAsync().Result
               .Where(p => p.UserID == user.id).ToList();
                return View(pagoViewModel);
            }

            SavePagoViewModel pago = await pagoService.Avance(pagoViewModel);
            if (pago.HasError)
            {
                pago.Products = productService.GetAsync().Result
              .Where(p => p.UserID == user.id).ToList();
                return View(pago);
            }
            else
            {
                return RedirectToRoute(new { controller = "ClientUser", action = "Index" });
            }

        }

        public async Task<IActionResult> Transferencia()
        {
            SavePagoViewModel pago = new();
            pago.Products = productService.GetAsync().Result
              .Where(p => p.UserID == user.id).ToList();
            return View(pago);
        }

        [HttpPost]
        public async Task<IActionResult> Transferencia(SavePagoViewModel pagoViewModel)
        {
            if (!ModelState.IsValid)
            {
                pagoViewModel.Products = productService.GetAsync().Result
               .Where(p => p.UserID == user.id).ToList();
                return View(pagoViewModel);
            }

            SavePagoViewModel pago = await pagoService.Transferencia(pagoViewModel);
            if (pago.HasError)
            {
                pago.Products = productService.GetAsync().Result
              .Where(p => p.UserID == user.id).ToList();
                return View(pago);
            }
            else
            {
                return RedirectToRoute(new { controller = "ClientUser", action = "Index" });
            }

        }



        public async Task<IActionResult> Delete(int id)
        {
            return View("Delete", await productService.GetEditAsync(id));
        }


        [HttpPost]
        public async Task<IActionResult> Delete(SaveProductViewModel saveprodvm)
        {
            SaveProductViewModel productview = await productService.GetEditAsync(saveprodvm.id);
            productview.BeneficiarioID = null;
            await productService.EditAsync(productview, productview.id);

            return RedirectToRoute(new { controller = "ClientUser", action = "Index" });
        }
    }
}

