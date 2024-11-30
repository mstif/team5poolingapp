using json.service.Consumers;
using MassTransit;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<MongoClient>(x =>
    new(builder.Configuration.GetConnectionString("MongoDBConnection")));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<DeliveryContractConsumer>();
});

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<DeliveryContractConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq://host.docker.internal:5672", c =>
        {
            c.Username("guest");
            c.Password("guest");
        });

        cfg.ReceiveEndpoint("DeliveryContractConsumerQueue", e =>
        {
            e.ConfigureConsumer<DeliveryContractConsumer>(context);
        });

        cfg.ClearSerialization();
        cfg.UseRawJsonSerializer();
        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
