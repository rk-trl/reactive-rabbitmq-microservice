using RabbitMQ;
using RabbitMQ.Client;
using ReportService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddSingleton<IConnectionProvider>(new ConnectionProvider("amqp://guest:guest@localhost:5672"));
builder.Services.AddSingleton<ISubscriber>(x => new Subscriber(x.GetService<IConnectionProvider>(),
        "report_exchange",
        "report_queue",
        "report.*",
        ExchangeType.Topic));

builder.Services.AddSingleton<IMemoryReportStorage, MemoryReportStorage>();
builder.Services.AddHostedService<ReportDataCollector>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
