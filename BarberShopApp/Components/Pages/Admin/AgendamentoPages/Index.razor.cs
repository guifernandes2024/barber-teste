using Microsoft.AspNetCore.Components;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using BarberShopApp.Data;
using BarberShopApp.Core.Models;

namespace BarberShopApp.Components.Pages.Admin.AgendamentoPages
{
    public partial class Index
    {
        [Inject] 
        public IDbContextFactory<ApplicationDbContext> DbFactory { get; set; } = default!; // Use default! para garantir n�o nulo

        private List<Agendamento> Agendamentos { get; set; } = new();
        private List<Agendamento> AgendamentosFiltrados { get; set; } = new();
        private List<Servico> TodosServicos { get; set; } = new();

        // Filtros
        private DateTime? FiltroDataInicial { get; set; }
        private DateTime? FiltroDataFinal { get; set; }

        private string FiltroDataInicialString
        {
            get => FiltroDataInicial?.ToString("yyyy-MM-dd") ?? string.Empty;
        }

        private string FiltroDataFinalString
        {
            get => FiltroDataFinal?.ToString("yyyy-MM-dd") ?? string.Empty;
        }
        public string FiltroCliente
        {
            get => _FiltroCliente;
            set
            {
                if (_FiltroCliente != value)
                {
                    _FiltroCliente = value;
                }
            }
        }
        private string _FiltroCliente = string.Empty;


        // Setter para FiltroServicoId
        public int FiltroServicoId
        {
            get => _FiltroServicoId;
            set
            {
                if (_FiltroServicoId != value)
                {
                    _FiltroServicoId = value;
                }
            }
        }
        private int _FiltroServicoId = 0;


        protected override async Task OnInitializedAsync()
        {
            await CarregarDados();
            FiltrarAgendamentos();
        }

        private async Task CarregarDados()
        {
            using var context = DbFactory.CreateDbContext();

            Agendamentos = await context.Agendamento
                .Include(a => a.Servicos)
                .OrderBy(a => a.DataHora)
                .ToListAsync();

            TodosServicos = await context.Servico
                .OrderBy(s => s.Nome)
                .ToListAsync();
        }

        private void OnFiltroDataInicialChanged(ChangeEventArgs e)
        {
            var dateString = e.Value?.ToString();

            if (string.IsNullOrEmpty(dateString))
            {
                FiltroDataInicial = null;
            }
            else
            {
                if (DateTime.TryParseExact(dateString, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
                {
                    FiltroDataInicial = parsedDate;
                }
                else
                {
                    Console.WriteLine($"ERRO DE PARSE: N�o foi poss�vel converter '{dateString}' para data inicial.");
                    FiltroDataInicial = null;
                }
            }
            // REMOVIDO: FiltrarAgendamentos();
            // StateHasChanged(); // Pode ser �til se voc� quiser que o campo se "limpe" visualmente ao digitar algo inv�lido.
        }

        private void OnFiltroDataFinalChanged(ChangeEventArgs e)
        {
            var dateString = e.Value?.ToString();

            if (string.IsNullOrEmpty(dateString))
            {
                FiltroDataFinal = null;
            }
            else
            {
                if (DateTime.TryParseExact(dateString, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
                {
                    FiltroDataFinal = parsedDate;
                }
                else
                {
                    Console.WriteLine($"ERRO DE PARSE: N�o foi poss�vel converter '{dateString}' para data final.");
                    FiltroDataFinal = null;
                }
            }
            // REMOVIDO: FiltrarAgendamentos();
            // StateHasChanged(); // Pode ser �til se voc� quiser que o campo se "limpe" visualmente ao digitar algo inv�lido.
        }

        // Este m�todo agora � o �nico respons�vel por aplicar os filtros e atualizar a UI
        private void FiltrarAgendamentos()
        {
            var query = Agendamentos.AsQueryable();

            if (!string.IsNullOrWhiteSpace(FiltroCliente))
            {
                query = query.Where(a => a.NomeDoCliente.Contains(FiltroCliente, StringComparison.OrdinalIgnoreCase));
            }

            if (FiltroDataInicial.HasValue)
            {
                query = query.Where(a => a.DataHora.Date >= FiltroDataInicial.Value.Date);
            }

            if (FiltroDataFinal.HasValue)
            {
                query = query.Where(a => a.DataHora.Date <= FiltroDataFinal.Value.Date);
            }

            if (FiltroServicoId > 0)
            {
                query = query.Where(a => a.Servicos.Select(s => s.Id).Contains(FiltroServicoId));
            }

            AgendamentosFiltrados = query.ToList();
            StateHasChanged(); // ESSENCIAL: Garante que a UI � atualizada
        }

        private void LimparFiltros()
        {
            FiltroCliente = string.Empty;
            FiltroDataInicial = null;
            FiltroDataFinal = null;
            FiltroServicoId = 0;

            // Depois de limpar as vari�veis, chame o m�todo de filtragem para re-aplicar o filtro (agora vazio)
            // e atualizar a UI.
            FiltrarAgendamentos();
            // StateHasChanged() j� est� em FiltrarAgendamentos()
        }
    }
}
