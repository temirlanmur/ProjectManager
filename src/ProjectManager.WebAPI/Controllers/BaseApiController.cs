using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ProjectManager.WebAPI.Controllers;

[ApiController, Authorize]
[Route("api")]
[Produces("application/json")]
public class BaseApiController : ControllerBase
{
    protected Guid AuthenticatedUserId
    {
        get
        {
            Claim sub = User.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub);
            return Guid.Parse(sub.Value);
        }
    }
}
