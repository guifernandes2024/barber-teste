using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace BarberShopApp.Core.Models
{
    public class PhoneNumberAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return ValidationResult.Success; // Permite valores nulos/vazios se não for obrigatório
            }

            string phoneNumber = value.ToString()!;
            
            // Remove todos os caracteres não numéricos
            string cleanNumber = Regex.Replace(phoneNumber, @"[^\d]", "");
            
            // Valida se tem entre 10 e 11 dígitos (DDD + número)
            if (cleanNumber.Length < 10 || cleanNumber.Length > 11)
            {
                return new ValidationResult("O número de telefone deve ter entre 10 e 11 dígitos.");
            }
            
            // Valida se começa com DDD válido (11-99)
            string ddd = cleanNumber.Substring(0, 2);
            if (!int.TryParse(ddd, out int dddNumber) || dddNumber < 11 || dddNumber > 99)
            {
                return new ValidationResult("DDD inválido. Deve estar entre 11 e 99.");
            }
            
            return ValidationResult.Success;
        }
    }
} 