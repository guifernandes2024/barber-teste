using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using teste.Models;

namespace teste.Data.Configurations
{
    public class ServicoConfiguration : IEntityTypeConfiguration<Servico>
    {
        public void Configure(EntityTypeBuilder<Servico> builder)
        {
            // Configuração da tabela
            builder.ToTable("Servicos");

            // Configuração da chave primária
            builder.HasKey(s => s.Id);

            // Configuração das propriedades
            builder.Property(s => s.Id)
                .HasColumnName("Id")
                .ValueGeneratedOnAdd()
                .IsRequired();

            builder.Property(s => s.Nome)
                .HasColumnName("Nome")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(s => s.Descricao)
                .HasColumnName("Descricao")
                .HasMaxLength(500)
                .IsRequired(false);

            builder.Property(s => s.Preco)
                .HasColumnName("Preco")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(s => s.DuracaoEmMinutos)
                .HasColumnName("DuracaoEmMinutos")
                .IsRequired();

            // Configuração de índices
            builder.HasIndex(s => s.Nome)
                .HasDatabaseName("IX_Servicos_Nome");

            // Configuração de relacionamentos
            builder.HasMany(s => s.Agendamentos)
                .WithOne(a => a.Servico)
                .HasForeignKey(a => a.ServicoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
} 