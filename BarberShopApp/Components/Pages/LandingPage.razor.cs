using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using BarberShopApp.Core.Models;
using BarberShopApp.Core.Services;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Identity;
using BarberShopApp.Data;

namespace BarberShopApp.Components.Pages
{
    public partial class LandingPage
    {
        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;
        [Inject] private ConfiguracaoBarbeariaService ConfiguracaoService { get; set; } = default!;
        [Inject] private UserManager<ApplicationUser> UserManager { get; set; } = default!;
        [Inject] private SignInManager<ApplicationUser> SignInManager { get; set; } = default!;
        [Inject] private AgendamentoTempService AgendamentoTempService { get; set; } = default!;
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;
        
        private List<Servico> Servicos { get; set; } = new();
        private List<Servico> ServicosSelecionados { get; set; } = new();
        private List<Profissional> Profissionais { get; set; } = new();
        private Profissional? ProfissionalSelecionado { get; set; }
        private Agendamento Agendamento { get; set; } = new();
        private DateTime? HorarioSelecionado { get; set; }
        private DateTime? DataSelecionada { get; set; } = DateTime.Today;
        private List<DateTime> HorariosDisponiveis { get; set; } = new();
        private bool isLoading { get; set; } = false;
        private string mensagemFeedback { get; set; } = string.Empty;
        private string tipoMensagem { get; set; } = string.Empty;
        private ConfiguracaoBarbearia? configuracao { get; set; }

        // User authentication properties
        private bool isUserLoggedIn { get; set; } = false;
        private ApplicationUser? currentUser { get; set; }

        // Multi-step properties
        private int CurrentStep { get; set; } = 1;
        private int TotalSteps { get; set; } = 4;
        private string GetProfissionalNome(Profissional profissional)
        {
            // Buscar o nome através do ApplicationUser associado
            using var context = DbFactory.CreateDbContext();
            var user = context.Users.FirstOrDefault(u => u.ProfissionalId == profissional.Id);

            var nome = user?.Nome ?? "Profissional";

            return nome;
        }
        protected override async Task OnInitializedAsync()
        {
            try
            {
                // Inicialização explícita das listas
                Servicos = new List<Servico>();
                ServicosSelecionados = new List<Servico>();
                Profissionais = new List<Profissional>();
                Agendamento = new Agendamento();
                
                // Verificar se o usuário está logado
                await VerificarUsuario();
                
                // Carregar configurações da barbearia
                configuracao = await ConfiguracaoService.ObterOuCriarConfiguracaoAsync();
                
                // Carregar dados do banco
                await CarregarServicos();
                await CarregarProfissionais();
                
                // Restaurar agendamento temporário se existir
                await RestaurarAgendamentoTemp();
                
                // Forçar atualização da UI
                StateHasChanged();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro na inicialização: {ex.Message}");
                await ExibirMensagem($"Erro ao carregar dados: {ex.Message}", "error");
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                try
                {
                    await JSRuntime.InvokeVoidAsync("reinitializePhoneMasks");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao inicializar JavaScript: {ex.Message}");
                }
            }
        }

        public async Task CarregarServicos()
        {
            try
            {
                Console.WriteLine("Carregando serviços...");
                using var context = DbFactory.CreateDbContext();
                
                Servicos = await context.Servico.ToListAsync();

                
                StateHasChanged();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao carregar serviços: {ex.Message}");
                await ExibirMensagem($"Erro ao carregar serviços: {ex.Message}", "error");
            }
        }

