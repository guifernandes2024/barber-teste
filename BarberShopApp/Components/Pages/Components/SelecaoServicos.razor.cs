using Microsoft.AspNetCore.Components;
using BarberShopApp.Core.Models;

namespace BarberShopApp.Components.Pages.Components
{
    public partial class SelecaoServicos
    {
        [Parameter]
        public List<Servico> Servicos { get; set; } = new();

        [Parameter]
        public List<Servico> ServicosSelecionados { get; set; } = new();

        [Parameter]
        public EventCallback<List<Servico>> ServicosSelecionadosChanged { get; set; }

        private async Task ToggleServico(Servico servico)
        {
            if (ServicosSelecionados.Any(s => s.Id == servico.Id))
            {
                // Remove o serviço se já estiver selecionado
                ServicosSelecionados.RemoveAll(s => s.Id == servico.Id);
            }
            else
            {
                // Adiciona o serviço se não estiver selecionado
                ServicosSelecionados.Add(servico);
            }

            await ServicosSelecionadosChanged.InvokeAsync(ServicosSelecionados);
            StateHasChanged();
        }
    }
} 