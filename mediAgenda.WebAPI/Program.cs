using mediAgenda.DataAccess;
using mediAgenda.IDataAccess;
using mediAgenda.ILogicaNegocio;
using mediAgenda.LogicaNegocio;
using mediAgenda.Dominio;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.HttpOverrides;
using AspNetCoreRateLimit;
using mediAgenda.WebAPI.Filters;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<SanitizarInputFilter>();
builder.Services.AddControllers(options =>
{
    options.Filters.Add<SanitizarInputFilter>();
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// Railway (y cualquier PaaS) hace de proxy: sin esto, HttpContext ve la IP interna
// del proxy para todas las requests, no la del cliente real. Necesario para que el
// rate limiting por IP y la detección de HTTPS (X-Forwarded-Proto) funcionen bien.
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(options =>
{
    options.EnableEndpointRateLimiting = true;
    options.StackBlockedRequests = false;
    options.HttpStatusCode = 429;
    options.GeneralRules = new List<RateLimitRule>
    {
        new RateLimitRule
        {
            Endpoint = "post:/api/auth/login",
            Period = "15m",
            Limit = 5
        },
        new RateLimitRule
        {
            Endpoint = "post:/api/auth/olvide-password",
            Period = "15m",
            Limit = 5
        }
    };
});
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

builder.Services.AddDbContext<MediAgendaContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")),
    ServiceLifetime.Scoped);

builder.Services.AddScoped<IRepositorio<Paciente>, Repositorio<Paciente>>();
builder.Services.AddScoped<IRepositorio<Medico>, Repositorio<Medico>>();
builder.Services.AddScoped<IRepositorio<Especialidad>, Repositorio<Especialidad>>();
builder.Services.AddScoped<IRepositorio<Sede>, Repositorio<Sede>>();
builder.Services.AddScoped<IRepositorio<Turno>, Repositorio<Turno>>();
builder.Services.AddScoped<IRepositorio<Usuario>, Repositorio<Usuario>>();

builder.Services.AddScoped<IPacienteServicio, PacienteServicio>();
builder.Services.AddScoped<IMedicoServicio, MedicoServicio>();
builder.Services.AddScoped<IEspecialidadServicio, EspecialidadServicio>();
builder.Services.AddScoped<ISedeServicio, SedeServicio>();
builder.Services.AddScoped<ITurnoServicio, TurnoServicio>();
builder.Services.AddScoped<IAuthServicio, AuthServicio>();
builder.Services.AddSingleton<IEmailSender, LoggingEmailSender>();



var pinnedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? new[]
    {
        "http://localhost:5173",
        "https://mediagenda-sand.vercel.app"
    };

// Vercel genera una URL nueva por cada deploy/preview (hash aleatorio o nombre de
// rama), así que además de los orígenes fijos de arriba, se permite cualquier
// subdominio que termine en "-medi-agenda1.vercel.app": ese sufijo es el slug del
// equipo de Vercel del proyecto, no adivinable ni reclamable por terceros.
const string vercelTeamSuffix = "-medi-agenda1.vercel.app";

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.SetIsOriginAllowed(origin =>
                pinnedOrigins.Contains(origin) ||
                (Uri.TryCreate(origin, UriKind.Absolute, out var uri)
                    && uri.Scheme == "https"
                    && uri.Host.EndsWith(vercelTeamSuffix, StringComparison.OrdinalIgnoreCase)))
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]!))
        };

        // Revocación de tokens: si el usuario fue eliminado o cambió/reseteó su
        // password después de que se emitió este token (comparando contra el
        // claim "iat"), se rechaza aunque la firma y el vencimiento sean válidos.
        options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var usuarioIdClaim = context.Principal?.FindFirst("usuarioId")?.Value;
                if (!int.TryParse(usuarioIdClaim, out var usuarioId))
                {
                    context.Fail("Token inválido");
                    return;
                }

                var repo = context.HttpContext.RequestServices.GetRequiredService<IRepositorio<Usuario>>();
                var usuario = await repo.ObtenerPorIdAsync(usuarioId);
                if (usuario == null)
                {
                    context.Fail("La cuenta ya no existe");
                    return;
                }

                var iatClaim = context.Principal?.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Iat)?.Value;
                if (usuario.TokensValidosDesde.HasValue && long.TryParse(iatClaim, out var iatUnix))
                {
                    var emitidoEn = DateTimeOffset.FromUnixTimeSeconds(iatUnix).UtcDateTime;
                    if (emitidoEn < usuario.TokensValidosDesde.Value)
                        context.Fail("Token revocado");
                }
            }
        };
    });

var app = builder.Build();

app.UseForwardedHeaders();

app.UseMiddleware<mediAgenda.WebAPI.Middleware.ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseHttpsRedirection();
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "mediAgenda API v1");
        c.RoutePrefix = "swagger";
    });
    app.UseHttpsRedirection();
}

app.UseCors("AllowFrontend");
app.UseIpRateLimiting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();