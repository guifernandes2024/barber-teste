using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BarberShopApp.Core.Models;

namespace BarberShopApp.Data.Configurations
{
    public class ServicoConfiguration : IEntityTypeConfiguration<Servico>
    {
        public void Configure(EntityTypeBuilder<Servico> builder)
        {
            builder.ToTable("Servicos");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.Nome)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(s => s.Descricao)
                .HasMaxLength(500);

            builder.Property(s => s.Preco)
                .HasColumnType("decimal(18, 2)") // Specify precision and scale for decimal
                .IsRequired();

            builder.Property(s => s.ImgUrl)
                .IsRequired()
                .HasMaxLength(500); // A reasonable max length for URLs

            builder.Property(s => s.DuracaoEmMinutos)
                .IsRequired();

            // Configure Many-to-Many relationship with Agendamento
            builder.HasMany(s => s.Agendamentos)
                .WithMany(a => a.Servicos)
                .UsingEntity(j => j.ToTable("AgendamentoServicos")); // Should match the name used in AgendamentoConfiguration

            // Configure Many-to-Many relationship with Profissional (Especialidades)
            builder.HasMany(s => s.Profissionals)
                .WithMany(p => p.Especialidades)
                .UsingEntity(j => j.ToTable("ProfissionalEspecialidades")); // Should match the name used in ProfissionalConfiguration
        }
    }
}
