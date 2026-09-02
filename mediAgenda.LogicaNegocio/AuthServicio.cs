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
    private readonly IRepositorio<Paciente> _pacienteRepositorio;
    private readonly IRepositorio<Medico> _medicoRepositorio;
    private readonly IConfiguration _configuration;

    public AuthServicio(
        IRepositorio<Usuario> usuarioRepositorio,
        IRepositorio<Paciente> pacienteRepositorio,
        IRepositorio<Medico> medicoRepositorio,
        IConfiguration configuration)
    {
        _usuarioRepositorio = usuarioRepositorio;
        _pacienteRepositorio = pacienteRepositorio;
        _medicoRepositorio = medicoRepositorio;
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

    public async Task<Usuario> RegistrarPacienteAsync(string email, string password, string cedula, string nombre, string apellido, string telefono, DateTime fechaNacimiento)
    {
        var medicos = await _medicoRepositorio.ObtenerTodosAsync();
        if (medicos.Any(m => m.Cedula == cedula))
            throw new InvalidOperationException("Esta cédula pertenece a un médico registrado. Contactá al administrador.");

        var pacientes = await _pacienteRepositorio.ObtenerTodosAsync();
        if (pacientes.Any(p => p.Cedula == cedula))
            throw new InvalidOperationException("Ya existe un paciente registrado con esta cédula.");
        if (pacientes.Any(p => p.Email == email))
            throw new InvalidOperationException("Ya existe un paciente registrado con este email.");

        var usuarios = await _usuarioRepositorio.ObtenerTodosAsync();
        if (usuarios.Any(u => u.Email == email))
            throw new InvalidOperationException("Ya existe una cuenta registrada con este email.");

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
        var paciente = new Paciente
        {
            Nombre = nombre,
            Apellido = apellido,
            Email = email,
            Password = passwordHash,
            Cedula = cedula,
            Telefono = telefono,
            FechaNacimiento = DateTime.SpecifyKind(fechaNacimiento, DateTimeKind.Utc)
        };
        await _pacienteRepositorio.AgregarAsync(paciente);

        try
        {
            var usuario = new Usuario
            {
                Email = email,
                Password = passwordHash,
                Rol = RolUsuario.Paciente,
                PacienteId = paciente.Id
            };
            await _usuarioRepositorio.AgregarAsync(usuario);
            return usuario;
        }
        catch
        {
            await _pacienteRepositorio.EliminarAsync(paciente.Id);
            throw;
        }
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
            new Claim("pacienteId", usuario.PacienteId?.ToString() ?? "")
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