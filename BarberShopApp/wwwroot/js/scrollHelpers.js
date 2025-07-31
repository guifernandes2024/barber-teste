window.scrollToElement = function(elementId) {
    const element = document.getElementById(elementId);
    if (element) {
        element.scrollIntoView({ 
            behavior: 'smooth', 
            block: 'start' 
        });
        return true;
    } else {
        return false;
    }
};

window.scrollToTop = function() {
    window.scrollTo({ 
        top: 0, 
        behavior: 'smooth' 
    });
};
