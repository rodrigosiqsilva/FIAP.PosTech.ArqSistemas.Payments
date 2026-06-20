using FIAP.PosTech.ArqSistemas.PaymentsWS.Services;
using FIAP.PosTech.ArqSistemas.PaymentsWS.Workers;

var builder = Host.CreateApplicationBuilder(args);

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

builder.Services.AddHttpClient<IOrderGameService, OrderGameService>();
builder.Services.AddHostedService<KafkaConsumerWorker>();
builder.Services.AddScoped<IPaymentProcessedNotificationService, PaymentProcessedNotificationService>();

var host = builder.Build();
host.Run();