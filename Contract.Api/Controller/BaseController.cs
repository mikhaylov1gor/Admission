using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace Contract.Api.Controller;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseController: ControllerBase
{
    protected Guid UserId
    {
        get
        {
            var value = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return User.Identity?.IsAuthenticated == null || value == null
                ? Guid.Empty
                : Guid.Parse(value);
        }
    }
}