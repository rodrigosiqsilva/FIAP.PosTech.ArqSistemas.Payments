
using FIAP.PosTech.ArqSistemas.PaymentsWS.Events;

namespace FIAP.PosTech.ArqSistemas.PaymentsWS.Workers
{
    public class KafkaConsumerWorker : BackgroundService
    {
        private readonly OrderPlacedEventConsumer _consumerOrderPlaced;

        public KafkaConsumerWorker(IConfiguration configuration)
        {
            var bootstrapServers = configuration["KafkaConfig:BootstrapServers"];
            var topicNameUserCreated = configuration["KafkaConfig:TopicNameUserCreated"];
            var topicNamePaymentProcessed = configuration["KafkaConfig:TopicNameOrderPlaced"];
            var groupId = configuration["KafkaConfig:GroupId"];

            _consumerOrderPlaced = new OrderPlacedEventConsumer(bootstrapServers, topicNameUserCreated, groupId, configuration);
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