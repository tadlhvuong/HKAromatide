const productThumbnail = function (e) {
    let thumbnails = $('.product-single-thumbnails');
    let thumbs = $('.product-single-thumbs');
    if (thumbnails.length <= 0 || thumbs.length <= 0) return;
    $('.product-single__thumbs .image').show();
    $('.product-single-thumbnails .image').show();
    $('.product-single-thumbnails .grid__column').show();

    thumbnails.on('init', function (event, slick) {
        const slideActiveCurrent = document.querySelector('.product-single-thumbnails .slick-slide.slick-current.slick-active')
        if (slideActiveCurrent && slideActiveCurrent.classList.contains('product-video-item')) {
            slideActiveCurrent.muted = true;
            slideActiveCurrent.play();
        }
    })
    thumbnails.slick({
        asNavFor: '.product-single-thumbs',
        slidesToShow: 1,
        slidesToScroll: 1,
        arrows: true,
        fade: false,
        adaptiveHeight: true,
        infinite: false,
        responsive: [
            {
                breakpoint: 768,
                settings: {
                    slidesToShow: 1,
                    slidesToScroll: 1,
                    infinite: false,
                    arrows: false,
                    variableWidth: true
                }
            },
            {
                breakpoint: 991,
                settings: {
                    slidesToShow: 1,
                    slidesToScroll: 1,
                }
            },
        ]
    }).on('afterChange', function (event, slick, currentSlide) {
        const slideActiveCurrent = event.target.querySelector('.slick-slide.slick-current.slick-active')
        if (slideActiveCurrent && slideActiveCurrent.classList.contains('product-video-item')) {
            slideActiveCurrent.play()
        } else {
            const parent = event.target.closest('.product-single__thumbnails')
            if (parent) {
                const videoItem = parent.querySelector('video.product-video-item')
                if (videoItem) {
                    videoItem.pause()
                }
            }
        }
    });

    let args_thumbs = {
        slidesToShow: 5,
        slidesToScroll: 1,
        asNavFor: '.product-single-thumbnails',
        dots: false,
        arrows: false,
        vertical: true,
        adaptiveHeight: true,
        focusOnSelect: true,
        infinite: false,
    }

    if (thumbs.children().length <= 5) {
        args_thumbs = {
            ...args_thumbs,
            slidesToScroll: 5,
        }
    }

    thumbs.slick(args_thumbs);
}
function changeQuantity() {
    $(document).on('click', '.product-quantity-change .plus', function (e) {
        e.preventDefault();

        const input = $(this).siblings('input[type="number"]');
        const max = input.attr('max');
        let oldValue = parseFloat(input.val());

        if (oldValue >= max) {
            var newVal = max;
        } else {
            var newVal = oldValue + 1;
        }
        1
        input.val(newVal);
        input.trigger("change");

    });

    $(document).on('click', '.product-quantity-change .minus', function (e) {
        e.preventDefault();
        const input = $(this).siblings('input[type="number"]');
        const min = input.attr('min');
        let oldValue = parseFloat(input.val());

        if (oldValue <= min) {
            var newVal = min;
        } else {
            var newVal = oldValue - 1;
        }

        input.val(newVal);
        input.trigger("change");
    });
}

