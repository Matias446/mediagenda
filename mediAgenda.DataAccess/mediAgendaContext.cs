using mediAgenda.Dominio;
using Microsoft.EntityFrameworkCore;

namespace mediAgenda.DataAccess;

public class MediAgendaContext : DbContext
{
    public MediAgendaContext(DbContextOptions<MediAgendaContext> options) : base(options) { }

    public DbSet<Paciente> Pacientes { get; set; }
    public DbSet<Medico> Medicos { get; set; }
    public DbSet<Especialidad> Especialidades { get; set; }
    public DbSet<Sede> Sedes { get; set; }
    public DbSet<Turno> Turnos { get; set; }
    public DbSet<HorarioDisponible> HorariosDisponibles { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Paciente>()
            .HasIndex(p => p.Email)
            .IsUnique();

        modelBuilder.Entity<Paciente>()
            .HasIndex(p => p.Cedula)
            .IsUnique();

        modelBuilder.Entity<Medico>()
            .HasOne(m => m.Especialidad)
            .WithMany(e => e.Medicos)
            .HasForeignKey(m => m.EspecialidadId);

        modelBuilder.Entity<Medico>()
            .HasOne(m => m.Sede)
            .WithMany(s => s.Medicos)
            .HasForeignKey(m => m.SedeId);

        modelBuilder.Entity<Turno>()
            .HasOne(t => t.Paciente)
            .WithMany(p => p.Turnos)
            .HasForeignKey(t => t.PacienteId);

        modelBuilder.Entity<Turno>()
            .HasOne(t => t.Medico)
            .WithMany(m => m.Turnos)
            .HasForeignKey(t => t.MedicoId);

        modelBuilder.Entity<HorarioDisponible>()
            .HasOne(h => h.Medico)
            .WithMany(m => m.Horarios)
            .HasForeignKey(h => h.MedicoId);
    }
}