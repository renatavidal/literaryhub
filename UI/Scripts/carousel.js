(function initCarouselWhenReady() {
    function init() {
        document.querySelectorAll('.carousel').forEach(function (carousel) {
            var track = carousel.querySelector('.carousel-track');
            if (!track) return;
            var slides = Array.prototype.slice.call(track.children);
            if (slides.length <= 1) return; // nada que rotar

            var prev = carousel.querySelector('.carousel-btn.prev');
            var next = carousel.querySelector('.carousel-btn.next');
            var dots = Array.prototype.slice.call(carousel.querySelectorAll('.carousel-dots button'));

            function indexFromScroll() {
                var i = Math.round(track.scrollLeft / track.clientWidth);
                return Math.max(0, Math.min(i, slides.length - 1));
            }
            function goTo(i) {
                track.scrollTo({ left: i * track.clientWidth, behavior: 'smooth' });
                dots.forEach(function (d) { d.classList.remove('active'); });
                if (dots[i]) dots[i].classList.add('active');
            }
            function nextIndex() {
                var i = indexFromScroll();
                return (i + 1) % slides.length;
            }

            // Controles
            if (prev) prev.addEventListener('click', function (e) {
                e.preventDefault(); e.stopPropagation();
                var i = Math.max(0, indexFromScroll() - 1);
                goTo(i);
                restartAutoplaySoon();
            });
            if (next) next.addEventListener('click', function (e) {
                e.preventDefault(); e.stopPropagation();
                goTo(nextIndex());
                restartAutoplaySoon();
            });
            dots.forEach(function (dot, i) {
                dot.addEventListener('click', function (e) {
                    e.preventDefault(); e.stopPropagation();
                    goTo(i);
                    restartAutoplaySoon();
                });
            });

            // Actualiza “dot” activo al hacer swipe/scroll
            var ticking = false;
            track.addEventListener('scroll', function () {
                if (!ticking) {
                    window.requestAnimationFrame(function () {
                        var i = indexFromScroll();
                        dots.forEach(function (d) { d.classList.remove('active'); });
                        if (dots[i]) dots[i].classList.add('active');
                        ticking = false;
                    });
                    ticking = true;
                }
            });

            // === Autoplay ===
            var reduceMotion = window.matchMedia && window.matchMedia('(prefers-reduced-motion: reduce)').matches;
            var wantsAutoplay = (carousel.dataset.autoplay || '').toLowerCase() === 'true';
            var intervalMs = parseInt(carousel.dataset.interval, 10);
            if (!intervalMs || isNaN(intervalMs)) intervalMs = 6000;

            var timer = null;
            function startAutoplay() {
                if (!wantsAutoplay || reduceMotion) return;
                stopAutoplay();
                timer = setInterval(function () { goTo(nextIndex()); }, intervalMs);
            }
            function stopAutoplay() {
                if (timer) { clearInterval(timer); timer = null; }
            }
            var restartTimeout = null;
            function restartAutoplaySoon() {
                // Pausa un ratito después de interacción manual y luego reanuda
                stopAutoplay();
                if (restartTimeout) clearTimeout(restartTimeout);
                restartTimeout = setTimeout(function () { startAutoplay(); }, 4000);
            }

            // Pausa en hover/focus; reanuda al salir
            carousel.addEventListener('mouseenter', stopAutoplay);
            carousel.addEventListener('mouseleave', startAutoplay);
            carousel.addEventListener('focusin', stopAutoplay);
            carousel.addEventListener('focusout', startAutoplay);

            // Pausa si la pestaña no está visible
            document.addEventListener('visibilitychange', function () {
                if (document.hidden) stopAutoplay(); else startAutoplay();
            });

            // Ajuste al redimensionar
            window.addEventListener('resize', function () { goTo(indexFromScroll()); });

            // Arranque inicial
            startAutoplay();
        });
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', init);
    } else {
        init();
    }
})();
