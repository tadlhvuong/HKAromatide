var dt_users, btnAdd, url_CRUD, url_R, url_C, url_U, url_D, columnsDT, rowId, Columns_Def;

var loadAction = "/Admin/Member/LoadMemberList"
    detailsAction = "/Admin/thanh-vien/chi-tiet/",
    detailsPageAction = "/Admin/thanh-vien/chi-tiet/",
    editAction = "/Admin/thanh-vien/chinh-sua/";

var statusObj = {
    0: { title: 'Inactive', class: 'bg-secondary' },
    1: { title: 'Active', class: 'bg-success' },
    2: { title: 'Suspend', class: 'bg-warning' },
}, roleObj = {
    0: { class: 'bg-success' },
    1: { class: 'bg-info' },
    2: { class: 'bg-warning' },
    3: { class: 'bg-purple' },
};

var isConfirm = [
    '',
    '<i style="color: green;" class="icon fas fa-check-circle float-right mt-1 ml-1" title="Đã xác thực">',
    ' <i style="color: red;" class="icon fas fa-times-circle float-right mt-1 ml-1" title="Chưa xác thực">'
]

var initData = null, getData = null;
function initDataTable() {
    $.ajax({
        type: "GET",
        url: "/Admin/Member/LoadMemberList",
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
    dt_users = $('#table_users').DataTable({
        data: initData,
        rowId: "Id",
        columns: [
            { data: '' },
            { data: 'UserName' },
            { data: 'RoleName' },
            { data: 'Status' },
            { data: 'action' }
        ],
        columnDefs: [
            {
                // For Responsive
                className: 'control',
                searchable: false,
                orderable: false,
                responsivePriority: 2,
                targets: 0,
                render: function (data, type, full, meta) {
                    return '';
                }
            },
            {
                targets: 1,
                responsivePriority: 1,
                render: function (data, type, full, meta) {
                    var $name = full['UserName'],
                        $email = full['Email'],
                        $image = (full['AvatarImg'] != null) ? full['AvatarImg'] : "/images/logo.webp",
                        $isConfirmEmail = (full['EmailConfirmed']) ? isConfirm[0] : isConfirm[1];
                    var $output =
                        '<img src="' + $image +'" alt="Avatar" class="img-circle elevation-2 table-avatar">';
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
                        detailsAction +
                        full["Id"]  +
                        '" class="text-body text-truncate"><span class="fw-medium">' +
                        $name +
                        '</span></a>' +
                        '<small class="text-muted">' +
                        $email + $isConfirmEmail +
                        '</small>' +
                        '</div>' +
                        '</div>';
                    return $row_output;
                }
            },
            {
                targets: 2,
                render: function (data, type, full, meta) {
                    var $RoleName = (full['Roles'] == null) ? "" : full['Roles'];
                    var htmlRole = "";
                    for (var i = 0; i < $RoleName.length; i++) {
                        var temp = 0;
                        if (i > 3) temp = 0;
                        else temp = i;
                        htmlRole += ' <span class="badge ' +
                            roleObj[temp].class +
                            '"text-capitalized>' +
                            $RoleName[temp] +
                            '</span> '
                    }
                    return (
                        '<div class="form-group" style="margin:0 auto; text-align: center">' +
                        htmlRole + 
                        '</div>'
                    );
                }
            },
            {
                // User Status
                targets: 3,
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
                responsivePriority: 1,
                render: function (data, type, full, meta) {
                    var $status = full['Status'];
                    var $btnSuspend = null;
                    if ($status == 1)
                        $btnSuspend = '<a id="suspendAccount" type="button" class="btn btn-danger btn-sm" onclick="formSuspend(\'/Admin/Member/SuspendAccount/'
                            + full['Id'] + '\'); " title="Khóa"><i class="fa fa-ban"></i> <span class="d-none d-md-inline"> Khóa</span></a>';
                    if ($status == 2)
                        $btnSuspend = '<a id="suspendAccount" type="button" class="btn btn-success btn-sm" onclick="formUnSuspend(\'/Admin/Member/UnSuspendAccount/'
                            + full['Id'] + '\'); " title="Mở"><i class="fa fa-unlock-alt"></i> <span class="d-none d-md-inline"> Mở</span></a>';
                    
                    return ('<div class="d-flex align-items-center">' +
                        '<a type="button" class="btn btn-warning btn-sm mr-2" onclick="location.href = (\'' + detailsPageAction +
                        full["Id"] + '\'); " title="Chi tiết"><i class="fa fa-address-card"></i><span class="d-none d-md-inline"> Chi tiết</span></a>' +
                        $btnSuspend +
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
                    "data-modal": "",
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
                        return 'Details of ' + data['UserName'];
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

                    return data ? $('<table class="table projects"/><tbody />').append(data) : false;
                }
            },

        },
        initComplete: function () {
        }
    });

    //$('.dataTables_filter .form-control').removeClass('form-control-sm');
    //$('.dataTables_length .custom-select').removeClass('custom-select-sm');
    //$('.dt-action-buttons button:eq(0)').removeClass('btn-secondary');

    // Filter form control to default size
    // ? setTimeout used for multilingual table initialization
    setTimeout(() => {
        $('.dataTables_filter .form-control').removeClass('form-control-sm');
        $('.dataTables_length .custom-select').removeClass('custom-select-sm');
        $('.dt-action-buttons button:eq(0)').removeClass('btn-secondary');
    }, 300);
    return dt_users;
};
function formSuspend(href) {
    modalContent = $('#modalContent');
    modalContent.addClass('bg-danger');
    modalContent.find('.modal-title').text('Suspend');

    modalContent.load(href, function () {
        $('#modalContainer').modal({
            keyboard: true,
        });
        $("form").removeData("validator");
        $("form").removeData("unobtrusiveValidation");
        $.validator.unobtrusive.parse("form");
    });
}
function formUnSuspend(href) {
    modalContent = $('#modalContent');
    modalContent.addClass('bg-success');
    modalContent.find('.modal-title').text('UnSuspend');

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
        });
    }
});

$(document).ready(function () {
    dt_users = initDataTable();

    $(document).on("submit", ".modalSuspend", function (e) {
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
                    dt_users.row('#' + idItem).data(JSON.parse(result.data)).draw()

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