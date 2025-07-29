using Microsoft.EntityFrameworkCore;
using teste.Data.Configurations;

namespace teste.Data
{
    public static class EntityConfigurations
    {
        public static void ApplyConfigurations(this ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ServicoConfiguration());
            modelBuilder.ApplyConfiguration(new AgendamentoConfiguration());
        }
    }
} 