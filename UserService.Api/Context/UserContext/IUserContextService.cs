namespace UserService.Api.Context.UserContext;

public interface IUserContextService
{
    Guid GetCurrentUserId();
}