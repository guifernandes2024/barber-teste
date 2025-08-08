using BarberShopApp.Core.Models;

namespace BarberShopApp.Core.Services
{
    public class AgendamentoTempService
    {
        public AgendamentoTemp? AgendamentoTemp { get; private set; }

        public void SalvarAgendamentoTemp(AgendamentoTemp agendamento)
        {
            AgendamentoTemp = agendamento;
        }

        public AgendamentoTemp? ObterAgendamentoTemp()
        {
            return AgendamentoTemp;
        }

        public void LimparAgendamentoTemp()
        {
            AgendamentoTemp = null;
        }
    }

    public class AgendamentoTemp
    {
        public List<int> ServicosIds { get; set; } = new();
        public int? ProfissionalId { get; set; }
        public DateTime? DataHora { get; set; }
        public string? Observacoes { get; set; }
    }
}