        public async Task InicializarDados()
        {
            try
            {
                Console.WriteLine("Inicializando dados no banco...");
                using var context = DbFactory.CreateDbContext();
                
                // Verificar se o banco existe
                if (!await context.Database.CanConnectAsync())
                {
                    Console.WriteLine("Banco de dados não encontrado. Criando...");
                    await context.Database.EnsureCreatedAsync();
                }
                
                // Verificar se já existem dados
                var servicosExistentes = await context.Servico.CountAsync();
                if (servicosExistentes > 0)
                {
                    await ExibirMensagem("Dados já foram inicializados anteriormente.", "warning");
                    await CarregarServicos();
                    return;
                }
                
                // Criar serviços
                var servicos = new List<Servico>
                {
                    new Servico { Nome = "Corte Masculino", Descricao = "Corte tradicional masculino", Preco = 25.00m, DuracaoEmMinutos = 30, ImgUrl = "https://images.unsplash.com/photo-1503951914875-452162b0f3f1?ixlib=rb-4.0.3&auto=format&fit=crop&w=400&q=80" },
                    new Servico { Nome = "Barba", Descricao = "Acabamento de barba", Preco = 15.00m, DuracaoEmMinutos = 20, ImgUrl = "https://images.unsplash.com/photo-1503951914875-452162b0f3f1?ixlib=rb-4.0.3&auto=format&fit=crop&w=400&q=80" },
                    new Servico { Nome = "Corte + Barba", Descricao = "Corte completo com barba", Preco = 35.00m, DuracaoEmMinutos = 45, ImgUrl = "https://images.unsplash.com/photo-1503951914875-452162b0f3f1?ixlib=rb-4.0.3&auto=format&fit=crop&w=400&q=80" },
                    new Servico { Nome = "Hidratação", Descricao = "Hidratação capilar", Preco = 40.00m, DuracaoEmMinutos = 60, ImgUrl = "https://images.unsplash.com/photo-1503951914875-452162b0f3f1?ixlib=rb-4.0.3&auto=format&fit=crop&w=400&q=80" }
                };

                context.Servico.AddRange(servicos);
                await context.SaveChangesAsync();
                
                await ExibirMensagem($"Dados inicializados com sucesso! Criados {servicos.Count} serviços.", "success");
                await CarregarServicos();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao inicializar dados: {ex.Message}");
                await ExibirMensagem($"Erro ao inicializar dados: {ex.Message}", "error");
            }
        }

