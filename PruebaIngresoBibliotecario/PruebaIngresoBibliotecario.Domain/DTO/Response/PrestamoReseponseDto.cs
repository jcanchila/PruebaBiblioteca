using System;

namespace PruebaIngresoBibliotecario.Domain.DTO.Response
{
    public class PrestamoReseponseDto
    {
        /// <summary>
        /// Identificador unico del libro
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Fecha maxima de devolucion del prestamo
        /// </summary>
        public DateTime FechaMaximaDevolucion { get; set; }
    }
}
