// Mobile Menu Functions
window.toggleMobileMenu = () => {
    console.log('toggleMobileMenu called');
    
    // Tentar primeiro navbarNav (Landing Page)
    let targetElement = document.getElementById('navbarNav');
    let isBootstrap = true;
    
    // Se não encontrar, tentar mobileNav (Admin)
    if (!targetElement) {
        targetElement = document.getElementById('mobileNav');
        isBootstrap = false;
    }
    
    console.log('Target element found:', targetElement, 'isBootstrap:', isBootstrap);
    
    if (targetElement) {
        if (isBootstrap && typeof bootstrap !== 'undefined' && bootstrap.Collapse) {
            // Usar Bootstrap collapse para navbarNav
            try {
                console.log('Using Bootstrap collapse');
                const bsCollapse = new bootstrap.Collapse(targetElement, {
                    toggle: false
                });
                bsCollapse.toggle();
                return;
            } catch (e) {
                console.log('Bootstrap toggle failed, falling back to manual toggle', e);
            }
        }
        
        // Fallback manual para ambos os casos
        console.log('Using manual toggle');
        if (targetElement.classList.contains('show')) {
            targetElement.classList.remove('show');
            console.log('Removed show class');
        } else {
            targetElement.classList.add('show');
            console.log('Added show class');
        }
    } else {
        console.log('No target element found');
    }
};

function closeMobileMenu() {
    const mobileNav = document.getElementById('mobileNav');
    if (mobileNav && mobileNav.classList.contains('show')) {
        mobileNav.classList.remove('show');
    }
}

// Close menu when clicking outside
document.addEventListener('click', function(event) {
    const mobileNav = document.getElementById('mobileNav');
    const mobileMenu = document.querySelector('.mobile-menu');
    
    if (mobileNav && mobileMenu && mobileNav.classList.contains('show')) {
        if (!mobileMenu.contains(event.target) && !mobileNav.contains(event.target)) {
            mobileNav.classList.remove('show');
        }
    }
});

// Close menu on window resize (if switching to desktop)
window.addEventListener('resize', function() {
    if (window.innerWidth >= 641) {
        const mobileNav = document.getElementById('mobileNav');
        if (mobileNav && mobileNav.classList.contains('show')) {
            mobileNav.classList.remove('show');
        }
    }
});

// Prevent navigation flickering by adding smooth transitions
document.addEventListener('DOMContentLoaded', function() {
    // Add smooth transitions to all navigation links
    const navLinks = document.querySelectorAll('a[href]');
    navLinks.forEach(link => {
        link.addEventListener('click', function(e) {
            // Only apply to internal links
            if (this.href && this.href.startsWith(window.location.origin)) {
                // Add a small delay to allow for smooth transition
                setTimeout(() => {
                    // The navigation will happen naturally
                }, 50);
            }
        });
    });
}); 