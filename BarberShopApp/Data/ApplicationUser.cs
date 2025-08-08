using Microsoft.AspNetCore.Identity;
using BarberShopApp.Core.Models;
using System.ComponentModel.DataAnnotations;

namespace BarberShopApp.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public string Nome { get; set; } = string.Empty;
        public virtual Cliente? Cliente { get; set; }
        public int? ProfissionalId { get; set; }
        public virtual Profissional? Profissional { get; set; }
        
        // Data de criação do usuário
        public DateTime? CreatedAt { get; set; }
        
        // Sobrescreve a propriedade PhoneNumber para adicionar validação
        [PhoneNumber(ErrorMessage = "Formato de telefone inválido. Use o formato (99) 99999-9999")]
        public override string? PhoneNumber { get; set; }
    }
}
