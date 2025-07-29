using Microsoft.EntityFrameworkCore;
using teste.Data;
using teste.Models;

namespace teste.Services
{
    public class HorarioService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

        public HorarioService(IDbContextFactory<ApplicationDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        // Horário de funcionamento da barbearia
        private readonly TimeSpan _horaInicio = new TimeSpan(8, 0, 0); // 08:00
        private readonly TimeSpan _horaFim = new TimeSpan(18, 0, 0);   // 18:00
        private readonly int _intervaloMinutos = 30; // Intervalo entre horários

        public async Task<List<DateTime>> GetHorariosDisponiveisAsync(DateTime data, int servicoId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            
            // Buscar o serviço para obter a duração
            var servico = await context.Servico.FindAsync(servicoId);
            if (servico == null)
                return new List<DateTime>();

            var duracaoServico = servico.DuracaoEmMinutos;

            // Buscar agendamentos existentes para a data
            var agendamentosDoDia = await context.Agendamento
                .Include(a => a.Servico)
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
                horarioProposto < a.DataHora.AddMinutes(a.Servico.DuracaoEmMinutos) && 
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

            // Verificar se há conflito com agendamentos existentes
            var agendamentosConflitantes = await context.Agendamento
                .Include(a => a.Servico)
                .Where(a => a.DataHora.Date == dataHora.Date)
                .Where(a => dataHora < a.DataHora.AddMinutes(a.Servico.DuracaoEmMinutos) && 
                           horarioFim > a.DataHora)
                .AnyAsync();

            return !agendamentosConflitantes;
        }

        public TimeSpan GetHoraInicio() => _horaInicio;
        public TimeSpan GetHoraFim() => _horaFim;
        public int GetIntervaloMinutos() => _intervaloMinutos;
    }
} 