var builder = DistributedApplication.CreateBuilder(args);

var kafkaService = builder.AddKafka("kafka-service")
    .WithDataVolume("kafka-volume")
    .WithKafkaUI();

builder.AddProject<Projects.Demo_Api>("demo-api")
    .WithReference(kafkaService)
    .WaitFor(kafkaService)
    ;
builder.Build().Run();