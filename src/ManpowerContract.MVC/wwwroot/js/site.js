// ═══════════════════════════════════════════════════════════════
// Manpower Contract Management - Global JS
// ═══════════════════════════════════════════════════════════════

// ── Sidebar Toggle ──
$(document).ready(function () {
    $('#sidebarOpen').on('click', function () {
        $('#sidebar').addClass('show');
        $('#sidebarOverlay').addClass('show');
    });

    $('#sidebarClose, #sidebarOverlay').on('click', function () {
        $('#sidebar').removeClass('show');
        $('#sidebarOverlay').removeClass('show');
    });
});

// ── Toast Notification ──
function showToast(message, type) {
    type = type || 'success';
    var bgClass = type === 'success' ? 'bg-success' : (type === 'error' ? 'bg-danger' : 'bg-warning');
    var icon = type === 'success' ? 'fa-check-circle' : (type === 'error' ? 'fa-times-circle' : 'fa-exclamation-triangle');

    var toastHtml =
        '<div class="toast align-items-center text-white ' + bgClass + ' border-0 show" role="alert">' +
            '<div class="d-flex">' +
                '<div class="toast-body"><i class="fas ' + icon + ' me-2"></i>' + message + '</div>' +
                '<button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>' +
            '</div>' +
        '</div>';

    var $container = $('#toastContainer');
    if ($container.length === 0) {
        $('body').append('<div class="toast-container" id="toastContainer"></div>');
        $container = $('#toastContainer');
    }

    var $toast = $(toastHtml);
    $container.append($toast);

    setTimeout(function () {
        $toast.fadeOut(300, function () { $(this).remove(); });
    }, 4000);
}

// ── API Helper ──
function apiCall(url, method, data, onSuccess, onError) {
    var options = {
        url: url,
        type: method || 'GET',
        contentType: 'application/json',
        dataType: 'json',
        success: function (result) {
            if (result.success) {
                if (onSuccess) onSuccess(result);
            } else {
                showToast(result.message || 'Operation failed.', 'error');
                if (onError) onError(result);
            }
        },
        error: function (xhr) {
            if (xhr.status === 401) {
                window.location.href = '/Account/Login';
                return;
            }
            showToast('Server error. Please try again.', 'error');
            if (onError) onError(xhr);
        }
    };

    if (data && (method === 'POST' || method === 'PUT')) {
        options.data = JSON.stringify(data);
    }

    $.ajax(options);
}
