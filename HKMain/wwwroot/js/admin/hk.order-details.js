var dt_orderItems, columnsDT, rowId, Columns_Def;

//change lang text table
var urlCurrent = window.location.href.split('/');
var culture = urlCurrent != null ? urlCurrent[4] : 'vi';
var addText = (culture === "vi") ? "Thêm mới" : "Add";
var exportText = (culture === "vi") ? "Tải xuống" : "Export";
var searchText = (culture === "vi") ? "Tìm kiếm..." : "Search...";
var infoText = (culture === "vi") ? "Hiện _START_ / _END_ Tổng _TOTAL_ mục" : "Displaying _START_ to _END_ of _TOTAL_ entries";
var prevText = (culture === "vi") ? "Trước" : "Prev";
var nextText = (culture === "vi") ? "Sau" : "Next";
var detailsText = (culture === "vi") ? "Chi tiết" : "Details";
var editText = (culture === "vi") ? "Sửa" : "Edit";
var removeText = (culture === "vi") ? "Xóa" : "Remove";
var emptyDTText = (culture === "vi") ? "Không có dữ liệu" : "No data available in table";
var emptyRecord = (culture === "vi") ? "Không tìm thấy kết quả" : "No matching records found";
var filterText = (culture === "vi") ? "(Lọc từ _MAX_ mục)" : "(filtered from _MAX_ records)";
var confirmText = (culture === "vi") ? "Đã xác thực" : "Confirmed";
var unConfirmText = (culture === "vi") ? "Chưa xác thực" : "Unconfirmed ";

var orderId = urlCurrent[7];
var loadAction = "/Admin/Order/LoadOrderItemList/" + orderId;
createAction = (culture === "vi") ? "/Admin/vi/don-hang/tao-hoa-don" : "/Admin/en/Order/CreateOrder",
    detailsAction = (culture === "vi") ? "/Admin/vi/don-hang/chi-tiet/" : "/Admin/en/Order/Details/",
    editAction = (culture === "vi") ? "/Admin/vi/don-hang/chinh-sua/" : "/Admin/en/Order/Edit/",
    deleteAction = (culture === "vi") ? "/Admin/vi/don-hang/chinh-sua/" : "/Admin/en/Order/Delete/";
var detailsMember = (culture === "vi") ? "/Admin/vi/thanh-vien/chi-tiet/" : "/Admin/en/Member/Details/";

var statusObj = {
    0: { title: 'Pending', class: 'bg-default' },
    1: { title: 'Processing', class: 'bg-warning' },
    2: { title: 'Delivering', class: 'bg-info' },
    3: { title: 'Delivered', class: 'bg-success' },
    4: { title: 'Canceled', class: 'bg-danger' }
},

    paymentObj = {
        0: { title: 'None', class: 'bg-default' },
        1: { title: 'Partly', class: 'bg-warning' },
        2: { title: 'Fully', class: 'bg-success' },
        3: { title: 'Failed', class: 'bg-danger' },
        4: { title: 'Refunded', class: 'bg-info' }
    };
