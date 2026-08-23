using mediAgenda.Dominio;
using mediAgenda.IDataAccess;
using mediAgenda.ILogicaNegocio;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace mediAgenda.LogicaNegocio;

public class AuthServicio : IAuthServicio
{
    private readonly IRepositorio<Usuario> _usuarioRepositorio;
    private readonly IConfiguration _configuration;

    public AuthServicio(IRepositorio<Usuario> usuarioRepositorio, IConfiguration configuration)
    {
        _usuarioRepositorio = usuarioRepositorio;
        _configuration = configuration;
    }

    public async Task<string?> LoginAsync(string email, string password)
    {
        var usuarios = await _usuarioRepositorio.ObtenerTodosAsync();
        var usuario = usuarios.FirstOrDefault(u => u.Email == email);
        if (usuario == null) return null;
        if (!BCrypt.Net.BCrypt.Verify(password, usuario.Password)) return null;
        return GenerarToken(usuario);
    }

    public async Task<Usuario> RegistrarAsync(string email, string password, RolUsuario rol, int? pacienteId, int? medicoId)
    {
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
        var usuario = new Usuario
        {
            Email = email,
            Password = passwordHash,
            Rol = rol,
            PacienteId = pacienteId,
            MedicoId = medicoId
        };
        await _usuarioRepositorio.AgregarAsync(usuario);
        return usuario;
    }

    private string GenerarToken(Usuario usuario)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim(ClaimTypes.Role, usuario.Rol.ToString()),
            new Claim("usuarioId", usuario.Id.ToString()),
            new Claim("pacienteId", usuario.PacienteId?.ToString() ?? ""),
            new Claim("medicoId", usuario.MedicoId?.ToString() ?? "")
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(24),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}