using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using PruebaIngresoBibliotecario.Domain.DTO.Request;
using PruebaIngresoBibliotecario.Domain.Entities;
using PruebaIngresoBibliotecario.Domain.Enum;
using PruebaIngresoBibliotecario.Domain.Interfaces;
using PruebaIngresoBibliotecario.Domain.Services;
using PruebaIngresoBibliotecario.Infrastructure;
using PruebaIngresoBibliotecario.Infrastructure.SeedDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace PruebaIngresoBibliotecario.Domain.Tests.Domain
{
    public sealed class PrestamoServiceTest
    {
        private readonly PrestamoService _mockPrestamoService;
        private readonly Mock<IGenericRepository<PrestamoEntity>> _mockRepository;        
        private readonly IConfiguration _mockConfiguration;

        PersistenceContext _mockContext;

        DbContextOptions<PersistenceContext> dbOptions = new DbContextOptionsBuilder<PersistenceContext>()
                             .UseInMemoryDatabase(databaseName: "PruebaIngreso")
                             .Options;
        
        public PrestamoServiceTest()
        {
            var myConfiguration = new Dictionary<string, string>
            {
                { "SchemaName", "mySchema"}
            };

            _mockConfiguration  = new ConfigurationBuilder()
            .AddInMemoryCollection(myConfiguration)
            .Build();

            _mockContext = new PersistenceContext(dbOptions, _mockConfiguration);
            _mockContext.Database.EnsureCreated();
            SeedDatabase();

            _mockRepository = new Mock<IGenericRepository<PrestamoEntity>>();
            _mockPrestamoService = new PrestamoService(_mockRepository.Object);
        }

        [Fact]
        public async Task Debe_Mostrar_Prestamo_Si_Existe_EnBasedeDatos()
        {
            //Arrange
            var firstElement = _mockContext.PrestamoEntity.FirstOrDefault();
            Guid id = firstElement.Id;
            _mockRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<PrestamoEntity, bool>>>()))
                .ReturnsAsync((Expression<Func<PrestamoEntity, bool>> arg) => _mockContext.PrestamoEntity.Where(x => x.Id == firstElement.Id));
            //Act
            var sut = await _mockPrestamoService.ConsultarPrestamo(id).ConfigureAwait(false);
            //Assert
            Assert.True(sut.Any());
        }

        [Fact]
        public async Task Debe_Retornar_Vacio_Si_Id_NoExiste_EnBasedeDatos()
        {
            //Arrange            
            Guid id = Guid.NewGuid();
            _mockRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<PrestamoEntity, bool>>>()))
                .ReturnsAsync((Expression<Func<PrestamoEntity, bool>> arg) => new List<PrestamoEntity>());
            //Act
            var sut = await _mockPrestamoService.ConsultarPrestamo(id).ConfigureAwait(false);
            //Assert
            Assert.False(sut.Any());
        }

        [Fact]
        public async Task Debe_Retornar_Falso_Si_Usuario_Invitado_Tiene_Prestamo_Existente()
        {
            //Arrange            
            var invitado = _mockContext.PrestamoEntity.Where(x => x.TipoUsuario == PruebaIngresoBibliotecario.Domain.Enum.TipoUsuarioEnum.INVITADO);
            _mockRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<PrestamoEntity, bool>>>()))
                .ReturnsAsync((Expression<Func<PrestamoEntity, bool>> arg) => invitado);

            PrestamoRequestDto mockRequest = new PrestamoRequestDto
            {
                TipoUsuario = invitado.FirstOrDefault().TipoUsuario,
                IdentificacionUsuario = invitado.FirstOrDefault().IdentificacionUsuario,
                Isbn = invitado.FirstOrDefault().Isbn
            };
            //Act
            var sut = await _mockPrestamoService.EsValidoPrestamoAsync(mockRequest).ConfigureAwait(false);
            //Assert
            Assert.False(sut);
        }

        [Fact]
        public async Task Debe_RetornarTrue_Si_Usuario_No_Es_Invitado_Y_Tiene_Prestamo_Existente()
        {
            //Arrange            
            var invitado = _mockContext.PrestamoEntity.Where(x => x.TipoUsuario == PruebaIngresoBibliotecario.Domain.Enum.TipoUsuarioEnum.EMPLEADO);
            _mockRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<PrestamoEntity, bool>>>()))
                .ReturnsAsync((Expression<Func<PrestamoEntity, bool>> arg) => invitado);

            PrestamoRequestDto mockRequest = new PrestamoRequestDto
            {
                TipoUsuario = invitado.FirstOrDefault().TipoUsuario,
                IdentificacionUsuario = invitado.FirstOrDefault().IdentificacionUsuario,
                Isbn = invitado.FirstOrDefault().Isbn
            };
            //Act
            var sut = await _mockPrestamoService.EsValidoPrestamoAsync(mockRequest).ConfigureAwait(false);
            //Assert
            Assert.True(sut);
        }

        [Fact]
        public async Task Debe_RetornarTrue_Si_Usuario_No_Tiene_Prestamo_Existente()
        {
            //Arrange                        
            _mockRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<PrestamoEntity, bool>>>()))
                .ReturnsAsync((Expression<Func<PrestamoEntity, bool>> arg) => null);

            PrestamoRequestDto mockRequest = new PrestamoRequestDto
            {
                TipoUsuario = PruebaIngresoBibliotecario.Domain.Enum.TipoUsuarioEnum.AFILIADO,
                IdentificacionUsuario = "1234567890",
                Isbn = Guid.NewGuid()
            };
            //Act
            var sut = await _mockPrestamoService.EsValidoPrestamoAsync(mockRequest).ConfigureAwait(false);
            //Assert
            Assert.True(sut);
        }

        [Fact]
        public async Task Debe_Registrar_Prestamo_En_BasedeDatos()
        {
            //Arrange            
            PrestamoRequestDto mockRequest = new PrestamoRequestDto
            {
                TipoUsuario = TipoUsuarioEnum.AFILIADO,
                IdentificacionUsuario = "1234567890",
                Isbn = Guid.NewGuid()
            };

            PrestamoEntity prestamoResponse = new PrestamoEntity
            {
                Id = Guid.NewGuid(),
                IdentificacionUsuario = mockRequest.IdentificacionUsuario,
                TipoUsuario = mockRequest.TipoUsuario,
                Isbn = mockRequest.Isbn,                
            };

            DateTime expected = CalcularFechaEntrega(mockRequest.TipoUsuario);

            _mockRepository.Setup(x => x.PostAsync(It.IsAny<PrestamoEntity> ()))
                .ReturnsAsync(prestamoResponse);
            
            //Act
            var sut = await _mockPrestamoService.RealizarPrestamoAsync(mockRequest).ConfigureAwait(false);
            //Assert
            Assert.Equal(expected.ToShortDateString(), sut.FechaMaximaDevolucion.ToShortDateString());
        }

        private void SeedDatabase()
        {
            List<PrestamoEntity> seedData = PrestamoEntitySeed.GetPrestamoEntitySeedData();
            var context = _mockContext;
            if (!context.Set<PrestamoEntity>().Any())
            {
                context.Set<PrestamoEntity>().AddRange(
                    seedData.ToArray()
                );
                context.SaveChanges();
            }
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
