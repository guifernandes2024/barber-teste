using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using teste.Models;

namespace teste.Components.Pages
{
    public partial class LandingPage
    {
        private List<Servico> Servicos { get; set; } = new();
        private Agendamento Agendamento { get; set; } = new();
        private DateTime? HorarioSelecionado { get; set; }
        private bool isLoading { get; set; } = false;
        private string mensagemFeedback { get; set; } = string.Empty;
        private string tipoMensagem { get; set; } = string.Empty;
        private bool jsDisponivel = false;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                await CarregarServicos();
                Agendamento = new Agendamento();
            }
            catch (Exception ex)
            {
                await ExibirMensagem($"Erro ao carregar dados: {ex.Message}", "error");
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                jsDisponivel = true;
                StateHasChanged();
            }
        }

        private async Task ScrollToAgendamento()
        {
            if (jsDisponivel)
                await JS.InvokeVoidAsync("scrollToElementById", "agendamento");
        }
        private async Task ScrollToServicos()
        {
            if (jsDisponivel)
                await JS.InvokeVoidAsync("scrollToElementById", "servicos");
        }
        private void OnNumeroDoClienteInput(ChangeEventArgs e)
        {
            var input = e.Value?.ToString() ?? string.Empty;
            var maskedPhone = ApplyPhoneMask(input);
            Agendamento.NumeroDoCliente = maskedPhone;
            StateHasChanged();
        }

        private static string ApplyPhoneMask(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
            {
                return string.Empty;
            }

            var digits = new string(phoneNumber.Where(char.IsDigit).ToArray());

            return digits.Length switch
            {
                <= 2 => digits,
                <= 6 => $"({digits.Substring(0, 2)}) {digits.Substring(2)}",
                <= 10 => $"({digits.Substring(0, 2)}) {digits.Substring(2, 4)}-{digits.Substring(6)}",
                11 => $"({digits.Substring(0, 2)}) {digits.Substring(2, 5)}-{digits.Substring(7, 4)}",
                _ => $"({digits.Substring(0, 2)}) {digits.Substring(2, 5)}-{digits.Substring(7, 4)}"
            };
        }

        private static string CleanPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
            {
                return string.Empty;
            }
            return new string(phoneNumber.Where(char.IsDigit).ToArray());
        }
        private async Task CarregarServicos()
        {
            try
            {
                using var context = DbFactory.CreateDbContext();
                Servicos = await context.Servico
                    .OrderBy(s => s.Nome)
                    .ToListAsync();
            }
            catch (Exception)
            {
                // Se falhar, mantém lista vazia
                Servicos = new List<Servico>();
                throw; // Re-throw para ser tratado no OnInitializedAsync
            }
        }

        private async Task OnHorarioSelecionado(DateTime horario)
        {
            HorarioSelecionado = horario;
            Agendamento.DataHora = horario;

            if (Servicos.Any())
            {
                Agendamento.ServicoId = Servicos.First().Id;
            }

            // Limpar mensagens anteriores
            LimparMensagens();
        }

        private async Task AddAgendamento()
        {
            if (!HorarioSelecionado.HasValue)
            {
                await ExibirMensagem("Por favor, selecione um horário.", "error");
                return;
            }
            // (Isso deve vir do componente CalendarioHorarios ou de uma seleção explícita do usuário)
            if (Agendamento.ServicoId == 0)
            {
                mensagemFeedback = "Por favor, selecione um serviço para o agendamento.";
                tipoMensagem = "danger";
                return;
            }

            // Limpa o número de telefone antes de salvar no banco de dados
            Agendamento.NumeroDoCliente = CleanPhoneNumber(Agendamento.NumeroDoCliente);

            isLoading = true;
            LimparMensagens();

            try
            {
                Agendamento.DataHora = HorarioSelecionado.Value;

                using var context = DbFactory.CreateDbContext();

                // Verificar se o horário ainda está disponível
                var horarioExistente = await context.Agendamento
                    .AnyAsync(a => a.DataHora == Agendamento.DataHora && a.Id != Agendamento.Id);

                if (horarioExistente)
                {
                    await ExibirMensagem("Este horário já foi agendado. Por favor, escolha outro horário.", "error");
                    return;
                }

                context.Agendamento.Add(Agendamento);
                await context.SaveChangesAsync();

                await ExibirMensagem("Agendamento realizado com sucesso! Entraremos em contato em breve.", "success");

                // Limpar formulário após sucesso
                await Task.Delay(2000); // Mostrar mensagem por 2 segundos
                LimparFormulario();
            }
            catch (Exception ex)
            {
                await ExibirMensagem($"Erro ao realizar agendamento: {ex.Message}", "error");
            }
            finally
            {
                isLoading = false;
                StateHasChanged();
            }
        }

        private void LimparFormulario()
        {
            Agendamento = new Agendamento();
            HorarioSelecionado = null;
            LimparMensagens();
            StateHasChanged();
        }

        private async Task ExibirMensagem(string mensagem, string tipo)
        {
            mensagemFeedback = mensagem;
            tipoMensagem = tipo;
            StateHasChanged();

            // Auto-limpar mensagem após 5 segundos
            await Task.Delay(5000);
            if (mensagemFeedback == mensagem) // Só limpa se for a mesma mensagem
            {
                LimparMensagens();
                StateHasChanged();
            }
        }

        private void LimparMensagens()
        {
            mensagemFeedback = string.Empty;
            tipoMensagem = string.Empty;
        }
    }
}
