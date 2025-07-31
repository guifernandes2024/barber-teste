// Função simples para scroll
function scrollToSection(elementId) {
    console.log('Scroll para:', elementId);
    const element = document.getElementById(elementId);
    if (element) {
        console.log('Elemento encontrado, fazendo scroll');
        element.scrollIntoView({ behavior: 'smooth', block: 'start' });
    } else {
        console.log('Elemento não encontrado:', elementId);
    }
}

// Função principal para scroll (mantida para compatibilidade)
window.scrollToElementById = (elementId) => {
    console.log('Tentando scroll para:', elementId);
    const el = document.getElementById(elementId);
    if (el) {
        console.log('Elemento encontrado, fazendo scroll');
        el.scrollIntoView({ behavior: 'smooth', block: 'start' });
        return true;
    } else {
        console.log('Elemento não encontrado:', elementId);
        return false;
    }
};

// Função alternativa usando scroll nativo
window.scrollToElementNative = (elementId) => {
    const el = document.getElementById(elementId);
    if (el) {
        el.scrollIntoView({ behavior: 'smooth', block: 'start' });
        return true;
    }
    return false;
};

// Função de teste para verificar se o JavaScript está carregado
window.testJavaScript = () => {
    console.log('JavaScript carregado com sucesso!');
    return true;
}; 