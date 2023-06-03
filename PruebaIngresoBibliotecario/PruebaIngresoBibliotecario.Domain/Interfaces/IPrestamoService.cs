using PruebaIngresoBibliotecario.Domain.DTO.Request;
using PruebaIngresoBibliotecario.Domain.DTO.Response;
using PruebaIngresoBibliotecario.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PruebaIngresoBibliotecario.Domain.Interfaces
{
    public interface IPrestamoService
    {
        Task<PrestamoReseponseDto> RealizarPrestamoAsync(PrestamoRequestDto prestamo);

        Task<bool> EsValidoPrestamoAsync(PrestamoRequestDto prestamo);

        Task<IEnumerable<PrestamoEntity>> ConsultarPrestamo(Guid? idPrestamo = null);
    }
}
