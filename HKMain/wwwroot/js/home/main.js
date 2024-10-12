

function scrollHeaderMenu(idD) {
    let valS = parseInt($('#' + idD).scrollLeft() + 150);
    $('#' + idD).animate({
        scrollLeft: valS
    }, 150);
}

function showHideSearchBox(idE, actionE = 'show') {
    if (actionE == 'show') {
        $('#' + idE).css({
            "opacity": "1",
            "width": "100%",
            "z-index": "999999"
        }).unbind();

        $('.search-result-mb__input').focus();

    } else if (actionE == 'hide') {
        $('#' + idE).css({
            "opacity": "0",
            "width": "0",
            "z-index": "-1"
        }).unbind();
    }
}
$(window).scroll(function () {
    showHideSearchBox('searchBox-mobile', 'hide');
});
$(window).scroll(function () {
    const siteHeader = document.querySelector('.site-header');
    if (siteHeader) {
        const siteHeaderTopSale = siteHeader.querySelector('.site-header__top-sale')
        if (siteHeaderTopSale) {
            if (siteHeader.classList.contains('is-sticky')) {
                siteHeaderTopSale.style.display = 'none';
            } else {
                siteHeaderTopSale.style.display = 'block';
            }
        }
    }
});

function showHideListMenuMobile(thisD) {
    let elemtC = $(thisD).parent().find('ul');
    let displayC = elemtC.css('display');
    if (displayC == 'none') {
        elemtC.css('display', 'block');
        $(thisD).html('<i class="fas fa-chevron-down"></i>');
    } else {
        elemtC.css('display', 'none');
        $(thisD).html('<i class="fas fa-chevron-right"></i>');
    }
}

function openCloseElemt(idElemt, target) {
    const navMb = $('#nav-mobile')
    if (target.classList.contains('icon-close-mb') || target.classList.contains('nav-mobile_bg')) {
        if (navMb.hasClass('d-block')) {
            toggleIconHambuger()
            $('body').css('overflow', 'unset');
            navMb.removeClass('d-block')
        }

        return
    }

    let displayE = $('#' + idElemt).css('display');
    if (displayE == 'none') {
        $('#' + idElemt).addClass('d-block');
        $('body').css('overflow', 'hidden');
    } else {
        $('#' + idElemt).removeClass('d-block');
        $('body').css('overflow', 'unset');
    }
    toggleIconHambuger()
}

//function loadMegaMenuProduct(thisD) {
//    var idD = $(thisD).data('menu');
//    console.log(idD);
//    var idWrite = $(thisD).data('write');
//    console.log(idWrite);
//    // bật selected
//    var parentD = $(thisD).parent();
//    parentD.children().each(function () {
//        $(this).removeClass('selected');
//    });
//    $(thisD).addClass('selected');
//    if ($('#' + idD).length) {
//        // === trường hợp đã load rồi => không load nữa
//        // ẩn tất cả
//        $('#' + idWrite).children().each(function () {
//            $(this).css('display', 'none');
//        });
//        // hiển thị lại 1
//        $('#' + idD).css({
//            'display': 'flex',
//            'height': '100%',
//            'flex-direction': 'column',
//            'justify-content': 'flex-end'
//        });
//    } else {
//        // bật loading
//        $('#' + idWrite + ' .loading-megaMenu').css('display', 'flex');
//        // cố định height
//        let heightWrite = $('#' + idWrite).height();
//        $('#' + idWrite).css('height', heightWrite);
//        // trường hợp chưa load => bắt đầu tải
//        var url = '/collection/customProducts/' + idD;
//        $.ajax({
//            url: url,
//            type: 'GET'
//        }).done(function (dataJson) {
//            setTimeout(function () {
//                // ẩn tất cả
//                $('#' + idWrite).children().each(function () {
//                    $(this).css('display', 'none');
//                });
//                // append thông tin vào
//                $('#' + idWrite).append('<div id="' + idD +
//                    '" style="display:flex;height:100%;flex-direction:column;justify-content:flex-end;">' +
//                    dataJson.data + '</div>');
//            }, 1);
//        });
//    }
//}

function menuActionEnd(menu) {
    $(document).on('click', function (e) {
        let target = $(e.target);
        if (!target.hasClass('open-popup')) return;
        e.preventDefault();

        if ($(menu).hasClass('is-active')) {
            const dataMenuActive = $(menu).attr('data-menu-active');
            $(menu).removeClass('is-active');
            $('[data-menu-id="' + dataMenuActive + '"]').removeClass('is-active');
            theme.openPopup('remove');
        }
    })
}

