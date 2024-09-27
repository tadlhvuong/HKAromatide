function homeSlider() {
    const _homeSlider = $('.home-banner--slide');
    if (_homeSlider.length <= 0) return;
    var firstSlideHeight = 0;
    _homeSlider.on('init', function (event, slick) {
        if (window.innerWidth > 1024) {

            firstSlideHeight = slick.$slides.eq(0).find('div a').height();
            $('.home-banner--slide .slick-list .slick-track').height(firstSlideHeight);
            slick.$slides.eq(0).find('div video').height(firstSlideHeight);
        } else {
            if (slick.$slides.eq(0).find('div video').length === 0) {
                var firstImageHeight = $('.home-banner--slide .banner-slide img').first().height();
                $('.home-banner--slide .slick-list .slick-track').height(firstImageHeight);
            }
        }
    });

    _homeSlider.slick({
        dots: true,
        arrows: true,
        autoplay: true,
        infinite: true,
        autoplaySpeed: 1500000,
        lazyLoad: 'ondemand'
    });

    _homeSlider.on('beforeChange', function (event, slick, currentSlide, nextSlide) {
        var newHeight = slick.$slides.eq(nextSlide).find('div').height();
        $('.home-banner--slide .slick-list .slick-track').height(window.innerWidth > 1024 ? (nextSlide !== 0 ? newHeight : firstSlideHeight) : newHeight);
    });
}
const homeBannerSecondSlide = function () {
    const _slide = $('.bannerSecond--slide');
    if (_slide.length <= 0) return;
    _slide.slick({
        infinite: false,
        slidesToShow: 1,
        slidesToScroll: 1,
        dots: true,
        arrows: false,
        responsive: [
            {
                breakpoint: 768,
                settings: {
                    slidesToShow: 1,
                    slidesToScroll: 1,
                    infinite: false,
                    arrows: false,
                    dots: true
                }
            }
        ]
    });
}

const homeProductSlider = function () {
    const _slide = $('.home-product--slider');
    if (_slide.length <= 0) return;
    _slide.slick({
        infinite: false,
        slidesToShow: 3.97,
        slidesToScroll: 4.015,
        dots: false,
        arrows: true,
        responsive: [
            {
                breakpoint: 577,
                settings: {
                    slidesToShow: 2.2,
                    slidesToScroll: 1,
                    nfinite: false,
                    arrows: false
                }
            },
            {
                breakpoint: 767,
                settings: {
                    slidesToShow: 2.6,
                    slidesToScroll: 2,
                    nfinite: false,
                    arrows: false
                }
            },
            {
                breakpoint: 991,
                settings: {
                    slidesToShow: 3.6,
                    slidesToScroll: 3.6,
                }
            }
        ]
    });
}

const homeAboutSlide = function () {
    const _slider = $('.home-abouts--slide');
    if (_slider.length <= 0) return;
    _slider.slick({
        infinite: false,
        slidesToShow: 2,
        slidesToScroll: 2,
        dots: true,
        arrows: false,
        appendDots: $('#abouts-dots'),
        responsive: [
            {
                breakpoint: 767,
                settings: {
                    slidesToShow: 1,
                    slidesToScroll: 2
                }
            }
        ]
    });
}

const homeResources = function () {
    const _slider = $('.home-resources--slide');
    if (_slider.length <= 0) return;
    _slider.slick({
        infinite: false,
        slidesToShow: 4,
        slidesToScroll: 1,
        variableWidth: true,
        dots: false,
        prevArrow: $('#resources-slide-prev'),
        nextArrow: $('#resources-slide-next'),
        responsive: [
            {
                breakpoint: 769,
                settings: {
                    slidesToShow: 1,
                }
            },
            {
                breakpoint: 1300,
                settings: {
                    slidesToShow: 2,
                }
            },
            {
                breakpoint: 1850,
                settings: {
                    slidesToShow: 3,
                }
            },
        ]
    });
}
$(function () {
    homeSlider();
    homeBannerSecondSlide();
    homeProductSlider();
    homeAboutSlide();
    homeResources();
});