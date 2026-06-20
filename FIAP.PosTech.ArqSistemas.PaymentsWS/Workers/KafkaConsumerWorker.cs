using FIAP.PosTech.ArqSistemas.PaymentsWS.Events;
using FIAP.PosTech.ArqSistemas.PaymentsWS.Services;

namespace FIAP.PosTech.ArqSistemas.PaymentsWS.Workers
{
    public class KafkaConsumerWorker : BackgroundService
    {
        private readonly OrderPlacedEventConsumer _consumerOrderPlaced;

        public KafkaConsumerWorker(IConfiguration configuration, IServiceProvider serviceProvider)
        {
            var bootstrapServers = configuration["KafkaConfig:BootstrapServers"];
            var topicNamePaymentProcessed = configuration["KafkaConfig:TopicNameOrderPlaced"];
            var groupId = configuration["KafkaConfig:GroupId"];

            // PASSAMOS O serviceProvider EM VEZ DO SERVIÇO DIRETO
            _consumerOrderPlaced = new OrderPlacedEventConsumer(
                bootstrapServers,
                topicNamePaymentProcessed,
                groupId,
                configuration,
                serviceProvider); // <-- Alterado aqui
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("[Worker] Iniciando o consumo de mensagens do Kafka...");
            await _consumerOrderPlaced.StartConsumingAsync(stoppingToken);
        }

        public override void Dispose()
        {
            _consumerOrderPlaced.Dispose();
            base.Dispose();
        }
    }
}