using Confluent.Kafka;
using Demo.Api;
using Demo.Api.Consumers;
using Demo.ServiceDefaults;
using MassTransit;
using MassTransit.KafkaIntegration.Serializers;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddMassTransit(x =>
{
    x.UsingInMemory();
    x.AddRider(rider =>
    {
        rider.AddConsumer<UserLoggedInConsumer>();
        rider.UsingKafka((context, configurator) =>
        {
            configurator.Host(builder.Configuration.GetConnectionString("kafka-service"));
            configurator.TopicEndpoint<UserLoggedInMessageModel>("user-logged-in-model", "user-logged-in-group-id",
                endpointConfigurator =>
                {
                    endpointConfigurator.AutoOffsetReset = AutoOffsetReset.Earliest;
                    endpointConfigurator.AutoStart = true;
                    endpointConfigurator.CreateIfMissing();
                    endpointConfigurator.SetValueDeserializer(new MassTransitJsonDeserializer<UserLoggedInMessageModel>());
                    endpointConfigurator.ConfigureConsumer<UserLoggedInConsumer>(context);
                } );
        });
        rider.AddProducer<UserLoggedInMessageModel>("user-logged-in-model", (_, producerConfigurator) =>
        {
            producerConfigurator.SetValueSerializer(new MassTransitJsonSerializer<UserLoggedInMessageModel>());
        });
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
    app.MapSwagger();
    app.UseSwaggerUI();
}

app.MapPost("/UserLoggedIn", async (ITopicProducer<UserLoggedInMessageModel> producer) =>
{
    var model = new UserLoggedInMessageModel
    {
        UserId = Guid.NewGuid(),
        LoggedInAt = DateTime.Now,
    };

    await producer.Produce(model);
    return TypedResults.Ok(model);
});

app.UseHttpsRedirection();

app.Run();