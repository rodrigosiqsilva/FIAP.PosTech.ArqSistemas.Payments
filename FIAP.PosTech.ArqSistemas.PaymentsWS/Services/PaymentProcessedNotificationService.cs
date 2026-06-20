using FIAP.PosTech.ArqSistemas.PaymentsWS.Events;
using FIAP.PosTech.ArqSistemas.PaymentsWS.DTOs;

namespace FIAP.PosTech.ArqSistemas.PaymentsWS.Services
{

    public class PaymentProcessedNotificationService : IPaymentProcessedNotificationService
    {
        private readonly IConfiguration _configuration;

        public PaymentProcessedNotificationService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendNotificationPaymentProcessed(OrderDto order)
        {
            string bootstrapServers = _configuration["KafkaConfig:BootstrapServers"];
            string topicName = _configuration["KafkaConfig:TopicNamePaymentProcessed"];

            // Cria o evento
            var newEvent = new PaymentProcessedCreatedEvent(
                Order: order,
                CreatedAt: DateTime.UtcNow
            );

            using (var publisher = new PaymentProcessedEventPublisher(bootstrapServers, topicName))
            {
                try
                {
                    Console.WriteLine("Publicando evento...");
                    await publisher.PublishOrderPlacedEventAsync(newEvent);
                    Console.WriteLine($"Evento publicado com sucesso! {newEvent}");

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao publicar evento: {ex.Message}");
                }
            }
        }

    }
}
