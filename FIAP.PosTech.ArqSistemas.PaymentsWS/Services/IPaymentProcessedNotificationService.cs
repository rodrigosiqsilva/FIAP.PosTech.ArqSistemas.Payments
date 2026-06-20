using FIAP.PosTech.ArqSistemas.PaymentsWS.DTOs;

namespace FIAP.PosTech.ArqSistemas.PaymentsWS.Services
{
    public interface IPaymentProcessedNotificationService
    {
        Task SendNotificationPaymentProcessed(OrderDto order);
    }
}
