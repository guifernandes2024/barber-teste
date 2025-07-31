using Microsoft.EntityFrameworkCore;
using BarberShopApp.Data;

namespace BarberShopApp.Core.Services
{
    public class HorarioService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

        public HorarioService(IDbContextFactory<ApplicationDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        // Horário de funcionamento da barbearia (aplicado a todos os profissionais)
        private readonly TimeSpan _horaInicio = new TimeSpan(8, 0, 0); // 08:00
        private readonly TimeSpan _horaFim = new TimeSpan(18, 0, 0);   // 18:00
        private readonly int _intervaloMinutos = 30; // Intervalo entre horários

        // Método legado - mantido para compatibilidade
        public async Task<List<DateTime>> GetHorariosDisponiveisAsync(DateTime data, int servicoId)
        {
            using var context = _dbContextFactory.CreateDbContext();

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

            // Gerar todos os horários possíveis
            var horariosDisponiveis = new List<DateTime>();
            var horaAtual = _horaInicio;

            while (horaAtual.Add(TimeSpan.FromMinutes(duracaoServico)) <= _horaFim)
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

                horaAtual = horaAtual.Add(TimeSpan.FromMinutes(_intervaloMinutos));
            }

            return horariosDisponiveis;
        }

        // Método principal - busca horários disponíveis para um profissional específico
        public async Task<List<DateTime>> GetHorariosDisponiveisAsync(DateTime data, int profissionalId, int duracaoTotal)
        {
            using var context = _dbContextFactory.CreateDbContext();

            // Buscar agendamentos existentes para a data e profissional específico
            var agendamentosDoDia = await context.Agendamento
                .Include(a => a.Servicos)
                .Where(a => a.DataHora.Date == data.Date && a.ProfissionalId == profissionalId)
                .ToListAsync();

            // Gerar todos os horários possíveis
            var horariosDisponiveis = new List<DateTime>();
            var horaAtual = _horaInicio;

            // Se for hoje, começar a partir da hora atual (com margem de 30 minutos)
            if (data.Date == DateTime.Today)
            {
                var horaAgora = DateTime.Now.TimeOfDay;
                var horaMinima = horaAgora.Add(TimeSpan.FromMinutes(30)); // Margem de 30 minutos
                
                if (horaMinima > _horaFim)
                {
                    return horariosDisponiveis; // Não há horários disponíveis hoje
                }
                
                horaAtual = horaMinima > _horaInicio ? horaMinima : _horaInicio;
            }

            while (horaAtual.Add(TimeSpan.FromMinutes(duracaoTotal)) <= _horaFim)
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

                horaAtual = horaAtual.Add(TimeSpan.FromMinutes(_intervaloMinutos));
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

            var servico = await context.Servico.FindAsync(servicoId);
            if (servico == null)
                return false;

            var duracaoServico = servico.DuracaoEmMinutos;
            var horarioFim = dataHora.AddMinutes(duracaoServico);

            // Verificar se está dentro do horário de funcionamento
            if (dataHora.TimeOfDay < _horaInicio || horarioFim.TimeOfDay > _horaFim)
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

            var horarioFim = dataHora.AddMinutes(duracaoTotal);

            // Verificar se está dentro do horário de funcionamento
            if (dataHora.TimeOfDay < _horaInicio || horarioFim.TimeOfDay > _horaFim)
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

        public TimeSpan GetHoraInicio() => _horaInicio;
        public TimeSpan GetHoraFim() => _horaFim;
        public int GetIntervaloMinutos() => _intervaloMinutos;
    }
} 
