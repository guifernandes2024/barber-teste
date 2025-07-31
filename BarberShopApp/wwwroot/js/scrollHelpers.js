// Função para scroll suave para elementos
window.scrollToElement = function(elementId) {
    console.log('scrollToElement chamado com:', elementId);
    const element = document.getElementById(elementId);
    if (element) {
        console.log('Elemento encontrado, fazendo scroll');
        element.scrollIntoView({ 
            behavior: 'smooth', 
            block: 'start' 
        });
        return true;
    } else {
        console.log('Elemento não encontrado:', elementId);
        return false;
    }
};

// Função para scroll suave para o topo
window.scrollToTop = function() {
    window.scrollTo({ 
        top: 0, 
        behavior: 'smooth' 
    });
};

// Função de teste para verificar se o JavaScript está carregado
window.testJavaScript = function() {
    console.log('JavaScript carregado com sucesso!');
    return true;
};

// Função para verificar se todas as funções estão disponíveis
window.checkFunctions = function() {
    console.log('Verificando funções JavaScript...');
    console.log('scrollToElement:', typeof window.scrollToElement);
    console.log('scrollToTop:', typeof window.scrollToTop);
    console.log('testJavaScript:', typeof window.testJavaScript);
    return true;
};
