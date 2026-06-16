using Confluent.Kafka;
using FIAP.PosTech.ArqSistemas.PaymentsWS.DTOs;
using System.Text.Json;

namespace FIAP.PosTech.ArqSistemas.PaymentsWS.Events
{
    public record OrderPlacedEventCreated(OrderDto Order, DateTime CreatedAt, string? CorrelationId);

    public class OrderPlacedEventConsumer : IDisposable
    {
        private readonly IConsumer<string, string> _consumer;
        private readonly string _topicName;
        private readonly IConfiguration _configuration;

        // Adicionamos IConfiguration no construtor
        public OrderPlacedEventConsumer(string bootstrapServers, string topicName, string groupId, IConfiguration configuration)
        {
            _topicName = topicName;
            _configuration = configuration;

            var config = new ConsumerConfig
            {
                BootstrapServers = bootstrapServers,
                GroupId = groupId,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = true
            };

            _consumer = new ConsumerBuilder<string, string>(config).Build();
        }

        public Task StartConsumingAsync(CancellationToken cancellationToken)
        {
            _consumer.Subscribe(_topicName);

            // Marcamos a Action como async para podermos usar o "await" lá embaixo no EmailService
            return Task.Run(async () =>
            {
                try
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        try
                        {
                            var consumeResult = _consumer.Consume(cancellationToken);

                            if (consumeResult != null)
                            {
                                var options = new JsonSerializerOptions
                                {
                                    PropertyNameCaseInsensitive = true
                                };

                                var orderEvent = JsonSerializer.Deserialize<OrderPlacedEventCreated>(consumeResult.Message.Value, options);

                                if (orderEvent != null && orderEvent.Order != null)
                                {
                                    int orderId = orderEvent.Order.Id;
                                    string nome = orderEvent.Order.Usuario;
                                    string emailUsuario = orderEvent.Order.EmailUser;

                                    Console.WriteLine($"[Kafka] Novo pedido recebido! ID: {orderId} | Nome: {nome} | E-mail: {emailUsuario}");

                                    orderEvent.Order.Status = Enums.OrderStatus.Approved;

                                }
                            }
                        }
                        catch (ConsumeException e)
                        {
                            Console.WriteLine($"[Kafka Consumer] Erro ao consumir mensagem: {e.Error.Reason}");
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("[Kafka Consumer] Encerramento solicitado.");
                }
                finally
                {
                    _consumer.Close();
                }
            }, cancellationToken);
        }

        public void Dispose()
        {
            _consumer?.Dispose();
        }
    }
}