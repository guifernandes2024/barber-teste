using Microsoft.AspNetCore.Components;
using System.Globalization;
using teste.Models;
using Microsoft.EntityFrameworkCore;
using teste.Data;

namespace teste.Components.Pages.Admin.AgendamentoPages
{
    public partial class Index
    {
        [Inject] 
        public IDbContextFactory<ApplicationDbContext> DbFactory { get; set; } = default!; // Use default! para garantir não nulo

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
                .Include(a => a.Servico)
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
                    Console.WriteLine($"ERRO DE PARSE: Não foi possível converter '{dateString}' para data inicial.");
                    FiltroDataInicial = null;
                }
            }
            // REMOVIDO: FiltrarAgendamentos();
            // StateHasChanged(); // Pode ser útil se você quiser que o campo se "limpe" visualmente ao digitar algo inválido.
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
                    Console.WriteLine($"ERRO DE PARSE: Não foi possível converter '{dateString}' para data final.");
                    FiltroDataFinal = null;
                }
            }
            // REMOVIDO: FiltrarAgendamentos();
            // StateHasChanged(); // Pode ser útil se você quiser que o campo se "limpe" visualmente ao digitar algo inválido.
        }

        // Este método agora é o único responsável por aplicar os filtros e atualizar a UI
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
                query = query.Where(a => a.ServicoId == FiltroServicoId);
            }

            AgendamentosFiltrados = query.ToList();
            StateHasChanged(); // ESSENCIAL: Garante que a UI é atualizada
        }

        private void LimparFiltros()
        {
            FiltroCliente = string.Empty;
            FiltroDataInicial = null;
            FiltroDataFinal = null;
            FiltroServicoId = 0;

            // Depois de limpar as variáveis, chame o método de filtragem para re-aplicar o filtro (agora vazio)
            // e atualizar a UI.
            FiltrarAgendamentos();
            // StateHasChanged() já está em FiltrarAgendamentos()
        }
    }
}
