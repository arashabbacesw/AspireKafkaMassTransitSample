namespace Demo.Api;

public class UserLoggedInMessageModel
{
    public Guid UserId { get; set; }
    public DateTime LoggedInAt { get; set; }
}