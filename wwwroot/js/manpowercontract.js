// ================================================================
// MANPOWER CONTRACT MANAGEMENT — JS
// Stack: jQuery, DataTables, Select2, SweetAlert2, Bootstrap 5
// ================================================================

var contractTable;
var csrfToken = $('input[name="__RequestVerificationToken"]').val();

// ── PAGE LOAD ────────────────────────────────────────────────────
$(document).ready(function () {
    InitDropdowns();
    LoadGrid();
    BindColumnSearch();

    // Cascading: Group Company → Plant
    $(document).on('change', '#groupCompanyId', function () {
        LoadPlantsByGroupCompany($(this).val(), '#plantId');
    });

    // Cascading filter: Group Company filter → Plant filter
    $('#filterGroupCompany').on('change', function () {
        LoadPlantsByGroupCompany($(this).val(), '#filterPlant');
    });
});

// ── INIT DROPDOWNS ───────────────────────────────────────────────
function InitDropdowns() {
    // Initialise Select2 on filter panel
    $('.select2').select2({ placeholder: 'Select', allowClear: true, width: '100%' });

    // Populate modal dropdowns from server-passed data
    PopulateSelect('#groupCompanyId', dropdownData.groupCompany);
    PopulateSelect('#supplierId', dropdownData.supplier);
    PopulateSelect('#currencyId', dropdownData.currency);
    PopulateSelect('#month', dropdownData.month);
    PopulateSelect('#workerType', dropdownData.workerType);

    // Initialise Select2 on modal elements
    $('.select2-modal').select2({ dropdownParent: $('#contractModal'), placeholder: 'Select', allowClear: true, width: '100%' });
}

function PopulateSelect(selector, items) {
    var $sel = $(selector);
    items.forEach(function (item) {
        $sel.append(new Option(item.text, item.value));
    });
}

function LoadPlantsByGroupCompany(groupCompanyId, targetSelector) {
    $.get('/ManpowerContract/GetPlants', { groupCompanyId: groupCompanyId || '' }, function (data) {
        var $target = $(targetSelector).empty().append('<option value="">Select Plant</option>');
        data.forEach(function (item) {
            $target.append(
                $('<option>', {
                    value: item.value,
                    text: item.text
                }).attr('data-country', item.country || '')
            );
        });
        $target.trigger('change.select2');
    });
}

// Auto-fill Plant Country whenever Plant dropdown changes (modal)
$(document).on('change', '#plantId', function () {
    var selectedOption = $(this).find('option:selected');
    var country = selectedOption.attr('data-country') || '';
    $('#plantCountry').val(country);
});

// ── FILTER STATE — shared between Search and Grid ────────────────
var activeFilters = {};