function menuToggle(e) {
    const menuListing = '.mobile-menu--listing',
        menuActions = '.header-actions--menu';

    theme.menuToggle(menuActions, function (e) {
        const menu = $(menuListing),
            dataMenuActive = menu.attr('data-menu-active');
        theme.openPopup('add');
        menu.removeClass('is-active');
        const menuActive = $('[data-menu-id="' + dataMenuActive + '"]');
        menuActive.removeClass('is-active');
        $('.mobile-menu').removeClass('is-active');
    }, menuActionEnd(menuActions));

    theme.menuToggle(menuListing);

    $(document).on('mousedown touchstart', function (e) {
        let _target = $(e.target);
        if (_target.parents('[data-menu-id]').length <= 0 &&
            !_target.attr('data-menu-active') &&
            !_target.attr('data-menu-id') &&
            _target.parents('[data-menu-active]').length <= 0) {
            $('[data-menu-id]').removeClass('is-active');
            setTimeout(function (e) {
                $('.mobile-menu').removeClass('is-active');
                theme.openPopup('remove');
            }, 100)
        }
    })

    $(document).on('click', '.menu-close', function (e) {
        $('[data-menu-id]').removeClass('is-active');
        $('[data-menu-active]').removeClass('is-active');
        $('.mobile-menu').removeClass('is-active');
        theme.openPopup('remove');
    })
}

function handleSearchSpotlight(element, value, callback) {
    let url = settings.APP_URL + '/search';
    $.ajax({
        url: url,
        type: "get",
        data: {
            keyword: value,
            ajax: true
        },
        dataType: 'json'
    }).done(function (res) {
        const {
            html,
            success
        } = res;
        if (success && html) {
            element.html(html);
            let keywords = localStorage.getItem("search_keywords");
            if (!keywords) keywords = [];
            else keywords = JSON.parse(keywords);

            if (value) {
                keywords.unshift(value);
                if (keywords.length > 5) keywords.pop();
                localStorage.setItem("search_keywords", JSON.stringify(keywords));
            } else searchHistory();

            if (typeof callback === 'function') {
                callback();
            }
        }
    })
}

function searchSpotlight() {
    let timeOut;

    $(document).on('keyup focus', '#spotlight-search', function (e) {
        const _this = $(this),
            headerSpotlightSearch = $('#header-spotlight-search'),
            element = headerSpotlightSearch.find('.sportlight-search'),
            value = _this.val();
        headerSpotlightSearch.addClass('is-active');
        if (timeOut) {
            clearTimeout(timeOut);
        }
        if (value.length !== 0) {
            if (!element.hasClass('is-search')) {
                element.addClass('is-search');
                element.html(`<div class="loading-data"></div>`);
            }

            timeOut = setTimeout(function (e) {
                handleSearchSpotlight(element, value, function () {
                    element.removeClass('is-search');
                });
            }, 500);
        } else {
            element.removeClass("is-searching");
            element.empty();
            $('<div class="spotlight__content spotlight__searched" id="spotLightSearchHistory"></div>')
                .appendTo(element);
            searchHistory();
        }
    });

    $(window).scroll(function () {
        const headerSpotlightSearch = $('#header-spotlight-search');
        if (headerSpotlightSearch.hasClass('is-active')) {
            headerSpotlightSearch.removeClass('is-active');
        }
    })

    $(document).on('click', function (e) {
        const _this = $(this);
        const headerSpotlightSearch = $('#header-spotlight-search');
        if (_this.closest('#header-spotlight-search').length <= 0 && headerSpotlightSearch.hasClass(
            'is-active') && $(e.target).attr('id') !== 'spotlight-search') {
            headerSpotlightSearch.removeClass('is-active');
        }
    })
}

function formatCurrency(price) {
    return price.toLocaleString('vi', { style: 'currency', currency: 'VND' })
}

