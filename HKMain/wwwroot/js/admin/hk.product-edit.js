$(function () {
    $('.select2').select2()

    //Initialize Select2 Elements
    $('.select2bs4').select2({
        width: '100%',
        theme: 'bootstrap4'
    })

    // Summernote
    $('#summernote').summernote()

    // Basic Tags
    const tagifyBasicEl = document.querySelector('#Tags');
    new Tagify(tagifyBasicEl);

    var variant = null;
    var attr = null;
    var idProduct = $('#Id').val();
    $.ajax({
        type: "GET",
        url: "/Admin/Product/GetVariant/" + idProduct,
        dataType: "json",
        contentType: 'application/json; charset=utf-8',
        cache: false,
        processData: false,
        global: false,
        async: false,
        success: function (result) {
            if (result.code == 1) {
                if (result.message != null) {
                    variant = result.data;
                    attr = result.subData;
                }
            } else {
                toastr.error(result.message);
                variant = null;
                attr = null;
            }
        }
    });
    var arrVariant = JSON.parse(variant);
    console.log(arrVariant);
    if (arrVariant.length == 0) {
        $('.repeater-default').hide();
        $('#variant-empty').show();
    } else {
        $('.repeater-default').show();
        $('#variant-empty').hide();
    }
    var repeater = $('.repeater-default').repeater({
        initEmpty: true,
        show: function () {
            var formControl = $(this).find('.form-control');
            console.log(formControl);
            arrVariant.forEach((element) => {
                $(formControl[0]).append('<option value="' + element['Id'] + '">' + element['Text'] + '</option>');
            });
            $(formControl[0]).on('change', function (e) {
                var idParentChange = $(formControl[0]).val();
                var arrVariantChild = null;
                arrVariant.forEach((element) => {
                    if (element['Id'] == idParentChange)
                        arrVariantChild = element['ItemsChild'];
                });
                var length = formControl[1].options.length;
                for (i = length - 1; i > 0; i--) {
                    formControl[1].options[i] = null;
                }
                if (arrVariantChild != null) {
                    arrVariantChild.forEach((element) => {
                        $(formControl[1]).append('<option value="' + element['Id'] + '">' + element["Text"] + '</option>');
                    });
                }
            })
            $(this).slideDown();
            $(formControl).on('change', function (e) {
                $('#Attrubutes').val(JSON.stringify(repeater.repeaterVal().Attrubutes));
            });
        },
        hide: function (deleteElement) {
            //$(this).slideUp(deleteElement);
            Swal.fire({
                title: "Bạn muốn xóa biến thể này?",
                text: "",
                icon: "question",
                showCancelButton: true,
                confirmButtonText: "Đồng ý xóa",
                cancelButtonText: "Hủy bỏ",
                reverseButtons: true,
            }).then(function (result) {
                if (result.value) {

                    $(this).slideUp(function () {
                        deleteElement();
                        var formControl = $(this).find('.form-control');
                        if (formControl.length > 0)
                            $('#Attrubutes').val(JSON.stringify(repeater.repeaterVal().Attrubutes));
                        else
                            $('#Attrubutes').val("");
                    });

                } else if (result.dismiss === "cancel") {

                }
            });
        },
        ready: function (setIndexes) {
        },
    });

    //jQuery(".drag").sortable({
    //    axis: "y",
    //    cursor: 'pointer',
    //    opacity: 0.5,
    //    placeholder: "row-dragging",
    //    delay: 150,
    //    update: function (event, ui) {
    //    }

    //}).disableSelection();

    ////get arr variant
    //load update function
    if (idProduct != 0) {
        //create select init
        var objectAttr = {};
        objectAttr.Attrubutes = JSON.parse(attr);
        if (objectAttr.Attrubutes.length > 0) {
            repeater.setList(objectAttr["Attrubutes"]);
            var formControl = $('.repeater-default').find('.form-control');
            var k = 0;
            var currentVal = null;
            for (var i = 0; i < formControl.length; i += 2) {
                for (var j = 1; j < formControl[i].options.length; j++) {
                    var val = formControl[i].options[j].value;
                    if (objectAttr.Attrubutes[k]['Parent'] == val) {
                        formControl[i].options.selectedIndex = j;
                        currentVal = val;
                    }
                }
                var arrSubVariant = null;
                arrVariant.forEach(element => {
                    if (element.Id == currentVal)
                        arrSubVariant = element;
                })
                if (arrSubVariant != undefined) {
                    arrSubVariant.ItemsChild.forEach((ele) => {
                        if (ele.Id == objectAttr.Attrubutes[k]['Child'])
                            $(formControl[i + 1]).append('<option value="' + ele['Id'] + '" selected>' + ele["Text"] + '</option>');
                        else
                            $(formControl[i + 1]).append('<option value="' + ele['Id'] + '">' + ele["Text"] + '</option>');
                    });
                }
                k++;
            }
            $('#Attrubutes').val(JSON.stringify(repeater.repeaterVal().Attrubutes));
        }
    }
});