using PruebaIngresoBibliotecario.Domain.Entities;
using System;
using System.Collections.Generic;

namespace PruebaIngresoBibliotecario.Infrastructure.SeedDatabase
{
    public static class PrestamoEntitySeed
    {        
        public static List<PrestamoEntity> GetPrestamoEntitySeedData()
        {
            return new List<PrestamoEntity>
            {
                new PrestamoEntity
                {
                    Id = Guid.NewGuid(),
                    Isbn = Guid.NewGuid(),
                    IdentificacionUsuario = "123456",
                    TipoUsuario = Domain.Enum.TipoUsuarioEnum.AFILIADO,
                    FechaMaximaDevolucion =  DateTime.Now.AddDays(10),                    
                },
                new PrestamoEntity
                {
                    Id = Guid.NewGuid(),
                    Isbn = Guid.NewGuid(),
                    IdentificacionUsuario = "741852",
                    TipoUsuario = Domain.Enum.TipoUsuarioEnum.EMPLEADO,
                    FechaMaximaDevolucion =  DateTime.Now.AddDays(8)
                },
                new PrestamoEntity
                {
                    Id = Guid.NewGuid(),
                    Isbn = Guid.NewGuid(),
                    IdentificacionUsuario = "369258",
                    TipoUsuario = Domain.Enum.TipoUsuarioEnum.INVITADO,
                    FechaMaximaDevolucion =  DateTime.Now.AddDays(7)
                }
            };
        }
    }
}
