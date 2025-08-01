// Dashboard Charts
window.renderizarGraficos = function (dadosAgendamentosPorMes, dadosServicosPopulares, dadosHorariosProcurados) {
    // Cores para os gráficos
    const cores = [
        '#007bff', '#28a745', '#ffc107', '#dc3545', '#6f42c1',
        '#fd7e14', '#20c997', '#e83e8c', '#6c757d', '#17a2b8'
    ];

    // Gráfico de Agendamentos por Mês (Linha)
    const ctxAgendamentosPorMes = document.getElementById('agendamentosPorMesChart');
    if (ctxAgendamentosPorMes) {
        new Chart(ctxAgendamentosPorMes, {
            type: 'line',
            data: {
                labels: dadosAgendamentosPorMes.map(d => d.label),
                datasets: [{
                    label: 'Agendamentos',
                    data: dadosAgendamentosPorMes.map(d => d.valor),
                    borderColor: '#007bff',
                    backgroundColor: 'rgba(0, 123, 255, 0.1)',
                    borderWidth: 2,
                    fill: true,
                    tension: 0.4
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        display: false
                    }
                },
                scales: {
                    y: {
                        beginAtZero: true,
                        ticks: {
                            stepSize: 1
                        }
                    }
                }
            }
        });
    }

    // Gráfico de Serviços Mais Populares (Pizza)
    const ctxServicosPopulares = document.getElementById('servicosPopularesChart');
    if (ctxServicosPopulares) {
        new Chart(ctxServicosPopulares, {
            type: 'doughnut',
            data: {
                labels: dadosServicosPopulares.map(d => d.label),
                datasets: [{
                    data: dadosServicosPopulares.map(d => d.valor),
                    backgroundColor: cores.slice(0, dadosServicosPopulares.length),
                    borderWidth: 2,
                    borderColor: '#fff'
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        position: 'bottom',
                        labels: {
                            padding: 20,
                            usePointStyle: true
                        }
                    }
                }
            }
        });
    }

    // Gráfico de Horários Mais Procurados (Barras)
    const ctxHorariosProcurados = document.getElementById('horariosProcuradosChart');
    if (ctxHorariosProcurados) {
        new Chart(ctxHorariosProcurados, {
            type: 'bar',
            data: {
                labels: dadosHorariosProcurados.map(d => d.label),
                datasets: [{
                    label: 'Agendamentos',
                    data: dadosHorariosProcurados.map(d => d.valor),
                    backgroundColor: '#28a745',
                    borderColor: '#28a745',
                    borderWidth: 1
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        display: false
                    }
                },
                scales: {
                    y: {
                        beginAtZero: true,
                        ticks: {
                            stepSize: 1
                        }
                    }
                }
            }
        });
    }
};

// Função para atualizar dados em tempo real (opcional)
window.atualizarDashboard = function () {
    // Aqui você pode implementar atualizações em tempo real
    // Por exemplo, usando SignalR ou polling
    console.log('Dashboard atualizado');
};

// Inicializar tooltips do Bootstrap
document.addEventListener('DOMContentLoaded', function() {
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });
}); 