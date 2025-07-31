using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BarberShopApp.Core.Models;
using BarberShopApp.Data;

namespace BarberShopApp.Components.Pages.Admin
{
    public partial class PerfilProfissional
    {
        [Inject] private IDbContextFactory<ApplicationDbContext> DbFactory { get; set; } = default!;
        [Inject] private UserManager<ApplicationUser> UserManager { get; set; } = default!;
        [Inject] private SignInManager<ApplicationUser> SignInManager { get; set; } = default!;
        [Inject] private NavigationManager Navigation { get; set; } = default!;

        private ApplicationUser? user;
        private Profissional? profissional;
        private List<Servico> servicos = new();
        private List<int> servicoSelecionado = new();
        private bool isLoading = true;
        private bool isSaving = false;
        private string mensagem = string.Empty;
        private string tipoMensagem = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                // Verificar se o usuário está logado
                var currentUser = await UserManager.GetUserAsync(SignInManager.Context.User);
                if (currentUser == null)
                {
                    Navigation.NavigateTo("/login");
                    return;
                }

                // Verificar se o usuário é um profissional
                var isProfissional = await UserManager.IsInRoleAsync(currentUser, "Profissional");
                if (!isProfissional)
                {
                    Navigation.NavigateTo("/admin");
                    return;
                }

                using var context = DbFactory.CreateDbContext();

                // Carregar dados do usuário
                user = await context.Users
                    .Include(u => u.Profissional)
                    .FirstOrDefaultAsync(u => u.Id == currentUser.Id);

                if (user?.Profissional == null)
                {
                    mensagem = "Profissional não encontrado.";
                    tipoMensagem = "warning";
                    return;
                }

                profissional = user.Profissional;

                // Carregar serviços
                servicos = await context.Servico.ToListAsync();

                // Carregar especialidades do profissional
                await context.Entry(profissional)
                    .Collection(p => p.Especialidades)
                    .LoadAsync();

                // Inicializar lista de serviços selecionados
                servicoSelecionado = profissional.Especialidades.Select(s => s.Id).ToList();
            }
            catch (Exception ex)
            {
                mensagem = $"Erro ao carregar dados: {ex.Message}";
                tipoMensagem = "danger";
            }
            finally
            {
                isLoading = false;
                StateHasChanged();
            }
        }

        private async Task SalvarPerfil()
        {
            if (user == null || profissional == null)
                return;

            try
            {
                isSaving = true;
                StateHasChanged();

                using var context = DbFactory.CreateDbContext();

                // Atualizar dados do usuário
                var userToUpdate = await context.Users.FindAsync(user.Id);
                if (userToUpdate != null)
                {
                    userToUpdate.Nome = user.Nome;
                    userToUpdate.Email = user.Email;
                    userToUpdate.PhoneNumber = user.PhoneNumber;
                }

                // Atualizar dados do profissional
                var profissionalToUpdate = await context.Profissional
                    .Include(p => p.Especialidades)
                    .FirstOrDefaultAsync(p => p.Id == profissional.Id);

                if (profissionalToUpdate != null)
                {
                    profissionalToUpdate.DataNacimento = profissional.DataNacimento;
                    profissionalToUpdate.TipoDocumento = profissional.TipoDocumento;
                    profissionalToUpdate.Documento = profissional.Documento;
                    profissionalToUpdate.ImgUrl = profissional.ImgUrl;
                    profissionalToUpdate.PercentualDeComissao = profissional.PercentualDeComissao;
                    profissionalToUpdate.Fumante = profissional.Fumante;
                    profissionalToUpdate.DataAtualizacao = DateTime.Now;

                    // Atualizar especialidades
                    var servicosSelecionados = await context.Servico
                        .Where(s => servicoSelecionado.Contains(s.Id))
                        .ToListAsync();

                    profissionalToUpdate.Especialidades.Clear();
                    foreach (var servico in servicosSelecionados)
                    {
                        profissionalToUpdate.Especialidades.Add(servico);
                    }
                }

                await context.SaveChangesAsync();

                // Atualizar dados do Identity
                var identityUser = await UserManager.FindByIdAsync(user.Id);
                if (identityUser != null)
                {
                    identityUser.Nome = user.Nome;
                    identityUser.Email = user.Email;
                    identityUser.PhoneNumber = user.PhoneNumber;
                    await UserManager.UpdateAsync(identityUser);
                }

                mensagem = "Perfil atualizado com sucesso!";
                tipoMensagem = "success";
            }
            catch (Exception ex)
            {
                mensagem = $"Erro ao salvar perfil: {ex.Message}";
                tipoMensagem = "danger";
            }
            finally
            {
                isSaving = false;
                StateHasChanged();
            }
        }

        private void OnServicoChanged(int servicoId, object? value)
        {
            if (value is bool isChecked)
            {
                if (isChecked)
                {
                    if (!servicoSelecionado.Contains(servicoId))
                    {
                        servicoSelecionado.Add(servicoId);
                    }
                }
                else
                {
                    servicoSelecionado.Remove(servicoId);
                }
            }
        }
    }
} 