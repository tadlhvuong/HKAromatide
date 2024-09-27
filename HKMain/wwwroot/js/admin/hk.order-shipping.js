

var dt_shipping, rowId;

//change lang text table
var urlCurrent = window.location.href.split('/');
var culture = urlCurrent != null ? urlCurrent[4] : 'vi';
var addText = (culture === "vi") ? "Thêm mới" : "Add";
var exportText = (culture === "vi") ? "Tải xuống" : "Export";
var searchText = (culture === "vi") ? "Tìm kiếm..." : "Search...";
var infoText = (culture === "vi") ? "Hiện _START_ / _END_ Tổng _TOTAL_ mục" : "Displaying _START_ to _END_ of _TOTAL_ entries";
var prevText = (culture === "vi") ? "Trước" : "Prev";
var nextText = (culture === "vi") ? "Sau" : "Next";
var editText = (culture === "vi") ? "Sửa" : "Edit";
var removeText = (culture === "vi") ? "Xóa" : "Remove";
var emptyDTText = (culture === "vi") ? "Không có dữ liệu" : "No data available in table";
var emptyRecord = (culture === "vi") ? "Không tìm thấy kết quả" : "No matching records found";
var filterText = (culture === "vi") ? "(Lọc từ _MAX_ mục)" : "(filtered from _MAX_ records)";
var confirmText = (culture === "vi") ? "Đã xác thực" : "Confirmed";
var unConfirmText = (culture === "vi") ? "Chưa xác thực" : "Unconfirmed ";

