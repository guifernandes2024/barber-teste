using Microsoft.EntityFrameworkCore;
using BarberShopApp.Data.Configurations;

namespace BarberShopApp.Data
{
    public static class EntityConfigurations
    {
        public static void ApplyConfigurations(this ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new AgendamentoConfiguration());
            modelBuilder.ApplyConfiguration(new ProfissionalConfiguration());
            modelBuilder.ApplyConfiguration(new ServicoConfiguration());
            modelBuilder.ApplyConfiguration(new ClienteConfiguration());
        }
    }
} 
