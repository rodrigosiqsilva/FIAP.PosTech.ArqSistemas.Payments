using Confluent.Kafka;
using System.Text.Json;
using FIAP.PosTech.ArqSistemas.PaymentsWS.Models;

namespace FIAP.PosTech.ArqSistemas.PaymentsWS.Events
{
    public record PaymentProcessedCreatedEvent(Order Order, DateTime CreatedAt);

    public class PaymentProcessedEventPublisher : IDisposable
    {
        private readonly IProducer<string, string> _producer;
        private readonly string _topicName;

        public PaymentProcessedEventPublisher(string bootstrapServers, string topicName)
        {
            _topicName = topicName;

            // Configuração do produtor
            var config = new ProducerConfig
            {
                BootstrapServers = bootstrapServers,
                // Opcional: Configurações adicionais para confiabilidade
                Acks = Acks.All,
                MessageSendMaxRetries = 3
            };

            // Constrói o produtor definindo a Chave (Key) e o Valor (Value) como strings
            _producer = new ProducerBuilder<string, string>(config).Build();
        }

        public async Task PublishOrderPlacedEventAsync(PaymentProcessedCreatedEvent orderEvent)
        {
            try
            {
                // Serializa o objeto do evento para JSON
                var messageValue = JsonSerializer.Serialize(orderEvent);

                // Prepara a mensagem do Kafka
                var message = new Message<string, string>
                {
                    Key = orderEvent.Order.Id.ToString(), // Usar o ID como chave garante a ordenação na mesma partição
                    Value = messageValue
                };

                // Envia a mensagem para o tópico
                var deliveryResult = await _producer.ProduceAsync(_topicName, message);
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Garante que os buffers internos do Kafka sejam esvaziados antes da classe ser destruída
        public void Dispose()
        {
            _producer?.Flush(TimeSpan.FromSeconds(10));
            _producer?.Dispose();
        }

    }
}
