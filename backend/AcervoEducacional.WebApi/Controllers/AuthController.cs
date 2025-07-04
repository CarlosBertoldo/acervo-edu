using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace AcervoEducacional.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] object request)
    {
        await Task.Delay(1); // Simular async
        
        return Ok(new { 
            success = true,
            token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.test-payload.test-signature",
            refreshToken = "test-refresh-token",
            user = new { 
                Id = 1, 
                Nome = "Admin", 
                Email = "admin@acervo.com",
                Tipo = "Admin" 
            }
        });
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser()
    {
        await Task.Delay(1); // Simular async
        
        return Ok(new { 
            Id = 1, 
            Nome = "Admin", 
            Email = "admin@acervo.com",
            Tipo = "Admin" 
        });
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await Task.Delay(1); // Simular async
        return Ok(new { success = true, message = "Logout realizado com sucesso" });
    }
}