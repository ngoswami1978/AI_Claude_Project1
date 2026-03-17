-- =============================================
-- SEED DATA — Manpower Contract Management
-- =============================================

-- Departments
IF NOT EXISTS (SELECT 1 FROM MST_DEPARTMENT WHERE DEPARTMENT_NAME = 'IT')
    INSERT INTO MST_DEPARTMENT (DEPARTMENT_NAME) VALUES ('IT'), ('HR'), ('Finance'), ('Operations');
GO

-- Roles
IF NOT EXISTS (SELECT 1 FROM MST_ROLE WHERE ROLE_NAME = 'Admin')
    INSERT INTO MST_ROLE (ROLE_NAME, DESCRIPTION) VALUES ('Admin', 'Full system access');
IF NOT EXISTS (SELECT 1 FROM MST_ROLE WHERE ROLE_NAME = 'Manager')
    INSERT INTO MST_ROLE (ROLE_NAME, DESCRIPTION) VALUES ('Manager', 'Department level access');
IF NOT EXISTS (SELECT 1 FROM MST_ROLE WHERE ROLE_NAME = 'Viewer')
    INSERT INTO MST_ROLE (ROLE_NAME, DESCRIPTION) VALUES ('Viewer', 'Read-only access');
GO

-- Modules (Parent groups + Children)
IF NOT EXISTS (SELECT 1 FROM MST_MODULE WHERE MODULE_CODE = 'ADMIN_GROUP')
BEGIN
    -- Parent: Administration
    INSERT INTO MST_MODULE (MODULE_CODE, MODULE_NAME, PARENT_MODULE_ID, ICON_CLASS, URL_PATH, DISPLAY_ORDER)
    VALUES ('ADMIN_GROUP', 'Administration', NULL, 'bi bi-gear', NULL, 1);

    DECLARE @AdminGroupId INT = SCOPE_IDENTITY();

    INSERT INTO MST_MODULE (MODULE_CODE, MODULE_NAME, PARENT_MODULE_ID, ICON_CLASS, URL_PATH, DISPLAY_ORDER) VALUES
    ('DASHBOARD',    'Dashboard',           @AdminGroupId, 'bi bi-speedometer2', '/Dashboard',         1),
    ('USER_MGMT',    'User Management',     @AdminGroupId, 'bi bi-people',       '/UserManagement',    2),
    ('ROLE_MGMT',    'Role Management',     @AdminGroupId, 'bi bi-shield-check', '/RoleManagement',    3),
    ('PERM_MGMT',    'Permission Management', @AdminGroupId, 'bi bi-key',        '/RolePermission',    4);

    -- Parent: Operations
    INSERT INTO MST_MODULE (MODULE_CODE, MODULE_NAME, PARENT_MODULE_ID, ICON_CLASS, URL_PATH, DISPLAY_ORDER)
    VALUES ('OPS_GROUP', 'Operations', NULL, 'bi bi-briefcase', NULL, 2);

    DECLARE @OpsGroupId INT = SCOPE_IDENTITY();

    INSERT INTO MST_MODULE (MODULE_CODE, MODULE_NAME, PARENT_MODULE_ID, ICON_CLASS, URL_PATH, DISPLAY_ORDER) VALUES
    ('MANPOWER_CONTRACT', 'Manpower Contract', @OpsGroupId, 'bi bi-file-text', '/ManpowerContract', 1);
END
GO

-- Admin permissions (ALL true for all modules)
IF NOT EXISTS (SELECT 1 FROM MST_ROLE_PERMISSION WHERE ROLE_ID = 1)
BEGIN
    INSERT INTO MST_ROLE_PERMISSION (ROLE_ID, MODULE_ID, CAN_CREATE, CAN_DISABLE, CAN_VIEW, CAN_UPDATE, CAN_DOWNLOAD)
    SELECT 1, MODULE_ID, 1, 1, 1, 1, 1
    FROM MST_MODULE WHERE PARENT_MODULE_ID IS NOT NULL;
END
GO

-- Admin User (admin@company.com / Admin@123)
-- BCrypt hash verified with BCrypt.Net-Next 4.0.3, workFactor: 12
IF NOT EXISTS (SELECT 1 FROM MST_USER WHERE EMAIL = 'admin@company.com')
BEGIN
    INSERT INTO MST_USER (FULL_NAME, EMAIL, PASSWORD_HASH, PASSWORD_SALT, ROLE_ID, DEPARTMENT_ID, IS_ACTIVE)
    VALUES ('System Admin', 'admin@company.com',
            '$2a$12$rzJXGw8FLjqJkwKIQU9gk.W528iHc.TUsMGCL.z6J0Egzj/5FxQGu', -- Admin@123 (verified)
            'BCryptManaged', 1, 1, 1);
END
GO

-- If admin user already exists with wrong hash, update it
UPDATE MST_USER
SET PASSWORD_HASH = '$2a$12$rzJXGw8FLjqJkwKIQU9gk.W528iHc.TUsMGCL.z6J0Egzj/5FxQGu',
    PASSWORD_SALT = 'BCryptManaged'
WHERE EMAIL = 'admin@company.com';
GO

PRINT '✅ Seed data inserted successfully.';
PRINT '   Default Login: admin@company.com / Admin@123';
GO
