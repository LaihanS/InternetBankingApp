using AutoMapper;
using Banking.Core.Application.Dtos.Account;
using Banking.Core.Application.IServices;
using Banking.Core.Application.Services;
using Banking.Core.Application.ViewModels.Home;
using Banking.Core.Application.ViewModels.Product;
using Banking.Core.Application.ViewModels.UserVMS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Crypto;
using System.Data;
using WebApp.Banking.Middlewares;

namespace WebApp.Banking.Controllers
{
    [Authorize(Roles = "SuperAdmin, Admin")]
    public class UserMantainmentController : Controller
    {
      
        private readonly IMapper _mapper;
        private readonly IUserService userService;
        private readonly IProductService productService;
        private readonly IPagoService pagoService;
        private readonly ITransactionService transaction;
        private readonly IHttpContextAccessor http;
        public UserMantainmentController(IPagoService pagoService, ITransactionService transaction, IProductService productService, IHttpContextAccessor http, IMapper _mapper, IUserService userService)
        {
            this.pagoService = pagoService;
            this.transaction = transaction;
            this.productService = productService;
            this.http = http;
            this._mapper = _mapper;
            this.userService = userService;

        }

        public async Task<IActionResult> Mantain()
        {
            return View(await userService.GetAllUsersAsyncJoined());
        }

        public async Task<IActionResult> Register()
        {

            return View(new SaveUserViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Register(SaveUserViewModel saveuservm)
        {
            if (!ModelState.IsValid)
            {
                return View("Register", saveuservm);

            }
            string origin = Request.Headers["origin"];
            RegisterResponse responserregister = await userService.RegisterAsync(saveuservm, origin);
            if (responserregister.HasError)
            {
                saveuservm.HasError = responserregister.HasError;
                saveuservm.ErrorDetails = responserregister.ErrorDetails;
                return View("Register", saveuservm);
            }
            return RedirectToRoute(new { controller = "UserMantainment", action = "Mantain" });
        }

        public async Task<IActionResult> Edit(string id)
        {
            List<SaveUserViewModel> users = _mapper.Map<List<SaveUserViewModel>>( await userService.GetAllUsersAsyncJoined());
            SaveUserViewModel user = users.Find(us => us.Id == id);
            return View("Register", user);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SaveUserViewModel saveuservm)
        {
            if (!ModelState.IsValid)
            {
                List<SaveUserViewModel> users = _mapper.Map<List<SaveUserViewModel>>(await userService.GetAllUsersAsyncJoined());
                SaveUserViewModel user = users.Find(us => us.Id == saveuservm.Id);
                saveuservm.Roles = user.Roles;
                return View("Register", saveuservm);

            } 
             await userService.EditUser(saveuservm);
          
            return RedirectToRoute(new { controller = "UserMantainment", action = "Index" });
        }

        public async Task<IActionResult> Delete(string id)
        {
            return View("DeleteUser", await userService.GetEditAsync(id));
        }


        [HttpPost]
        public async Task<IActionResult> Delete(SaveUserViewModel saveuservm)
        {      
            UserViewModel user = _mapper.Map<UserViewModel>(await userService.GetEditAsync(saveuservm.Id)); 
            await userService.DeleteUserAsync(user);

            return RedirectToRoute(new { controller = "UserMantainment", action = "Mantain" });
        }


        public async Task<IActionResult> UsActivate(string id)
        {
            return View("Activate", await userService.GetEditAsync(id));
        }
        public async Task<IActionResult> InActivate(string id)
        {
            return View("Inactivate", await userService.GetEditAsync(id));
        }

        [HttpPost]
        public async Task<IActionResult> Activate(string id)
        {
            await userService.ActivateOrInactivate(id);
            return RedirectToRoute(new { controller = "UserMantainment", action = "Mantain" });
        }

        public async Task<IActionResult> AddProduct(SaveProductViewModel saveProduct)
        {
            List<ProductViewModel> products = _mapper.Map<List<ProductViewModel>>(await productService.GetAsync());
            saveProduct.products = products.Where(p => p.UserID == saveProduct.UserID).ToList();
            return View("ProductAdd", saveProduct);
        }


        [HttpPost]
        public async Task<IActionResult> ProductAdd(SaveProductViewModel saveProduct)
        {
            //if (!ModelState.IsValid)
            //{
            //    return View("Register", saveProduct);

            //}
            SaveProductViewModel responserregister = await productService.AddProductAsync(saveProduct);
           
            return RedirectToRoute(new { controller = "UserMantainment", action = "Mantain" });
        }

        public async Task<IActionResult> Index()
        {
            HomeViewModel home = new();
            home.PagosToday = await pagoService.GetPagosToday();
            home.Pagos = await pagoService.GetAsync();
            home.Transacciones = await transaction.GetAsync();
            home.TransaccionesToday = await transaction.GetTransactionsToday();
            home.ClientesActivos = await userService.ActiveClients();
            home.ClientesInactivos = await userService.UnactiveClients();
            home.Productos = await productService.ProductAsigned();

            return View(home);
        }


        public async Task<IActionResult> DeleteProduct(int id)
        {
            return View("DeleteProduct", await productService.GetEditAsync(id));
        }


        [HttpPost]
        public async Task<IActionResult> DeleteProduct(SaveProductViewModel saveprodvm)
        {
            SaveProductViewModel produ = await productService.GetEditAsync(saveprodvm.id);
            await productService.Delete(produ, saveprodvm.id);

            return RedirectToRoute(new { controller = "UserMantainment", action = "Mantain" });
        }

    }
}
