using mediAgenda.IDataAccess;
using Microsoft.EntityFrameworkCore;

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
        await _context.SaveChangesAsync();
    }

    public async Task ActualizarAsync(T entidad)
    {
        _dbSet.Update(entidad);
        await _context.SaveChangesAsync();
    }

    public async Task EliminarAsync(int id)
    {
        var entidad = await ObtenerPorIdAsync(id);
        if (entidad != null)
        {
            _dbSet.Remove(entidad);
            await _context.SaveChangesAsync();
        }
    }
}