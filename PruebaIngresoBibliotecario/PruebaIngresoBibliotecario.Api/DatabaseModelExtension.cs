using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PruebaIngresoBibliotecario.Domain.Entities;
using PruebaIngresoBibliotecario.Infrastructure;
using PruebaIngresoBibliotecario.Infrastructure.SeedDatabase;
using System.Collections.Generic;
using System.Linq;

namespace PruebaIngresoBibliotecario.Api
{
    public static class DatabaseModelExtension
    {
        public static void SeedDataBase(this IApplicationBuilder applicationBuilder)
        {
            List<PrestamoEntity> seedData = PrestamoEntitySeed.GetPrestamoEntitySeedData();
            using var serviceScope = applicationBuilder.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var context = serviceScope.ServiceProvider.GetService<PersistenceContext>();
            if (!context.Set<PrestamoEntity>().Any())
            {
                context.Set<PrestamoEntity>().AddRange(
                    seedData.ToArray()
                );
                context.SaveChanges();
            }
        }
    }
}