function infomationLoadMore() {
    const infomation = $('.infomations__descriptions');
    const contentHeight = infomation.height();

    if (contentHeight < 10) {
        infomation.addClass('is-active');
        $('.infomations-loadmore').hide();
    }
    $(".infomations-collapse").hide();

    $(document).on('click', '.infomations-loadmore', function (e) {
        e.preventDefault();
        infomation.addClass('is-active');
        $(this).hide();

        if ($('.infomations__descriptions').hasClass('is-active')) {
            $(".infomations-collapse").show();
            $(".infomations-collapse").on('click', function (e) {
                e.preventDefault();
                $(this).siblings('.infomations__descriptions').removeClass('is-active');
                $(this).hide();
                let siteHeaderHeight = $('.site-header').innerHeight();
                siteHeaderHeight = siteHeaderHeight + 15;
                $('html,body').animate({ scrollTop: $('#thong-tin-san-pham').offset().top - siteHeaderHeight }, 'slow');
                $(".infomations-loadmore").show();
            });
        }

    })
}
function addToCart() {
    $(".btn-addtocart").click(function (e) {
        e.preventDefault();
        let html = $(this).html();
        let url = '',
            button_loading = '<span class="loading-data loading-data--white"></span>';

        //if ($(this).hasClass('btn-addtocart--cart')) {
        //    url = '/Product/AddToCart';
        //    button_loading = '<span class="loading-data"></span>';
        //}

        const id = $("input[name='id']").val();
        const name = $(".product-single__main .heading").text();
        const currentprice = $(".product-single__main .product-single__prices")[0].dataset.currentprice;
        const regularprice = $(".product-single__main .product-single__prices")[0].dataset.regularprice;
        const quantity = $("input[name='quantity']").val();
        var varianElement = $('.options .option input:checked');
        const idVariantChild = (varianElement.length > 0) ? varianElement.val() : "";
        const nameVariantParent = (varianElement.length > 0) ? varianElement[0].dataset.nameparent : "";
        const nameVariantChild = (varianElement.length > 0) ? varianElement[0].dataset.namechild : "";
        let urlImage = "/media/" + document.querySelector('.product-single-thumbnails .slick-slide.slick-current.slick-active img').alt;
        
        if (!id) return;
        const cart = {
            product_id: id,
            product_name: name,
            product_image: urlImage,
            product_quantity: parseInt(quantity),
            product_pricecurrent: parseInt(currentprice),
            product_priceregular: parseInt(regularprice),
            product_variant: idVariantChild,
            nameParent: nameVariantParent,
            nameChild: nameVariantChild,
            urlReturn: "/san-pham/chi-tiet-san-pham?id="+ id,
        };
        var carts = [];
        (sessionStorage.getItem("Cart") == null) ? carts = [] : carts = JSON.parse(sessionStorage.getItem("Cart"));
        if (carts.length > 0 || carts != "[]") {
            var isSame = false;
            carts.forEach(e => {
                if (e.product_id == cart.product_id && e.product_image == cart.product_image && e.product_variant == cart.product_variant) {
                    e.product_quantity += cart.product_quantity;
                    isSame = true;
                }
            })
            if (!isSame) carts.push(cart);
        } else {
            carts.push(cart);
        }
        sessionStorage.setItem("Cart", JSON.stringify(carts));
        var myStorage = sessionStorage.getItem("Cart")
        var count = 0;
        var arrItem = (myStorage != null) ? JSON.parse(myStorage) : [];
        if (arrItem.length > 0) {
            arrItem.forEach(e => {
                count += e.product_quantity;
            })
        }
        updateCountCart();
        //// ===== START:: Notify Custom
        var data = '<div class="notifyCustom"><div class="notifyCustom_bg" onClick="javascript:closeNotifyCustom();"></div><div class="notifyCustom_box"><div class="notifyCustom_box_close" onClick="javascript:closeNotifyCustom();"><i class="fas fa-times"></i></div><div class="notifyCustom_box_header">Đã thêm <span>' + cart.product_quantity + '</span> sản phẩm vào giỏ hàng!</div><div class="notifyCustom_box_img"><img src="' + cart.product_image + '" alt="img-cart" title="img-cart"></div><div class="notifyCustom_box_content"><h3 class="maxLine_2">' + cart.product_name + '</h3><div class="notifyCustom_box_content__options">' + cart.nameParent + ': ' + cart.nameChild + '</div><div class="notifyCustom_box_content__price">' + formatCurrency(cart.product_pricecurrent) + '</div> </div><div class="notifyCustom_box_btn"><a href="/san-pham/thanh-toan">Thanh toán</a> </div></div></div>'
        
        setTimeout(() => {
            let widthS = $(window).width();
            let heightS = $(window).height();
            if (widthS < 567) {
                $('#notifyCustom').html(data).css({
                    'opacity': '1',
                    'z-index': '200',
                    'display': 'block'
                });
                $('.notifyCustom_box').css({
                    'width': 'calc(100% - 20px)',
                    'top': 'calc(50% - ' + parseInt($('.notifyCustom_box').outerHeight() / 2) + 'px)',
                    'left': 'calc(50% - ' + parseInt($('.notifyCustom_box').outerWidth() / 2) + 'px)'
                });
            } else {
                $('#notifyCustom').html(data).css({
                    'width': '400px',
                    'opacity': '1',
                    'z-index': '200',
                    'display': 'block'
                });
            }
        }, 1);
        setTimeout(() => {
            closeNotifyCustom();
        }, 4000);
        // ===== END:: Notify Custom
    })
}
function closeNotifyCustom() {
    $('#notifyCustom').css({
        'width': '0',
        'opacity': '0',
        'z-index': '-1',
        'display': 'none'
    });
    $('.notifyCustom_box').css({
        'width': '0'
    });
}

const updateImageByVariant = function (variant) {
    const { image } = variant;
    if (!image || image == '') return;
    const index = thumbnails.find('.image[data-id="' + image + '"]').attr('data-slick-index');
    thumbnails.slick('slickGoTo', index);
}

function optionChange() {
    const option = $('.product-single--options .option input');
    if (option.length < 0) return;

    option.on('change', function (e) {
        updateVariantSelect();
    })
}
function updateVariantSelect() {
    const variant = getVariantSelect();
    updateBuyNow(variant);
    updateSubTitleByVariant(variant);

    if (!variant) return;

    updateImageByVariant(variant);
    updateLimitQuantity(variant);
    updatePricesByVariant(variant);

    $('input[name="id"]').val(variant.id);
}

jQuery(document).ready(function (e) {
    productThumbnail();
    changeQuantity();
    infomationLoadMore();
    addToCart();
    //updateVariantSelect();
    //optionChange();
});