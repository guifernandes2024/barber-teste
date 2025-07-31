using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BarberShopApp.Data;

namespace BarberShopApp.Core.Models
{
    public class Cliente
    {
        [Key]
        public string Id { get; set; } = string.Empty; 

        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(100, ErrorMessage = "Nome deve ter no máximo 100 caracteres")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Telefone é obrigatório")]
        [StringLength(20, ErrorMessage = "Telefone deve ter no máximo 20 caracteres")]
        [PhoneNumber(ErrorMessage = "Formato de telefone inválido. Use o formato (99) 99999-9999")]
        public string Telefone { get; set; } = string.Empty;

        [ForeignKey("Id")]
        public virtual ApplicationUser ApplicationUser { get; set; } = null!;
    }
}
