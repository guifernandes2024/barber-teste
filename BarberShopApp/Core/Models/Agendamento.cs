using System.ComponentModel.DataAnnotations;

namespace BarberShopApp.Core.Models
{
    public class Agendamento : ModelBase
    {

        [Required(ErrorMessage = "Data e hora são obrigatórios")]
        public DateTime DataHora { get; set; }

        [Required(ErrorMessage = "Nome do cliente é obrigatório")]
        [StringLength(100, ErrorMessage = "Nome deve ter no máximo 100 caracteres")]
        public string NomeDoCliente { get; set; } = string.Empty;

        [Required(ErrorMessage = "Telefone é obrigatório")]
        [StringLength(20, ErrorMessage = "Telefone deve ter no máximo 20 caracteres")]
        [PhoneNumber(ErrorMessage = "Formato de telefone inválido. Use o formato (99) 99999-9999")]
        public string NumeroDoCliente { get; set; } = string.Empty;

        [Required(ErrorMessage = "Profissional é obrigatório")]
        public int ProfissionalId { get; set; }

        [StringLength(500, ErrorMessage = "Observações devem ter no máximo 500 caracteres")]
        public string Observacoes { get; set; } = string.Empty;

        public virtual ICollection<Servico> Servicos { get; set; } = new List<Servico>();
        public virtual Profissional Profissional { get; set; } = null!;

        public DateTime DataHoraFim => DataHora.AddMinutes(Servicos?.Sum( s => s.DuracaoEmMinutos) ?? 0);

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

        public Agendamento(DateTime dataHora, string nomeDoCliente, string numeroDoCliente, ICollection<Servico> servicos, Profissional profissional, string observacoes = "")
        {
            DataHora = dataHora;
            NomeDoCliente = nomeDoCliente;
            NumeroDoCliente = numeroDoCliente;
            Observacoes = observacoes;
            Servicos = servicos ?? throw new ArgumentNullException(nameof(servicos), "Serviços não podem ser nulos.");
            Profissional = profissional ?? throw new ArgumentNullException(nameof(profissional), "Profissional não pode ser nulo.");
        }
    }
}
