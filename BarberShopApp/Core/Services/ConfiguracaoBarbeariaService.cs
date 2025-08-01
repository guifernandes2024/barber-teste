using BarberShopApp.Core.Models;
using BarberShopApp.Data;
using Microsoft.EntityFrameworkCore;

namespace BarberShopApp.Core.Services
{
    public class ConfiguracaoBarbeariaService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;

        public ConfiguracaoBarbeariaService(IDbContextFactory<ApplicationDbContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public async Task<ConfiguracaoBarbearia?> ObterConfiguracaoAsync()
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            return await context.ConfiguracaoBarbearia.FirstOrDefaultAsync();
        }

        public async Task<ConfiguracaoBarbearia> ObterOuCriarConfiguracaoAsync()
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            var config = await context.ConfiguracaoBarbearia.FirstOrDefaultAsync();
            
            if (config == null)
            {
                config = new ConfiguracaoBarbearia
                {
                    NomeBarbearia = "Barbearia Profissional",
                    Descricao = "Transforme seu visual com nossos especialistas. Agende seu horário de forma rápida e descomplicada.",
                    Endereco = "Rua das Barbearias, 123 - Centro",
                    Telefone = "(11) 99999-9999",
                    Email = "contato@barbearia.com",
                    HorarioAbertura = new TimeSpan(8, 0, 0),
                    HorarioFechamento = new TimeSpan(18, 0, 0),
                    IntervaloAgendamentoMinutos = 30,
                    PrazoMinimoAgendamentoDias = 1,
                    PrazoMaximoAgendamentoDias = 30
                };

                context.ConfiguracaoBarbearia.Add(config);
                await context.SaveChangesAsync();
            }

            return config;
        }

        public async Task<bool> SalvarConfiguracaoAsync(ConfiguracaoBarbearia configuracao)
        {
            try
            {
                using var context = await _dbFactory.CreateDbContextAsync();
                
                var configExistente = await context.ConfiguracaoBarbearia.FirstOrDefaultAsync();
                
                if (configExistente == null)
                {
                    configuracao.DataCriacao = DateTime.Now;
                    configuracao.DataAtualizacao = DateTime.Now;
                    context.ConfiguracaoBarbearia.Add(configuracao);
                }
                else
                {
                    configuracao.Id = configExistente.Id;
                    configuracao.DataCriacao = configExistente.DataCriacao;
                    configuracao.DataAtualizacao = DateTime.Now;
                    context.Entry(configExistente).CurrentValues.SetValues(configuracao);
                }

                await context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> EstaAbertaAsync(DateTime data)
        {
            var config = await ObterConfiguracaoAsync();
            if (config == null) return false;
            
            return config.EstaAberta(data.DayOfWeek);
        }

        public async Task<TimeSpan> ObterHorarioAberturaAsync(DateTime data)
        {
            var config = await ObterConfiguracaoAsync();
            if (config == null) return new TimeSpan(8, 0, 0);
            
            return config.ObterHorarioAbertura(data.DayOfWeek);
        }

        public async Task<TimeSpan> ObterHorarioFechamentoAsync(DateTime data)
        {
            var config = await ObterConfiguracaoAsync();
            if (config == null) return new TimeSpan(18, 0, 0);
            
            return config.ObterHorarioFechamento(data.DayOfWeek);
        }

        public async Task<bool> PodeAgendarAsync(DateTime data)
        {
            var config = await ObterConfiguracaoAsync();
            if (config == null) return false;
            
            return config.PodeAgendar(data);
        }

        public async Task<List<DateTime>> ObterHorariosDisponiveisAsync(DateTime data, TimeSpan duracaoServico)
        {
            var config = await ObterConfiguracaoAsync();
            if (config == null) return new List<DateTime>();

            if (!config.EstaAberta(data.DayOfWeek))
                return new List<DateTime>();

            var horarioAbertura = config.ObterHorarioAbertura(data.DayOfWeek);
            var horarioFechamento = config.ObterHorarioFechamento(data.DayOfWeek);
            var intervalo = TimeSpan.FromMinutes(config.IntervaloAgendamentoMinutos);

            var horarios = new List<DateTime>();
            var horarioAtual = data.Date.Add(horarioAbertura);

            while (horarioAtual.Add(duracaoServico) <= data.Date.Add(horarioFechamento))
            {
                horarios.Add(horarioAtual);
                horarioAtual = horarioAtual.Add(intervalo);
            }

            return horarios;
        }
    }
} 