using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BarberShopApp.Core.Models;
using BarberShopApp.Data.Configurations;

namespace BarberShopApp.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {

        public DbSet<Agendamento> Agendamento { get; set; }
        public DbSet<Profissional> Profissional { get; set; }
        public DbSet<Servico> Servico { get; set; }
        public DbSet<Cliente> Cliente { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurations();
        }
    }
}
