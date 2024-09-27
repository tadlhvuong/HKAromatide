var dt_category, dt_collection, dt_vendor, dt_variant, rowId;

function initDataTable_Category() {
    var initData = null;
    $.ajax({
        type: "GET",
        url: "/Admin/Product/LoadCategoryList",
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
    if (initData != null) {
        initData = JSON.parse(initData.data);
    }
    dt_category = $('#table_categories').DataTable({
        data: initData,
        rowId: "Id",
        columns: [
            // columns according to JSON
            { data: '' },
            { data: '#' },
            { data: 'Name' },
            { data: 'ParentName' },
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
                    return '<span class="fw-medium">' + full['ParentName'] + '</span>';
                }
            },
            {
                // Actions
                targets: -1,
                searchable: false,
                orderable: false,
                responsivePriority: 4,
                render: function (data, type, full, meta) {
                    var url_U = "/Admin/Product/CategoryEdit/",
                        url_D = "/Admin/Product/CategoryDelete/";
                    return ('<div class="d-flex align-items-center">' +
                        '<a type="button" class="btn btn-warning btn-sm mr-2" data-modal="" onclick="formEdit(\'' + url_U +
                        + full["Id"] + '/2\'); " title="Sửa"><i class="fa fa-pencil-alt"></i><span class="d-none d-md-inline">  Sửa</span></a>' +
                        '<a type="button" class="btn btn-danger btn-sm" data-modal="" onclick="formDelete(\'' + url_D +
                        + full["Id"] + '/2\'); " title="Xóa"><i class="fa fa-trash"></i> <span class="d-none d-md-inline"> Xóa</span></a>' +
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
            searchPlaceholder: "Tìm kiếm...",
            info: "Hiện _START_/_END_. Tổng _TOTAL_ mục",
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
                    "onclick": "formEdit('/Admin/Product/CategoryCreate/2')"
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
        $('#table_categories_wrapper .dt-buttons button:eq(0)').removeClass('btn-secondary');
    }, 300);
    return dt_category;
};

function initDataTable_Collection() {
    var initData = null;
    $.ajax({
        type: "GET",
        url: "/Admin/Product/LoadCollectionList",
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
    if (initData != null) {
        initData = JSON.parse(initData.data);
    }
    dt_collection = $('#table_collections').DataTable({
        data: initData,
        rowId: "Id",
        columns: [
            // columns according to JSON
            { data: '' },
            { data: '#' },
            { data: 'Name' },
            { data: 'ParentName' },
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
                    return '<span class="fw-medium">' + full['ParentName'] + '</span>';
                }
            },
            {
                // Actions
                targets: -1,
                searchable: false,
                orderable: false,
                responsivePriority: 4,
                render: function (data, type, full, meta) {
                    var url_U = "/Admin/Product/CategoryEdit/",
                        url_D = "/Admin/Product/CategoryDelete/";
                    return ('<div class="d-flex align-items-center">' +
                        '<a type="button" class="btn btn-warning btn-sm mr-2" data-modal="" onclick="formEdit(\'' + url_U +
                        + full["Id"] + '/3\'); " title="Sửa"><i class="fa fa-pencil-alt"></i><span class="d-none d-md-inline"> Sửa</span></a>' +
                        '<a type="button" class="btn btn-danger btn-sm" data-modal="" onclick="formDelete(\'' + url_D +
                        + full["Id"] + '/3\'); " title="Xóa"><i class="fa fa-trash"></i> <span class="d-none d-md-inline"> Xóa</span></a>' +
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
            searchPlaceholder: "Tìm kiếm...",
            info: "Hiện _START_/_END_. Tổng _TOTAL_ mục",
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
                    "onclick": "formEdit('/Admin/Product/CategoryCreate/3')"
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
        $('#table_collections_wrapper .dt-buttons button:eq(0)').removeClass('btn-secondary');
    }, 300);
    return dt_collection;
};
function initDataTable_Vendor() {
    var initData = null;
    $.ajax({
        type: "GET",
        url: "/Admin/Product/LoadVendorList",
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
    if (initData != null) {
        initData = JSON.parse(initData.data);
    }
    dt_vendor = $('#table_vendors').DataTable({
        data: initData,
        rowId: "Id",
        columns: [
            // columns according to JSON
            { data: '' },
            { data: '#' },
            { data: 'Name' },
            { data: 'ParentName' },
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
                responsivePriority: 2,
                render: function (data, type, full, meta) {
                    return '<span class="fw-medium">' + full['Id'] + '</span>';
                }
            },
            {
                targets: 2,
                responsivePriority: 1,
                render: function (data, type, full, meta) {
                    return '<span class="fw-medium">' + full['Name'] + '</span>';
                }
            },
            {
                targets: 3,
                responsivePriority: 5,
                render: function (data, type, full, meta) {
                    return '<span class="fw-medium">' + full['ParentName'] + '</span>';
                }
            },
            {
                // Actions
                targets: -1,
                searchable: false,
                orderable: false,
                responsivePriority: 4,
                render: function (data, type, full, meta) {
                    var url_U = "/Admin/Product/CategoryEdit/",
                        url_D = "/Admin/Product/CategoryDelete/";
                    return ('<div class="d-flex align-items-center">' +
                        '<a type="button" class="btn btn-warning btn-sm mr-2" data-modal="" onclick="formEdit(\'' + url_U +
                        + full["Id"] + '/4\'); " title="Sửa"><i class="fa fa-pencil-alt"></i><span class="d-none d-md-inline"> Sửa</span></a>' +
                        '<a type="button" class="btn btn-danger btn-sm" data-modal="" onclick="formDelete(\'' + url_D +
                        + full["Id"] + '/4\'); " title="Xóa"><i class="fa fa-trash"></i> <span class="d-none d-md-inline"> Xóa</span></a>' +
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
            searchPlaceholder: "Tìm kiếm...",
            info: "Hiện _START_/_END_. Tổng _TOTAL_ mục",
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
                    "onclick": "formEdit('/Admin/Product/CategoryCreate/4')"
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
        $('#table_vendors_wrapper .dt-buttons button:eq(0)').removeClass('btn-secondary');
    }, 300);
    return dt_vendor;
};

function initDataTable_Variant() {
    var initData = null;
    $.ajax({
        type: "GET",
        url: "/Admin/Product/LoadVariantList",
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
    if (initData != null) {
        initData = JSON.parse(initData.data);
    }
    dt_variant = $('#table_variants').DataTable({
        data: initData,
        rowId: "Id",
        columns: [
            // columns according to JSON
            { data: '' },
            { data: '#' },
            { data: 'Name' },
            { data: 'ParentName' },
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
                responsivePriority: 2,
                render: function (data, type, full, meta) {
                    return '<span class="fw-medium">' + full['Id'] + '</span>';
                }
            },
            {
                targets: 2,
                responsivePriority: 1,
                render: function (data, type, full, meta) {
                    return '<span class="fw-medium">' + full['Name'] + '</span>';
                }
            },
            {
                targets: 3,
                responsivePriority: 5,
                render: function (data, type, full, meta) {
                    return '<span class="fw-medium">' + full['ParentName'] + '</span>';
                }
            },
            {
                // Actions
                targets: -1,
                searchable: false,
                orderable: false,
                responsivePriority: 4,
                render: function (data, type, full, meta) {
                    var url_U = "/Admin/Product/CategoryEdit/",
                        url_D = "/Admin/Product/CategoryDelete/";
                    return ('<div class="d-flex align-items-center">' +
                        '<a type="button" class="btn btn-warning btn-sm mr-2" data-modal="" onclick="formEdit(\'' + url_U +
                        + full["Id"] + '/6\'); " title="Sửa"><i class="fa fa-pencil-alt"></i><span class="d-none d-md-inline"> Sửa</span></a>' +
                        '<a type="button" class="btn btn-danger btn-sm" data-modal="" onclick="formDelete(\'' + url_D +
                        + full["Id"] + '/6\'); " title="Xóa"><i class="fa fa-trash"></i> <span class="d-none d-md-inline"> Xóa</span></a>' +
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
            searchPlaceholder: "Tìm kiếm...",
            info: "Hiện _START_/_END_. Tổng _TOTAL_ mục",
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
                    "onclick": "formEdit('/Admin/Product/CategoryCreate/6')"
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
        $('#table_variants_wrapper .dt-buttons button:eq(0)').removeClass('btn-secondary');
    }, 300);
    return dt_variant;
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
    modalContent.find('.modal-title').text('Xóa album');

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
});

$(document).ready(function () {
    dt_category = initDataTable_Category();
    dt_collection = initDataTable_Collection();
    dt_vendor = initDataTable_Vendor();
    dt_variant = initDataTable_Variant();

    //edit data table
    $(document).on("submit", ".modalForm", function (e) {
        e.preventDefault();
        var idItem = $(this).find("input#Id").val();
        $("#modalContent").removeClass('bg-danger');
        $("#modalContainer").modal("show");
        $(this).find("button[type='submit']").hide();
        $("#progress").show();
        var url = this.action.split("/");
        var typeCate = url[url.length - 1].slice(0, 1);
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
                    console.log(result.data);

                    if (idItem == 0) {
                        if (typeCate == 2)
                            dt_category.row.add(JSON.parse(result.data)).draw();
                        else if (typeCate == 3)
                            dt_collection.row.add(JSON.parse(result.data)).draw();
                        else if (typeCate == 4)
                            dt_vendor.row.add(JSON.parse(result.data)).draw();
                        else if (typeCate == 6)
                            dt_variant.row.add(JSON.parse(result.data)).draw();
                    } else {
                        console.log(result.data);
                        if (typeCate == 2)
                            dt_category.row('#' + idItem).data(JSON.parse(result.data)).draw();
                        else if (typeCate == 3)
                            dt_collection.row('#' + idItem).data(JSON.parse(result.data)).draw();
                        else if (typeCate == 4)
                            dt_vendor.row('#' + idItem).data(JSON.parse(result.data)).draw();
                        else if (typeCate == 6)
                            dt_variant.row('#' + idItem).data(JSON.parse(result.data)).draw();
                    }
                    toastr.success(result.message);

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

    //Remove data table
    $(document).on("submit", ".modalFormDelete", function (e) {
        e.preventDefault();
        var idItem = $(this).find("input").val();
        $(this).find("button[type='submit']").hide();
        $("#progress").show();
        var url = this.action.split("/");
        var typeCate = url[url.length - 1].slice(0, 1);
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
                    if (typeCate == 2)
                        dt_category.row('#' + idItem).remove().draw();
                    else if (typeCate == 3)
                        dt_collection.row('#' + idItem).remove().draw();
                    else if (typeCate == 4)
                        dt_vendor.row('#' + idItem).remove().draw();
                    else if (typeCate == 6)
                        dt_variant.row('#' + idItem).remove().draw();

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