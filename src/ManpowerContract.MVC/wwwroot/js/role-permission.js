// ═══════════════════════════════════════════════════════════════
// Role Permission Management JS
// Collapsible module groups with Copy from Role / User
// ═══════════════════════════════════════════════════════════════

var permissionData = [];

function loadPermissions() {
    var roleId = $('#ddlRole').val();
    if (!roleId) {
        showToast('Please select a role.', 'error');
        return;
    }

    apiCall('/RolePermission/GetPermissions?roleId=' + roleId, 'GET', null, function (result) {
        permissionData = result.data || [];
        renderPermissions();
        $('#btnSavePerms').prop('disabled', false);
    });
}

function renderPermissions() {
    var $tbody = $('#tbodyPermissions');
    $tbody.empty();

    if (!permissionData || permissionData.length === 0) {
        $tbody.html('<tr><td colspan="6" class="text-center text-muted py-4">No modules found</td></tr>');
        return;
    }

    // Group by parent module
    var groups = {};
    var standalone = [];

    $.each(permissionData, function (i, p) {
        if (p.parentModuleId && p.parentModuleName) {
            if (!groups[p.parentModuleId]) {
                groups[p.parentModuleId] = {
                    id: p.parentModuleId,
                    name: p.parentModuleName,
                    children: []
                };
            }
            groups[p.parentModuleId].children.push(p);
        } else if (!p.parentModuleId && hasNoChildren(p.moduleId)) {
            standalone.push(p);
        }
    });

    // Render grouped modules
    $.each(groups, function (key, group) {
        // Group header row (collapsible)
        var headerRow =
            '<tr class="permission-group-header" data-group="' + group.id + '" onclick="toggleGroup(' + group.id + ')">' +
                '<td colspan="6">' +
                    '<i class="fas fa-chevron-down toggle-icon"></i>' +
                    '<i class="fas fa-folder-open me-2" style="color: var(--secondary);"></i>' +
                    group.name +
                    ' <span class="text-muted small">(' + group.children.length + ' modules)</span>' +
                '</td>' +
            '</tr>';
        $tbody.append(headerRow);

        // Child module rows
        $.each(group.children, function (j, p) {
            $tbody.append(buildPermissionRow(p, group.id));
        });
    });

    // Render standalone modules (no parent)
    $.each(standalone, function (i, p) {
        $tbody.append(buildPermissionRow(p, 'standalone'));
    });
}

function hasNoChildren(moduleId) {
    // Check if this module is NOT a parent of any other module
    var isParent = false;
    $.each(permissionData, function (i, p) {
        if (p.parentModuleId === moduleId) {
            isParent = true;
            return false;
        }
    });
    return !isParent;
}

function buildPermissionRow(p, groupId) {
    return '<tr class="permission-row" data-group="' + groupId + '" data-module-id="' + p.moduleId + '">' +
        '<td>' + escapeHtml(p.moduleName) + '</td>' +
        '<td class="text-center"><input type="checkbox" class="permission-checkbox" data-field="canCreate" ' + (p.canCreate ? 'checked' : '') + ' /></td>' +
        '<td class="text-center"><input type="checkbox" class="permission-checkbox" data-field="canDisable" ' + (p.canDisable ? 'checked' : '') + ' /></td>' +
        '<td class="text-center"><input type="checkbox" class="permission-checkbox" data-field="canView" ' + (p.canView ? 'checked' : '') + ' /></td>' +
        '<td class="text-center"><input type="checkbox" class="permission-checkbox" data-field="canUpdate" ' + (p.canUpdate ? 'checked' : '') + ' /></td>' +
        '<td class="text-center"><input type="checkbox" class="permission-checkbox" data-field="canDownload" ' + (p.canDownload ? 'checked' : '') + ' /></td>' +
    '</tr>';
}

function toggleGroup(groupId) {
    var $header = $('tr.permission-group-header[data-group="' + groupId + '"]');
    var $rows = $('tr.permission-row[data-group="' + groupId + '"]');

    $header.toggleClass('collapsed');
    $rows.toggle();
}

function savePermissions() {
    var roleId = $('#ddlRole').val();
    if (!roleId) {
        showToast('Please select a role.', 'error');
        return;
    }

    var permissions = [];
    $('tr.permission-row').each(function () {
        var $row = $(this);
        var moduleId = parseInt($row.data('module-id'));
        permissions.push({
            moduleId: moduleId,
            canCreate: $row.find('[data-field="canCreate"]').is(':checked'),
            canDisable: $row.find('[data-field="canDisable"]').is(':checked'),
            canView: $row.find('[data-field="canView"]').is(':checked'),
            canUpdate: $row.find('[data-field="canUpdate"]').is(':checked'),
            canDownload: $row.find('[data-field="canDownload"]').is(':checked')
        });
    });

    var dto = {
        roleId: parseInt(roleId),
        permissions: permissions
    };

    apiCall('/RolePermission/Save', 'POST', dto, function () {
        showToast('Permissions saved successfully.');
    });
}

// ── Copy from Role ──
function openCopyFromRoleModal() {
    var roleId = $('#ddlRole').val();
    if (!roleId) {
        showToast('Please select a target role first.', 'error');
        return;
    }
    $('#ddlSourceRole').val('');
    new bootstrap.Modal('#copyRoleModal').show();
}

function copyFromRole() {
    var targetRoleId = $('#ddlRole').val();
    var sourceRoleId = $('#ddlSourceRole').val();

    if (!sourceRoleId) {
        showToast('Please select a source role.', 'error');
        return;
    }
    if (sourceRoleId === targetRoleId) {
        showToast('Source and target role cannot be the same.', 'error');
        return;
    }

    var dto = {
        targetRoleId: parseInt(targetRoleId),
        sourceRoleId: parseInt(sourceRoleId)
    };

    apiCall('/RolePermission/CopyFromRole', 'POST', dto, function () {
        showToast('Permissions copied from role successfully.');
        bootstrap.Modal.getInstance(document.getElementById('copyRoleModal')).hide();
        loadPermissions(); // Reload the grid
    });
}

// ── Copy from User ──
function openCopyFromUserModal() {
    var roleId = $('#ddlRole').val();
    if (!roleId) {
        showToast('Please select a target role first.', 'error');
        return;
    }
    $('#ddlSourceUser').val('');
    new bootstrap.Modal('#copyUserModal').show();
}

function copyFromUser() {
    var targetRoleId = $('#ddlRole').val();
    var sourceUserId = $('#ddlSourceUser').val();

    if (!sourceUserId) {
        showToast('Please select a source user.', 'error');
        return;
    }

    var dto = {
        targetRoleId: parseInt(targetRoleId),
        sourceUserId: parseInt(sourceUserId)
    };

    apiCall('/RolePermission/CopyFromUser', 'POST', dto, function () {
        showToast('Permissions copied from user successfully.');
        bootstrap.Modal.getInstance(document.getElementById('copyUserModal')).hide();
        loadPermissions(); // Reload the grid
    });
}

function escapeHtml(text) {
    if (!text) return '';
    var div = document.createElement('div');
    div.appendChild(document.createTextNode(text));
    return div.innerHTML;
}
