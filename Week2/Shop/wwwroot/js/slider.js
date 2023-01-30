const swiper = new Swiper('.swiper', {
    // Optional parameters
    loop: false,
    freeMode: true,
    // Navigation arrows
    navigation: {
        nextEl: '.swiper-button-next',
        prevEl: '.swiper-button-prev',
    },
    spaceBetween: 10,
    watchOverflow: true,
    breakpoints: {
        0: {
            slidesPerView: 2
        },
        500: {
            slidesPerView: 3
        },
        768: {
            slidesPerView: 4
        }
    }
});