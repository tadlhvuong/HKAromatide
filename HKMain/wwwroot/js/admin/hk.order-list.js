var dt_orders, columnsDT, rowId, Columns_Def;

var loadAction = "/Admin/Order/LoadOrderList"
createAction = "/Admin/don-hang/tao-hoa-don",
    detailsAction = "/Admin/don-hang/chi-tiet/",
    editAction = "/Admin/don-hang/chinh-sua/",
    deleteAction = "/Admin/don-hang/xoa/";
var detailsMember = "/Admin/thanh-vien/chi-tiet/";

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
    dt_orders = $('#table_orders').DataTable({
        data: initData,
        rowId: "Id",
        columns: [
            // columns according to JSON
            { data: '' },
            { data: '#' },
            { data: 'Date' },
            { data: 'Customers' },
            { data: 'Payment' },
            { data: 'Status' },
            { data: 'Method' },
            { data: 'Action' },
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
                responsivePriority: 2,
                render: function (data, type, full, meta) {
                    return '<span class="fw-medium">' + full['Date'] + '</span>';
                }
            },
            {
                targets: 3,
                render: function (data, type, full, meta) {
                    var $name = full['NameUser'],
                        $email = full['EmailUser'],
                        $image = full['AvatarUser'],
                        $detaisLink = (full["UserId"] != null) ? detailsMember + full["UserId"] : "javascript:;";
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
                        '<a href="' +
                        $detaisLink +
                        '" class="text-body text-truncate"><span class="fw-medium">' +
                        $name +
                        '</span></a>' +
                        '<small class="text-muted">' +
                        $email +
                        '</small>' +
                        '</div>' +
                        '</div>';
                    return $row_output;
                }
            },
            {
                targets: 4,
                render: function (data, type, full, meta) {
                    var $status = full['PaymentStatus'];
                    return (
                        '<div class="form-group" style="margin:0 auto; text-align: center">' +
                        '<span class="badge ' +
                        paymentObj[$status].class +
                        '"text-capitalized>' +
                        paymentObj[$status].title +
                        '</span></div>'
                    );
                }
            },
            {
                // User Status
                targets: 5,
                render: function (data, type, full, meta) {
                    var $status = full['Status'];
                    return (
                        '<div class="form-group" style="margin:0 auto; text-align: center">' +
                        '<span class="badge ' +
                        statusObj[$status].class +
                        '"text-capitalized>' +
                        statusObj[$status].title +
                        '</span></div>'
                    );
                }
            },
            {
                // User Status
                targets: 6,
                render: function (data, type, full, meta) {
                    return '<span class="fw-medium">' + full['Method'] + '</span>';
                }
            },
            {
                // Actions
                targets: -1,
                searchable: false,
                orderable: false,
                responsivePriority: 3,
                render: function (data, type, full, meta) {
                    return ('<div class="d-flex align-items-center">' +
                        '<a type="button" class="btn btn-warning btn-sm mr-2" onclick="location.href = (\''+ editAction +
                        + full["Id"] + '\'); " title="Sửa"><i class="fa fa-pencil-alt"></i></a>' +
                        '<a type="button" class="btn btn-danger btn-sm" onclick="formDelete(\'' + deleteAction +
                        + full["Id"] + '\'); " title="Xóa"><i class="fa fa-trash"></i></a>' +
                        '</div>')
                }
            }
        ],
        order: [[1, 'desc']],
        dom:
            '<"pb-md-2 d-flex flex-column flex-md-row justify-content-md-between align-items-center align-items-md-start row"<"d-flex justify-content-md-start mt-2 mt-md-0 gap-2"<"dt-action-buttons"B>><"d-flex justify-content-md-start mt-2 mt-md-0 gap-2"lf>' +
            '>t' +
            '<"row mx-2"' +
            '<"col-sm-12 col-md-6"i>' +
            '<"col-sm-12 col-md-6"p>' +
            '>',
        lengthMenu: [10, 20, 50, 100], //for length of menu
        language: {
            sLengthMenu: '_MENU_',
            search: '',
            searchPlaceholder: "Tìm kiếm...",
            info: "Hiện _START_ / _END_ Tổng _TOTAL_ mục",
            paginate: {
                previous: "Trước",
                next: "Sau"
            },
            emptyTable: "Không có dữ liệu",
            infoEmpty: "",
            zeroRecords: "Không tìm thấy kết quả",
            infoFiltered: "(Lọc từ _MAX_ mục)"
        },
        // Buttons with Dropdown
        buttons: [
            {
                text: '<i class="fas fa-plus"></i> <span> Thêm mới</span>',
                className: 'add-new btn btn-primary',
                attr: {
                    "onclick": "location.href = ('" + editAction + "')"
                }
            },
            {
                extend: 'collection',
                className: 'btn btn-label-secondary dropdown-toggle mx-3',
                text: '<i class="fas fa-download"></i> Tải xuống',
                buttons: [
                    {
                        extend: 'print',
                        text: '<i class="fas fa-print">Print',
                        className: 'dropdown-item',
                        exportOptions: {
                            columns: [1, 2, 3, 4, 5],
                            // prevent avatar to be print
                            format: {
                                body: function (inner, coldex, rowdex) {
                                    if (inner.length <= 0) return inner;
                                    var el = $.parseHTML(inner);
                                    var result = '';
                                    $.each(el, function (index, item) {
                                        if (item.classList !== undefined && item.classList.contains('user-name')) {
                                            result = result + item.lastChild.firstChild.textContent;
                                        } else if (item.innerText === undefined) {
                                            result = result + item.textContent;
                                        } else result = result + item.innerText;
                                    });
                                    return result;
                                }
                            }
                        },
                        customize: function (win) {
                            //customize print view for dark
                            $(win.document.body)
                                .css('color', "#fff")
                                .css('border-color', "#007bff")
                                .css('background-color', "#007bff");
                            $(win.document.body)
                                .find('table')
                                .addClass('compact')
                                .css('color', 'inherit')
                                .css('border-color', 'inherit')
                                .css('background-color', 'inherit');
                        }
                    },
                    {
                        extend: 'csv',
                        text: '<i class="fas fa-file"> Csv',
                        className: 'dropdown-item',
                        exportOptions: {
                            columns: [1, 2, 3, 4],
                            // prevent avatar to be display
                            format: {
                                body: function (inner, coldex, rowdex) {
                                    if (inner.length <= 0) return inner;
                                    var el = $.parseHTML(inner);
                                    var result = '';
                                    $.each(el, function (index, item) {
                                        if (item.classList !== undefined && item.classList.contains('user-name')) {
                                            result = result + item.lastChild.firstChild.textContent;
                                        } else if (item.innerText === undefined) {
                                            result = result + item.textContent;
                                        } else result = result + item.innerText;
                                    });
                                    return result;
                                }
                            }
                        }
                    },
                    {
                        extend: 'excel',
                        text: '<i class="fas fa-file-excel"> Excel',
                        className: 'dropdown-item',
                        exportOptions: {
                            columns: [1, 2, 3, 4, 5],
                            // prevent avatar to be display
                            format: {
                                body: function (inner, coldex, rowdex) {
                                    if (inner.length <= 0) return inner;
                                    var el = $.parseHTML(inner);
                                    var result = '';
                                    $.each(el, function (index, item) {
                                        if (item.classList !== undefined && item.classList.contains('user-name')) {
                                            result = result + item.lastChild.firstChild.textContent;
                                        } else if (item.innerText === undefined) {
                                            result = result + item.textContent;
                                        } else result = result + item.innerText;
                                    });
                                    return result;
                                }
                            }
                        }
                    },
                    {
                        extend: 'pdf',
                        text: '<i class="fas fa-file-pdf"> Pdf',
                        className: 'dropdown-item',
                        exportOptions: {
                            columns: [1, 2, 3, 4],
                            // prevent avatar to be display
                            format: {
                                body: function (inner, coldex, rowdex) {
                                    if (inner.length <= 0) return inner;
                                    var el = $.parseHTML(inner);
                                    var result = '';
                                    $.each(el, function (index, item) {
                                        if (item.classList !== undefined && item.classList.contains('user-name')) {
                                            result = result + item.lastChild.firstChild.textContent;
                                        } else if (item.innerText === undefined) {
                                            result = result + item.textContent;
                                        } else result = result + item.innerText;
                                    });
                                    return result;
                                }
                            }
                        }
                    },
                    {
                        extend: 'copy',
                        text: '<i class="fas fa-copy"> Copy',
                        className: 'dropdown-item',
                        exportOptions: {
                            columns: [1, 2, 3, 4],
                            // prevent avatar to be display
                            format: {
                                body: function (inner, coldex, rowdex) {
                                    if (inner.length <= 0) return inner;
                                    var el = $.parseHTML(inner);
                                    var result = '';
                                    $.each(el, function (index, item) {
                                        if (item.classList !== undefined && item.classList.contains('user-name')) {
                                            result = result + item.lastChild.firstChild.textContent;
                                        } else if (item.innerText === undefined) {
                                            result = result + item.textContent;
                                        } else result = result + item.innerText;
                                    });
                                    return result;
                                }
                            }
                        }
                    }
                ]
            }
        ],
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

    // Filter form control to default size
    // ? setTimeout used for multilingual table initialization
    setTimeout(() => {
        $('.dataTables_filter .form-control').removeClass('form-control-sm');
        $('.dataTables_length .custom-select').removeClass('custom-select-sm');
        $('.dt-action-buttons button:eq(0)').removeClass('btn-secondary');
    }, 300);
    return dt_orders;
};
function formReset(form) {
    $('#submitWait').hide();
    $(form).find("button[type='submit']").show();

    $('#captcha').val('');
    $('.input-captcha').removeClass('state-success');
    $('#captchaImg').removeAttr('src').attr('src', '/Home/Captcha');
}
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
});

$(document).ready(function () {
    dt_orders = initDataTable();

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
                        dt_orders.row.add(JSON.parse(result.data)).draw();
                    } else {
                        dt_orders.row('#' + idItem).data(JSON.parse(result.data)).draw()
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
                    dt_orders.row('#' + idItem).remove().draw();

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