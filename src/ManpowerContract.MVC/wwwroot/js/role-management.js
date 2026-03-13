// ═══════════════════════════════════════════════════════════════
// Role Management JS
// ═══════════════════════════════════════════════════════════════

$(document).ready(function () {
    searchRoles();
});

function searchRoles() {
    var dto = {
        searchText: $('#txtSearch').val(),
        isActive: $('#ddlStatusFilter').val() !== '' ? ($('#ddlStatusFilter').val() === 'true') : null
    };

    apiCall('/RoleManagement/Search', 'POST', dto, function (result) {
        renderRoles(result.data || []);
    });
}

function renderRoles(roles) {
    var $tbody = $('#tbodyRoles');
    $tbody.empty();

    if (!roles || roles.length === 0) {
        $tbody.html('<tr><td colspan="5" class="text-center text-muted py-4">No roles found</td></tr>');
        return;
    }

    $.each(roles, function (i, r) {
        var statusBadge = r.isActive
            ? '<span class="badge-active">Active</span>'
            : '<span class="badge-inactive">Inactive</span>';

        var row =
            '<tr>' +
                '<td>' + (i + 1) + '</td>' +
                '<td>' + escapeHtml(r.roleName) + '</td>' +
                '<td>' + escapeHtml(r.description || '') + '</td>' +
                '<td>' + statusBadge + '</td>' +
                '<td>' +
                    '<button class="action-btn action-btn-edit me-1" onclick="editRole(' + r.roleId + ')" title="Edit"><i class="fas fa-pen"></i></button>' +
                    '<button class="action-btn action-btn-delete" onclick="deleteRole(' + r.roleId + ', \'' + escapeHtml(r.roleName) + '\')" title="Delete"><i class="fas fa-trash"></i></button>' +
                '</td>' +
            '</tr>';
        $tbody.append(row);
    });
}

function clearFilters() {
    $('#txtSearch').val('');
    $('#ddlStatusFilter').val('');
    searchRoles();
}

function openCreateModal() {
    $('#roleModalTitle').text('Add Role');
    $('#hdnRoleId').val('');
    $('#txtRoleName').val('');
    $('#txtDescription').val('');
    $('#chkActive').prop('checked', true);
    $('#statusGroup').hide();
    new bootstrap.Modal('#roleModal').show();
}

function editRole(id) {
    apiCall('/RoleManagement/GetById?id=' + id, 'GET', null, function (result) {
        var r = result.data;
        $('#roleModalTitle').text('Edit Role');
        $('#hdnRoleId').val(r.roleId);
        $('#txtRoleName').val(r.roleName);
        $('#txtDescription').val(r.description || '');
        $('#chkActive').prop('checked', r.isActive);
        $('#statusGroup').show();
        new bootstrap.Modal('#roleModal').show();
    });
}

function saveRole() {
    var roleId = $('#hdnRoleId').val();
    var isEdit = roleId !== '';

    var roleName = $('#txtRoleName').val().trim();
    if (!roleName) { showToast('Role name is required.', 'error'); return; }

    if (isEdit) {
        var updateDto = {
            roleId: parseInt(roleId),
            roleName: roleName,
            description: $('#txtDescription').val().trim(),
            isActive: $('#chkActive').is(':checked')
        };
        apiCall('/RoleManagement/Update', 'POST', updateDto, function () {
            showToast('Role updated successfully.');
            bootstrap.Modal.getInstance(document.getElementById('roleModal')).hide();
            searchRoles();
        });
    } else {
        var createDto = {
            roleName: roleName,
            description: $('#txtDescription').val().trim()
        };
        apiCall('/RoleManagement/Create', 'POST', createDto, function () {
            showToast('Role created successfully.');
            bootstrap.Modal.getInstance(document.getElementById('roleModal')).hide();
            searchRoles();
        });
    }
}

function deleteRole(id, name) {
    if (!confirm('Are you sure you want to delete role "' + name + '"?')) return;

    apiCall('/RoleManagement/Delete?id=' + id, 'POST', null, function () {
        showToast('Role deleted successfully.');
        searchRoles();
    });
}

function escapeHtml(text) {
    if (!text) return '';
    var div = document.createElement('div');
    div.appendChild(document.createTextNode(text));
    return div.innerHTML;
}
