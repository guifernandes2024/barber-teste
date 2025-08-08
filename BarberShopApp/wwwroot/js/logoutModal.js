// Modal de Logout
window.showLogoutModal = () => {
    // Criar o modal HTML
    const modalHtml = `
        <div class="modal fade" id="logoutModal" tabindex="-1" aria-labelledby="logoutModalLabel" aria-hidden="true">
            <div class="modal-dialog modal-dialog-centered">
                <div class="modal-content">
                    <div class="modal-header bg-warning text-dark">
                        <h5 class="modal-title" id="logoutModalLabel">
                            <i class="fas fa-sign-out-alt me-2"></i>Sair da Conta
                        </h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <div class="modal-body text-center">
                        <div class="mb-4">
                            <i class="fas fa-question-circle fa-3x text-warning mb-3"></i>
                            <h4>Tem certeza que deseja sair?</h4>
                            <p class="text-muted">Você será desconectado da sua conta</p>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">
                            <i class="fas fa-times me-2"></i>Cancelar
                        </button>
                        <button type="button" class="btn btn-warning" id="confirmLogoutBtn">
                            <i class="fas fa-sign-out-alt me-2"></i>Sim, sair
                        </button>
                    </div>
                </div>
            </div>
        </div>
    `;

    // Remover modal existente se houver
    const existingModal = document.getElementById('logoutModal');
    if (existingModal) {
        existingModal.remove();
    }

    // Adicionar o modal ao body
    document.body.insertAdjacentHTML('beforeend', modalHtml);

    // Configurar o evento de confirmação
    const confirmBtn = document.getElementById('confirmLogoutBtn');
    if (confirmBtn) {
        confirmBtn.addEventListener('click', () => {
            // Fechar o modal
            const modal = bootstrap.Modal.getInstance(document.getElementById('logoutModal'));
            if (modal) {
                modal.hide();
            }
            
            // Fazer o logout via POST
            performLogout();
        });
    }

    // Mostrar o modal
    const modalElement = document.getElementById('logoutModal');
    if (modalElement && typeof bootstrap !== 'undefined') {
        const modal = new bootstrap.Modal(modalElement);
        modal.show();
        
        // Limpar o modal do DOM quando for fechado
        modalElement.addEventListener('hidden.bs.modal', () => {
            modalElement.remove();
        });
    }
};

// Função para executar o logout
async function performLogout() {
    try {
        // Fazer logout via AJAX simples
        const response = await fetch('/api/logout', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            credentials: 'same-origin'
        });
        
        // Sempre recarregar a página para atualizar o estado
        window.location.reload();
    } catch (error) {
        console.error('Erro ao fazer logout:', error);
        // Em caso de erro, recarregar a página
        window.location.reload();
    }
}
