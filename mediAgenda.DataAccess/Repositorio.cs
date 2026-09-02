using mediAgenda.IDataAccess;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace mediAgenda.DataAccess;

public class Repositorio<T> : IRepositorio<T> where T : class
{
    private readonly MediAgendaContext _context;
    private readonly DbSet<T> _dbSet;

    public Repositorio(MediAgendaContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<IEnumerable<T>> ObtenerTodosAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<T?> ObtenerPorIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task AgregarAsync(T entidad)
    {
        await _dbSet.AddAsync(entidad);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: "23505" })
        {
            throw new InvalidOperationException("Ya existe un registro con esos datos.");
        }
    }

    public async Task ActualizarAsync(T entidad)
    {
        _dbSet.Update(entidad);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: "23505" })
        {
            throw new InvalidOperationException("Ya existe un registro con esos datos.");
        }
    }

    public async Task EliminarAsync(int id)
    {
        var entidad = await ObtenerPorIdAsync(id);
        if (entidad == null) return;

        _dbSet.Remove(entidad);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: "23503" })
        {
            throw new InvalidOperationException("No se puede eliminar: hay otros registros que dependen de este.");
        }
    }
}