using mediAgenda.DataAccess;
using mediAgenda.IDataAccess;
using mediAgenda.ILogicaNegocio;
using mediAgenda.LogicaNegocio;
using mediAgenda.Dominio;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

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



builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.SetIsOriginAllowed(origin =>
                origin == "http://localhost:5173" ||
                (Uri.TryCreate(origin, UriKind.Absolute, out var uri)
                    && uri.Scheme == "https"
                    && uri.Host.EndsWith(".vercel.app")
                    && uri.Host.StartsWith("mediagenda-")))
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
    });

var app = builder.Build();

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
}

app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();