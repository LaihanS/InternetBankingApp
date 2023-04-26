using Banking.Core.Application.ViewModels.Pago;
using Banking.Core.Domain.Entities;

namespace Banking.Core.Application.IServices
{
    public interface IPagoService: IGenericAppService<PagoViewModel, SavePagoViewModel, Pagos>
    {
        Task<SavePagoViewModel> ValidatePayment(SavePagoViewModel pago);
        Task<SavePagoViewModel> ConcretePayment(SavePagoViewModel pago);

        Task<List<PagoViewModel>> GetPagosToday();
        Task<SavePagoViewModel> PaymentTarjeta(SavePagoViewModel pago);

        Task<SavePagoViewModel> Avance(SavePagoViewModel pago);
        Task<SavePagoViewModel> PaymentPréstamo(SavePagoViewModel pago);

        Task<SavePagoViewModel> Transferencia(SavePagoViewModel pago);
    }
}