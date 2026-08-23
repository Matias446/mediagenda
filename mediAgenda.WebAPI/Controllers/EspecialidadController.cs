using mediAgenda.Dominio;
using mediAgenda.ILogicaNegocio;
using mediAgenda.WebAPI.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace mediAgenda.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class EspecialidadController : ControllerBase
{
    private readonly IEspecialidadServicio _servicio;

    public EspecialidadController(IEspecialidadServicio servicio)
    {
        _servicio = servicio;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> ObtenerTodos()
    {
        var especialidades = await _servicio.ObtenerTodosAsync();
        var dto = especialidades.Select(e => new EspecialidadDTO
        {
            Id = e.Id,
            Nombre = e.Nombre
        });
        return Ok(dto);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var especialidad = await _servicio.ObtenerPorIdAsync(id);
        if (especialidad == null) return NotFound();
        return Ok(new EspecialidadDTO { Id = especialidad.Id, Nombre = especialidad.Nombre });
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearEspecialidadDTO dto)
    {
        var especialidad = new Especialidad { Nombre = dto.Nombre };
        var creada = await _servicio.CrearAsync(especialidad);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = creada.Id },
            new EspecialidadDTO { Id = creada.Id, Nombre = creada.Nombre });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        await _servicio.EliminarAsync(id);
        return NoContent();
    }
}