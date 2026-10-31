using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiJwt.Controllers;

[ApiController]
[Route("api/usuario")]
[Authorize]
public class UsuarioController : ControllerBase
{
    [HttpGet("perfil")]
    public IActionResult ObterPerfil()
    {
        var nome = User.Identity?.Name;

        return Ok(new { mensagem = $"Olá, {nome}! Você está autenticado." });
    }
}