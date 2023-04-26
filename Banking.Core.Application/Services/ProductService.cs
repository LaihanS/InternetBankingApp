using AutoMapper;
using Banking.Core.Application.Dtos.Account;
using Banking.Core.Application.Dtos.ImportantDto;
using Banking.Core.Application.Enums;
using Banking.Core.Application.Helpers;
using Banking.Core.Application.IRepositories;
using Banking.Core.Application.IServices;
using Banking.Core.Application.ViewModels.Home;
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
    public class ProductService : GenericAppService<ProductViewModel, SaveProductViewModel, Products>, IProductService
    {
        private readonly IProductRepository productRepository;
        private readonly IUserRepository userRepository;
        private readonly ITransactionRepository transactionRepository;

        private readonly IMapper imapper;
        IHttpContextAccessor httpContextAccessor;
        AuthenticationResponse User = new();
        private readonly Sequence9Digit sequence9 = new();
        public ProductService(ITransactionRepository transactionRepository, IHttpContextAccessor httpContextAccessor, IUserRepository userRepository, IProductRepository productRepository, IMapper imapper) : base(imapper, productRepository)
        {
            this.transactionRepository = transactionRepository;
            this.productRepository = productRepository;
            this.imapper = imapper;
            this.userRepository = userRepository;
            User = httpContextAccessor.HttpContext.Session.Get<AuthenticationResponse>("user");
        }


        public async Task<List<ProductViewModel>> ProductAsigned()
        {
            List<ProductViewModel> prod = imapper.Map<List<ProductViewModel>>(await productRepository.GetAsync());
            List<ProductViewModel> prodasign = new();
            if (prod != null)
            {
                prodasign = prod.Where(p => p.UserID != null).ToList();
            }
            return prodasign;
        }
     
        public override async Task<SaveProductViewModel> Delete(SaveProductViewModel saveproductvm, int id)
        {
            ProductViewModel product = imapper.Map<ProductViewModel>(await productRepository.GetByidAsync(id));
            SaveProductViewModel Saveproduct = imapper.Map<SaveProductViewModel>(product);
            List<Transacciones> transacciones = await transactionRepository.GetAsync();
            List<Products> prod = await productRepository.GetAsync();
            Products productprinc = new();

            if (product.IsPrincipalAccount)
            {
                return Saveproduct;
            }

            if (product.UserHasDebt)
            {
                return Saveproduct;
            }

            if (prod != null)
            {
                if (product.ProductAmount != null && product.ProductType.Equals(ProductTypeEnum.Cuenta_Ahorro.ToString()))
                {
                    productprinc = prod.Find(p => p.UserID == product.UserID && p.IsPrincipalAccount);
                    productprinc.ProductAmount += saveproductvm.ProductAmount;
                    await productRepository.EditAsync(productprinc, productprinc.id);
                }
            }
            
            await base.Delete(saveproductvm, product.id);

            return Saveproduct;

        }
      
        public async Task<ProductViewModel> AddMember(string userid, string account)
        {
            List<ProductViewModel> productlist = imapper.Map<List<ProductViewModel>>(await productRepository.GetAsync());
            ProductViewModel product = productlist.Find(p => p.UnicDigitSequence == account);
            if (product == null)
            {
                return new ProductViewModel();
            }

              product.BeneficiarioID = userid;

            if (product.UserID == userid || product.ProductType.Equals(ProductTypeEnum.Tarjeta.ToString()) || product.ProductType.Equals(ProductTypeEnum.Préstamo.ToString()))
            {
                return new ProductViewModel();
            }
            else
            {
                await productRepository.EditAsync(imapper.Map<Products>(product), product.id);
                return product;
            } 
          
        }

        public async Task<UserViewModel> GetUser(string userid)
        {
            UserViewModel user = imapper.Map<UserViewModel>(await userRepository.GetByidAsync(userid));
            List<ProductViewModel> productlist = imapper.Map<List<ProductViewModel>>(await productRepository.GetAsync());
            productlist = productlist.Where(p => p.BeneficiarioID == user.Id).ToList();
            List<UserViewModel> userlist = imapper.Map<List<UserViewModel>>(await userRepository.GetAsync());

            foreach (ProductViewModel product in productlist)
            {
                product.Usuario = userlist.Find(us => us.Id == product.UserID);
            }

            user.Beneficiarios = productlist;
            return user;

        }


        public async Task<SaveProductViewModel> AddProductAsync(SaveProductViewModel saveproductvm)
         {
            if (saveproductvm == null)
            {
                return saveproductvm;
            }

            if (saveproductvm.ProductType.Equals(ProductTypeEnum.Préstamo.ToString()))
            {
                List<Products> productlist = await productRepository.GetAsync();
                if (productlist.Count() == 0)
                {
                     return new SaveProductViewModel();
                }
                else
                {
                    Products cuentaprincipal = productlist.Find(res => res.UserID == saveproductvm.UserID && res.IsPrincipalAccount);
                    if (cuentaprincipal != null)
                    {
                        double? val = saveproductvm.DebtAmount;
                        if (val.HasValue)
                        {
                            cuentaprincipal.ProductAmount = cuentaprincipal.ProductAmount.GetValueOrDefault() + val.Value;
                        }
                    }

                    await productRepository.EditAsync(cuentaprincipal, cuentaprincipal.id);

                    List<string> sequences = productRepository.GetAsync().Result.Select(p => p.UnicDigitSequence).ToList();
                    saveproductvm.UnicDigitSequence = sequence9.Secuencia(sequences);

                    Products toadd = imapper.Map<Products>(saveproductvm);

                    if (toadd.DebtAmount > 0)
                    {
                        toadd.UserHasDebt = true;
                    }
                    //else if (toadd.DebtAmount == null || toadd.DebtAmount == 0)
                    //{
                    //    toadd.DebtAmount = 0;
                    //}

                    Products PrestamoAdded = await productRepository.AddAsync(toadd);

                    return imapper.Map<SaveProductViewModel>(PrestamoAdded);
                }

            }
            else if (saveproductvm.ProductType.Equals(ProductTypeEnum.Cuenta_Ahorro.ToString()))
            {
                List<string> sequences = productRepository.GetAsync().Result.Select(p => p.UnicDigitSequence).ToList();
                saveproductvm.UnicDigitSequence = sequence9.Secuencia(sequences);

                Products toadd = imapper.Map<Products>(saveproductvm);
                toadd.ProductAmount = 0;
                Products CuentaAdded = await productRepository.AddAsync(toadd);

                return imapper.Map<SaveProductViewModel>(CuentaAdded);
            }
            else if (saveproductvm.ProductType.Equals(ProductTypeEnum.Tarjeta.ToString()))
            {

                List<string> sequences = productRepository.GetAsync().Result.Select(p => p.UnicDigitSequence).ToList();
                saveproductvm.UnicDigitSequence = sequence9.Secuencia(sequences);

                Products toadd = imapper.Map<Products>(saveproductvm);
               
                if (toadd.DebtAmount > 0)
                {
                    toadd.UserHasDebt = true;
                }
                else if(toadd.DebtAmount == null || toadd.DebtAmount ==0)
                {
                    toadd.DebtAmount = 0;
                }

                Products Tarjetadd = await productRepository.AddAsync(toadd);

                return imapper.Map<SaveProductViewModel>(Tarjetadd);
            }
            else
            {
                return new SaveProductViewModel();
            }
          
        }

      
    }
}
