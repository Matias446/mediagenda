using mediAgenda.Dominio;
using mediAgenda.IDataAccess;
using mediAgenda.ILogicaNegocio;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace mediAgenda.LogicaNegocio;

public class AuthServicio : IAuthServicio
{
    private readonly IRepositorio<Usuario> _usuarioRepositorio;
    private readonly IRepositorio<Paciente> _pacienteRepositorio;
    private readonly IRepositorio<Medico> _medicoRepositorio;
    private readonly IConfiguration _configuration;
    private readonly IEmailSender _emailSender;

    public AuthServicio(
        IRepositorio<Usuario> usuarioRepositorio,
        IRepositorio<Paciente> pacienteRepositorio,
        IRepositorio<Medico> medicoRepositorio,
        IConfiguration configuration,
        IEmailSender emailSender)
    {
        _usuarioRepositorio = usuarioRepositorio;
        _pacienteRepositorio = pacienteRepositorio;
        _medicoRepositorio = medicoRepositorio;
        _configuration = configuration;
        _emailSender = emailSender;
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
        catch (Exception ex)
        {
            await _pacienteRepositorio.EliminarAsync(paciente.Id);
            if (ex is InvalidOperationException)
                throw new InvalidOperationException("Ya existe una cuenta con ese email");
            throw;
        }
    }

    public async Task<Usuario> RegistrarAdminAsync(string email, string password, RolUsuario rol)
    {
        if (rol != RolUsuario.Admin && rol != RolUsuario.Administrativo)
            throw new InvalidOperationException("El rol debe ser Admin o Administrativo.");

        var usuarios = await _usuarioRepositorio.ObtenerTodosAsync();
        if (usuarios.Any(u => u.Email == email))
            throw new InvalidOperationException("Ya existe una cuenta registrada con este email.");

        var pacientes = await _pacienteRepositorio.ObtenerTodosAsync();
        if (pacientes.Any(p => p.Email == email))
            throw new InvalidOperationException("Ya existe una cuenta registrada con este email.");

        var usuario = new Usuario
        {
            Email = email,
            Password = BCrypt.Net.BCrypt.HashPassword(password),
            Rol = rol,
            PacienteId = null
        };
        await _usuarioRepositorio.AgregarAsync(usuario);
        return usuario;
    }

    public async Task CambiarPasswordAsync(int usuarioId, string passwordActual, string passwordNueva)
    {
        var usuario = await _usuarioRepositorio.ObtenerPorIdAsync(usuarioId);
        if (usuario == null) throw new KeyNotFoundException("Usuario no encontrado");
        if (!BCrypt.Net.BCrypt.Verify(passwordActual, usuario.Password))
            throw new InvalidOperationException("La contraseña actual es incorrecta");

        usuario.Password = BCrypt.Net.BCrypt.HashPassword(passwordNueva);
        usuario.TokensValidosDesde = DateTime.UtcNow;
        await _usuarioRepositorio.ActualizarAsync(usuario);
    }

    public async Task OlvidePasswordAsync(string email)
    {
        var usuarios = await _usuarioRepositorio.ObtenerTodosAsync();
        var usuario = usuarios.FirstOrDefault(u => u.Email == email);
        if (usuario == null) return; // no revelar si el email existe o no

        var passwordTemporal = GenerarPasswordTemporal();
        usuario.Password = BCrypt.Net.BCrypt.HashPassword(passwordTemporal);
        usuario.TokensValidosDesde = DateTime.UtcNow;
        await _usuarioRepositorio.ActualizarAsync(usuario);

        await _emailSender.EnviarAsync(
            email,
            "Tu nueva contraseña temporal - mediAgenda",
            $"Generamos una contraseña temporal para tu cuenta: {passwordTemporal}\n\n" +
            "Iniciá sesión con ella y cambiala desde tu perfil apenas puedas.");
    }

    private static string GenerarPasswordTemporal()
        => Convert.ToBase64String(RandomNumberGenerator.GetBytes(9))
            .Replace("+", "").Replace("/", "").Replace("=", "");

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
            new Claim(JwtRegisteredClaimNames.Iat,
                DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64)
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