// Mobile Menu Functions
function toggleMobileMenu() {
    console.log('toggleMobileMenu function called');
    const mobileNav = document.getElementById('mobileNav');
    console.log('mobileNav element:', mobileNav);
    if (mobileNav) {
        const hasShow = mobileNav.classList.contains('show');
        console.log('Current show state:', hasShow);
        mobileNav.classList.toggle('show');
        console.log('Mobile menu toggled. New state:', mobileNav.classList.contains('show'));
    } else {
        console.error('mobileNav element not found');
    }
}

function closeMobileMenu() {
    console.log('closeMobileMenu function called');
    const mobileNav = document.getElementById('mobileNav');
    if (mobileNav) {
        mobileNav.classList.remove('show');
        console.log('Mobile menu closed');
    } else {
        console.error('mobileNav element not found in closeMobileMenu');
    }
}

// Close menu when clicking outside
document.addEventListener('click', function(event) {
    const mobileNav = document.getElementById('mobileNav');
    const mobileMenu = document.querySelector('.mobile-menu');
    
    if (mobileNav && mobileMenu) {
        if (!mobileMenu.contains(event.target) && !mobileNav.contains(event.target)) {
            mobileNav.classList.remove('show');
            console.log('Menu closed by clicking outside');
        }
    }
});

// Close menu on window resize (if switching to desktop)
window.addEventListener('resize', function() {
    if (window.innerWidth >= 641) {
        const mobileNav = document.getElementById('mobileNav');
        if (mobileNav) {
            mobileNav.classList.remove('show');
            console.log('Menu closed on resize');
        }
    }
});

// Debug: Log when script loads
console.log('mobileMenu.js loaded successfully'); 