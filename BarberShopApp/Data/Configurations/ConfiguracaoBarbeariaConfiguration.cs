using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BarberShopApp.Core.Models;

namespace BarberShopApp.Data.Configurations
{
    public class ConfiguracaoBarbeariaConfiguration : IEntityTypeConfiguration<ConfiguracaoBarbearia>
    {
        public void Configure(EntityTypeBuilder<ConfiguracaoBarbearia> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.NomeBarbearia)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Descricao)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(x => x.Endereco)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Telefone)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Site)
                .HasMaxLength(200);

            builder.Property(x => x.Instagram)
                .HasMaxLength(200);

            builder.Property(x => x.Facebook)
                .HasMaxLength(200);

            builder.Property(x => x.WhatsApp)
                .HasMaxLength(200);

            builder.Property(x => x.HorarioAbertura)
                .IsRequired();

            builder.Property(x => x.HorarioFechamento)
                .IsRequired();

            // Dias de funcionamento
            builder.Property(x => x.SegundaAberta)
                .HasDefaultValue(true);

            builder.Property(x => x.TercaAberta)
                .HasDefaultValue(true);

            builder.Property(x => x.QuartaAberta)
                .HasDefaultValue(true);

            builder.Property(x => x.QuintaAberta)
                .HasDefaultValue(true);

            builder.Property(x => x.SextaAberta)
                .HasDefaultValue(true);

            builder.Property(x => x.SabadoAberta)
                .HasDefaultValue(true);

            builder.Property(x => x.DomingoAberta)
                .HasDefaultValue(false);

            // Configurações de agendamento
            builder.Property(x => x.IntervaloAgendamentoMinutos)
                .HasDefaultValue(30);

            builder.Property(x => x.PrazoMinimoAgendamentoDias)
                .HasDefaultValue(1);

            builder.Property(x => x.PrazoMaximoAgendamentoDias)
                .HasDefaultValue(30);

            builder.Property(x => x.PoliticasCancelamento)
                .HasMaxLength(500);

            builder.Property(x => x.Observacoes)
                .HasMaxLength(500);

            builder.Property(x => x.DataCriacao)
                .HasDefaultValueSql("GETDATE()");

            builder.Property(x => x.DataAtualizacao)
                .HasDefaultValueSql("GETDATE()");

            // Configurar para ter apenas uma configuração
            builder.HasIndex(x => x.Id).IsUnique();
        }
    }
} 