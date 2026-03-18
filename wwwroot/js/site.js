/* =========================================================
   site.js — The Weeb Den 2.0
   Micro-interactions, notifications, scroll effects, and
   progressive-enhancement utilities.
   ========================================================= */

;(function () {
  'use strict'

  /* ------ Toast Notifications ------ */
  const TOAST_DURATION = 3500
  const TOAST_ANIM_MS = 360

  function showNotification(message, type) {
    type = type || 'success'

    var existing = document.querySelector('.wd-toast')
    if (existing) existing.remove()

    var icon =
      type === 'success'
        ? 'fa-check-circle'
        : type === 'error'
        ? 'fa-circle-xmark'
        : 'fa-circle-info'

    var colorVar =
      type === 'success'
        ? 'var(--color-success, #059669)'
        : type === 'error'
        ? 'var(--color-danger, #dc2626)'
        : 'var(--color-warning, #d97706)'

    var toast = document.createElement('div')
    toast.className = 'wd-toast'
    toast.setAttribute('role', 'alert')
    toast.setAttribute('aria-live', 'polite')
    toast.innerHTML =
      '<div class="wd-toast__inner" style="--toast-accent:' +
      colorVar +
      '">' +
      '<i class="fas ' +
      icon +
      ' wd-toast__icon"></i>' +
      '<span class="wd-toast__msg">' +
      escapeHtml(message) +
      '</span>' +
      '<button class="wd-toast__close" aria-label="Dismiss">&times;</button>' +
      '</div>'

    document.body.appendChild(toast)

    // Trigger entrance
    requestAnimationFrame(function () {
      toast.classList.add('wd-toast--visible')
    })

    var dismiss = function () {
      toast.classList.remove('wd-toast--visible')
      toast.classList.add('wd-toast--exit')
      setTimeout(function () {
        toast.remove()
      }, TOAST_ANIM_MS)
    }

    toast.querySelector('.wd-toast__close').addEventListener('click', dismiss)
    setTimeout(dismiss, TOAST_DURATION)
  }

  // Expose globally for Razor inline scripts
  window.showNotification = showNotification

  /* ------ HTML escape ------ */
  function escapeHtml(str) {
    var div = document.createElement('div')
    div.appendChild(document.createTextNode(str))
    return div.innerHTML
  }

  /* ------ Utility functions ------ */
  window.formatPrice = function (price) {
    return (
      '\u20B1' +
      price.toLocaleString('en-PH', {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2,
      })
    )
  }

  window.generateStars = function (rating) {
    var out = ''
    var full = Math.floor(rating)
    var half = rating % 1 >= 0.5

    for (var i = 0; i < full; i++)
      out += '<i class="fas fa-star text-warning"></i>'
    if (half) {
      out += '<i class="fas fa-star-half-alt text-warning"></i>'
      full++
    }
    for (var j = full; j < 5; j++)
      out += '<i class="far fa-star text-warning"></i>'
    return out
  }

  /* ------ Scroll-to-top ------ */
  function initScrollToTop() {
    var btn = document.createElement('button')
    btn.className = 'wd-scroll-top'
    btn.setAttribute('aria-label', 'Scroll to top')
    btn.innerHTML = '<i class="fas fa-chevron-up"></i>'
    document.body.appendChild(btn)

    var ticking = false
    window.addEventListener(
      'scroll',
      function () {
        if (!ticking) {
          requestAnimationFrame(function () {
            btn.classList.toggle('wd-scroll-top--show', window.scrollY > 400)
            ticking = false
          })
          ticking = true
        }
      },
      { passive: true }
    )

    btn.addEventListener('click', function () {
      window.scrollTo({ top: 0, behavior: 'smooth' })
    })
  }

  /* ------ Navbar shrink on scroll ------ */
  function initNavShrink() {
    var nav = document.querySelector('.navbar')
    if (!nav) return

    var ticking = false
    window.addEventListener(
      'scroll',
      function () {
        if (!ticking) {
          requestAnimationFrame(function () {
            nav.classList.toggle('navbar--scrolled', window.scrollY > 60)
            ticking = false
          })
          ticking = true
        }
      },
      { passive: true }
    )
  }

  /* ------ Fade-in on scroll (IntersectionObserver) ------ */
  function initScrollReveal() {
    if (!('IntersectionObserver' in window)) return

    var targets = document.querySelectorAll(
      '.card, .feature-card, .hero-stats .stat-item, .about-story, .about-card, .contact-info-card, .cart-item, .checkout-section, section > .container > .row'
    )

    var observer = new IntersectionObserver(
      function (entries) {
        entries.forEach(function (entry) {
          if (entry.isIntersecting) {
            entry.target.classList.add('wd-revealed')
            observer.unobserve(entry.target)
          }
        })
      },
      { threshold: 0.08, rootMargin: '0px 0px -40px 0px' }
    )

    targets.forEach(function (el) {
      el.classList.add('wd-reveal')
      observer.observe(el)
    })
  }

  /* ------ Active nav link highlighting ------ */
  function initActiveNav() {
    var path = window.location.pathname.toLowerCase()
    var links = document.querySelectorAll('.navbar-nav .nav-link')
    links.forEach(function (link) {
      var href = (link.getAttribute('href') || '').toLowerCase()
      if (href && href !== '/' && path.startsWith(href)) {
        link.classList.add('active')
      } else if (href === '/' && path === '/') {
        link.classList.add('active')
      }
    })
  }

  /* ------ Quantity stepper animation ------ */
  function initQuantitySteppers() {
    document.addEventListener('click', function (e) {
      var btn = e.target.closest('.qty-btn, .btn-qty')
      if (!btn) return
      btn.style.transform = 'scale(0.9)'
      setTimeout(function () {
        btn.style.transform = ''
      }, 150)
    })
  }

  /* ------ Image error fallback ------ */
  function initImageFallbacks() {
    document.querySelectorAll('img[src]').forEach(function (img) {
      img.addEventListener('error', function () {
        if (!this.dataset.fallback) {
          this.dataset.fallback = '1'
          this.style.opacity = '0.4'
          this.alt = 'Image unavailable'
        }
      })
    })
  }

  /* ------ TempData handler ------ */
  function handleTempData() {
    var el = document.getElementById('tempdata-message')
    if (!el) return
    var msg = el.getAttribute('data-message')
    var type = el.getAttribute('data-type') || 'success'
    if (msg) showNotification(msg, type)
  }

  /* ------ Init ------ */
  document.addEventListener('DOMContentLoaded', function () {
    initScrollToTop()
    initNavShrink()
    initScrollReveal()
    initActiveNav()
    initQuantitySteppers()
    initImageFallbacks()
    handleTempData()
  })
})()
