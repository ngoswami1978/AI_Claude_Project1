// ═══════════════════════════════════════════════════════════════
// User Management JS
// ═══════════════════════════════════════════════════════════════

$(document).ready(function () {
    searchUsers();
});

function searchUsers() {
    var dto = {
        searchText: $('#txtSearch').val(),
        roleId: $('#ddlRoleFilter').val() ? parseInt($('#ddlRoleFilter').val()) : null,
        departmentId: $('#ddlDeptFilter').val() ? parseInt($('#ddlDeptFilter').val()) : null,
        isActive: $('#ddlStatusFilter').val() !== '' ? ($('#ddlStatusFilter').val() === 'true') : null
    };

    apiCall('/UserManagement/Search', 'POST', dto, function (result) {
        renderUsers(result.data || []);
    });
}

function renderUsers(users) {
    var $tbody = $('#tbodyUsers');
    $tbody.empty();

    if (!users || users.length === 0) {
        $tbody.html('<tr><td colspan="7" class="text-center text-muted py-4">No users found</td></tr>');
        return;
    }

    $.each(users, function (i, u) {
        var statusBadge = u.isActive
            ? '<span class="badge-active">Active</span>'
            : '<span class="badge-inactive">Inactive</span>';

        var row =
            '<tr>' +
                '<td>' + (i + 1) + '</td>' +
                '<td>' + escapeHtml(u.fullName) + '</td>' +
                '<td>' + escapeHtml(u.email) + '</td>' +
                '<td>' + escapeHtml(u.roleName || '') + '</td>' +
                '<td>' + escapeHtml(u.departmentName || '') + '</td>' +
                '<td>' + statusBadge + '</td>' +
                '<td>' +
                    '<button class="action-btn action-btn-edit me-1" onclick="editUser(' + u.userId + ')" title="Edit"><i class="fas fa-pen"></i></button>' +
                    '<button class="action-btn action-btn-delete" onclick="deleteUser(' + u.userId + ', \'' + escapeHtml(u.fullName) + '\')" title="Delete"><i class="fas fa-trash"></i></button>' +
                '</td>' +
            '</tr>';
        $tbody.append(row);
    });
}

function clearFilters() {
    $('#txtSearch').val('');
    $('#ddlRoleFilter').val('');
    $('#ddlDeptFilter').val('');
    $('#ddlStatusFilter').val('');
    searchUsers();
}

function openCreateModal() {
    $('#userModalTitle').text('Add User');
    $('#hdnUserId').val('');
    $('#txtFullName').val('');
    $('#txtEmail').val('').prop('readonly', false);
    $('#ddlRole').val('');
    $('#ddlDept').val('');
    $('#txtPassword').val('');
    $('#txtMobile').val('');
    $('#txtEmpCode').val('');
    $('#chkActive').prop('checked', true);
    $('#passwordGroup').show();
    $('#statusGroup').hide();
    new bootstrap.Modal('#userModal').show();
}

function editUser(id) {
    apiCall('/UserManagement/GetById?id=' + id, 'GET', null, function (result) {
        var u = result.data;
        $('#userModalTitle').text('Edit User');
        $('#hdnUserId').val(u.userId);
        $('#txtFullName').val(u.fullName);
        $('#txtEmail').val(u.email).prop('readonly', true);
        $('#ddlRole').val(u.roleId);
        $('#ddlDept').val(u.departmentId);
        $('#txtMobile').val(u.mobile || '');
        $('#txtEmpCode').val(u.employeeCode || '');
        $('#chkActive').prop('checked', u.isActive);
        $('#passwordGroup').hide();
        $('#statusGroup').css('display', '').show();
        new bootstrap.Modal('#userModal').show();
    });
}

function saveUser() {
    var userId = $('#hdnUserId').val();
    var isEdit = userId !== '';

    var fullName = $('#txtFullName').val().trim();
    var email = $('#txtEmail').val().trim();
    var roleId = $('#ddlRole').val();
    var departmentId = $('#ddlDept').val();

    if (!fullName) { showToast('Full Name is required.', 'error'); return; }
    if (!email) { showToast('Email is required.', 'error'); return; }
    if (!roleId) { showToast('Role is required.', 'error'); return; }
    if (!departmentId) { showToast('Department is required.', 'error'); return; }

    if (isEdit) {
        var updateDto = {
            userId: parseInt(userId),
            fullName: fullName,
            roleId: parseInt(roleId),
            departmentId: parseInt(departmentId),
            mobile: $('#txtMobile').val().trim(),
            employeeCode: $('#txtEmpCode').val().trim(),
            isActive: $('#chkActive').is(':checked')
        };
        apiCall('/UserManagement/Update', 'POST', updateDto, function () {
            showToast('User updated successfully.');
            bootstrap.Modal.getInstance(document.getElementById('userModal')).hide();
            searchUsers();
        });
    } else {
        var password = $('#txtPassword').val();
        if (!password || password.length < 6) { showToast('Password must be at least 6 characters.', 'error'); return; }

        var createDto = {
            fullName: fullName,
            email: email,
            password: password,
            roleId: parseInt(roleId),
            departmentId: parseInt(departmentId),
            mobile: $('#txtMobile').val().trim(),
            employeeCode: $('#txtEmpCode').val().trim()
        };
        apiCall('/UserManagement/Create', 'POST', createDto, function () {
            showToast('User created successfully.');
            bootstrap.Modal.getInstance(document.getElementById('userModal')).hide();
            searchUsers();
        });
    }
}

function deleteUser(id, name) {
    if (!confirm('Are you sure you want to delete user "' + name + '"?')) return;

    apiCall('/UserManagement/Delete?id=' + id, 'POST', null, function () {
        showToast('User deleted successfully.');
        searchUsers();
    });
}

function escapeHtml(text) {
    if (!text) return '';
    var div = document.createElement('div');
    div.appendChild(document.createTextNode(text));
    return div.innerHTML;
}
