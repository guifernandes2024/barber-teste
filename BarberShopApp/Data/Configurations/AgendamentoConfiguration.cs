using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BarberShopApp.Core.Models;

namespace BarberShopApp.Data.Configurations
{
    public class AgendamentoConfiguration : IEntityTypeConfiguration<Agendamento>
    {
        public void Configure(EntityTypeBuilder<Agendamento> builder)
        {
            builder.ToTable("Agendamentos");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.DataHora)
                .IsRequired();

            builder.Property(a => a.NomeDoCliente)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.NumeroDoCliente)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(a => a.ProfissionalId)
                .IsRequired();

            builder.Property(a => a.Observacoes)
                .HasMaxLength(500);

            // Configure Many-to-Many relationship with Servico
            builder.HasMany(a => a.Servicos)
                .WithMany(s => s.Agendamentos)
                .UsingEntity(j => j.ToTable("AgendamentoServicos")); // Optional: specify join table name

            // Configure Many-to-One relationship with Profissional
            builder.HasOne(a => a.Profissional)
                .WithMany(p => p.Agendamentos)
                .HasForeignKey(a => a.ProfissionalId)
                .IsRequired();

            // Ignore calculated properties
            builder.Ignore(a => a.DataHoraFim);
            builder.Ignore(a => a.HoraInicioString);
            builder.Ignore(a => a.HoraFimString);
            builder.Ignore(a => a.DataString);
            builder.Ignore(a => a.DataHoraCompleta);
            builder.Ignore(a => a.DataApenas);
            builder.Ignore(a => a.HoraApenas);
        }
    }
}