// ── DATATABLE GRID — SERVER-SIDE PAGING ──────────────────────────
function LoadGrid(filters) {

    // Store filters so DataTables uses them on every page/sort/search request
    if (filters !== undefined) activeFilters = filters || {};

    if ($.fn.DataTable.isDataTable('#contractTable')) {
        $('#contractTable').DataTable().destroy();
        $('#contractTable tbody').empty();
    }

    contractTable = $('#contractTable').DataTable({

        // ── Server-side mode: SQL does paging, only 1 page comes back ──
        serverSide: true,
        processing: true,

        ajax: {
            url: '/ManpowerContract/GetAll',
            type: 'GET',
            dataType: 'json',
            data: function (d) {
                return {
                    draw: d.draw,
                    start: d.start,
                    length: d.length,
                    searchValue: d.search ? d.search.value : '',
                    orderColumnIndex: d.order && d.order[0] ? d.order[0].column : 0,
                    orderDir: d.order && d.order[0] ? d.order[0].dir : 'desc',
                    groupCompanyId: activeFilters.groupCompanyId || '',
                    plantId: activeFilters.plantId || '',
                    supplierId: activeFilters.supplierId || '',
                    supplierCountry: activeFilters.supplierCountry || '',
                    contracted: activeFilters.contracted || '',
                    workerType: activeFilters.workerType || ''
                };
            },
            // dataSrc tells DataTables which key in the JSON holds the rows array
            dataSrc: function (json) {
                // Debug: log the raw response so we can see exact key names
                console.log('DataTables response:', json);
                // Support both camelCase (data) returned by controller
                return json.data || json.Data || [];
            },
            error: function (xhr, error, thrown) {
                console.error('DataTables AJAX error:', xhr.responseText);
                ShowToast('Failed to load data. Please try again.', 'error');
            }
        },

        columns: [
            {
                data: 'contractId', orderable: false,
                render: function (id) {
                    return '<a href="javascript:void(0)" class="text-primary me-1" onclick="EditRecord(' + id + ')">Edit</a>' +
                        ' <span class="text-muted">|</span> ' +
                        '<a href="javascript:void(0)" class="text-danger ms-1" onclick="ViewHistory(' + id + ')">Hist</a>';
                }
            },
            { data: 'year' },
            { data: 'month' },
            { data: 'rco' },
            { data: 'division' },
            { data: 'groupCompanyName', defaultContent: '' },
            { data: 'plantName', defaultContent: '' },
            { data: 'plantCountry', defaultContent: '' },
            { data: 'globalSupplierName', defaultContent: '' },
            { data: 'erpSupplierName', defaultContent: '' },
            { data: 'supplierNameRemarks', defaultContent: '', orderable: false },
            { data: 'supplierCountry', defaultContent: '', orderable: false },
            {
                data: 'annualSupplierSpend',
                render: function (val) {
                    return val ? parseFloat(val).toLocaleString() : '0';
                }
            },
            { data: 'currencyCode', defaultContent: '' },
            {
                data: 'contracted',
                render: function (val) {
                    var cls = val === 'Yes' ? 'bg-success' : 'bg-secondary';
                    return '<span class="badge ' + cls + '">' + (val || 'No') + '</span>';
                }
            },
            { data: 'contractedStartDate', defaultContent: '' },
            { data: 'contractedEndDate', defaultContent: '' }
        ],

        pageLength: 10,
        lengthMenu: [10, 25, 50, 100],

        // Hide default search box — we use our own filter panel
        dom: 'rtip',

        scrollX: true,

        language: {
            processing: '<div class="spinner-border spinner-border-sm text-secondary"></div> Loading...',
            info: '_START_ - _END_ of _TOTAL_ records',
            infoEmpty: '0 records',
            paginate: { first: '|◄', previous: '◄ Prev', next: 'Next ►', last: '►|' }
        },

        initComplete: function () {
            BindColumnSearch();
        }
    });
}

// ── COLUMN-LEVEL SEARCH (server-side aware) ───────────────────────
function BindColumnSearch() {
    $('#contractTable .col-search').each(function (i) {
        var colIndex = i + 1;
        var debounceTimer;
        $(this).off('keyup').on('keyup', function () {
            var val = this.value;
            clearTimeout(debounceTimer);
            // Debounce 400ms so we don't fire on every keystroke
            debounceTimer = setTimeout(function () {
                contractTable.column(colIndex).search(val).draw();
            }, 400);
        });
    });
}

// ── SEARCH / FILTER ──────────────────────────────────────────────
function SearchRecords() {
    var filters = {
        groupCompanyId: $('#filterGroupCompany').val() || null,
        plantId: $('#filterPlant').val() || null,
        supplierId: $('#filterSupplier').val() || null,
        supplierCountry: $('#filterSupplierCountry').val() || null,
        contracted: $('#filterContracted').val() || null,
        workerType: $('#filterWorkerType').val() || null
    };
    // Pass filters and rebuild — server will apply them + reset to page 1
    LoadGrid(filters);
}

function ClearFilters() {
    $('#filterGroupCompany, #filterPlant, #filterSupplier, ' +
        '#filterSupplierCountry, #filterContracted, #filterWorkerType')
        .val(null).trigger('change');
    LoadGrid({});
}

// ── OPEN ADD MODAL ───────────────────────────────────────────────
function OpenAddModal() {
    ResetForm();
    $('#modalTitleText').text('Add New Contract');
    $('#contractModal').modal('show');
}

