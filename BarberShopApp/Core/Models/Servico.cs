using System.ComponentModel.DataAnnotations;

namespace BarberShopApp.Core.Models
{
    public class Servico() : ModelBase
    {

        [Required(ErrorMessage = "Nome � obrigat�rio")]
        public string Nome { get; set; } = string.Empty;

        public string Descricao { get; set; } = string.Empty;

        [Required(ErrorMessage = "Pre�o � obrigat�rio")]
        public decimal Preco { get; set; }

        [Required(ErrorMessage = "A URL da imagem � obrigat�ria")]
        [Url(ErrorMessage = "A URL da imagem � inv�lida.")]
        public string ImgUrl { get; set; } = string.Empty;

        [Required(ErrorMessage = "A dura��o � obrigat�rio")]
        [Range(1, 600, ErrorMessage = "A dura��o deve estar entre 1 e 600 minutos.")]
        public int DuracaoEmMinutos { get; set; }

        public virtual ICollection<Agendamento> Agendamentos { get; set; } = new List<Agendamento>();
        public virtual ICollection<Profissional> Profissionals { get; set; } = new List<Profissional>();
    }
}