var statusObj = {
    0: { title: 'Inactive', class: 'bg-secondary' },
    1: { title: 'Active', class: 'bg-success' },
    2: { title: 'Suspend', class: 'bg-warning' },
};
function initDataTable_Shipping() {
    var initData = null;
    $.ajax({
        type: "GET",
        url: "/Admin/Order/LoadShippingList",
        dataType: "json",
        contentType: 'application/json; charset=utf-8',
        cache: false,
        processData: false,
        global: false,
        async: false,
        success: function (result) {
            if (result.code == 1) {
                if (result.message != null) {
                    initData = result;
                }
            } else {
                toastr.error(result.message);
                initData = null;
            }
        }
    });
    console.log('as');
    console.log(initData);
    if (initData != null) {
        initData = JSON.parse(initData.data);
    }
    dt_unit = $('#table_shippings').DataTable({
        data: initData,
        rowId: "Id",
        columns: [
            // columns according to JSON
            { data: '' },
            { data: '#' },
            { data: 'Name' },
            { data: 'Phone' },
            { data: 'Address' },
            { data: 'ZipCode' },
            { data: 'Status' },
            { data: '' }
        ],
        columnDefs: [
            {
                // For Responsive
                className: 'control',
                searchable: false,
                orderable: false,
                responsivePriority: 1,
                targets: 0,
                render: function (data, type, full, meta) {
                    return '';
                }
            },
            {
                targets: 1,
                responsivePriority: 3,
                render: function (data, type, full, meta) {
                    return '<span class="fw-medium">' + full['Id'] + '</span>';
                }
            },
            {
                targets: 2,
                responsivePriority: 2,
                render: function (data, type, full, meta) {
                    return '<span class="fw-medium">' + full['Name'] + '</span>';
                }
            },
            {
                targets: 3,
                responsivePriority: 5,
                render: function (data, type, full, meta) {
                    return '<span class="fw-medium">' + full['Phone'] + '</span>';
                }
            },
            {
                targets: 4,
                responsivePriority: 6,
                render: function (data, type, full, meta) {
                    return '<span class="fw-medium">' + full['Address'] + '</span>';
                }
            },
            {
                targets: 5,
                responsivePriority: 7,
                render: function (data, type, full, meta) {
                    return '<span class="fw-medium">' + full['ZipCode'] + '</span>';
                }
            },
            {
                targets: 6,
                responsivePriority: 8,
                render: function (data, type, full, meta) {
                    var $status = full['Status'];

                    return (
                        '<span class="badge ' +
                        statusObj[$status].class +
                        '"text-capitalized>' +
                        statusObj[$status].title +
                        '</span>'
                    );
                }
            },
            {
                // Actions
                targets: -1,
                searchable: false,
                orderable: false,
                responsivePriority: 4,
                render: function (data, type, full, meta) {
                    var url_U = "/Admin/" + culture +"/Order/ShippingEdit/",
                        url_D = "/Admin/Order/ShippingDelete/";
                    return ('<div class="d-flex align-items-center">' +
                        '<a type="button" class="btn btn-warning btn-sm mr-2" data-modal="" onclick="formEdit(\'' + url_U +
                        + full["Id"] + '\'); " title="' + editText + '"><i class="fa fa-pencil-alt"></i><span class="d-none d-md-inline">  ' + editText + '</span></a>' +
                        '<a type="button" class="btn btn-danger btn-sm" data-modal="" onclick="formDelete(\'' + url_D +
                        + full["Id"] + '\'); " title="' + removeText + '"><i class="fa fa-trash"></i> <span class="d-none d-md-inline">  ' + removeText + '</span></a>' +
                        '</div>')
                }
            }
        ],
        order: [[1, 'desc']],
        dom:
            '<"pb-md-2 d-flex flex-column flex-md-row justify-content-md-between align-items-center align-items-md-start row"<"d-flex justify-content-md-start mt-2 mt-md-0 gap-2"<"dt-action-buttons">B><"d-flex justify-content-md-start mt-2 mt-md-0 gap-2"f>' +
            '>t' +
            '<"row mx-2"' +
            '<"col-sm-12 col-md-6"i>' +
            '<"col-sm-12 col-md-6"p>' +
            '>',
        lengthMenu: [10, 20, 50, 100], //for length of menu
        language: {
            sLengthMenu: '_MENU_',
            search: '',
            searchPlaceholder: searchText,
            info: infoText,
            paginate: {
                previous: prevText,
                next: nextText
            },
            emptyTable: emptyDTText,
            infoEmpty: "",
            zeroRecords: emptyRecord,
            infoFiltered: filterText
        },
        buttons: [
            {
                text: '<i class="fas fa-plus"></i> <span>' + addText + '</span>',
                className: 'add-new btn btn-primary',
                attr: {
                    "onclick": "formEdit('/Admin/" + culture + "/Order/ShippingEdit/0')"
                }
            },
        ],
        // For responsive popup
        "autoWidth": false,
        responsive: {
            details: {
                display: $.fn.dataTable.Responsive.display.modal({
                    header: function (row) {
                        var data = row.data();
                        return 'Details of ' + data['Name'];
                    }
                }),
                type: 'column',
                renderer: function (api, rowIdx, columns) {
                    var data = $.map(columns, function (col, i) {
                        return col.title !== '' // ? Do not show row in modal popup if title is blank (for check box)
                            ? '<tr data-dt-row="' +
                            col.rowIndex +
                            '" data-dt-column="' +
                            col.columnIndex +
                            '">' +
                            '<td>' +
                            col.title +
                            ':' +
                            '</td> ' +
                            '<td>' +
                            col.data +
                            '</td>' +
                            '</tr>'
                            : '';
                    }).join('');

                    return data ? $('<table class="table"/><tbody />').append(data) : false;
                }
            },
        },
        initComplete: function () {
        }
    });

    // Filter form control to default size
    // ? setTimeout used for multilingual table initialization
    setTimeout(() => {
        $('.dataTables_filter .form-control').removeClass('form-control-sm');
        $('.dataTables_length .custom-select').removeClass('custom-select-sm');
        $('#table_unit_wrapper .dt-buttons button:eq(0)').removeClass('btn-secondary');
    }, 300);
    return dt_unit;
};
function formEdit(href) {
    console.log('a');
    modalContent = $('#modalContent');
    modalContent.removeClass('bg-primary');

    modalContent.load(href, function () {
        $('#modalContainer').modal({
            keyboard: true
        });
        $("form").removeData("validator");
        $("form").removeData("unobtrusiveValidation");
        $.validator.unobtrusive.parse("form");
    });
}
function formView(href) {
    console.log('b');
    modalContent = $('#modalContent');
    modalContent.removeClass('bg-primary');

    modalContent.load(href, function () {
        $('#modalContainer').modal({
            keyboard: true
        });
        $("form").removeData("validator");
        $("form").removeData("unobtrusiveValidation");
        $.validator.unobtrusive.parse("form");
    });
}
function formDelete(href) {
    modalContent = $('#modalContent');
    modalContent.addClass('bg-danger');
    modalContent.find('.modal-title').text('Xóa shipping address');

    modalContent.load(href, function () {
        $('#modalContainer').modal({
            keyboard: true,
        });
        $("form").removeData("validator");
        $("form").removeData("unobtrusiveValidation");
        $.validator.unobtrusive.parse("form");
    });
}
function initModalForms() {
    $.ajaxSetup({ cache: false });
    $(document).on('click', '.modal-opener', function (e) {
        e.preventDefault();
        formEdit(this.href);
    });
    $(document).on('click', '.modal-closer', function (e) {
        e.preventDefault();
        $('#modalContainer').modal('hide');
    });
    $(document).on('click', '.modal-refresh', function (e) {
        e.preventDefault();
        location.reload();
    });
    $('#modalContainer').on('hidden.bs.modal', function (e) {
        $('#modalContent').html('');
    })
}
function initAnimation() {
    if (($("[data-animation-effect]").length > 0) && !Modernizr.touch) {
        $("[data-animation-effect]").each(function () {
            var item = $(this),
                animationEffect = item.attr("data-animation-effect");

            if (Modernizr.mq('only all and (min-width: 768px)') && Modernizr.csstransitions) {
                item.appear(function () {
                    setTimeout(function () {
                        item.addClass('animated object-visible ' + animationEffect);
                    }, item.attr("data-effect-delay"));
                }, { accX: 0, accY: -130 });
            } else {
                item.addClass('object-visible');
            }
        });
    };
}
$(function () {
    initModalForms();
    initAnimation();

    //Scroll totop
    //-----------------------------------------------
    $(window).scroll(function () {
        if ($(this).scrollTop() != 0) {
            $(".scrollToTop").fadeIn();
        } else {
            $(".scrollToTop").fadeOut();
        }
    });

    $(".scrollToTop").click(function () {
        $("body,html").animate({ scrollTop: 0 }, 800);
    });

    //Modal
    //-----------------------------------------------
    if ($(".modal").length > 0) {
        $(".modal").each(function () {
            $(".modal").prependTo("body");
            $(".modal").css("z-index", "1500");
        });
    }

    dt_shipping = initDataTable_Shipping();

});


