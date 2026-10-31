using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ApiJwt.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ApiJwt.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    // Credenciais fixas, apenas para fins didáticos
    private const string UsuarioFixo = "admin";
    private const string SenhaFixa = "123456";

    private readonly IConfiguration _configuration;

    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        if (request.Usuario != UsuarioFixo || request.Senha != SenhaFixa)
        {
            return Unauthorized(new { mensagem = "Usuário ou senha inválidos." });
        }

        var token = GerarToken(request.Usuario);
        return Ok(new { token });
    }

    private string GerarToken(string usuario)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, usuario)
        };

        var chave = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var credenciais = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: credenciais);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}