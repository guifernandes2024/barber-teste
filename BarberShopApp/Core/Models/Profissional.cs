using System.ComponentModel.DataAnnotations;

namespace BarberShopApp.Core.Models
{
    public class Profissional : ModelBase
    {
        [Required(ErrorMessage = "Tipo do Documento é obrigatório")]
        public DocumentType TipoDocumento { get; set; }

        [Required(ErrorMessage = "Documento é obrigatório")]
        public string Documento { get; set; } = string.Empty;

        [Required(ErrorMessage = "Data de nascimento é obrigatória")]
        public DateTime DataNacimento { get; set; }
        public bool Fumante { get; set; }
        [Required(ErrorMessage = "A URL da imagem é obrigatória")]
        [Url(ErrorMessage = "A URL da imagem é inválida.")]
        public string ImgUrl { get; set; } = string.Empty;

        public int? PercentualDeComissao { get; set; }
        public virtual ICollection<Servico> Especialidades { get; set; } = new List<Servico>();
        public virtual ICollection<Agendamento> Agendamentos { get; set; } = new List<Agendamento>();

        public  DateTime DataAniversario
        {
            get
            {
                DateTime today = DateTime.Today;
                DateTime nextBirthday = new (today.Year, DataNacimento.Month, DataNacimento.Day);

                if (nextBirthday < today)
                {
                    nextBirthday = nextBirthday.AddYears(1);
                }

                return nextBirthday;
            }

        }
    }

    public enum DocumentType
    {
        CPF,
        CNPJ,
        RG,
        Passaporte
    }
}