function showMiniCart() {
    let timeOut;

    $("#headerMiniCart").mouseenter(function (e) {
        $("#miniCart").show();
        var miniCart = sessionStorage.getItem("Cart");
        if (miniCart == null || miniCart == "[]") {
            let urlBuy = "/san-pham";
            $("#miniCart").html('<div class="cart-list"><p>Giỏ hàng của bạn đang trống</p> \n <a href="' + urlBuy +'"> Mua thêm </a></div>');
        }
        else {
            var arrItem = JSON.parse(miniCart);
            var header = "<div class=\"cart-list\">\n",
                footer = "\n<div class=\"cart-list__remove\">\n <a onclick='removeAllCart();'>\n <svg width=\"12\" height=\"12\" fill=\"none\" xmlns=\"http:\/\/www.w3.org\/2000\/svg\">\n                <path d=\"M10.666 3.083v7.584A1.166 1.166 0 019.5 11.833h-7a1.167 1.167 0 01-1.167-1.166V3.083H.167V1.917h11.666v1.166h-1.167zm-8.166 0v7.584h7V3.083h-7zM5.417 4.25h1.166v1.167H5.416V4.25zm0 1.75h1.166v1.167H5.416V6zm0 1.75h1.166v1.167H5.416V7.75zM3.083.167h5.833v1.166H3.083V.167z\" fill=\"currentColor\" \/>\n<\/svg>\nXóa tất cả\n <\/a>\n<\/div>\n<\/div>",
                body = ""
            arrItem.forEach(function (e, i) {
                let urlProduct = "/san-pham/chi-tiet-san-pham?id=" + e.product_id;
                let textPriceCurrent = formatCurrency(e.product_pricecurrent);
                let textPriceRegular = formatCurrency(e.product_priceregular);
                body += "<div class='cart-item'>" +
                    "<div class='thumbnail'><img src='" + e.product_image + "' alt='Ảnh đại diện'></div>" +
                    "<div class='content''>" +
                    " <div class='top'>" +
                    "<div class='info'><span class='title'><a href='" + urlProduct + "'" + "> " + e.product_name + "\tx" + e.product_quantity + " </a></span></div>" +
                    "<div id='cart-remove-item' style='cursor: pointer'><a onclick='removeItemCart(" + i
                    + ");'> <i class='fas fa-trash'></i></a>  </div> </div>" +
                    "<span>" + e.nameParent + ":</span> <span>" + e.nameChild + "</span>" +
                    "<div class='bottom'><div class='prices'><span><ins>" + textPriceCurrent + "</ins></span> <span><del>" + textPriceRegular + "</del></span></div></div></div></div>";
                
            })
            var html = header + body + footer;
             
            $("#miniCart").html(html);
        }
    });
}
function updateCountCart() {
    var myStorage = sessionStorage.getItem("Cart")
    var count = 0;
    var arrItem = (myStorage != null) ? JSON.parse(myStorage) : [];
    if (arrItem.length > 0) {
        arrItem.forEach(e => {
            count += e.product_quantity;
        })
        $(".header-custom_item__icon").addClass("icon-cart")
    } else {
        $(".header-custom_item__icon").removeClass("icon-cart")
    }
    $("#headerMiniCart .header-cart__count")[0].innerText = count; //pc
    
    return count;
}

function updatePriceCart() {
    var myStorage = sessionStorage.getItem("Cart")
    var price = 0;
    var totalPrice = 0;
    var arrItem = (myStorage != null) ? JSON.parse(myStorage) : [];
    if (arrItem.length > 0) {
        arrItem.forEach(e => {
            price += e.product_pricecurrent;
            totalPrice = price + 0;// phi ship
        })
        $("#cart .cart__price")[0].innerText = formatCurrency(price);
        $("#cart .cart__total")[0].innerText = formatCurrency(totalPrice); 
    }
}

function loadCartPayment() {
    var cart = sessionStorage.getItem("Cart");

    if (cart == null || cart == "[]") {
        $("#payment").css("display", "none");
        $("#buynow").css("display", "block");
    }
    else {
        $("#payment").css("display", "flex");
        $("#buynow").css("display", "none");
        $("#Items").val(cart);
        var arrItem = JSON.parse(cart);
        var footer = "\n<div class=\"cart-list__remove\">\n <a onclick='removeAllCart();'>\n <svg width=\"12\" height=\"12\" fill=\"none\" xmlns=\"http:\/\/www.w3.org\/2000\/svg\">\n                <path d=\"M10.666 3.083v7.584A1.166 1.166 0 019.5 11.833h-7a1.167 1.167 0 01-1.167-1.166V3.083H.167V1.917h11.666v1.166h-1.167zm-8.166 0v7.584h7V3.083h-7zM5.417 4.25h1.166v1.167H5.416V4.25zm0 1.75h1.166v1.167H5.416V6zm0 1.75h1.166v1.167H5.416V7.75zM3.083.167h5.833v1.166H3.083V.167z\" fill=\"currentColor\" \/>\n<\/svg>\nXóa tất cả\n <\/a>\n<\/div>",
            body = "";
        arrItem.forEach(function (e, i) {
            let urlProduct = "/san-pham/chi-tiet-san-pham?id=" + e.product_id;
            let textPriceCurrent = formatCurrency(e.product_pricecurrent);
            let textPriceRegular = formatCurrency(e.product_priceregular);
            body += "<div class='cart-item'>" +
                "<div class='thumbnail'><img src='" + e.product_image + "' alt='Ảnh đại diện'></div>" +
                "<div class='content''>" +
                " <div class='top'>" +
                "<div class='info'><span class='title'><a href='" + urlProduct + "'" + "> " + e.product_name + "\tx" + e.product_quantity + " </a></span></div>" +
                "<div id='cart-remove-item' style='cursor: pointer'><a onclick='removeItemCart(" + i
                + ");'> <i class='fas fa-trash'></i></a>  </div> </div>" +
                "<span>" + e.nameParent + ":</span> <span>" + e.nameChild + "</span>" +
                "<div class='bottom'><div class='prices'><span><ins>" + textPriceCurrent + "</ins></span> <span><del>" + textPriceRegular + "</del></span></div></div></div></div>";
        })
        
        var html = body + footer;
        $(".cart-list").html(html);
    }
}

