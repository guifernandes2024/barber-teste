using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BarberShopApp.Core.Models;

namespace BarberShopApp.Data.Configurations
{
    public class ProfissionalConfiguration : IEntityTypeConfiguration<Profissional>
    {
        public void Configure(EntityTypeBuilder<Profissional> builder)
        {
            builder.ToTable("Profissionais");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Nome)
                .IsRequired();

            builder.Property(p => p.Email)
                .IsRequired()
                .HasMaxLength(255); // A typical email max length

            builder.HasIndex(p => p.Email)
                .IsUnique(); // Ensure emails are unique

            builder.Property(p => p.Telefone)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(p => p.TipoDocumento)
                .IsRequired()
                .HasConversion<string>(); // Store enum as string in DB

            builder.Property(p => p.Documento)
                .IsRequired()
                .HasMaxLength(50); // Adjust as needed for different document types

            builder.Property(p => p.DataNacimento)
                .IsRequired();

            builder.Property(p => p.Fumante)
                .IsRequired();

            builder.Property(p => p.ImgUrl)
                .IsRequired()
                .HasMaxLength(500); // A reasonable max length for URLs

            builder.Property(p => p.PercentualDeComissao); // Nullable int doesn't need .IsRequired()

            // Configure Many-to-Many relationship with Servico (Especialidades)
            builder.HasMany(p => p.Especialidades)
                .WithMany(s => s.Profissionals)
                .UsingEntity(j => j.ToTable("ProfissionalEspecialidades")); // Optional: specify join table name

            // Configure One-to-Many relationship with Agendamento
            builder.HasMany(p => p.Agendamentos)
                .WithOne(a => a.Profissional)
                .HasForeignKey(a => a.ProfissionalId)
                .OnDelete(DeleteBehavior.Restrict); // Example: prevent deletion of professional if they have appointments

            // Ignore calculated property
            builder.Ignore(p => p.DataAniversario);
        }
    }
}
