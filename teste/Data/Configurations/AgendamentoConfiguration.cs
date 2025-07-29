using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using teste.Models;

namespace teste.Data.Configurations
{
    public class AgendamentoConfiguration : IEntityTypeConfiguration<Agendamento>
    {
        public void Configure(EntityTypeBuilder<Agendamento> builder)
        {
            // Configuração da tabela
            builder.ToTable("Agendamentos");

            // Configuração da chave primária
            builder.HasKey(a => a.Id);

            // Configuração das propriedades
            builder.Property(a => a.Id)
                .HasColumnName("Id")
                .ValueGeneratedOnAdd()
                .IsRequired();

            builder.Property(a => a.DataHora)
                .HasColumnName("DataHora")
                .HasColumnType("datetime2")
                .IsRequired();

            builder.Property(a => a.NomeDoCliente)
                .HasColumnName("NomeDoCliente")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(a => a.NumeroDoCliente)
                .HasColumnName("NumeroDoCliente")
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(a => a.ServicoId)
                .HasColumnName("ServicoId")
                .IsRequired();

            builder.Property(a => a.Observacoes)
                .HasColumnName("Observacoes")
                .HasMaxLength(1000)
                .IsRequired(false);

            // Configuração de índices
            builder.HasIndex(a => a.DataHora)
                .HasDatabaseName("IX_Agendamentos_DataHora");

            builder.HasIndex(a => a.NomeDoCliente)
                .HasDatabaseName("IX_Agendamentos_NomeDoCliente");

            builder.HasIndex(a => a.ServicoId)
                .HasDatabaseName("IX_Agendamentos_ServicoId");

            // Configuração de relacionamentos
            builder.HasOne(a => a.Servico)
                .WithMany(s => s.Agendamentos)
                .HasForeignKey(a => a.ServicoId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
} 