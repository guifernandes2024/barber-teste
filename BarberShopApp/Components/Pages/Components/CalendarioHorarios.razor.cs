using Microsoft.AspNetCore.Components;
using BarberShopApp.Core.Models;
using BarberShopApp.Core.Services;

namespace BarberShopApp.Components.Pages.Components
{
    public partial class CalendarioHorarios
    {
        [Parameter]
        public List<Servico> ServicosSelecionados { get; set; } = new();

        [Parameter]
        public Profissional? ProfissionalSelecionado { get; set; }

        [Parameter]
        public DateTime? HorarioSelecionado { get; set; }

        [Parameter]
        public EventCallback<DateTime> HorarioSelecionadoChanged { get; set; }

        private DateTime? DataSelecionada { get; set; } = DateTime.Today;
        private List<DateTime> HorariosDisponiveis { get; set; } = new();

        private async Task OnDataChanged(ChangeEventArgs e)
        {
            if (DateTime.TryParseExact(e.Value?.ToString(), "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime data))
            {
                DataSelecionada = data;
            }
            else
            {
                DataSelecionada = null;
                Console.WriteLine($"AVISO: Formato de data inválido ou data vazia: {e.Value}");
            }

            await CarregarHorarios();
        }

        private async Task CarregarHorarios()
        {
            if (ServicosSelecionados.Any() && ProfissionalSelecionado != null && DataSelecionada.HasValue)
            {
                try
                {
                    // Calcula a duração total dos serviços selecionados
                    int duracaoTotal = ServicosSelecionados.Sum(s => s.DuracaoEmMinutos);
                    
                    // Busca horários disponíveis considerando o profissional e a duração total
                    HorariosDisponiveis = await HorarioService.GetHorariosDisponiveisAsync(
                        DataSelecionada.Value, 
                        ProfissionalSelecionado.Id, 
                        duracaoTotal);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao carregar horários: {ex.Message}");
                    HorariosDisponiveis = new List<DateTime>();
                }
            }
            else
            {
                HorariosDisponiveis = new List<DateTime>();
            }
            StateHasChanged();
        }

        private async Task SelecionarHorario(DateTime horario)
        {
            HorarioSelecionado = horario;
            await HorarioSelecionadoChanged.InvokeAsync(horario);
            StateHasChanged();
        }

        // Método para recarregar horários quando serviços ou profissional mudam
        protected override async Task OnParametersSetAsync()
        {
            await CarregarHorarios();
        }
    }
}