// ── OPEN EDIT MODAL ──────────────────────────────────────────────
function EditRecord(id) {
    ShowLoader(true);
    $.get('/ManpowerContract/GetById', { id: id }, function (res) {
        ShowLoader(false);
        if (!res.success) { ShowToast(res.message, 'error'); return; }

        var d = res.data;
        ResetForm();
        $('#modalTitleText').text('Edit Contract');

        $('#contractId').val(d.contractId);
        $('#year').val(d.year);
        SetSelect2('#month', d.month);
        $('#rco').val(d.rco);
        $('#division').val(d.division);
        SetSelect2('#groupCompanyId', d.groupCompanyId);

        // Load plants for this group company then set value + auto-fill country
        LoadPlantsByGroupCompany(d.groupCompanyId, '#plantId');
        setTimeout(function () {
            SetSelect2('#plantId', d.plantId);
            // Trigger change so country auto-fills from data-country attribute
            $('#plantId').trigger('change');
            // Fallback: if data-country not yet on option, set directly
            if (!$('#plantCountry').val()) {
                $('#plantCountry').val(d.plantCountry);
            }
        }, 450);

        $('#plantCountry').val(d.plantCountry);
        SetSelect2('#supplierId', d.supplierId);
        $('#globalSupplierName').val(d.globalSupplierName);
        $('#supplierNameRemarks').val(d.supplierNameRemarks);
        $('#supplierCountry').val(d.supplierCountry);
        $('#annualSupplierSpend').val(d.annualSupplierSpend);
        SetSelect2('#currencyId', d.currencyId);
        SetSelect2('#contracted', d.contracted);
        SetSelect2('#workerType', d.workerType);
        $('#contractedStartDate').val(FormatDateInput(d.contractedStartDate));
        $('#contractedEndDate').val(FormatDateInput(d.contractedEndDate));

        $('#contractModal').modal('show');
    }).fail(function () {
        ShowLoader(false);
        ShowToast('Failed to fetch record.', 'error');
    });
}

// ── SAVE ─────────────────────────────────────────────────────────
function SaveRecord() {
    if (!ValidateForm()) return;

    var payload = {
        contractId: parseInt($('#contractId').val()) || 0,
        year: parseInt($('#year').val()),
        month: $('#month').val(),
        rco: $('#rco').val().trim(),
        division: $('#division').val().trim(),
        groupCompanyId: parseInt($('#groupCompanyId').val()),
        plantId: parseInt($('#plantId').val()),
        plantCountry: $('#plantCountry').val(),
        supplierId: parseInt($('#supplierId').val()),
        globalSupplierName: $('#globalSupplierName').val().trim(),
        supplierNameRemarks: $('#supplierNameRemarks').val().trim(),
        supplierCountry: $('#supplierCountry').val().trim(),
        annualSupplierSpend: parseFloat($('#annualSupplierSpend').val()) || 0,
        currencyId: parseInt($('#currencyId').val()),
        contracted: $('#contracted').val() || 'No',
        workerType: $('#workerType').val(),
        contractedStartDate: $('#contractedStartDate').val(),
        contractedEndDate: $('#contractedEndDate').val()
    };

    ShowLoader(true);
    $.ajax({
        url: '/ManpowerContract/Save',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(payload),
        headers: { 'RequestVerificationToken': csrfToken },
        success: function (res) {
            ShowLoader(false);
            if (res.success) {
                $('#contractModal').modal('hide');
                ShowToast(res.message, 'success');
                LoadGrid();
            } else {
                ShowToast(res.message, 'error');
            }
        },
        error: function () {
            ShowLoader(false);
            ShowToast('An unexpected error occurred.', 'error');
        }
    });
}

// ── DELETE ───────────────────────────────────────────────────────
function DeleteRecord(id) {
    Swal.fire({
        title: 'Are you sure?',
        text: 'This record will be deleted.',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#6c757d',
        confirmButtonText: 'Yes, delete it!'
    }).then(function (result) {
        if (result.isConfirmed) {
            ShowLoader(true);
            $.ajax({
                url: '/ManpowerContract/Delete',
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(id),
                headers: { 'RequestVerificationToken': csrfToken },
                success: function (res) {
                    ShowLoader(false);
                    ShowToast(res.message, res.success ? 'success' : 'error');
                    if (res.success) LoadGrid();
                },
                error: function () {
                    ShowLoader(false);
                    ShowToast('Delete failed.', 'error');
                }
            });
        }
    });
}

function ViewHistory(id) {
    // Placeholder — implement history modal as needed
    Swal.fire('History', 'History for Contract ID: ' + id, 'info');
}