var initData = null, getData = null;
function initDataTable() {
    $.ajax({
        type: "GET",
        url: loadAction,
        dataType: "json",
        contentType: 'application/json; charset=utf-8',
        cache: false,
        processData: false,
        global: false,
        async: false,
        success: function (result) {
            if (result.code == 1) {
                if (result.message != null) {
                    getData = result;
                }
            } else {
                toastr.error(result.message);
                getData = null;
            }
        }
    });
    if (getData != null) {
        initData = JSON.parse(getData.data);
    }
    dt_orderItems = $('#table_orderItems').DataTable({
        data: initData,
        rowId: "Id",
        columns: [
            // columns according to JSON
            { data: '' },
            { data: '#' },
            { data: 'Product' },
            { data: 'Price' },
            { data: 'Quantity' },
            { data: 'Total' },
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
                render: function (data, type, full, meta) {
                    return '<span class="fw-medium">' + full['Id'] + '</span>';
                }
            },
            {
                targets: 2,
                render: function (data, type, full, meta) {
                    //var $name = full['NameItem'],
                    //    $material = full['Material'],
                    //    $image = full['AvatarItem'];
                    var $name = 'Wooden Chair',
                        $material = 'Material: Wooden',
                        $image = full['AvatarItem'];
                    var $output =
                        '<img src="/images/logo.webp" alt="Avatar" class="img-circle elevation-2 table-avatar">';
                    // Creates full output for row
                    var $row_output =
                        '<div class="d-flex justify-content-start align-items-center user-name">' +
                        '<div class="avatar-wrapper user-pane">' +
                        '<div class="image mr-2">' +
                        $output +
                        '</div>' +
                        '</div>' +
                        '<div class="d-flex flex-column">' +
                        '<span class="fw-medium">' +
                        $name +
                        '</span>' +
                        '<small class="text-muted">' +
                        $material +
                        '</small>' +
                        '</div>' +
                        '</div>';
                    return $row_output;
                }
            },
            {
                targets: 3,
                render: function (data, type, full, meta) {
                    return '<span class="fw-medium">' + full['Price'] + '</span>';
                }
            },
            {
                // User Status
                targets: 4,
                render: function (data, type, full, meta) {
                    return '<span class="fw-medium">' + full['Quantity'] + '</span>';
                }
            },
            {
                // User Status
                targets: 5,
                render: function (data, type, full, meta) {
                    return '<span class="fw-medium">' + full['Total'] + '</span>';
                }
            }
        ],
        order: [[1, 'desc']],
        dom: '',
        lengthMenu: [10, 20, 50, 100], //for length of menu
        language: {
            sLengthMenu: '_MENU_',
            search: '',
            searchPlaceholder: searchText,
            emptyTable: emptyDTText,
            infoEmpty: "",
            zeroRecords: emptyRecord,
            infoFiltered: filterText
        },

        info: false,
        paging: false,
        // For responsive popup
        "autoWidth": false,
        responsive: {
            details: {
                display: $.fn.dataTable.Responsive.display.modal({
                    header: function (row) {
                        var data = row.data();
                        return 'Details of ' + data[columnsDT[2].data];
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

    setTimeout(() => {
        $('.dataTables_filter .form-control').removeClass('form-control-sm');
        $('.dataTables_length .custom-select').removeClass('custom-select-sm');
        $('.dt-action-buttons button:eq(0)').removeClass('btn-secondary');
    }, 300);
    return dt_orderItems;
};

function formEdit(href) {
    modalContent = $('#modalContent');
    modalContent.removeClass('bg-danger');

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
    modalContent.find('.modal-title').text('Xóa product item');

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
    $(document).on('click', '#CaptchaImg', function (e) {
        e.preventDefault();
        $('#CaptchaImg').removeAttr('src').attr('src', '/Home/Captcha');
    });
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

    dt_orderItems = initDataTable();

});

$(document).on("submit", ".modalForm", function (e) {
    e.preventDefault();
    var idItem = $(this).find("input#Id").val();
    $("#modalContent").removeClass('bg-danger');
    $("#modalContainer").modal("show");
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
            if (result.code === 1) {
                $("#modalContainer").modal("hide");
                $("#progress").hide();
                toastr.success(result.message);
                if (idItem == 0) {
                    dt_orderItems.row.add(JSON.parse(result.data)).draw();
                } else {
                    dt_orderItems.row('#' + idItem).data(JSON.parse(result.data)).draw()
                }

            } else {
                $("#progress").hide();
                $("#modalContainer").modal({ keyboard: true }, "show");
                $("#modalContent").html(result);
            }
        }
    });
});
$(document).ready(function () {
    //edit data table
    $(document).on("submit", ".modalForm", function (e) {
        e.preventDefault();
        var idItem = $(this).find("input#Id").val();
        $("#modalContent").removeClass('bg-danger');
        $("#modalContainer").modal("show");
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
                if (result.code === 1) {
                    $("#modalContainer").modal("hide");
                    $("#progress").hide();
                    toastr.success(result.message);
                    if (idItem == 0) {
                        dt_orderItems.row.add(JSON.parse(result.data)).draw();
                    } else {
                        dt_orderItems.row('#' + idItem).data(JSON.parse(result.data)).draw()
                    }

                } else {
                    $("#progress").hide();
                    $("#modalContainer").modal({ keyboard: true }, "show");
                    $("#modalContent").html(result);
                }
            }
        });
    });

    //Remove data table
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
                    dt_orderItems.row('#' + idItem).remove().draw();

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
});