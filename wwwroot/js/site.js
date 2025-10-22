// Contact Manager App JavaScript
document.addEventListener('DOMContentLoaded', function() {
    
    // Auto-hide alerts after 5 seconds
    const alerts = document.querySelectorAll('.alert');
    alerts.forEach(alert => {
        setTimeout(() => {
            alert.style.opacity = '0';
            alert.style.transform = 'translateY(-20px)';
            setTimeout(() => alert.remove(), 300);
        }, 5000);
    });

    // Form submission with loading state
    const forms = document.querySelectorAll('form');
    forms.forEach(form => {
        form.addEventListener('submit', function(e) {
            const submitBtn = form.querySelector('button[type="submit"]');
            if (submitBtn) {
                submitBtn.classList.add('loading');
                submitBtn.disabled = true;
                submitBtn.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Saving...';
            }
        });
    });

    // Contact card hover effects
    const contactCards = document.querySelectorAll('.contact-card');
    contactCards.forEach(card => {
        card.addEventListener('mouseenter', function() {
            this.classList.add('success-animation');
        });
        
        card.addEventListener('mouseleave', function() {
            this.classList.remove('success-animation');
        });
    });

    // Search input enhancement
    const searchInput = document.querySelector('.search-input');
    if (searchInput) {
        searchInput.addEventListener('input', function() {
            const searchBtn = document.querySelector('.btn-search');
            if (this.value.trim()) {
                searchBtn.style.background = 'var(--success-color)';
                searchBtn.innerHTML = '<i class="fas fa-search"></i> Search';
            } else {
                searchBtn.style.background = 'var(--primary-color)';
                searchBtn.innerHTML = '<i class="fas fa-search"></i> Search';
            }
        });
    }

    // Form validation enhancement
    const formControls = document.querySelectorAll('.form-control');
    formControls.forEach(control => {
        control.addEventListener('blur', function() {
            validateField(this);
        });
        
        control.addEventListener('input', function() {
            if (this.classList.contains('is-invalid')) {
                validateField(this);
            }
        });
    });

    function validateField(field) {
        const value = field.value.trim();
        const fieldName = field.name;
        let isValid = true;
        let errorMessage = '';

        // Remove existing validation classes
        field.classList.remove('is-valid', 'is-invalid');
        const existingError = field.parentNode.querySelector('.text-danger');
        if (existingError) {
            existingError.remove();
        }

        // Validation rules
        switch (fieldName) {
            case 'Name':
                if (value.length < 2) {
                    isValid = false;
                    errorMessage = 'Name must be at least 2 characters long';
                }
                break;
            case 'Email':
                const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
                if (!emailRegex.test(value)) {
                    isValid = false;
                    errorMessage = 'Please enter a valid email address';
                }
                break;
            case 'PhoneNumber':
                const phoneRegex = /^\+?[1-9]\d{1,14}$/;
                if (!phoneRegex.test(value)) {
                    isValid = false;
                    errorMessage = 'Please enter a valid phone number';
                }
                break;
        }

        // Apply validation result
        if (isValid && value) {
            field.classList.add('is-valid');
        } else if (!isValid) {
            field.classList.add('is-invalid');
            showFieldError(field, errorMessage);
        }
    }

    function showFieldError(field, message) {
        const errorDiv = document.createElement('div');
        errorDiv.className = 'text-danger';
        errorDiv.textContent = message;
        field.parentNode.appendChild(errorDiv);
    }

    // Smooth scrolling for anchor links
    const anchorLinks = document.querySelectorAll('a[href^="#"]');
    anchorLinks.forEach(link => {
        link.addEventListener('click', function(e) {
            e.preventDefault();
            const target = document.querySelector(this.getAttribute('href'));
            if (target) {
                target.scrollIntoView({
                    behavior: 'smooth',
                    block: 'start'
                });
            }
        });
    });

    // Add success animation to newly created contacts
    const newContact = document.querySelector('.contact-card[data-new="true"]');
    if (newContact) {
        newContact.classList.add('success-animation');
        setTimeout(() => {
            newContact.classList.remove('success-animation');
            newContact.removeAttribute('data-new');
        }, 1000);
    }

    // Confirmation dialogs for delete actions
    const deleteButtons = document.querySelectorAll('a[href*="Delete"]');
    deleteButtons.forEach(button => {
        button.addEventListener('click', function(e) {
            if (!confirm('Are you sure you want to delete this contact? This action cannot be undone.')) {
                e.preventDefault();
            }
        });
    });

    // Keyboard shortcuts
    document.addEventListener('keydown', function(e) {
        // Ctrl/Cmd + K to focus search
        if ((e.ctrlKey || e.metaKey) && e.key === 'k') {
            e.preventDefault();
            const searchInput = document.querySelector('.search-input');
            if (searchInput) {
                searchInput.focus();
                searchInput.select();
            }
        }
        
        // Escape to clear search
        if (e.key === 'Escape') {
            const searchInput = document.querySelector('.search-input');
            if (searchInput && searchInput.value) {
                searchInput.value = '';
                searchInput.dispatchEvent(new Event('input'));
            }
        }
    });

    // Add tooltips to action buttons
    const actionButtons = document.querySelectorAll('.contact-actions .btn');
    actionButtons.forEach(button => {
        button.addEventListener('mouseenter', function() {
            this.style.transform = 'translateY(-3px) scale(1.1)';
        });
        
        button.addEventListener('mouseleave', function() {
            this.style.transform = 'translateY(0) scale(1)';
        });
    });

    console.log('Contact Manager App initialized successfully! 🚀');
});
