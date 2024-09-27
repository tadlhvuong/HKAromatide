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

function loadCart() {
    var model = JSON.parse((sessionStorage.getItem("Cart") == null) ? [] : sessionStorage.getItem("Cart"));
    $("#Items").val(JSON.stringify(model));
    console.log(model);
}
//function checkout() {
//    var model = {
//        id: $("#Id").val(),
//        name: $("#Name").val(),
//        email: $("#Email").val(),
//        phoneNumber: $("#PhoneNumber").val(),
//        address: $("#Address").val(),
//        note: $("#Note").val(),
//        payment: $("#Payment").val(),
//        isAgressPrivacy: $("#isAgressPrivacy").val(),
//        items: $("#Items").val()
//    }
//    console.log(model);
//}
jQuery(document).ready(function (e) {
    loadCart();
});