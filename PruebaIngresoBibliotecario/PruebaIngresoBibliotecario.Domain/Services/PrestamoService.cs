using AutoMapper;
using PruebaIngresoBibliotecario.Domain.DTO.Request;
using PruebaIngresoBibliotecario.Domain.DTO.Response;
using PruebaIngresoBibliotecario.Domain.Entities;
using PruebaIngresoBibliotecario.Domain.Enum;
using PruebaIngresoBibliotecario.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PruebaIngresoBibliotecario.Domain.Services
{
    public sealed class PrestamoService : IPrestamoService
    {
        private readonly IGenericRepository<PrestamoEntity> _repository;        
        public PrestamoService(IGenericRepository<PrestamoEntity> repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<IEnumerable<PrestamoEntity>> ConsultarPrestamo(Guid? idPrestamo = null)
        {            
            return await _repository.GetAsync(x => x.Id.Equals(idPrestamo.Value)).ConfigureAwait(false);
        }

        public async Task<bool> EsValidoPrestamoAsync(PrestamoRequestDto prestamo)
        {
            var prestamoEntity = await _repository.GetAsync(x => x.IdentificacionUsuario.Equals(prestamo.IdentificacionUsuario, StringComparison.InvariantCultureIgnoreCase)).ConfigureAwait(false);
            if(prestamoEntity == null)
            {
                return true;
            }

            if(prestamoEntity.Any(x => x.TipoUsuario == TipoUsuarioEnum.INVITADO))
            {
                return false;
            }

            return true;
        }

        public async Task<PrestamoReseponseDto> RealizarPrestamoAsync(PrestamoRequestDto prestamo)
        {
            var prestamoEntity = new PrestamoEntity
            {
                IdentificacionUsuario = prestamo.IdentificacionUsuario,
                Isbn = prestamo.Isbn,
                TipoUsuario = prestamo.TipoUsuario
            };

            prestamoEntity.FechaMaximaDevolucion = CalcularFechaEntrega(prestamoEntity.TipoUsuario);
            await _repository.PostAsync(prestamoEntity);
            PrestamoReseponseDto response = new PrestamoReseponseDto
            {
                Id = prestamoEntity.Id,
                FechaMaximaDevolucion = prestamoEntity.FechaMaximaDevolucion
            };
            return response;
        }

        private DateTime CalcularFechaEntrega(TipoUsuarioEnum tipoUsuario)
        {
            var weekend = new[] { DayOfWeek.Saturday, DayOfWeek.Sunday };
            var fechaDevolucion = DateTime.Now;
            int diasPrestamo = tipoUsuario switch
            {
                TipoUsuarioEnum.AFILIADO => 10,
                TipoUsuarioEnum.EMPLEADO => 8,
                TipoUsuarioEnum.INVITADO => 7,
                _ => -1,
            };

            for (int i = 0; i < diasPrestamo;)
            {
                fechaDevolucion = fechaDevolucion.AddDays(1);
                i = (!weekend.Contains(fechaDevolucion.DayOfWeek)) ? ++i : i;
            }

            return fechaDevolucion;
        }        
    }
}
