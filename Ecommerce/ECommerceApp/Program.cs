using Ecomm.DataAccess;
using Ecomm;
using RabbitMQ.Client;
using RabbitMQ;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
var services = builder.Services;

services.AddControllersWithViews();

var connectionString = builder.Configuration["ConnectionString"];

services.AddSingleton<IOrderDetailsProvider, OrderDetailsProvider>();
services.AddSingleton<IInventoryProvider>(new InventoryProvider(connectionString));
services.AddSingleton<IProductProvider>(new ProductProvider(connectionString));
services.AddSingleton<IInventoryUpdator>(new InventoryUpdator(connectionString));

services.AddHttpClient("order", config =>
    config.BaseAddress = new System.Uri("http://localhost:5048/"));

services.AddSingleton<IConnectionProvider>(new ConnectionProvider("amqp://guest:guest@localhost:5672"));
services.AddSingleton<IPublisher>(x => new Publisher(x.GetService<IConnectionProvider>(),
        "inventory_exchange",
        ExchangeType.Topic));
services.AddSingleton<ISubscriber>(x => new Subscriber(x.GetService<IConnectionProvider>(),
    "order_exchange",
    "order_response",
    "order.created",
    ExchangeType.Topic));

services.AddHostedService<OrderCreatedListener>();

services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Ecomm service",
        Version = "v1"
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSwagger();
app.UseSwaggerUI(c => {
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ecomm Service");
});

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.Run();
