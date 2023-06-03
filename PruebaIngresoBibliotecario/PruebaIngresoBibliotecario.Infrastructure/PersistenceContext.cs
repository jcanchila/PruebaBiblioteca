using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PruebaIngresoBibliotecario.Domain.Entities;
using System.Threading.Tasks;

namespace PruebaIngresoBibliotecario.Infrastructure
{
    public class PersistenceContext : DbContext
    {
        public DbSet<PrestamoEntity> PrestamoEntity { get; set; }
        private readonly IConfiguration Config;

        public PersistenceContext(DbContextOptions<PersistenceContext> options, IConfiguration config) : base(options)
        {
            Config = config;
        }

        public async Task CommitAsync()
        {
            await SaveChangesAsync().ConfigureAwait(false);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(Config.GetValue<string>("SchemaName"));

            modelBuilder.Entity<PrestamoEntity>(model =>
            {
                model.HasKey(key => key.Id);

                model.Property(prop => prop.Id)
                     .ValueGeneratedOnAdd();

                model.Property(prop => prop.Isbn)
                     .ValueGeneratedOnAdd();

                model.Property(prop => prop.IdentificacionUsuario)
                     .HasMaxLength(10)
                     .IsRequired();
            });

            base.OnModelCreating(modelBuilder);
        }      
    }
}
