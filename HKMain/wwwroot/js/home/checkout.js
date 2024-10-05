function checkoutLoading(status) {
    let checkout_wrapper = $('body');
    if (status === true) {
        checkout_wrapper.addClass('loading');
    } else {
        checkout_wrapper.removeClass('loading');
    }
}
function changeQuantity(type, el) {
    //if (!type) $(el).parents(".form-update-quantity").submit();

    let quantityEl;
    if (type === "down") quantityEl = $(el).next();
    else quantityEl = $(el).prev();

    let quantity = $(quantityEl).val();
    if (type === "up") quantity++;
    else quantity--;

    if (quantity > 0) {
        checkoutLoading(true);
        $(quantityEl).val(quantity);
        $(el).parents(".form-update-quantity").submit();
    }
}
function cancelChangeVariant(el) {
    $(el).parents(".info").children("form").hide();
    $(el).parents(".info").children("#current-option").show();
}

function changeOption(el) {
    $(el).parents(".options").hide();
    $(el).parents(".info").children("form").show();
}

function confirmChangeVariant(el) {
    checkoutLoading(true);
    $(el).parents("form").submit();
}
jQuery(document).ready(function (e) {
    loadCartPayment();
    updateCountCart();
    updatePriceCart();
});