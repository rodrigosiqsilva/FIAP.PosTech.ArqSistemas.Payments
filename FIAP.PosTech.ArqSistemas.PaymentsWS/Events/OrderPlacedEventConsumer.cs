using Confluent.Kafka; 
using FIAP.PosTech.ArqSistemas.PaymentsWS.DTOs;
using FIAP.PosTech.ArqSistemas.PaymentsWS.Enums;
using FIAP.PosTech.ArqSistemas.PaymentsWS.Models;
using FIAP.PosTech.ArqSistemas.PaymentsWS.Services;
using System.Text.Json;

namespace FIAP.PosTech.ArqSistemas.PaymentsWS.Events
{

    public record OrderPlacedEventCreated(OrderDto Order, DateTime CreatedAt, string? CorrelationId);

    public class OrderPlacedEventConsumer : IDisposable
    {
        private readonly IConsumer<Ignore, string> _consumer;
        private readonly IServiceProvider _serviceProvider;
        private readonly PaymentProcessedEventPublisher _eventPublisher;    

        public OrderPlacedEventConsumer(
            string bootstrapServers,
            string topic,
            string groupId,
            IConfiguration configuration,
            IServiceProvider serviceProvider,
            PaymentProcessedEventPublisher eventPublisher)
        {
            _serviceProvider = serviceProvider;
            _eventPublisher = eventPublisher;

            var config = new ConsumerConfig
            {
                BootstrapServers = bootstrapServers,
                GroupId = groupId,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            _consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            _consumer.Subscribe(topic);
        }

        public async Task StartConsumingAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = _consumer.Consume(stoppingToken);
                    if (consumeResult != null)
                    {
                        using var scope = _serviceProvider.CreateScope();

                        var orderGameService = scope.ServiceProvider.GetRequiredService<IOrderGameService>();

                        await ProcessarMensagemAsync(consumeResult.Message.Value, orderGameService, _eventPublisher);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao consumir mensagem: {ex.Message}");
                }
            }
        }

        private async Task ProcessarMensagemAsync(string messageJson, IOrderGameService orderGameService,
            PaymentProcessedEventPublisher eventPublisher)
        {
            Console.WriteLine($"Processando mensagem novo pedido criado: {messageJson}");

            try
            {
                var orderDto = JsonSerializer.Deserialize<OrderPlacedEventCreated>(messageJson);

                if (orderDto != null)
                {
                    ApiResponse<OrderDto> retorno = await orderGameService.AlterarStatusAsync(orderDto.Order, OrderStatus.Approved);

                    if (retorno != null)
                    {
                        Console.WriteLine($"Status do pedido {orderDto.Order.Id} alterado para Aprovado com sucesso.");

                        if (retorno.Dados == null)
                        {
                            Console.WriteLine($"Dados do pedido {orderDto.Order.Id} não encontrados após a alteração de status.");
                            return;
                        }
                        var paymentEvent = new PaymentProcessedCreatedEvent(orderDto.Order, DateTime.UtcNow);

                        await eventPublisher.PublishOrderPlacedEventAsync(paymentEvent);

                        Console.WriteLine($"Evento de pagamento processado publicado para o pedido {orderDto.Order.Id}.");
                    }
                    else
                    {
                        Console.WriteLine($"Falha ao alterar o status do pedido {orderDto.Order.Id} no banco de dados. Mensagem não publicada.");
                    }
                }
                else
                {
                    Console.WriteLine($"Dados inválidos do pedido. Não foi possível desserializar: {messageJson}");
                }
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Erro ao desserializar a mensagem JSON: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao processar a mensagem: {ex.Message}");
                throw;
            }
        }

        public void Dispose()
        {
            _consumer?.Close();
            _consumer?.Dispose();
        }
    }
}