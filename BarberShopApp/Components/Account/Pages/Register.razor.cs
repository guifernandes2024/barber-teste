using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using BarberShopApp.Data;
using BarberShopApp.Core.Models;

namespace BarberShopApp.Components.Account.Pages
{
    public partial class Register
    {
        private IEnumerable<IdentityError>? identityErrors;
        [SupplyParameterFromForm]
        private InputModel Input { get; set; } = new();

        [SupplyParameterFromQuery]
        private string? ReturnUrl { get; set; }

        private string? Message => identityErrors is null ? null : $"Erro: {string.Join(", ", identityErrors.Select(error => error.Description))}";

        // Novo m�todo para lidar com a mudan�a no InputSelect
        private void OnRegistrationTypeChange(ChangeEventArgs e)
        {
            StateHasChanged();
        }

        public async Task RegisterUser(EditContext editContext)
        {
            identityErrors = null; // Limpar erros anteriores
            var isValid = editContext.Validate(); // Execute a valida��o do EditContext

            if (!isValid)
            {
                return;
            }

            // Valida��o de campos do profissional se o tipo for Profissional
            if (Input.RegistrationType == UserRegistrationType.Professional)
            {
                // Seus ifs de valida��o manual
                if (string.IsNullOrWhiteSpace(Input.NomeProfissional)) AddIdentityError("O nome do profissional � obrigat�rio.");
                if (string.IsNullOrWhiteSpace(Input.TelefoneProfissional)) AddIdentityError("O telefone do profissional � obrigat�rio.");
                if (!Input.TipoDocumentoProfissional.HasValue) AddIdentityError("O tipo de documento do profissional � obrigat�rio.");
                if (string.IsNullOrWhiteSpace(Input.DocumentoProfissional)) AddIdentityError("O documento do profissional � obrigat�rio.");
                if (!Input.DataNascimentoProfissional.HasValue) AddIdentityError("A data de nascimento do profissional � obrigat�ria.");
                if (string.IsNullOrWhiteSpace(Input.ImgUrlProfissional)) AddIdentityError("A URL da imagem do profissional � obrigat�ria.");
                else if (!Uri.IsWellFormedUriString(Input.ImgUrlProfissional, UriKind.Absolute)) AddIdentityError("A URL da imagem do profissional � inv�lida.");

                if (identityErrors != null && identityErrors.Any())
                {
                    return;
                }
            }

            // Valida��o de campos do cliente se o tipo for Cliente
            if (Input.RegistrationType == UserRegistrationType.Client)
            {
                // Seus ifs de valida��o manual
                if (string.IsNullOrWhiteSpace(Input.NomeCliente)) AddIdentityError("O nome do cliente � obrigat�rio.");
                if (string.IsNullOrWhiteSpace(Input.TelefoneCliente)) AddIdentityError("O telefone do cliente � obrigat�rio.");

                if (identityErrors != null && identityErrors.Any())
                {
                    return;
                }
            }

            var user = CreateUser();

            await UserStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
            var emailStore = GetEmailStore();
            await emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
            var result = await UserManager.CreateAsync(user, Input.Password);

            if (!result.Succeeded)
            {
                identityErrors = result.Errors;
                return;
            }

            Logger.LogInformation("User created a new account with password.");

            switch (Input.RegistrationType)
            {
                case UserRegistrationType.Admin:
                    await UserManager.AddToRoleAsync(user, "Admin");
                    Logger.LogInformation("User assigned 'Admin' role.");
                    break;
                case UserRegistrationType.Professional:
                    await UserManager.AddToRoleAsync(user, "Profissional");
                    Logger.LogInformation("User assigned 'Profissional' role.");

                    var profissional = new Profissional
                    {
                        Nome = Input.NomeProfissional!,
                        Email = Input.Email,
                        Telefone = Input.TelefoneProfissional!,
                        TipoDocumento = Input.TipoDocumentoProfissional!.Value,
                        Documento = Input.DocumentoProfissional!,
                        DataNacimento = Input.DataNascimentoProfissional!.Value,
                        Fumante = Input.FumanteProfissional,
                        ImgUrl = Input.ImgUrlProfissional!,
                        PercentualDeComissao = Input.PercentualDeComissaoProfissional,
                        DataCriacao = DateTime.UtcNow,
                        DataAtualizacao = DateTime.UtcNow
                    };

                    await DbContext.Profissional.AddAsync(profissional);
                    await DbContext.SaveChangesAsync();
                    Logger.LogInformation($"Profissional '{profissional.Nome}' created and associated.");
                    break;
                case UserRegistrationType.Client:
                default:
                    user.UserName = Input.NomeCliente;
                    await UserManager.AddToRoleAsync(user, "Client");

                    var cliente = new Cliente()
                    {
                        ApplicationUser = user,
                        Nome = user.UserName,
                        Telefone = Input.TelefoneCliente
                    };

                    await DbContext.Cliente.AddAsync(cliente);
                    Logger.LogInformation("User assigned 'Client' role.");
                    break;
            }

            await SignInManager.SignInAsync(user, isPersistent: false);
            RedirectManager.RedirectTo(ReturnUrl);
        }

        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor.");
            }
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!UserManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ApplicationUser>)UserStore;
        }

        // M�todo auxiliar para adicionar erros de identidade
        private void AddIdentityError(string description)
        {
            if (identityErrors == null)
            {
                identityErrors = new List<IdentityError>();
            }
            ((List<IdentityError>)identityErrors).Add(new IdentityError { Description = description });
        }

        // O InputModel e o enum UserRegistrationType devem ser definidos aqui ou em um arquivo separado
        private sealed class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; } = "";

            [Required]
            [StringLength(100, ErrorMessage = "A {0} deve ter no m�nimo {2} e no m�ximo {1} caracteres.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Senha")]
            public string Password { get; set; } = "";

            [DataType(DataType.Password)]
            [Display(Name = "Confirmar Senha")]
            [Compare("Password", ErrorMessage = "A senha e a confirma��o de senha n�o coincidem.")]
            public string ConfirmPassword { get; set; } = "";

            [Required(ErrorMessage = "O tipo de registro � obrigat�rio.")]
            public UserRegistrationType RegistrationType { get; set; } = UserRegistrationType.Client;
            [Display(Name = "Nome")]
            [Required(ErrorMessage = "Nome do cliente � obrigat�rio.")]
            public string? NomeCliente { get; set; }

            [Display(Name = "Telefone")]
            [Required(ErrorMessage = "Telefone do cliente � obrigat�rio.")]
            public string? TelefoneCliente { get; set; }

            [Display(Name = "Nome")]
            [Required(ErrorMessage = "Nome do profissional � obrigat�rio.")]
            public string? NomeProfissional { get; set; }

            [Display(Name = "Telefone")]
            [Required(ErrorMessage = "Telefone do profissional � obrigat�rio.")]
            public string? TelefoneProfissional { get; set; }

            [Display(Name = "Tipo de Documento")]
            public DocumentType? TipoDocumentoProfissional { get; set; }

            [Display(Name = "Documento")]
            public string? DocumentoProfissional { get; set; }

            [Display(Name = "Data de Nascimento")]
            public DateTime? DataNascimentoProfissional { get; set; }

            [Display(Name = "Fumante")]
            public bool FumanteProfissional { get; set; }

            [Display(Name = "URL da Imagem")]
            public string? ImgUrlProfissional { get; set; }

            [Display(Name = "Percentual de Comiss�o")]
            public int? PercentualDeComissaoProfissional { get; set; }
        }

        public enum UserRegistrationType
        {
            Client,
            Admin,
            Professional
        }
    }
}