        private async Task CarregarProfissionais()
        {
            try
            {
                using var context = DbFactory.CreateDbContext();
                Profissionais = await context.Profissional
                    .Include(p => p.Especialidades)
                    .ToListAsync();
                
                // Se não há profissionais, criar dados de exemplo
                if (!Profissionais.Any())
                {
                    Profissionais = new List<Profissional>
                    {
                        new Profissional 
                        { 
                            ImgUrl = "https://images.unsplash.com/photo-1503951914875-452162b0f3f1?ixlib=rb-4.0.3&auto=format&fit=crop&w=400&q=80",
                            Especialidades = Servicos.Take(2).ToList()
                        },
                        new Profissional 
                        { 
                            ImgUrl = "https://images.unsplash.com/photo-1503951914875-452162b0f3f1?ixlib=rb-4.0.3&auto=format&fit=crop&w=400&q=80",
                            Especialidades = Servicos.Skip(1).Take(2).ToList()
                        }
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao carregar profissionais: {ex.Message}");
                await ExibirMensagem($"Erro ao carregar profissionais: {ex.Message}", "error");
            }
        }

        private void OnServicosSelecionadosChanged(List<Servico> servicos)
        {
            ServicosSelecionados = servicos;
            // Limpar seleções dependentes quando serviços mudam
            ProfissionalSelecionado = null;
            HorarioSelecionado = null;
            StateHasChanged();
        }

        private void OnProfissionalSelecionadoChanged(Profissional? profissional)
        {
            ProfissionalSelecionado = profissional;
            // Limpar horário quando profissional muda
            HorarioSelecionado = null;
            StateHasChanged();
        }

        private void OnHorarioSelecionadoChanged(DateTime horario)
        {
            HorarioSelecionado = horario;
            Agendamento.DataHora = horario;
            Agendamento.ProfissionalId = ProfissionalSelecionado?.Id ?? 0;
            StateHasChanged();
        }

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
            try
            {
                // Se não há data selecionada, usar hoje
                if (!DataSelecionada.HasValue)
                {
                    DataSelecionada = DateTime.Today;
                }

                // Gerar todos os horários de 30 em 30 min das 8:00 às 18:00
                var horariosBase = new List<DateTime>();
                var horaInicio = DataSelecionada.Value.Date.AddHours(8); // 8:00
                var horaFim = DataSelecionada.Value.Date.AddHours(18); // 18:00
                
                for (var hora = horaInicio; hora < horaFim; hora = hora.AddMinutes(30))
                {
                    horariosBase.Add(hora);
                }
                
                // Filtrar horários passados se for hoje
                if (DataSelecionada?.Date == DateTime.Today)
                {
                    var agora = DateTime.Now;
                    horariosBase = horariosBase.Where(h => h > agora).ToList();
                }

                // Se há profissional selecionado, verificar disponibilidade
                if (ProfissionalSelecionado != null)
                {
                    try
                    {
                        using var context = DbFactory.CreateDbContext();
                        var horarioService = new HorarioService(DbFactory, ConfiguracaoService);
                        
                        // Para cada horário base, verificar se está disponível para o profissional
                        var horariosDisponiveis = new List<DateTime>();
                        
                        foreach (var horario in horariosBase)
                        {
                            // Se há serviços selecionados, usar a duração total
                            int duracaoTotal = ServicosSelecionados.Any() 
                                ? ServicosSelecionados.Sum(s => s.DuracaoEmMinutos) 
                                : 30; // Duração padrão de 30 min se não há serviços
                            
                            // Verificar disponibilidade para este horário específico
                            bool disponivel = await horarioService.VerificarDisponibilidadeAsync(
                                horario, 
                                ProfissionalSelecionado.Id, 
                                duracaoTotal);
                            
                            if (disponivel)
                            {
                                horariosDisponiveis.Add(horario);
                            }
                        }
                        
                        HorariosDisponiveis = horariosDisponiveis;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao verificar disponibilidade: {ex.Message}");
                        // Em caso de erro, usar horários base
                        HorariosDisponiveis = horariosBase;
                    }
                }
                else
                {
                    // Se não há profissional selecionado, usar horários base
                    HorariosDisponiveis = horariosBase;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao carregar horários: {ex.Message}");
                HorariosDisponiveis = new List<DateTime>();
            }
            
            StateHasChanged();
        }

        private void SelecionarHorario(DateTime horario)
        {
            HorarioSelecionado = horario;
            Agendamento.DataHora = horario;
            Agendamento.ProfissionalId = ProfissionalSelecionado?.Id ?? 0;
            Console.WriteLine($"Horário selecionado: {horario}");
            StateHasChanged();
        }

        private static string CleanPhoneNumber(string phoneNumber)
        {
            return new string(phoneNumber.Where(char.IsDigit).ToArray());
        }

        // Multi-step navigation methods
        private void PreviousStep()
        {
            if (CurrentStep > 1)
            {
                CurrentStep--;
                StateHasChanged();
            }
        }

        private async Task NextStep()
        {
            if (CurrentStep < TotalSteps && CanProceedToNextStep)
            {
                // Verificar se está tentando ir para o passo 4 sem estar logado
                if (CurrentStep == 3 && !isUserLoggedIn)
                {
                    // Salvar estado do agendamento e redirecionar para login
                    await SalvarAgendamentoTemp();
                    await ExibirMensagem("Para continuar, você precisa estar logado. Redirecionando para login...", "info");
                    await Task.Delay(2000); // Aguardar 2 segundos para mostrar a mensagem
                    NavigationManager.NavigateTo("/cliente/login?returnUrl=/");
                    return;
                }

                CurrentStep++;
                
                // Se está indo para o Step 3 (horários), carregar horários automaticamente
                if (CurrentStep == 3)
                {
                    await CarregarHorarios();
                }
                
                StateHasChanged();
            }
        }

                private bool CanProceedToNextStep
        {
            get
            {
                return CurrentStep switch
                {
                    1 => ServicosSelecionados.Any(),
                    2 => ProfissionalSelecionado != null,
                    3 => HorarioSelecionado.HasValue,
                    4 => isUserLoggedIn || (!string.IsNullOrWhiteSpace(Agendamento.NomeDoCliente) &&
                         !string.IsNullOrWhiteSpace(Agendamento.NumeroDoCliente)),
                    _ => false
                };
            }
        }

        private async Task VerificarUsuario()
        {
            try
            {
                currentUser = await UserManager.GetUserAsync(SignInManager.Context.User);
                isUserLoggedIn = currentUser != null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao verificar usuário: {ex.Message}");
                isUserLoggedIn = false;
            }
        }

        private async Task SalvarAgendamentoTemp()
        {
            try
            {
                var agendamentoTemp = new AgendamentoTemp
                {
                    ServicosIds = ServicosSelecionados.Select(s => s.Id).ToList(),
                    ProfissionalId = ProfissionalSelecionado?.Id,
                    DataHora = HorarioSelecionado,
                    Observacoes = Agendamento.Observacoes
                };
                
                AgendamentoTempService.SalvarAgendamentoTemp(agendamentoTemp);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao salvar agendamento temporário: {ex.Message}");
            }
        }

        private async Task RestaurarAgendamentoTemp()
        {
            try
            {
                var agendamentoTemp = AgendamentoTempService.ObterAgendamentoTemp();
                if (agendamentoTemp != null && isUserLoggedIn)
                {
                    // Restaurar serviços selecionados
                    ServicosSelecionados = Servicos.Where(s => agendamentoTemp.ServicosIds.Contains(s.Id)).ToList();
                    
                    // Restaurar profissional selecionado
                    if (agendamentoTemp.ProfissionalId.HasValue)
                    {
                        ProfissionalSelecionado = Profissionais.FirstOrDefault(p => p.Id == agendamentoTemp.ProfissionalId.Value);
                    }
                    
                    // Restaurar horário selecionado
                    if (agendamentoTemp.DataHora.HasValue)
                    {
                        HorarioSelecionado = agendamentoTemp.DataHora.Value;
                        DataSelecionada = agendamentoTemp.DataHora.Value.Date;
                    }
                    
                    // Restaurar observações
                    Agendamento.Observacoes = agendamentoTemp.Observacoes ?? string.Empty;
                    
                    // Ir para o passo 4 (dados do cliente)
                    CurrentStep = 4;
                    
                    // Limpar agendamento temporário
                    AgendamentoTempService.LimparAgendamentoTemp();
                    
                    await ExibirMensagem("Seu agendamento foi restaurado! Complete os dados para finalizar.", "success");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao restaurar agendamento temporário: {ex.Message}");
            }
        }

        private async Task AddAgendamento()
        {
            if (!HorarioSelecionado.HasValue || ProfissionalSelecionado == null || !ServicosSelecionados.Any())
            {
                await ExibirMensagem("Por favor, complete todos os passos do agendamento.", "error");
                return;
            }

            if (!isUserLoggedIn || currentUser == null)
            {
                await ExibirMensagem("Você precisa estar logado para fazer um agendamento.", "error");
                return;
            }

            isLoading = true;
            StateHasChanged();

            try
            {
                using var context = DbFactory.CreateDbContext();

                // Limpa o número de telefone antes de salvar no banco de dados
                Agendamento.NumeroDoCliente = CleanPhoneNumber(Agendamento.NumeroDoCliente);

                // Configura o agendamento
                Agendamento.DataHora = HorarioSelecionado.Value;
                Agendamento.ProfissionalId = ProfissionalSelecionado.Id;
                Agendamento.ClienteId = currentUser.Id; // Vincular ao cliente logado
                
                // Preencher dados do cliente automaticamente se estiver logado
                if (isUserLoggedIn && currentUser != null)
                {
                    Agendamento.NomeDoCliente = currentUser.Nome;
                    Agendamento.NumeroDoCliente = currentUser.PhoneNumber ?? "";
                }

                // Busca os serviços existentes no banco de dados
                var servicosIds = ServicosSelecionados.Select(s => s.Id).ToList();
                var servicosExistentes = await context.Servico.Where(s => servicosIds.Contains(s.Id)).ToListAsync();
                Agendamento.Servicos = servicosExistentes;

                // Adiciona o agendamento ao contexto
                context.Agendamento.Add(Agendamento);
                await context.SaveChangesAsync();

                await ExibirMensagem("Agendamento realizado com sucesso! Você pode visualizar seus agendamentos na sua área do cliente.", "success");
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
            ServicosSelecionados.Clear();
            ProfissionalSelecionado = null;
            HorarioSelecionado = null;
            DataSelecionada = DateTime.Today;
            HorariosDisponiveis.Clear();
            CurrentStep = 1;
            mensagemFeedback = string.Empty;
            tipoMensagem = string.Empty;
            StateHasChanged();
        }

        private async Task ExibirMensagem(string mensagem, string tipo)
        {
            mensagemFeedback = mensagem;
            tipoMensagem = tipo;
            StateHasChanged();

            // Limpa a mensagem após 5 segundos
            await Task.Delay(5000);
            if (mensagemFeedback == mensagem)
            {
                mensagemFeedback = string.Empty;
                tipoMensagem = string.Empty;
                StateHasChanged();
            }
        }

        private async Task ScrollToAgendamento()
        {
            try
            {
                await JSRuntime.InvokeVoidAsync("scrollToElement", "agendamento");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao fazer scroll: {ex.Message}");
            }
        }

        private async Task ScrollToServicos()
        {
            try
            {
                await JSRuntime.InvokeVoidAsync("scrollToElement", "servicos");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao fazer scroll: {ex.Message}");
            }
        }

        public async Task ForcarCarregamentoDados()
        {
            try
            {
                Console.WriteLine("Forçando carregamento de dados...");
                
                using var context = DbFactory.CreateDbContext();
                
                // Verificar se o banco existe
                if (!await context.Database.CanConnectAsync())
                {
                    await context.Database.EnsureCreatedAsync();
                }
                
                // Se não há serviços no banco, criar dados de exemplo
                var servicosExistentes = await context.Servico.ToListAsync();
                if (!servicosExistentes.Any())
                {
                    var novosServicos = new List<Servico>
                    {
                        new Servico { Nome = "Corte Masculino", Descricao = "Corte tradicional masculino", Preco = 25.00m, DuracaoEmMinutos = 30, ImgUrl = "https://images.unsplash.com/photo-1503951914875-452162b0f3f1?ixlib=rb-4.0.3&auto=format&fit=crop&w=400&q=80" },
                        new Servico { Nome = "Barba", Descricao = "Acabamento de barba", Preco = 15.00m, DuracaoEmMinutos = 20, ImgUrl = "https://images.unsplash.com/photo-1503951914875-452162b0f3f1?ixlib=rb-4.0.3&auto=format&fit=crop&w=400&q=80" },
                        new Servico { Nome = "Corte + Barba", Descricao = "Corte completo com barba", Preco = 35.00m, DuracaoEmMinutos = 45, ImgUrl = "https://images.unsplash.com/photo-1503951914875-452162b0f3f1?ixlib=rb-4.0.3&auto=format&fit=crop&w=400&q=80" },
                        new Servico { Nome = "Hidratação", Descricao = "Hidratação capilar", Preco = 40.00m, DuracaoEmMinutos = 60, ImgUrl = "https://images.unsplash.com/photo-1503951914875-452162b0f3f1?ixlib=rb-4.0.3&auto=format&fit=crop&w=400&q=80" }
                    };
                    
                    context.Servico.AddRange(novosServicos);
                    await context.SaveChangesAsync();
                }
                
                // Recarregar serviços do banco
                Servicos = await context.Servico.ToListAsync();
                
                // Se não há profissionais no banco, criar dados de exemplo
                var profissionaisExistentes = await context.Profissional.ToListAsync();
                if (!profissionaisExistentes.Any())
                {
                    var novosProfissionais = new List<Profissional>
                    {
                        new Profissional 
                        { 
                            ImgUrl = "https://images.unsplash.com/photo-1503951914875-452162b0f3f1?ixlib=rb-4.0.3&auto=format&fit=crop&w=400&q=80",
                            Especialidades = Servicos.Take(2).ToList()
                        },
                        new Profissional 
                        { 
                            ImgUrl = "https://images.unsplash.com/photo-1503951914875-452162b0f3f1?ixlib=rb-4.0.3&auto=format&fit=crop&w=400&q=80",
                            Especialidades = Servicos.Skip(1).Take(2).ToList()
                        }
                    };
                    
                    context.Profissional.AddRange(novosProfissionais);
                    await context.SaveChangesAsync();
                }
                
                // Recarregar profissionais do banco
                Profissionais = await context.Profissional.Include(p => p.Especialidades).ToListAsync();
                
                StateHasChanged();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao forçar carregamento: {ex.Message}");
            }
        }

        private void ToggleServico(Servico servico)
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

            StateHasChanged();
        }

        private void SelecionarProfissional(Profissional profissional)
        {
            ProfissionalSelecionado = profissional;
            // Limpar horário quando profissional muda
            HorarioSelecionado = null;
            StateHasChanged();
        }

        private void HandleImageError(EventArgs e, int id, string type)
        {
            var randomImages = new Dictionary<string, string[]>
            {
                ["service"] = new[]
                {
                    "https://images.unsplash.com/photo-1503951914875-452162b0f3f1?ixlib=rb-4.0.3&auto=format&fit=crop&w=500&q=80",
                    "https://images.unsplash.com/photo-1585747860715-2ba37e788b70?ixlib=rb-4.0.3&auto=format&fit=crop&w=500&q=80",
                    "https://images.unsplash.com/photo-1622287162716-f311baa1a2b8?ixlib=rb-4.0.3&auto=format&fit=crop&w=500&q=80",
                    "https://images.unsplash.com/photo-1621605815971-fbc98d665033?ixlib=rb-4.0.3&auto=format&fit=crop&w=500&q=80"
                },
                ["professional"] = new[]
                {
                    "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?ixlib=rb-4.0.3&auto=format&fit=crop&w=500&q=80",
                    "https://images.unsplash.com/photo-1500648767791-00dcc994a43e?ixlib=rb-4.0.3&auto=format&fit=crop&w=500&q=80",
                    "https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?ixlib=rb-4.0.3&auto=format&fit=crop&w=500&q=80",
                    "https://images.unsplash.com/photo-1506794778202-cad84cf45f1d?ixlib=rb-4.0.3&auto=format&fit=crop&w=500&q=80"
                }
            };

            if (randomImages.ContainsKey(type))
            {
                var random = new Random();
                var randomImage = randomImages[type][random.Next(randomImages[type].Length)];
                
                // Atualizar a URL da imagem no objeto correspondente
                if (type == "service")
                {
                    var servico = Servicos.FirstOrDefault(s => s.Id == id);
                    if (servico != null)
                    {
                        servico.ImgUrl = randomImage;
                        StateHasChanged();
                    }
                }
                else if (type == "professional")
                {
                    var profissional = Profissionais.FirstOrDefault(p => p.Id == id);
                    if (profissional != null)
                    {
                        profissional.ImgUrl = randomImage;
                        StateHasChanged();
                    }
                }
            }
        }


    }
}