// ── IMPORT EXCEL ─────────────────────────────────────────────────
function ImportExcel() {
    $('#importFileInput').click();
}

// ── EXPORT TO EXCEL ──────────────────────────────────────────────
function ExportToExcel() {
    var params = new URLSearchParams({
        groupCompanyId: $('#filterGroupCompany').val() || '',
        plantId: $('#filterPlant').val() || '',
        supplierId: $('#filterSupplier').val() || '',
        supplierCountry: $('#filterSupplierCountry').val() || '',
        contracted: $('#filterContracted').val() || '',
        workerType: $('#filterWorkerType').val() || ''
    });
    window.location.href = '/ManpowerContract/ExportExcel?' + params.toString();
}

// ── FORM VALIDATION ──────────────────────────────────────────────
function ValidateForm() {
    var isValid = true;
    var msg = [];

    if (!$('#year').val()) { msg.push('Year is required.'); isValid = false; }
    if (!$('#month').val()) { msg.push('Month is required.'); isValid = false; }
    if (!$('#rco').val().trim()) { msg.push('RCO is required.'); isValid = false; }
    if (!$('#division').val().trim()) { msg.push('Division is required.'); isValid = false; }
    if (!$('#groupCompanyId').val()) { msg.push('Group Company is required.'); isValid = false; }
    if (!$('#plantId').val()) { msg.push('Plant Name is required.'); isValid = false; }
    if (!$('#supplierId').val()) { msg.push('ERP Supplier Name is required.'); isValid = false; }
    if (!$('#annualSupplierSpend').val()) { msg.push('Annual Supplier Spend is required.'); isValid = false; }
    if (!$('#currencyId').val()) { msg.push('Currency is required.'); isValid = false; }
    if (!$('#contractedStartDate').val()) { msg.push('Contracted Start Date is required.'); isValid = false; }
    if (!$('#contractedEndDate').val()) { msg.push('Contracted End Date is required.'); isValid = false; }

    if ($('#contractedStartDate').val() && $('#contractedEndDate').val()) {
        if (new Date($('#contractedEndDate').val()) <= new Date($('#contractedStartDate').val())) {
            msg.push('End Date must be after Start Date.');
            isValid = false;
        }
    }

    if (!isValid) {
        Swal.fire({ icon: 'warning', title: 'Validation', html: msg.map(m => '• ' + m).join('<br/>') });
    }
    return isValid;
}

// ── RESET FORM ───────────────────────────────────────────────────
function ResetForm() {
    $('#contractForm')[0].reset();
    $('#contractId').val(0);
    $('#plantCountry').val('');   // clear auto-filled field
    $('.select2-modal').val(null).trigger('change');
    $('.is-invalid').removeClass('is-invalid');
}

// ── TOAST ─────────────────────────────────────────────────────────
function ShowToast(message, type) {
    var icons = { success: 'success', error: 'error', warning: 'warning', info: 'info' };
    Swal.fire({
        toast: true,
        position: 'top-end',
        icon: icons[type] || 'info',
        title: message,
        showConfirmButton: false,
        timer: 3000,
        timerProgressBar: true
    });
}

// ── LOADER ────────────────────────────────────────────────────────
function ShowLoader(show) {
    if (show) {
        if (!$('#globalLoader').length) {
            $('body').append('<div id="globalLoader" style="position:fixed;top:0;left:0;width:100%;height:100%;background:rgba(0,0,0,.25);z-index:9999;display:flex;align-items:center;justify-content:center;">' +
                '<div class="spinner-border text-light" role="status"><span class="visually-hidden">Loading...</span></div></div>');
        }
        $('#globalLoader').show();
    } else {
        $('#globalLoader').hide();
    }
}

// ── HELPERS ───────────────────────────────────────────────────────
function FormatDate(dateStr) {
    if (!dateStr) return '';
    var d = new Date(dateStr);
    var dd = String(d.getDate()).padStart(2, '0');
    var mm = String(d.getMonth() + 1).padStart(2, '0');
    return dd + '-' + mm + '-' + d.getFullYear();
}

function FormatDateInput(dateStr) {
    if (!dateStr) return '';
    var d = new Date(dateStr);
    return d.toISOString().split('T')[0];
}

function SetSelect2(selector, value) {
    $(selector).val(value).trigger('change.select2');
}