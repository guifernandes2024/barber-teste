// Função para aplicar máscara de telefone no formato (99) 9999-9999
function applyPhoneMask(input) {
    // Remove todos os caracteres não numéricos
    let value = input.value.replace(/\D/g, '');
    
    // Aplica a máscara baseada no número de dígitos
    if (value.length <= 2) {
        input.value = value;
    } else if (value.length <= 6) {
        input.value = `(${value.substring(0, 2)}) ${value.substring(2)}`;
    } else if (value.length <= 10) {
        input.value = `(${value.substring(0, 2)}) ${value.substring(2, 6)}-${value.substring(6)}`;
    } else {
        // Para números com 11 dígitos (celular com 9)
        input.value = `(${value.substring(0, 2)}) ${value.substring(2, 7)}-${value.substring(7, 11)}`;
    }
}

// Função para limpar apenas números de uma string
function cleanPhoneNumber(phoneNumber) {
    return phoneNumber.replace(/\D/g, '');
}

// Função para validar se o telefone tem o formato correto
function validatePhoneNumber(phoneNumber) {
    const cleanNumber = cleanPhoneNumber(phoneNumber);
    return cleanNumber.length >= 10 && cleanNumber.length <= 11;
}

// Função para inicializar máscaras em todos os inputs de telefone
function initializePhoneMasks() {
    const phoneInputs = document.querySelectorAll('input[type="text"][id*="telefone"], input[type="text"][id*="phone"], input[type="text"][id*="celular"], input[type="text"][id*="whatsapp"], input[type="text"][id*="numero"]');
    
    phoneInputs.forEach(input => {
        // Verifica se já foi inicializado para evitar duplicação
        if (input.dataset.maskInitialized === 'true') {
            return;
        }
        
        // Marca como inicializado
        input.dataset.maskInitialized = 'true';
        
        // Adiciona placeholder se não existir
        if (!input.placeholder) {
            input.placeholder = "(11) 99999-9999";
        }
        
        // Adiciona maxlength se não existir
        if (!input.maxLength) {
            input.maxLength = 15;
        }
        
        // Adiciona evento de input
        input.addEventListener('input', function() {
            applyPhoneMask(this);
        });
        
        // Adiciona evento de blur para validação
        input.addEventListener('blur', function() {
            if (this.value && !validatePhoneNumber(this.value)) {
                this.classList.add('is-invalid');
            } else {
                this.classList.remove('is-invalid');
            }
        });
        
        // Adiciona evento de focus para remover classe de erro
        input.addEventListener('focus', function() {
            this.classList.remove('is-invalid');
        });
    });
}

// Inicializa as máscaras quando o DOM estiver pronto
document.addEventListener('DOMContentLoaded', function() {
    initializePhoneMasks();
});

// Função para ser chamada após renderização dinâmica de componentes Blazor
function reinitializePhoneMasks() {
    // Aguarda um pouco para garantir que o DOM foi atualizado
    setTimeout(function() {
        initializePhoneMasks();
    }, 100);
} 