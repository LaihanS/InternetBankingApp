using AutoMapper;
using Banking.Core.Application.Dtos.Account;
using Banking.Core.Application.Enums;
using Banking.Core.Application.Helpers;
using Banking.Core.Application.IRepositories;
using Banking.Core.Application.IServices;
using Banking.Core.Application.ViewModels.Home;
using Banking.Core.Application.ViewModels.Pago;
using Banking.Core.Application.ViewModels.Product;
using Banking.Core.Application.ViewModels.UserVMS;
using Banking.Core.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banking.Core.Application.Services
{
    public class PagoService : GenericAppService<PagoViewModel, SavePagoViewModel, Pagos>, IPagoService
    {
        private readonly IPagoRepository pagoRepository;
        private readonly IUserRepository userRepository;
        private readonly IProductRepository productRepository;
        private readonly ITransactionRepository transactionRepository;
        private readonly IMapper imapper;
        private readonly Sequence9Digit sequence9 = new();
        private readonly IHttpContextAccessor httpContextAccessor;

        AuthenticationResponse User = new(); 

    public PagoService(IHttpContextAccessor httpContextAccessor, ITransactionRepository transactionRepository, IUserRepository userRepository, IProductRepository productRepository, IPagoRepository pagoRepository, IMapper imapper) : base(imapper, pagoRepository)
        {
            this.User = httpContextAccessor.HttpContext.Session.Get<AuthenticationResponse>("user");
            this.transactionRepository = transactionRepository;
            this.pagoRepository = pagoRepository;
            this.imapper = imapper;
            this.userRepository = userRepository;
            this.productRepository = productRepository;
        }

        public async Task<List<PagoViewModel>> GetPagosToday()
        {
            List<PagoViewModel> viewModels = imapper.Map<List<PagoViewModel>>(await pagoRepository.GetAsync());
            List<PagoViewModel> todaypagos = new();

            if (viewModels != null)
            {
                todaypagos = viewModels.Where(p => p.created.Value.Day == DateTime.Now.Day).ToList();
            }

            return todaypagos;
        }


        public async Task<SavePagoViewModel> ValidatePayment(SavePagoViewModel pago)
        {
            SavePagoViewModel pagonew = new();

            ProductViewModel cuentafor = imapper.Map<ProductViewModel>(await productRepository.GetAccountByDigits(pago.PaymentFor));
            ProductViewModel cuentafrom = imapper.Map<ProductViewModel>(await productRepository.GetAccountByDigits(pago.PaymentFrom));

            if (pago.PaymentAmount == null || pago.PaymentAmount == 0)
            {
                pagonew.HasError = true;
                pagonew.ErrorDetails = "No puede transferir una nada";
                return pagonew;
            }

            if (cuentafor.UnicDigitSequence == null)
            {
                pagonew.HasError = true;
                pagonew.ErrorDetails = "No puede transferir a este usuario, ya que no existe";
                return pagonew;
            }

            if (cuentafrom.UnicDigitSequence == null)
            {
                pagonew.HasError = true;
                pagonew.ErrorDetails = "No puede transferir de este usuario, ya que no existe o no fue seleccionado";
                return pagonew;
            }

            UserViewModel users = imapper.Map<UserViewModel>(await userRepository.GetUserAccount(cuentafor.UserID));

            //if (users.Id != null)
            //{
            //    if (User.id == users.Id)
            //    {
            //        pagonew.HasError = true;
            //        pagonew.ErrorDetails = "No puedes transferirte a ti mismo locotron";
            //        return pagonew;
            //    }
            //}

             if (cuentafor.UnicDigitSequence == cuentafrom.UnicDigitSequence)
            {
                pagonew.HasError = true;
                pagonew.ErrorDetails = "No puede transferir a la misma cuenta que seleccionó";
                return pagonew;
            }

            if (users != null)
            {
                cuentafor.Usuario = users;
            }
          
            if (!cuentafor.ProductType.Equals(ProductTypeEnum.Cuenta_Ahorro.ToString()))
            {
                pagonew.HasError = true;
                pagonew.ErrorDetails = "Este número no es de una cuenta de ahorro";
                return pagonew;
            }

            if (cuentafrom.ProductAmount < pago.PaymentAmount)
            {
                pagonew.HasError = true;
                pagonew.ErrorDetails = "No tiene suficiente dinero en la cuenta para llevar a cabo la transferencia";
                return pagonew;
            }

            pagonew = pago;
            pagonew.Cuenta = cuentafor;
            pagonew.ProductID = cuentafor.id;

            return pagonew;

        }

        public async Task<SavePagoViewModel> ConcretePayment(SavePagoViewModel pago)
        {

            SavePagoViewModel pagonew = new();
            Products cuentafor = await productRepository.GetAccountByDigits(pago.PaymentFor);
            Products cuentafrom = await productRepository.GetAccountByDigits(pago.PaymentFrom);

            if (pago.PaymentAmount == null || pago.PaymentAmount == 0)
            {
                pagonew.HasError = true;
                pagonew.ErrorDetails = "No puede transferir una nada";
                return pagonew;
            }

            if (cuentafor.UnicDigitSequence == null)
            {
                pagonew.HasError = true;
                pagonew.ErrorDetails = "No puede transferir a este usuario, ya que no existe";
                return pagonew;
            }

            if (cuentafrom.UnicDigitSequence == null)
            {
                pagonew.HasError = true;
                pagonew.ErrorDetails = "No puede transferir a este usuario, ya que no existe";
                return pagonew;
            }

            double? val = pago.PaymentAmount;
            if (val.HasValue)
            {
                cuentafor.ProductAmount = cuentafor.ProductAmount.GetValueOrDefault() + val.Value;
                cuentafrom.ProductAmount -= val.Value;

                await productRepository.EditAsync(cuentafor, cuentafor.id);
                await productRepository.EditAsync(cuentafrom, cuentafrom.id);
            }
            SaveTransactionViewModel transaction = new();
            transaction = imapper.Map<SaveTransactionViewModel>(pago);
            if (pago.IsTransaction)
            {
                await transactionRepository.AddAsync(imapper.Map<Transacciones>(transaction));
            }
            else
            {
                await pagoRepository.AddAsync(imapper.Map<Pagos>(pago));
                await transactionRepository.AddAsync(imapper.Map<Transacciones>(transaction));
            }

            return pago;
        }

        public async Task<SavePagoViewModel> PaymentTarjeta(SavePagoViewModel pago)
        {

            SavePagoViewModel pagonew = new();

            List<ProductViewModel> listarjeta = imapper.Map<List<ProductViewModel>>(await productRepository.GetAsync());
            if (listarjeta == null)
            {
                pagonew.HasError = true;
                pagonew.ErrorDetails = "No hay productos en este sistema";
                return pagonew;
            }
            ProductViewModel tarjetafor = listarjeta.Find(p => p.UnicDigitSequence == pago.PaymentFor);
            List<ProductViewModel> Cuentalist = imapper.Map<List<ProductViewModel>>(await productRepository.GetAsync());

            if (pago.PaymentAmount == null || pago.PaymentAmount == 0)
            {
                pagonew.HasError = true;
                pagonew.ErrorDetails = "No puede transferir una nada";
                return pagonew;
            }

            ProductViewModel Cuentafrom = Cuentalist.Find(p => p.UnicDigitSequence == pago.PaymentFrom);
            
            if (pago.PaymentAmount == null || pago.PaymentAmount == 0)
            {
                pagonew.HasError = true;
                pagonew.ErrorDetails = "No puede transferir una nada";
                return pagonew;
            }

            if (tarjetafor == null)
            {
                pagonew.HasError = true;
                pagonew.ErrorDetails = "No puede transferir a esta tarjeta, ya que no existe";
                return pagonew;
            }

            if (Cuentafrom == null)
            {
                pagonew.HasError = true;
                pagonew.ErrorDetails = "No puede transferir de esta cuenta, ya que no existe";
                return pagonew;
            }

            List<UserViewModel> userlist = imapper.Map<List<UserViewModel>>(await userRepository.GetAsync());

            if (userlist == null)
            {
                pagonew.HasError = true;
                pagonew.ErrorDetails = "Hay un chukibobote. No hay usuarios";
                return pagonew;
            }

            UserViewModel user = userlist.Find(p => p.Id == tarjetafor.UserID);

            if (user != null)
            {
                tarjetafor.Usuario = user;
            }

            if (!tarjetafor.ProductType.Equals(ProductTypeEnum.Tarjeta.ToString()))
            {
                pagonew.HasError = true;
                pagonew.ErrorDetails = "Este número no es de una tarjeta de crédito";
                return pagonew;
            }

            if (Cuentafrom.ProductAmount < pago.PaymentAmount)
            {
                pagonew.HasError = true;
                pagonew.ErrorDetails = "No tiene suficiente dinero en la cuenta para llevar a cabo la transferencia";
                return pagonew;
            }

            double? val = pago.PaymentAmount;
            double? restval = 0f;
            if (val.HasValue)
            {
                if (val.Value > tarjetafor.DebtAmount)
                {
                    restval = tarjetafor.DebtAmount - val;
                    restval = restval + val;

                    tarjetafor.DebtAmount -= restval.Value;
                    Cuentafrom.ProductAmount -= restval.Value;
                }
                else
                {
                    tarjetafor.DebtAmount -= val.Value;
                    Cuentafrom.ProductAmount -= val.Value;
                }

                if (tarjetafor.DebtAmount == 0)
                {
                    tarjetafor.UserHasDebt = false; 
                }

                await productRepository.EditAsync(imapper.Map<Products>(tarjetafor), tarjetafor.id);
                await productRepository.EditAsync(imapper.Map<Products>(Cuentafrom), Cuentafrom.id);
            }


            pagonew = pago;
            pagonew.Cuenta = tarjetafor;
            pagonew.ProductID = tarjetafor.id;

            SaveTransactionViewModel transaction = new();
            transaction = imapper.Map<SaveTransactionViewModel>(pago);
            await transactionRepository.AddAsync(imapper.Map<Transacciones>(transaction));
            await pagoRepository.AddAsync(imapper.Map<Pagos>(pago));

            return pagonew;

        }


        public async Task<SavePagoViewModel> PaymentPréstamo(SavePagoViewModel pago)
        {

            SavePagoViewModel pagonew = new();

            List<ProductViewModel> listapréstamo = imapper.Map<List<ProductViewModel>>(await productRepository.GetAsync());
            if (listapréstamo == null)
            {
                pagonew.HasError = true;
                pagonew.ErrorDetails = "No hay productos en este sistema";
                return pagonew;
            }
            ProductViewModel prestamofor = listapréstamo.Find(p => p.UnicDigitSequence == pago.PaymentFor);
            List<ProductViewModel> Cuentalist = imapper.Map<List<ProductViewModel>>(await productRepository.GetAsync());

            if (pago.PaymentAmount == null || pago.PaymentAmount == 0)
            {
                pagonew.HasError = true;
                pagonew.ErrorDetails = "No puede transferir una nada";
                return pagonew;
            }

            ProductViewModel Cuentafrom = Cuentalist.Find(p => p.UnicDigitSequence == pago.PaymentFrom);
           
            if (pago.PaymentAmount == null || pago.PaymentAmount == 0)
            {
                pagonew.HasError = true;
                pagonew.ErrorDetails = "No puede transferir una nada";
                return pagonew;
            }


            if (prestamofor == null)
            {
                pagonew.HasError = true;
                pagonew.ErrorDetails = "No puede transferir a este préstamo, ya que no existe";
                return pagonew;
            }

            if (Cuentafrom == null)
            {
                pagonew.HasError = true;
                pagonew.ErrorDetails = "No puede transferir de esta cuenta, ya que no existe";
                return pagonew;
            }

            List<UserViewModel> userlist = imapper.Map<List<UserViewModel>>(await userRepository.GetAsync());

            if (userlist == null)
            {
                pagonew.HasError = true;
                pagonew.ErrorDetails = "Hay un chukibobote. No hay usuarios";
                return pagonew;
            }

            UserViewModel user = userlist.Find(p => p.Id == prestamofor.UserID);

            if (user != null)
            {
                prestamofor.Usuario = user;
            }

            if (!prestamofor.ProductType.Equals(ProductTypeEnum.Préstamo.ToString()))
            {
                pagonew.HasError = true;
                pagonew.ErrorDetails = "Este número no es de un préstamo";
                return pagonew;
            }

            if (Cuentafrom.ProductAmount < pago.PaymentAmount)
            {
                pagonew.HasError = true;
                pagonew.ErrorDetails = "No tiene suficiente dinero en la cuenta para llevar a cabo la transferencia";
                return pagonew;
            }

            double? val = pago.PaymentAmount;
            double? restval = 0;
            if (val.HasValue)
            {
                if (val.Value > prestamofor.DebtAmount)
                {
                    restval = prestamofor.DebtAmount - val;
                    restval = restval + val;

                    prestamofor.DebtAmount -= restval.Value;
                    Cuentafrom.ProductAmount -= restval.Value;
                }
                else
                {
                    prestamofor.DebtAmount -= val.Value;
                    Cuentafrom.ProductAmount -= val.Value;
                }

                if (prestamofor.DebtAmount == 0)
                {
                    prestamofor.UserHasDebt = false;
                }

                await productRepository.EditAsync(imapper.Map<Products>(prestamofor), prestamofor.id);
                await productRepository.EditAsync(imapper.Map<Products>(Cuentafrom), Cuentafrom.id);
            }


            pagonew = pago;
            pagonew.Cuenta = prestamofor;
            pagonew.ProductID = prestamofor.id;

            SaveTransactionViewModel transaction = new();
            transaction = imapper.Map<SaveTransactionViewModel>(pago);
            await transactionRepository.AddAsync(imapper.Map<Transacciones>(transaction));
            await pagoRepository.AddAsync(imapper.Map<Pagos>(pago));

            return pagonew;

        }

        public async Task<SavePagoViewModel> Avance(SavePagoViewModel pago)
        {

            SavePagoViewModel pagonew = new();

            List<ProductViewModel> cuentas = imapper.Map<List<ProductViewModel>>(await productRepository.GetAsync());
            if (cuentas == null)
            {
                pagonew.HasError = true;
                pagonew.ErrorDetails = "No hay cuentas con ese número";
                return pagonew;
            }
            ProductViewModel cuentafor = cuentas.Find(p => p.UnicDigitSequence == pago.PaymentFor);
            List<ProductViewModel> tarjetas = imapper.Map<List<ProductViewModel>>(await productRepository.GetAsync());

            if (tarjetas == null)
            {
                pagonew.HasError = true;
                pagonew.ErrorDetails = "No hay tarjeta con ese número";
                return pagonew;
            }
            ProductViewModel tarjetafrom = tarjetas.Find(p => p.UnicDigitSequence == pago.PaymentFrom);

            if (pago.PaymentAmount == null || pago.PaymentAmount == 0)
            {
                pagonew.HasError = true;
                pagonew.ErrorDetails = "No puede transferir una nada";
                return pagonew;
            }

            if (cuentafor == null)
            {
                pagonew.HasError = true;
                pagonew.ErrorDetails = "No puede transferir a esta cuenta, ya que no existe";
                return pagonew;
            }

            if (tarjetafrom == null)
            {
                pagonew.HasError = true;
                pagonew.ErrorDetails = "No puede transferir de esta tarjeta, ya que no existe";
                return pagonew;
            }
            List<UserViewModel> userlist = imapper.Map<List<UserViewModel>>(await userRepository.GetAsync());

            if (userlist == null)
            {
                pagonew.HasError = true;
                pagonew.ErrorDetails = "Hay un chukibobote. No hay usuarios";
                return pagonew;
            }

            UserViewModel user = userlist.Find(p => p.Id == cuentafor.UserID);

            if (user != null)
            {
                cuentafor.Usuario = user;
            }

            if (!tarjetafrom.ProductType.Equals(ProductTypeEnum.Tarjeta.ToString()))
            {
                pagonew.HasError = true;
                pagonew.ErrorDetails = "Este número no es de una tarjeta";
                return pagonew;
            }

            if (!cuentafor.ProductType.Equals(ProductTypeEnum.Cuenta_Ahorro.ToString()))
            {
                pagonew.HasError = true;
                pagonew.ErrorDetails = "Este número no es de una cuenta";
                return pagonew;
            }

            if (tarjetafrom.ProductAmount < pago.PaymentAmount)
            {
                pagonew.HasError = true;
                pagonew.ErrorDetails = $"El límite de su tarjeta es {tarjetafrom.ProductAmount} y usted intentó tomar prestado {pago.PaymentAmount}";
                return pagonew;
            }

            double? val = pago.PaymentAmount;
            double? restval = 0f;
            if (val.HasValue)
            {
               
                cuentafor.ProductAmount += val.Value;
                tarjetafrom.DebtAmount += (val.Value + (val.Value * 0.0625));

                if (tarjetafrom.DebtAmount >= tarjetafrom.ProductAmount)
                {
                    pagonew.HasError = true;
                    pagonew.ErrorDetails = "Ha superado el límite de la tarjeta";
                    return pagonew;
                }
                else
                {
                    tarjetafrom.UserHasDebt = true;
                    await productRepository.EditAsync(imapper.Map<Products>(cuentafor), cuentafor.id);
                    await productRepository.EditAsync(imapper.Map<Products>(tarjetafrom), tarjetafrom.id);
                }

               
            }


            pagonew = pago;
            pagonew.Cuenta = cuentafor;
            pagonew.ProductID = cuentafor.id;

            SaveTransactionViewModel transaction = new();
            transaction = imapper.Map<SaveTransactionViewModel>(pago);
            await transactionRepository.AddAsync(imapper.Map<Transacciones>(transaction));
            //await pagoRepository.AddAsync(imapper.Map<Pagos>(pago));

            return pagonew;

        }


        public async Task<SavePagoViewModel> Transferencia(SavePagoViewModel pago)
        {

            SavePagoViewModel pagonew = new();

            List<ProductViewModel> cuentas = imapper.Map<List<ProductViewModel>>(await productRepository.GetAsync());
            if (cuentas == null)
            {
                pagonew.HasError = true;
                pagonew.ErrorDetails = "No hay cuentas con ese número";
                return pagonew;
            }
            ProductViewModel cuentafor = cuentas.Find(p => p.UnicDigitSequence == pago.PaymentFor);
            List<ProductViewModel> tarjetas = imapper.Map<List<ProductViewModel>>(await productRepository.GetAsync());
            if (tarjetas == null)
            {
                pagonew.HasError = true;
                pagonew.ErrorDetails = "No hay cuenta con ese número";
                return pagonew;
            }
            ProductViewModel cuentafrom = tarjetas.Find(p => p.UnicDigitSequence == pago.PaymentFrom);

            if (pago.PaymentAmount == null || pago.PaymentAmount == 0)
            {
                pagonew.HasError = true;
                pagonew.ErrorDetails = "No puede transferir una nada";
                return pagonew;
            }

            if (cuentafor == null)
            {
                pagonew.HasError = true;
                pagonew.ErrorDetails = "No puede transferir a esta cuenta, ya que no existe";
                return pagonew;
            }

            if (cuentafrom == null)
            {
                pagonew.HasError = true;
                pagonew.ErrorDetails = "No puede transferir de esta tarjeta, ya que no existe";
                return pagonew;
            }
            List<UserViewModel> userlist = imapper.Map<List<UserViewModel>>(await userRepository.GetAsync());

            if (userlist == null)
            {
                pagonew.HasError = true;
                pagonew.ErrorDetails = "Hay un chukibobote. No hay usuarios";
                return pagonew;
            }

            UserViewModel user = userlist.Find(p => p.Id == cuentafor.UserID);

            if (user != null)
            {
                cuentafor.Usuario = user;
            }

            if (!cuentafrom.ProductType.Equals(ProductTypeEnum.Cuenta_Ahorro.ToString()) || !cuentafor.ProductType.Equals(ProductTypeEnum.Cuenta_Ahorro.ToString()))
            {
                pagonew.HasError = true;
                pagonew.ErrorDetails = "Este número no es de una cu";
                return pagonew;
            }

            if (cuentafrom.ProductAmount < pago.PaymentAmount)
            {
                pagonew.HasError = true;
                pagonew.ErrorDetails = "No tiene suficiente dinero en la cuenta para llevar a cabo la transferencia";
                return pagonew;
            }

            if (cuentafor.UnicDigitSequence == cuentafrom.UnicDigitSequence)
            {
                pagonew.HasError = true; 
                pagonew.ErrorDetails = "No puede transferir a la misma cuenta. Tú ta loco compai?";
                return pagonew;
            }

            double? val = pago.PaymentAmount;
            double? restval = 0f;
            if (val.HasValue)
            {
                cuentafor.ProductAmount = cuentafor.ProductAmount + val.Value;
                cuentafrom.ProductAmount -= val.Value;
                await productRepository.EditAsync(imapper.Map<Products>(cuentafor), cuentafor.id);
                await productRepository.EditAsync(imapper.Map<Products>(cuentafrom), cuentafrom.id);              
            }

            pagonew = pago;
            pagonew.Cuenta = cuentafor;
            pagonew.ProductID = cuentafor.id;

            SaveTransactionViewModel transaction = new();
            transaction = imapper.Map<SaveTransactionViewModel>(pago);
            await transactionRepository.AddAsync(imapper.Map<Transacciones>(transaction));
            //await pagoRepository.AddAsync(imapper.Map<Pagos>(pago));

            return pagonew;

        }

    }
}