$(document).on("submit", ".modalForm", function (e) {
    e.preventDefault();
    $("#modalContent").removeClass('bg-danger');
    $("#modalContainer").modal("show");
    $(this).find("button[type='submit']").hide();
    $("#progress").show();
    var idItem = $(this).find("input#Id").val();
    console.log(this.action)
    console.log(this.method)
    console.log(new FormData(this));
    $.ajax({
        url: this.action,
        type: this.method,
        data: new FormData(this),
        cache: false,
        contentType: false,
        processData: false,
        success: function (result) {
            if (result.code === 1) {
                $("#modalContainer").modal("hide");
                $("#progress").hide();
                toastr.success(result.message);
                console.log(JSON.parse(result.data));
                console.log("idiTem");
                console.log(idItem);
                if (idItem == 0) {
                    dt_shipping.row.add(JSON.parse(result.data)).draw();
                } else {
                    dt_shipping.row('#' + idItem).data(JSON.parse(result.data)).draw()
                }

            } else {
                $("#progress").hide();
                $("#modalContainer").modal({ keyboard: true }, "show");
                $("#modalContent").html(result);
                toastr.error(result.message);
            }
        },
        error: function (jqXHR, textStatus, errorMessage) {
            toastr.error('Lỗi hệ thống. Vui lòng liên hệ hỗ trợ');
        }
    });
});
$(document).on("submit", ".modalFormDelete", function (e) {
    e.preventDefault();
    var idItem = $(this).find("input").val();
    $(this).find("button[type='submit']").hide();
    $("#progress").show();
    $.ajax({
        url: this.action,
        type: this.method,
        data: new FormData(this),
        cache: false,
        contentType: false,
        processData: false,
        success: function (result) {
            if (result.code == 1) {
                toastr.success(result.message);
                dt_shipping.row('#' + idItem).remove().draw();

            } else {
                toastr.error(result.message);
            }
            $("#progress").hide();
            $("#modalContainer").modal("hide");
        },
        error: function (jqXHR, textStatus, errorMessage) {
            toastr.error('Lỗi hệ thống. Vui lòng liên hệ hỗ trợ');
        }
    });
});