using Microsoft.EntityFrameworkCore;
using BarberShopApp.Data;

namespace BarberShopApp.Core.Services
{
    public class HorarioService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
        private readonly ConfiguracaoBarbeariaService _configuracaoService;

        public HorarioService(IDbContextFactory<ApplicationDbContext> dbContextFactory, ConfiguracaoBarbeariaService configuracaoService)
        {
            _dbContextFactory = dbContextFactory;
            _configuracaoService = configuracaoService;
        }

        // Método legado - mantido para compatibilidade
        public async Task<List<DateTime>> GetHorariosDisponiveisAsync(DateTime data, int servicoId)
        {
            using var context = _dbContextFactory.CreateDbContext();

            // Verificar se a barbearia está aberta neste dia
            if (!await _configuracaoService.EstaAbertaAsync(data))
                return new List<DateTime>();

            // Buscar o serviço para obter a duração
            var servico = await context.Servico.FindAsync(servicoId);
            if (servico == null)
                return new List<DateTime>();

            var duracaoServico = servico.DuracaoEmMinutos;

            // Buscar agendamentos existentes para a data (todos os profissionais)
            var agendamentosDoDia = await context.Agendamento
                .Include(a => a.Servicos)
                .Where(a => a.DataHora.Date == data.Date)
                .ToListAsync();

            // Obter horários de funcionamento da configuração
            var horarioAbertura = await _configuracaoService.ObterHorarioAberturaAsync(data);
            var horarioFechamento = await _configuracaoService.ObterHorarioFechamentoAsync(data);
            var configuracao = await _configuracaoService.ObterConfiguracaoAsync();
            var intervalo = configuracao?.IntervaloAgendamentoMinutos ?? 30;

            // Gerar todos os horários possíveis
            var horariosDisponiveis = new List<DateTime>();
            var horaAtual = horarioAbertura;

            while (horaAtual.Add(TimeSpan.FromMinutes(duracaoServico)) <= horarioFechamento)
            {
                var horarioProposto = data.Date.Add(horaAtual);
                var horarioFim = horarioProposto.AddMinutes(duracaoServico);

                // Verificar se há conflito com agendamentos existentes
                var temConflito = agendamentosDoDia.Any(a =>
                horarioProposto < a.DataHora.AddMinutes(a.Servicos.Sum( s => s.DuracaoEmMinutos)) && 
                horarioFim > a.DataHora);

                if (!temConflito)
                {
                    horariosDisponiveis.Add(horarioProposto);
                }

                horaAtual = horaAtual.Add(TimeSpan.FromMinutes(intervalo));
            }

            return horariosDisponiveis;
        }

        // Método principal - busca horários disponíveis para um profissional específico
        public async Task<List<DateTime>> GetHorariosDisponiveisAsync(DateTime data, int profissionalId, int duracaoTotal)
        {
            using var context = _dbContextFactory.CreateDbContext();

            // Verificar se a barbearia está aberta neste dia
            if (!await _configuracaoService.EstaAbertaAsync(data))
                return new List<DateTime>();

            // Verificar se pode agendar nesta data
            if (!await _configuracaoService.PodeAgendarAsync(data))
                return new List<DateTime>();

            // Buscar agendamentos existentes para a data e profissional específico
            var agendamentosDoDia = await context.Agendamento
                .Include(a => a.Servicos)
                .Where(a => a.DataHora.Date == data.Date && a.ProfissionalId == profissionalId)
                .ToListAsync();

            // Obter horários de funcionamento da configuração
            var horarioAbertura = await _configuracaoService.ObterHorarioAberturaAsync(data);
            var horarioFechamento = await _configuracaoService.ObterHorarioFechamentoAsync(data);
            var configuracao = await _configuracaoService.ObterConfiguracaoAsync();
            var intervalo = configuracao?.IntervaloAgendamentoMinutos ?? 30;

            // Gerar todos os horários possíveis
            var horariosDisponiveis = new List<DateTime>();
            var horaAtual = horarioAbertura;

            // Se for hoje, começar a partir da hora atual (com margem de 30 minutos)
            if (data.Date == DateTime.Today)
            {
                var horaAgora = DateTime.Now.TimeOfDay;
                var horaMinima = horaAgora.Add(TimeSpan.FromMinutes(30)); // Margem de 30 minutos
                
                if (horaMinima > horarioFechamento)
                {
                    return horariosDisponiveis; // Não há horários disponíveis hoje
                }
                
                horaAtual = horaMinima > horarioAbertura ? horaMinima : horarioAbertura;
            }

            while (horaAtual.Add(TimeSpan.FromMinutes(duracaoTotal)) <= horarioFechamento)
            {
                var horarioProposto = data.Date.Add(horaAtual);
                var horarioFim = horarioProposto.AddMinutes(duracaoTotal);

                // Verificar se há conflito com agendamentos existentes do mesmo profissional
                var temConflito = agendamentosDoDia.Any(a =>
                    horarioProposto < a.DataHora.AddMinutes(a.Servicos.Sum(s => s.DuracaoEmMinutos)) && 
                    horarioFim > a.DataHora);

                if (!temConflito)
                {
                    horariosDisponiveis.Add(horarioProposto);
                }

                horaAtual = horaAtual.Add(TimeSpan.FromMinutes(intervalo));
            }

            return horariosDisponiveis;
        }

