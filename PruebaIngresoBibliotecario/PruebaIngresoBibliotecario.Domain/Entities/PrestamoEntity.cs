using PruebaIngresoBibliotecario.Domain.Enum;
using System;

namespace PruebaIngresoBibliotecario.Domain.Entities
{
    public sealed class PrestamoEntity
    {
        /// <summary>
        /// Id del prestamo
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Identificador unico del libro
        /// </summary>
        public Guid Isbn { get; set; }

        /// <summary>
        /// Identificacion del usuario
        /// </summary>        
        public string IdentificacionUsuario { get; set; }

        /// <summary>
        /// Tipo de usuario: 
        /// 1 = usuario afiliado,
        /// 2 = usuario empleado de la biblioteca,
        /// 3 = usuario invitado
        /// </summary>
        public TipoUsuarioEnum TipoUsuario { get; set; }

        /// <summary>
        /// Fecha de devolucion del libro
        /// </summary>
        public DateTime FechaMaximaDevolucion { get; set; }
    }
}
