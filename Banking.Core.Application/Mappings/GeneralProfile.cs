using AutoMapper;
using Banking.Core.Application.Dtos.Account;
using Banking.Core.Application.Dtos.ImportantDto;
using Banking.Core.Application.ViewModels.Home;
using Banking.Core.Application.ViewModels.Pago;
using Banking.Core.Application.ViewModels.Product;
using Banking.Core.Application.ViewModels.UserVMS;
using Banking.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banking.Core.Application.Mappings
{
    public class GeneralProfile: Profile
    {
        public GeneralProfile() {

            #region UserMappings
            CreateMap<AuthenticationRequest, LoginViewModel>().
                   ForMember(loginvm => loginvm.ErrorDetails, action => action.Ignore())
                    .ForMember(loginvm => loginvm.HasError, action => action.Ignore())
                   .ReverseMap();

            CreateMap<RegisterRequest, SaveUserViewModel>()
                .ForMember(loginvm => loginvm.ErrorDetails, action => action.Ignore())
                    .ForMember(loginvm => loginvm.HasError, action => action.Ignore())
                    .ForMember(loginvm => loginvm.Id, action => action.Ignore())
                  .ReverseMap();

            CreateMap<ForgotPassworRequest, ForgotPasswordViewModel>()
                .ForMember(loginvm => loginvm.ErrorDetails, action => action.Ignore())
                    .ForMember(loginvm => loginvm.HasError, action => action.Ignore())
               .ReverseMap();

            CreateMap<UserDto, UserViewModel>()
                 .ForMember(uservm => uservm.Roles, action => action.MapFrom(ac => ac.Roles.ToList()))
            .ReverseMap()
             .ForMember(uservm => uservm.Roles, action => action.MapFrom(ac => ac.Roles.ToList()));

            CreateMap<UserDto, SaveUserViewModel>()
                .ForMember(loginvm => loginvm.ErrorDetails, action => action.Ignore())
                 .ForMember(loginvm => loginvm.ErrorDetails, action => action.Ignore())
           .ReverseMap()
             .ForMember(loginvm => loginvm.Productos, action => action.Ignore());

            CreateMap<UserViewModel, SaveUserViewModel>()
             .ForMember(loginvm => loginvm.ErrorDetails, action => action.Ignore())
              .ForMember(loginvm => loginvm.ErrorDetails, action => action.Ignore())
        .ReverseMap()
          .ForMember(loginvm => loginvm.Productos, action => action.Ignore());

            CreateMap<User, UserDto>()
                .ForMember(loginvm => loginvm.Password, action => action.Ignore())
                .ForMember(loginvm => loginvm.ConfirmPassword, action => action.Ignore())
        .ReverseMap()
          .ForMember(loginvm => loginvm.TwoFactorEnabled, action => action.Ignore())
          .ForMember(loginvm => loginvm.SecurityStamp, action => action.Ignore())
           .ForMember(loginvm => loginvm.PhoneNumber, action => action.Ignore())
            .ForMember(loginvm => loginvm.LockoutEnabled, action => action.Ignore())
             .ForMember(loginvm => loginvm.AccessFailedCount, action => action.Ignore())
              .ForMember(loginvm => loginvm.LockoutEnd, action => action.Ignore())
               .ForMember(loginvm => loginvm.NormalizedEmail, action => action.Ignore())
                .ForMember(loginvm => loginvm.NormalizedUserName, action => action.Ignore())
                .ForMember(loginvm => loginvm.PasswordHash, action => action.Ignore())
          .ForMember(loginvm => loginvm.PhoneNumberConfirmed, action => action.Ignore());


            CreateMap<ResetPasswordRequest, ResetPasswordViewModel>()
            .ForMember(loginvm => loginvm.ErrorDetails, action => action.Ignore())
                .ForMember(loginvm => loginvm.HasError, action => action.Ignore())
           .ReverseMap();

            CreateMap<ProductDto, SaveProductViewModel>()
           .ReverseMap()
            .ForMember(loginvm => loginvm.Usuario, action => action.Ignore());


            #endregion

            #region ProductMappings

            CreateMap<Products, SaveProductViewModel>()
        .ReverseMap()
         .ForMember(loginvm => loginvm.created, action => action.Ignore())
         .ForMember(loginvm => loginvm.modifiedBy, action => action.Ignore())
         .ForMember(loginvm => loginvm.modifiedAt, action => action.Ignore())
         .ForMember(loginvm => loginvm.createdBy, action => action.Ignore());

            CreateMap<Products, ProductViewModel>()
            .ForMember(loginvm => loginvm.Usuario, action => action.Ignore())
          .ReverseMap()
           .ForMember(loginvm => loginvm.created, action => action.Ignore())
           .ForMember(loginvm => loginvm.modifiedBy, action => action.Ignore())
           .ForMember(loginvm => loginvm.modifiedAt, action => action.Ignore())
           .ForMember(loginvm => loginvm.createdBy, action => action.Ignore());

            CreateMap<ProductViewModel, SaveProductViewModel>()
          .ReverseMap()
           .ForMember(loginvm => loginvm.Usuario, action => action.Ignore());
            #endregion

            #region PagosMappings

            CreateMap<Pagos, SavePagoViewModel>()
        .ReverseMap()
         .ForMember(loginvm => loginvm.modifiedBy, action => action.Ignore())
         .ForMember(loginvm => loginvm.modifiedAt, action => action.Ignore())
         .ForMember(loginvm => loginvm.createdBy, action => action.Ignore());

            CreateMap<Pagos, PagoViewModel>()
          .ReverseMap()
           .ForMember(loginvm => loginvm.modifiedBy, action => action.Ignore())
           .ForMember(loginvm => loginvm.modifiedAt, action => action.Ignore())
           .ForMember(loginvm => loginvm.createdBy, action => action.Ignore());

            CreateMap<PagoViewModel, SavePagoViewModel>()
          .ReverseMap();
            #endregion

            #region Transac

            CreateMap<Transacciones, TransactionViewModel>()
          .ReverseMap()
           .ForMember(loginvm => loginvm.modifiedBy, action => action.Ignore())
           .ForMember(loginvm => loginvm.modifiedAt, action => action.Ignore())
           .ForMember(loginvm => loginvm.createdBy, action => action.Ignore());

            CreateMap<Transacciones, SaveTransactionViewModel>()
         .ReverseMap()
          .ForMember(loginvm => loginvm.modifiedBy, action => action.Ignore())
          .ForMember(loginvm => loginvm.modifiedAt, action => action.Ignore())
          .ForMember(loginvm => loginvm.createdBy, action => action.Ignore());

            CreateMap<SavePagoViewModel, SaveTransactionViewModel>()
       .ReverseMap()
        .ForMember(loginvm => loginvm.Products, action => action.Ignore())
        .ForMember(loginvm => loginvm.Cuenta, action => action.Ignore())
         .ForMember(loginvm => loginvm.ErrorDetails, action => action.Ignore())
        .ForMember(loginvm => loginvm.HasError, action => action.Ignore());


            CreateMap<TransactionViewModel, SaveTransactionViewModel>()
         .ReverseMap();
            #endregion
        }
    }
}
