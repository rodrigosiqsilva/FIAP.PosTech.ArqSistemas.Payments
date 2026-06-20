using FIAP.PosTech.ArqSistemas.PaymentsWS.Events;
using FIAP.PosTech.ArqSistemas.PaymentsWS.Services;

namespace FIAP.PosTech.ArqSistemas.PaymentsWS.Workers
{
    public class KafkaConsumerWorker : BackgroundService
    {
        private readonly OrderPlacedEventConsumer _consumerOrderPlaced;
        private readonly PaymentProcessedEventPublisher _eventPublisher;

        public KafkaConsumerWorker(IConfiguration configuration, IServiceProvider serviceProvider)
        {
            var bootstrapServers = configuration["KafkaConfig:BootstrapServers"];
            var topicNamePaymentProcessed = configuration["KafkaConfig:TopicNamePaymentProcessed"];
            var topicNameOrderPlaced = configuration["KafkaConfig:TopicNameOrderPlaced"];
            var groupId = configuration["KafkaConfig:GroupId"];

            _eventPublisher = new PaymentProcessedEventPublisher(bootstrapServers, topicNamePaymentProcessed);
            _consumerOrderPlaced = new OrderPlacedEventConsumer(
                bootstrapServers,
                topicNameOrderPlaced,
                groupId,
                configuration,
                serviceProvider,
                _eventPublisher); 
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