using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class ProtectedController : ControllerBase
{
    // Bu eyleme yalnızca JWT token ile kimliği doğrulanmış kullanıcılar erişebilir
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

    [HttpGet]
    [Authorize]
    public IActionResult Get()
    {
        return Ok("You have accessed a protected endpoint.");
    }
}
