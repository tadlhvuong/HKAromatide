$(function () {
    $('.select2').select2()

    //Initialize Select2 Elements
    $('.select2bs4').select2({
        width: '100%',
        theme: 'bootstrap4'
    })

    // Summernote
    $('#summernote').summernote({
        placeholder: 'Nhập nội dung cho sản phẩm',
        height: 260
    })

    $("input[data-type='currency']").on({
        keyup: function () {
            $(this).val($(this).val().replace(/\D/g, "").replace(/\B(?=(\d{3})+(?!\d))/g, "."))
        },
        blur: function () {
            $(this).val($(this).val().replace(/\D/g, "").replace(/\B(?=(\d{3})+(?!\d))/g, "."))
        }
    });


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
    if (arrVariant.length == 0) {
        $('.repeater-default').hide();
        $('#variant-empty').show();
    } else {
        $('.repeater-default').show();
        $('#variant-empty').hide();
    }

    var formRepeater = $('.form-repeater');
    // Form Repeater
    // ! Using jQuery each loop to add dynamic id and class for inputs. You may need to improve it based on form fields.
    // -----------------------------------------------------------------------------------------------------------------

    if (formRepeater.length) {
        formRepeater.repeater({
            initEmpty: true,
            defaultValues: {
                'Parent': '0',
                'Child': '0',
                'Price': '0',
            },
            show: function () {
                var formControl = $(this).find('.form-control');
                $(formControl[0]).on('change', function (e) {
                    var idParentChange = $(formControl[0]).val()
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
                    saveFormJSON();
                });
                $(this).slideDown();
            },
            hide: function (deleteElement) {
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
                            try {
                                saveFormJSON();
                            } catch (e) {
                                $('#Attributes').val("");
                            }
                        });

                    } else if (result.dismiss === "cancel") {

                    }
                });
            }
        });
    }
    if (idProduct != 0) {
        var objectAttr = JSON.parse(attr);
        if (objectAttr.length > 0) {
            formRepeater.setList(objectAttr);
            var formSelect = $(this).find('.form-select');
            for (var i = 0; i < formSelect.length; i += 2) {
                //get id selected
                var selected = null;
                for (var j = 1; j < formSelect[i].options.length; j++) {
                    selected = formSelect[i].options[formSelect[i].selectedIndex].value;
                    if (selected != null) break;
                }
                //get ids sub follow selected
                var arrSubVariant = [];
                arrVariant.forEach(element => {
                    if (element.Id == selected) {
                        element['ItemsChild'].forEach(sub => {
                            arrSubVariant.push(sub["Id"]);
                        })
                    };
                })
                for (var m = 1; m < formSelect[i + 1].options.length; m++) {
                    var exits = formSelect[i + 1].options[m].value;
                    if (arrSubVariant.includes(parseInt(exits))) {
                    } else {
                        formSelect[i + 1].options[m].remove();
                        m--;
                    }
                }

            }
            saveFormJSON();
        }
    }
});
function saveFormJSON() {
    var objVal = $(".form-repeater").repeaterVal();
    var jsonVal = null;
    if (objVal != null) jsonVal = JSON.stringify(objVal["FormAttributes"])
    $('#Attributes').val(jsonVal);
    // you can now post/get your myFormJson to your back-end for CRUD operation
}

function setFormJSON() {
    // you would first grab and set your myJson string from the database
    // I've included a sample myJson string for demonstration purposes

    var myJson = '{"Attributes":["Parent":"25","Child":"27", "Price":"23000"},{"Parent":"28","Child":"30", "Price":"23000"}]}';
    // myJson = '{"Attributes":[{"Image":["src","/media/Product_October2024/zeqzkw.webp"]}]}';
    var myObj = JSON.parse(myJson);
    return myObj;
}