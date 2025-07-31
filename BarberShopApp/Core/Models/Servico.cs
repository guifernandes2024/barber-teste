using System.ComponentModel.DataAnnotations;

namespace BarberShopApp.Core.Models
{
    public class Servico() : ModelBase
    {

        [Required(ErrorMessage = "Nome é obrigatório")]
        public string Nome { get; set; } = string.Empty;

        public string Descricao { get; set; } = string.Empty;

        [Required(ErrorMessage = "Preço é obrigatório")]
        public decimal Preco { get; set; }

        [Required(ErrorMessage = "A URL da imagem é obrigatória")]
        [Url(ErrorMessage = "A URL da imagem é inválida.")]
        public string ImgUrl { get; set; } = string.Empty;

        [Required(ErrorMessage = "A duração é obrigatório")]
        [Range(1, 600, ErrorMessage = "A duração deve estar entre 1 e 600 minutos.")]
        public int DuracaoEmMinutos { get; set; }

        public virtual ICollection<Agendamento>? Agendamentos { get; set; }
        public virtual ICollection<Profissional>? Profissionals { get; set; }
    }
}
