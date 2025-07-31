using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BarberShopApp.Data;
using BarberShopApp.Core.Models; // Para ApplicationUser

namespace BarberShopApp.Data.Configurations
{
    public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
    {
        public void Configure(EntityTypeBuilder<Cliente> builder)
        {
            builder.ToTable("Clientes");

            // Define que o Id de Cliente � a PK e tamb�m a FK para ApplicationUser
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Nome)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.Telefone)
                .IsRequired()
                .HasMaxLength(20);

            // Configura o relacionamento um-para-um (ApplicationUser <-> Cliente)
            builder.HasOne(c => c.ApplicationUser) // Cliente tem um ApplicationUser
                   .WithOne(au => au.Cliente)      // ApplicationUser tem um Cliente
                   .HasForeignKey<Cliente>(c => c.Id) // A chave estrangeira est� em Cliente.Id e aponta para ApplicationUser.Id
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
