using System.ComponentModel.DataAnnotations;

namespace teste.Models
{
    public class Agendamento
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Data e hora s�o obrigat�rios")]
        public DateTime DataHora { get; set; }

        [Required(ErrorMessage = "Nome do cliente � obrigat�rio")]
        [StringLength(100, ErrorMessage = "Nome deve ter no m�ximo 100 caracteres")]
        public string NomeDoCliente { get; set; } = string.Empty;

        [Required(ErrorMessage = "Telefone � obrigat�rio")]
        [StringLength(20, ErrorMessage = "Telefone deve ter no m�ximo 20 caracteres")]
        public string NumeroDoCliente { get; set; } = string.Empty;

        [Required(ErrorMessage = "Servi�o � obrigat�rio")]
        public int ServicoId { get; set; }

        [StringLength(500, ErrorMessage = "Observa��es devem ter no m�ximo 500 caracteres")]
        public string Observacoes { get; set; } = string.Empty;

        public virtual Servico Servico { get; set; } = null!;

        public DateTime DataHoraFim => DataHora.AddMinutes(Servico?.DuracaoEmMinutos ?? 0);

        public string HoraInicioString => DataHora.ToString("HH:mm");
        public string HoraFimString => DataHoraFim.ToString("HH:mm");
        public string DataString => DataHora.ToString("dd/MM/yyyy");
        public string DataHoraCompleta => DataHora.ToString("dd/MM/yyyy HH:mm");

        public DateOnly DataApenas => DateOnly.FromDateTime(DataHora);
        public TimeOnly HoraApenas => TimeOnly.FromDateTime(DataHora);

        public Agendamento()
        {
            DataHora = DateTime.Now;
        }

        public Agendamento(DateTime dataHora, string nomeDoCliente, string numeroDoCliente, int servicoId, string observacoes = "")
        {
            DataHora = dataHora;
            NomeDoCliente = nomeDoCliente;
            NumeroDoCliente = numeroDoCliente;
            ServicoId = servicoId;
            Observacoes = observacoes;
        }
    }
}