function removeItemCart(index) {
    var myStorage = sessionStorage.getItem("Cart")
    var arrItem = (myStorage != null) ? JSON.parse(myStorage) : [];
    if (arrItem.length > 0)
        arrItem.splice(index, 1)
    sessionStorage.setItem("Cart", JSON.stringify(arrItem));
    updateCountCart();
    updatePriceCart();
    loadCartPayment();
    $("#miniCart").hide();
}

function removeAllCart() {
    var myStorage = sessionStorage.getItem("Cart")
    if (myStorage == null) return;
    sessionStorage.clear();
    $("#miniCart").html('<div class="cart-list">Giỏ hàng của bạn đang trống</div>');
    updateCountCart();
    loadCartPayment();
    return;
}
function mobileSearchActive() {
    if (!theme.isMobile()) return;
    let firstScrollPoint = 0;

    $(document).on('click', '.header-actions--search', function (e) {
        e.preventDefault();
        const headerSearch = $('.header-search');
        headerSearch.addClass('is-active');
        setTimeout(function (e) {
            $("#spotlight-search").trigger('touchstart');
        }, 100);
        firstScrollPoint = $(window).scrollTop();
    })

    $(window).scroll(function () {
        const headerSpotlightSearch = $('.header-search');
        let scroll = $(window).scrollTop();
        if (headerSpotlightSearch.hasClass('is-active') && ((firstScrollPoint + 200) < scroll || (
            firstScrollPoint - 200) > scroll)) {
            headerSpotlightSearch.removeClass('is-active');
        }
    })
}

function searchHistory() {
    let keywords = localStorage.getItem("search_keywords");
    if (keywords) keywords = JSON.parse(keywords);
    let wrapper = $("#spotLightSearchHistory");
    wrapper.empty();
    if (keywords === null) {
        $('#header-spotlight-search').removeClass('is-active');
        return;
    }
    keywords.forEach(function (item, key) {
        let itemEl = $('<a href="' + encodeURI("/search?keyword=" + item) + '" class="sportlight-search__item">' +
            '<div class="thumbnail"><svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 511.999 511.999">' +
            '<path d="M225.773.616C101.283.616 0 101.622 0 225.773S101.284 450.93 225.773 450.93s225.774-101.006 225.774-225.157S350.263.616 225.773.616zm0 413.301c-104.084 0-188.761-84.406-188.761-188.145 0-103.745 84.677-188.145 188.761-188.145s188.761 84.4 188.761 188.145c.001 103.739-84.676 188.145-188.761 188.145z"/>' +
            '<path d="M506.547 479.756L385.024 358.85c-7.248-7.205-18.963-7.174-26.174.068-7.205 7.248-7.174 18.962.068 26.174l121.523 120.906a18.457 18.457 0 0013.053 5.385 18.45 18.45 0 0013.121-5.453c7.205-7.249 7.174-18.963-.068-26.174z"/>' +
            '</svg></div>' +
            '<div class="content">' + item + '</div>' +
            '</a>');
        itemEl.appendTo(wrapper);
    });
}

function toggleIconHambuger() {
    const iconHambugerMb = $('.icon-hambuger-mb')
    const iconClose = $('.icon-close-mb')

    iconClose.toggleClass('d-none')
    iconHambugerMb.toggleClass('d-none')
}

