using mediAgenda.Dominio;
using mediAgenda.ILogicaNegocio;
using mediAgenda.WebAPI.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace mediAgenda.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class SedeController : ControllerBase
{
    private readonly ISedeServicio _servicio;

    public SedeController(ISedeServicio servicio)
    {
        _servicio = servicio;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> ObtenerTodos()
    {
        var sedes = await _servicio.ObtenerTodosAsync();
        var dto = sedes.Select(s => new SedeDTO
        {
            Id = s.Id,
            Nombre = s.Nombre,
            Direccion = s.Direccion,
            Telefono = s.Telefono
        });
        return Ok(dto);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var sede = await _servicio.ObtenerPorIdAsync(id);
        if (sede == null) return NotFound();
        return Ok(new SedeDTO { Id = sede.Id, Nombre = sede.Nombre, Direccion = sede.Direccion, Telefono = sede.Telefono });
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearSedeDTO dto)
    {
        var sede = new Sede { Nombre = dto.Nombre, Direccion = dto.Direccion, Telefono = dto.Telefono };
        var creada = await _servicio.CrearAsync(sede);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = creada.Id },
            new SedeDTO { Id = creada.Id, Nombre = creada.Nombre, Direccion = creada.Direccion, Telefono = creada.Telefono });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        await _servicio.EliminarAsync(id);
        return NoContent();
    }
}