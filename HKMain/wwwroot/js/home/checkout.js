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

$(document).on("submit", ".checkout-form", function (e) {
    e.preventDefault();
    console.log('payment');
    var btn = $(this).find("button[type='submit']");
    btn.attr('disabled', 'disabled');
    btn.css("background", "#ccc");
    btn.text("Đang xác nhận...");
    var isAgree = $("#isAgressPrivacy").is(":checked");
    if (!isAgree) {
        btn.removeAttr('disabled');
        btn.css("background", "#e67e22");
        btn.text("Xác nhận đặt hàng");
        return toastr.warning("Đồng ý với chính sách để tiếp tục hoặc quay lại khi bạn đã đọc rõ chính sách và đồng ý");;
    }
    $.ajax({
        url: this.action,
        type: this.method,
        data: new FormData(this),
        cache: false,
        contentType: false,
        processData: false,
        success: function (result) {
            console.log(result);
            if (result.code === 1) {
                toastr.success("Đặt hàng thành công");
                removeAllCart();
            } else {
                toastr.error("Đặt hàng thất bại");
            }
            btn.removeAttr('disabled');
            btn.css("background", "#e67e22");
            btn.text("Xác nhận đặt hàng");
        }, error: function (XMLHttpRequest, textStatus, errorThrown) {
            toastr.error(errorThrown);
            btn.removeAttr('disabled');
            btn.css("background", "#e67e22");
            btn.text("Xác nhận đặt hàng");
        } 
    });
});