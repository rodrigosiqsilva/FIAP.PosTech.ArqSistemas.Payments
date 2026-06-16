using FIAP.PosTech.ArqSistemas.PaymentsWS.Models;

namespace FIAP.PosTech.ArqSistemas.PaymentsWS.Services
{
    public interface IPaymentProcessedNotificationService
    {
        Task SendNotificationPaymentProcessed(Order order);
    }
}
