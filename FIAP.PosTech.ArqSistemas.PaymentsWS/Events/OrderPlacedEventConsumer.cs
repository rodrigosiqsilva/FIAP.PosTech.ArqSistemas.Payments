using Confluent.Kafka; 
using FIAP.PosTech.ArqSistemas.PaymentsWS.Services;

namespace FIAP.PosTech.ArqSistemas.PaymentsWS.Events
{
    public class OrderPlacedEventConsumer : IDisposable
    {
        private readonly IConsumer<Ignore, string> _consumer;
        private readonly IServiceProvider _serviceProvider; 

        public OrderPlacedEventConsumer(
            string bootstrapServers,
            string topic,
            string groupId,
            IConfiguration configuration,
            IServiceProvider serviceProvider) 
        {
            _serviceProvider = serviceProvider;

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

                        await ProcessarMensagemAsync(consumeResult.Message.Value, orderGameService);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao consumir mensagem: {ex.Message}");
                }
            }
        }

        private async Task ProcessarMensagemAsync(string messageJson, IOrderGameService orderGameService)
        {
            Console.WriteLine($"Processando mensagem com um escopo exclusivo!");
            await Task.CompletedTask;
        }

        public void Dispose()
        {
            _consumer?.Close();
            _consumer?.Dispose();
        }
    }
}