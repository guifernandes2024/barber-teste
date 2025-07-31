using Microsoft.AspNetCore.Components;
using BarberShopApp.Core.Models;

namespace BarberShopApp.Components.Pages.Components
{
    public partial class SelecaoProfissional
    {
        [Parameter]
        public List<Profissional> Profissionais { get; set; } = new();

        [Parameter]
        public Profissional? ProfissionalSelecionado { get; set; }

        [Parameter]
        public EventCallback<Profissional?> ProfissionalSelecionadoChanged { get; set; }

        [Parameter]
        public List<Servico> ServicosSelecionados { get; set; } = new();

        private List<Profissional> ProfissionaisFiltrados => Profissionais
            .Where(p => p.Especialidades != null && 
                       ServicosSelecionados.All(s => p.Especialidades.Any(e => e.Id == s.Id)))
            .ToList();

        protected override async Task OnParametersSetAsync()
        {
            // Verifica se o profissional selecionado ainda está disponível após mudança nos serviços
            if (ProfissionalSelecionado != null && !ProfissionaisFiltrados.Any(p => p.Id == ProfissionalSelecionado.Id))
            {
                // Limpa a seleção se o profissional não está mais disponível
                await ProfissionalSelecionadoChanged.InvokeAsync(null);
            }
        }

        private async Task SelecionarProfissional(Profissional profissional)
        {
            ProfissionalSelecionado = profissional;
            await ProfissionalSelecionadoChanged.InvokeAsync(profissional);
            StateHasChanged();
        }
    }
} 