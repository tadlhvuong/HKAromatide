
var loadProductSelect = "/Admin/Order/LoadProductSelect/"
var dataProductSelect = null, dataVariantSelect = null, dataVariantChildSelect = null;
function productSelect() {
    console.log($("#Id").val());
    $.ajax({
        type: "GET",
        url: "/Admin/Order/LoadProductSelect/0",
        dataType: "json",
        contentType: 'application/json; charset=utf-8',
        cache: false,
        processData: false,
        global: false,
        async: false,
        success: function (result) {
            console.log(result);
            if (result.code == 1) {
                if (result.message != null) {
                    dataProductSelect = JSON.parse(result.data);
                }
            } else {
                toastr.error(result.message);
                dataProductSelect = null;
            }
        }
    });
}
function variantSelect(idP) {
    var link = "/Admin/Order/LoadVariantSelect/" + idP;
    $.ajax({
        type: "GET",
        url: "/Admin/Order/LoadVariantSelect/" + idP,
        dataType: "json",
        contentType: 'application/json; charset=utf-8',
        cache: false,
        processData: false,
        global: false,
        async: false,
        success: function (result) {
            console.log(result);
            if (result.code == 1) {
                if (result.message != null) {
                    dataVariantSelect = JSON.parse(result.data);
                }
            } else {
                toastr.error(result.message);
                dataVariantSelect = null;
            }
        }
    });
}
function createNoteSelect(id, idNode, data) {
    if (data != null) {
        console.log(data);
        console.log(idNode);
        console.log(id);
        document.getElementById(idNode).options.length = 0;
        $("#productselect").append('<option value="0">-- Lựa chọn --</option>');
        data.forEach((element) => {
            console.log(element['Value']);
            $("#productselect").append('<option value="' + element['Value'] + '">' + element["Text"] + '</option>');
        });
        console.log($("#" + idNode));
        document.getElementById(idNode).value = id;
    }
}
$(function () {
    console.log('a');
    //var select = $('.select2');
    //if (select.length > 0) {

    //    $('.select2').select2()

    //    //Initialize Select2 Elements
    //    $('.select2bs4').select2({
    //        width: '100%',
    //        theme: 'bootstrap4'
    //    })
    //}
    productSelect();
    createNoteSelect('0', "productselect", dataProductSelect);
    $('#productselect').on('change', function () {
        console.log($('#productselect').val());
    });
    
    //createNoteSelect('1000', "ProvinceId", dataProductSelect);
    //$('#productselect').on('change', function () {
    //    idProvince = $('#productselect').val();
       
    //})
})
