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

builder.Services.AddDbContext<MediAgendaContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")),
    ServiceLifetime.Scoped);

builder.Services.AddScoped<IRepositorio<Paciente>, Repositorio<Paciente>>();
builder.Services.AddScoped<IRepositorio<Medico>, Repositorio<Medico>>();
builder.Services.AddScoped<IRepositorio<Especialidad>, Repositorio<Especialidad>>();
builder.Services.AddScoped<IRepositorio<Sede>, Repositorio<Sede>>();
builder.Services.AddScoped<IRepositorio<Turno>, Repositorio<Turno>>();

builder.Services.AddScoped<IPacienteServicio, PacienteServicio>();
builder.Services.AddScoped<IMedicoServicio, MedicoServicio>();
builder.Services.AddScoped<IEspecialidadServicio, EspecialidadServicio>();
builder.Services.AddScoped<ISedeServicio, SedeServicio>();
builder.Services.AddScoped<ITurnoServicio, TurnoServicio>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();