        public async Task<List<DateTime>> GetHorariosDisponiveisParaSemanaAsync(DateTime dataInicio, int servicoId)
        {
            var horarios = new List<DateTime>();

            // Gerar horários para 7 dias a partir da data de início
            for (int i = 0; i < 7; i++)
            {
                var data = dataInicio.AddDays(i);
                var horariosDoDia = await GetHorariosDisponiveisAsync(data, servicoId);
                horarios.AddRange(horariosDoDia);
            }

            return horarios;
        }

        // Método legado - verifica disponibilidade geral da barbearia
        public async Task<bool> VerificarDisponibilidadeAsync(DateTime dataHora, int servicoId)
        {
            using var context = _dbContextFactory.CreateDbContext();

            // Verificar se a barbearia está aberta neste dia
            if (!await _configuracaoService.EstaAbertaAsync(dataHora))
                return false;

            var servico = await context.Servico.FindAsync(servicoId);
            if (servico == null)
                return false;

            var duracaoServico = servico.DuracaoEmMinutos;
            var horarioFim = dataHora.AddMinutes(duracaoServico);

            // Obter horários de funcionamento da configuração
            var horarioAbertura = await _configuracaoService.ObterHorarioAberturaAsync(dataHora);
            var horarioFechamento = await _configuracaoService.ObterHorarioFechamentoAsync(dataHora);

            // Verificar se está dentro do horário de funcionamento
            if (dataHora.TimeOfDay < horarioAbertura || horarioFim.TimeOfDay > horarioFechamento)
                return false;

            // Verificar se há conflito com agendamentos existentes (todos os profissionais)
            var agendamentosConflitantes = await context.Agendamento
                .Include(a => a.Servicos)
                .Where(a => a.DataHora.Date == dataHora.Date)
                .Where(a => dataHora < a.DataHora.AddMinutes(a.Servicos.Sum(s => s.DuracaoEmMinutos)) && 
                           horarioFim > a.DataHora)
                .AnyAsync();

            return !agendamentosConflitantes;
        }

        // Método principal - verifica disponibilidade para um profissional específico
        public async Task<bool> VerificarDisponibilidadeAsync(DateTime dataHora, int profissionalId, int duracaoTotal)
        {
            using var context = _dbContextFactory.CreateDbContext();

            // Verificar se a barbearia está aberta neste dia
            if (!await _configuracaoService.EstaAbertaAsync(dataHora))
                return false;

            // Verificar se pode agendar nesta data
            if (!await _configuracaoService.PodeAgendarAsync(dataHora))
                return false;

            var horarioFim = dataHora.AddMinutes(duracaoTotal);

            // Obter horários de funcionamento da configuração
            var horarioAbertura = await _configuracaoService.ObterHorarioAberturaAsync(dataHora);
            var horarioFechamento = await _configuracaoService.ObterHorarioFechamentoAsync(dataHora);

            // Verificar se está dentro do horário de funcionamento
            if (dataHora.TimeOfDay < horarioAbertura || horarioFim.TimeOfDay > horarioFechamento)
                return false;

            // Se for hoje, verificar se o horário não é no passado
            if (dataHora.Date == DateTime.Today)
            {
                var horaMinima = DateTime.Now.AddMinutes(30); // Margem de 30 minutos
                if (dataHora < horaMinima)
                    return false;
            }

            // Verificar se há conflito com agendamentos existentes do mesmo profissional
            var agendamentosConflitantes = await context.Agendamento
                .Include(a => a.Servicos)
                .Where(a => a.DataHora.Date == dataHora.Date && a.ProfissionalId == profissionalId)
                .Where(a => dataHora < a.DataHora.AddMinutes(a.Servicos.Sum(s => s.DuracaoEmMinutos)) && 
                           horarioFim > a.DataHora)
                .AnyAsync();

            return !agendamentosConflitantes;
        }

        // Métodos para obter informações de configuração
        public async Task<TimeSpan> GetHoraInicioAsync(DateTime data) => await _configuracaoService.ObterHorarioAberturaAsync(data);
        public async Task<TimeSpan> GetHoraFimAsync(DateTime data) => await _configuracaoService.ObterHorarioFechamentoAsync(data);
        public async Task<int> GetIntervaloMinutosAsync()
        {
            var config = await _configuracaoService.ObterConfiguracaoAsync();
            return config?.IntervaloAgendamentoMinutos ?? 30;
        }

        // Métodos legados para compatibilidade
        public TimeSpan GetHoraInicio() => new TimeSpan(8, 0, 0);
        public TimeSpan GetHoraFim() => new TimeSpan(18, 0, 0);
        public int GetIntervaloMinutos() => 30;
    }
} 
