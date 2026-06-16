using FIAP.PosTech.ArqSistemas.PaymentsWS;
using FIAP.PosTech.ArqSistemas.PaymentsWS.Services;
using FIAP.PosTech.ArqSistemas.PaymentsWS.Workers;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<KafkaConsumerWorker>();
builder.Services.AddScoped<IPaymentProcessedNotificationService, PaymentProcessedNotificationService>();

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var host = builder.Build();
host.Run();
