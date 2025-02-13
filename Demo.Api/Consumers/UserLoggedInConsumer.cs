using MassTransit;

namespace Demo.Api.Consumers;

public class UserLoggedInConsumer(ILogger<UserLoggedInConsumer> logger) : IConsumer<UserLoggedInMessageModel>
{
    public async Task Consume(ConsumeContext<UserLoggedInMessageModel> context)
    {
        logger.LogInformation($"User with Id {context.Message.UserId} logged in at {context.Message.LoggedInAt}");
        await Task.Delay(1_000);
    }
}