using Microsoft.AspNetCore.Mvc;
using PruebaIngresoBibliotecario.Domain.DTO.Request;
using PruebaIngresoBibliotecario.Domain.DTO.Response;
using PruebaIngresoBibliotecario.Domain.Entities;
using PruebaIngresoBibliotecario.Domain.Enum;
using PruebaIngresoBibliotecario.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PruebaIngresoBibliotecario.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrestamoController : ControllerBase
    {
        private readonly IPrestamoService _prestamoService;
        public PrestamoController(IPrestamoService prestamoService)
        {
            _prestamoService = prestamoService ?? throw new ArgumentNullException(nameof(prestamoService));
        }        

        [HttpGet("{idPrestamo}")]
        [ProducesResponseType(200, Type = typeof(PrestamoEntity))]
        public async Task<IActionResult> GetAsync(Guid idPrestamo)
        {
            var listaPrestamo = await _prestamoService.ConsultarPrestamo(idPrestamo).ConfigureAwait(false);
            if(listaPrestamo == null || !listaPrestamo.Any())
            {
                var errorMessage = new
                {
                    Mensaje = $"El prestamo con id {idPrestamo} no existe"
                };
                return NotFound(errorMessage);
            }
            return Ok(listaPrestamo.FirstOrDefault());
        }

        [HttpPost]
        [ProducesResponseType(200, Type = typeof(PrestamoResponseDto))]
        public async Task<IActionResult> PostAsync([FromBody] PrestamoRequestDto prestamo)
        {
            if(!Enum.IsDefined(typeof(TipoUsuarioEnum), prestamo.TipoUsuario))
            {
                return BadRequest("Tipo de usuario no valido en la operacion");
            }
            var esValidoPrestamo = await _prestamoService.EsValidoPrestamoAsync(prestamo);
            if (!esValidoPrestamo)
            {
                var errorMessage = new
                {
                    Mensaje = $"El usuario con identificacion {prestamo.IdentificacionUsuario} ya tiene un libro prestado por lo cual no se le puede realizar otro prestamo"
                };
                return BadRequest(errorMessage);
            }

            var prestamoRealizado = await _prestamoService.RealizarPrestamoAsync(prestamo);
            return Ok(prestamoRealizado);
        }        

    }
}