jQuery(document).ready(function (e) {

    showMiniCart();
    //addToCart();
    // active menu
    //menuToggle();
    //searchSpotlight();
    //mobileSearchActive();
    updateCountCart();

    const iconSeachMb = $('.icon-search-mb')
    iconSeachMb.on('click', function () {
        const searchResultMb = $(".search-result-mb")
        if (!searchResultMb.hasClass('show')) {
            const navMb = $('#nav-mobile')
            if (navMb.hasClass('d-block')) {
                navMb.removeClass('d-block')
                searchResultMb.addClass('show')
                return
            }

            searchResultMb.addClass('show')
            toggleIconHambuger()
        }
    })

    $('.icon-close-mb').on('click', function () {
        const searchResultMb = $(".search-result-mb")
        if (searchResultMb.hasClass('show')) {
            searchResultMb.removeClass('show')
            toggleIconHambuger()
        }
    })

    const headerMbTop = $('.header-mobile-top')
    const navMb = $('.nav-mobile')
    navMb.css('top', headerMbTop.outerHeight() + 'px')

    const navMbMain = $('.nav-mobile_main')
    navMbMain.css('height', `calc(100% - ${headerMbTop.outerHeight()}px)`)

    const menuItemTargetLinkHasSub = $('.menu-item__has-sub')
    menuItemTargetLinkHasSub.on('click', function (e) {
        if ($(this).hasClass('has-sub')) {
            const ulSubMenu = e.target.closest('li').querySelector('.sub-menu')
            ulSubMenu && ulSubMenu.classList.toggle('d-block')

            const iconArrow = e.target.closest('li').querySelector('.menu-item__icon-arrow')
            iconArrow && iconArrow.classList.toggle('active')
        }
    })
});

function openCloseFooterMenu(elemt) {
    let widthS = $(window).width();
    if (widthS < 421) {
        let menuE = $(elemt).parent().find('ul');
        let disE = menuE.css('display');
        let iconE = $(elemt).find('i');
        if (disE == 'none') {
            menuE.css('display', 'block');
            iconE.removeClass('fa-chevron-down').addClass('fa-chevron-up');
        } else {
            menuE.css('display', 'none');
            iconE.removeClass('fa-chevron-up').addClass('fa-chevron-down');
        }
    }
}
jQuery(document).ready(function (e) {
    $(document).on('submit', '.footer-form-subscribers', function (e) {
        e.preventDefault();
        const _this = $(this);
        const _url = _this.attr('action');
        const data = _this.serializeArray();
        const form__message = _this.find('.footer-form__message');
        const form_field = _this.find('.footer-form__field');
        form_field.addClass('is-loading');
        $.ajax({
            url: _url,
            method: 'POST',
            data: data,
            dataType: 'json',
            success: function (response) {
                const { success, messages } = response;
                if (messages) form__message.html(messages);
                if (success) {
                    form__message.addClass('success');
                    let email = $('input[name="email"]', form_field).val();
                } else {
                    form__message.removeClass('success');
                }
                form_field.removeClass('is-loading');
            },
            error: function (response) {
                form__message.html("Gửi đăng ký không thành công");
                form__message.removeClass('success');
                form_field.removeClass('is-loading');
            }
        });
    });

    //
    var modal2 = document.getElementById("modal-popup__sale");
    $('.site-header__top-sale').on('click', '.content', function () {
        modal2.classList.add('show');
    });

    var span2 = document.getElementsByClassName("close")[0];
    if (span2) {
        span2.onclick = function () {
            modal2.classList.remove('show');
        }
    }

    window.onclick = function (event) {
        if (event.target === modal2) {
            modal2.classList.remove('show');
        }
    }

});

//$(document).ready(function () {
//    const jsonData = $('#popup').data("json");
//    if (!jsonData) { return }

//    let hasSeenPopupSession = sessionStorage.getItem('popup');
//    if (!hasSeenPopupSession) {
//        setTimeout(function () {
//            $('.popup-content').removeClass('d-none');
//            $('#popup').removeClass('d-none');

//            sessionStorage.setItem('popup', true);
//        }, jsonData * 1000);

//    }
//    $('.popup-content').on('click', function () {
//        const overlayBody = $(this)
//        if (!overlayBody.hasClass('d-none')) {
//            overlayBody.addClass('d-none')
//        }
//        $('#popup').fadeOut();
//    });
//    $(".popup-close").on("click", function () {
//        if (!$('.popup-content').hasClass('d-none')) {
//            $('.popup-content').addClass('d-none')
//        }
//        $('#popup').fadeOut();
//    });
//});
