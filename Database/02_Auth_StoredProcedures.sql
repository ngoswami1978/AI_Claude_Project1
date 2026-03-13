-- =============================================
-- AUTH STORED PROCEDURES — Manpower Contract
-- =============================================

-- ═══ AUTH: LOGIN ═══
CREATE OR ALTER PROCEDURE [dbo].[usp_Auth_LOGIN]
    @Email          NVARCHAR(200),
    @Result         INT OUTPUT,
    @ErrorMessage   NVARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @Result = 0; SET @ErrorMessage = '';
    BEGIN TRY
        SELECT
            u.USER_ID, u.FULL_NAME, u.EMAIL,
            u.PASSWORD_HASH, u.PASSWORD_SALT,
            u.ROLE_ID, r.ROLE_NAME, u.IS_ACTIVE
        FROM MST_USER u WITH (NOLOCK)
        INNER JOIN MST_ROLE r WITH (NOLOCK) ON r.ROLE_ID = u.ROLE_ID
        WHERE u.EMAIL = @Email AND u.IS_DELETED = 0;
        SET @Result = 1;
    END TRY
    BEGIN CATCH
        SET @Result = 0;
        SET @ErrorMessage = ERROR_MESSAGE();
    END CATCH
END
GO

-- ═══ AUTH: CHANGE PASSWORD ═══
CREATE OR ALTER PROCEDURE [dbo].[usp_Auth_CHANGE_PASSWORD]
    @UserId         INT,
    @NewHash        NVARCHAR(500),
    @NewSalt        NVARCHAR(500),
    @Result         INT OUTPUT,
    @ErrorMessage   NVARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @Result = 0; SET @ErrorMessage = '';
    BEGIN TRY
        UPDATE MST_USER
        SET PASSWORD_HASH = @NewHash, PASSWORD_SALT = @NewSalt, M_DATETIME = GETDATE()
        WHERE USER_ID = @UserId AND IS_DELETED = 0;
        SET @Result = 1;
    END TRY
    BEGIN CATCH
        SET @Result = 0;
        SET @ErrorMessage = ERROR_MESSAGE();
    END CATCH
END
GO

-- ═══ USER: INSERT ═══
CREATE OR ALTER PROCEDURE [dbo].[usp_User_INSERT]
    @FullName       NVARCHAR(200),
    @Email          NVARCHAR(200),
    @PasswordHash   NVARCHAR(500),
    @PasswordSalt   NVARCHAR(500),
    @RoleId         INT,
    @DepartmentId   INT = NULL,
    @IsActive       BIT = 1,
    @CUserId        INT = NULL,
    @Result         INT OUTPUT,
    @ErrorMessage   NVARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @Result = 0; SET @ErrorMessage = '';
    BEGIN TRY
        IF EXISTS (SELECT 1 FROM MST_USER WITH (NOLOCK) WHERE EMAIL = @Email AND IS_DELETED = 0)
        BEGIN
            SET @ErrorMessage = 'Email already exists.';
            RETURN;
        END
        INSERT INTO MST_USER (FULL_NAME, EMAIL, PASSWORD_HASH, PASSWORD_SALT, ROLE_ID, DEPARTMENT_ID, IS_ACTIVE, C_USER_ID)
        VALUES (@FullName, @Email, @PasswordHash, @PasswordSalt, @RoleId, @DepartmentId, @IsActive, @CUserId);
        SET @Result = 1;
    END TRY
    BEGIN CATCH
        SET @Result = 0;
        SET @ErrorMessage = ERROR_MESSAGE();
    END CATCH
END
GO

-- ═══ USER: UPDATE ═══
CREATE OR ALTER PROCEDURE [dbo].[usp_User_UPDATE]
    @UserId         INT,
    @FullName       NVARCHAR(200),
    @Email          NVARCHAR(200),
    @RoleId         INT,
    @DepartmentId   INT = NULL,
    @IsActive       BIT = 1,
    @MUserId        INT = NULL,
    @Result         INT OUTPUT,
    @ErrorMessage   NVARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @Result = 0; SET @ErrorMessage = '';
    BEGIN TRY
        IF EXISTS (SELECT 1 FROM MST_USER WITH (NOLOCK) WHERE EMAIL = @Email AND USER_ID != @UserId AND IS_DELETED = 0)
        BEGIN
            SET @ErrorMessage = 'Email already exists.';
            RETURN;
        END
        UPDATE MST_USER
        SET FULL_NAME = @FullName, EMAIL = @Email, ROLE_ID = @RoleId,
            DEPARTMENT_ID = @DepartmentId, IS_ACTIVE = @IsActive,
            M_USER_ID = @MUserId, M_DATETIME = GETDATE()
        WHERE USER_ID = @UserId AND IS_DELETED = 0;
        SET @Result = 1;
    END TRY
    BEGIN CATCH
        SET @Result = 0;
        SET @ErrorMessage = ERROR_MESSAGE();
    END CATCH
END
GO

-- ═══ USER: DELETE (soft) ═══
CREATE OR ALTER PROCEDURE [dbo].[usp_User_DELETE]
    @UserId         INT,
    @MUserId        INT = NULL,
    @Result         INT OUTPUT,
    @ErrorMessage   NVARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @Result = 0; SET @ErrorMessage = '';
    BEGIN TRY
        UPDATE MST_USER SET IS_DELETED = 1, M_USER_ID = @MUserId, M_DATETIME = GETDATE()
        WHERE USER_ID = @UserId AND IS_DELETED = 0;
        SET @Result = 1;
    END TRY
    BEGIN CATCH
        SET @Result = 0;
        SET @ErrorMessage = ERROR_MESSAGE();
    END CATCH
END
GO

-- ═══ USER: GETBYID ═══
CREATE OR ALTER PROCEDURE [dbo].[usp_User_GETBYID]
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT u.USER_ID, u.FULL_NAME, u.EMAIL, u.ROLE_ID, r.ROLE_NAME,
           u.DEPARTMENT_ID, d.DEPARTMENT_NAME, u.IS_ACTIVE, u.C_DATETIME
    FROM MST_USER u WITH (NOLOCK)
    INNER JOIN MST_ROLE r WITH (NOLOCK) ON r.ROLE_ID = u.ROLE_ID
    LEFT JOIN MST_DEPARTMENT d WITH (NOLOCK) ON d.DEPARTMENT_ID = u.DEPARTMENT_ID
    WHERE u.USER_ID = @UserId AND u.IS_DELETED = 0;
END
GO

-- ═══ USER: SEARCH ═══
CREATE OR ALTER PROCEDURE [dbo].[usp_User_SEARCH]
    @FullName       NVARCHAR(200) = NULL,
    @Email          NVARCHAR(200) = NULL,
    @RoleId         INT = NULL,
    @DepartmentId   INT = NULL,
    @IsActive       BIT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT u.USER_ID, u.FULL_NAME, u.EMAIL, u.ROLE_ID, r.ROLE_NAME,
           u.DEPARTMENT_ID, d.DEPARTMENT_NAME, u.IS_ACTIVE, u.C_DATETIME
    FROM MST_USER u WITH (NOLOCK)
    INNER JOIN MST_ROLE r WITH (NOLOCK) ON r.ROLE_ID = u.ROLE_ID
    LEFT JOIN MST_DEPARTMENT d WITH (NOLOCK) ON d.DEPARTMENT_ID = u.DEPARTMENT_ID
    WHERE u.IS_DELETED = 0
      AND (@FullName IS NULL OR u.FULL_NAME LIKE '%' + @FullName + '%')
      AND (@Email IS NULL OR u.EMAIL LIKE '%' + @Email + '%')
      AND (@RoleId IS NULL OR u.ROLE_ID = @RoleId)
      AND (@DepartmentId IS NULL OR u.DEPARTMENT_ID = @DepartmentId)
      AND (@IsActive IS NULL OR u.IS_ACTIVE = @IsActive)
    ORDER BY u.C_DATETIME DESC;
END
GO

-- ═══ ROLE: INSERT ═══
CREATE OR ALTER PROCEDURE [dbo].[usp_Role_INSERT]
    @RoleName       NVARCHAR(100),
    @Description    NVARCHAR(500) = NULL,
    @IsActive       BIT = 1,
    @CUserId        INT = NULL,
    @Result         INT OUTPUT,
    @ErrorMessage   NVARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @Result = 0; SET @ErrorMessage = '';
    BEGIN TRY
        IF EXISTS (SELECT 1 FROM MST_ROLE WITH (NOLOCK) WHERE ROLE_NAME = @RoleName AND IS_DELETED = 0)
        BEGIN SET @ErrorMessage = 'Role name already exists.'; RETURN; END
        INSERT INTO MST_ROLE (ROLE_NAME, DESCRIPTION, IS_ACTIVE, C_USER_ID)
        VALUES (@RoleName, @Description, @IsActive, @CUserId);
        SET @Result = 1;
    END TRY
    BEGIN CATCH SET @Result = 0; SET @ErrorMessage = ERROR_MESSAGE(); END CATCH
END
GO

-- ═══ ROLE: UPDATE ═══
CREATE OR ALTER PROCEDURE [dbo].[usp_Role_UPDATE]
    @RoleId         INT,
    @RoleName       NVARCHAR(100),
    @Description    NVARCHAR(500) = NULL,
    @IsActive       BIT = 1,
    @MUserId        INT = NULL,
    @Result         INT OUTPUT,
    @ErrorMessage   NVARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @Result = 0; SET @ErrorMessage = '';
    BEGIN TRY
        IF EXISTS (SELECT 1 FROM MST_ROLE WITH (NOLOCK) WHERE ROLE_NAME = @RoleName AND ROLE_ID != @RoleId AND IS_DELETED = 0)
        BEGIN SET @ErrorMessage = 'Role name already exists.'; RETURN; END
        UPDATE MST_ROLE SET ROLE_NAME = @RoleName, DESCRIPTION = @Description,
            IS_ACTIVE = @IsActive, M_USER_ID = @MUserId, M_DATETIME = GETDATE()
        WHERE ROLE_ID = @RoleId AND IS_DELETED = 0;
        SET @Result = 1;
    END TRY
    BEGIN CATCH SET @Result = 0; SET @ErrorMessage = ERROR_MESSAGE(); END CATCH
END
GO

-- ═══ ROLE: DELETE (soft) ═══
CREATE OR ALTER PROCEDURE [dbo].[usp_Role_DELETE]
    @RoleId INT, @MUserId INT = NULL,
    @Result INT OUTPUT, @ErrorMessage NVARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON; SET @Result = 0; SET @ErrorMessage = '';
    BEGIN TRY
        UPDATE MST_ROLE SET IS_DELETED = 1, M_USER_ID = @MUserId, M_DATETIME = GETDATE()
        WHERE ROLE_ID = @RoleId AND IS_DELETED = 0;
        SET @Result = 1;
    END TRY
    BEGIN CATCH SET @Result = 0; SET @ErrorMessage = ERROR_MESSAGE(); END CATCH
END
GO

-- ═══ ROLE: GETBYID ═══
CREATE OR ALTER PROCEDURE [dbo].[usp_Role_GETBYID] @RoleId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ROLE_ID, ROLE_NAME, DESCRIPTION, IS_ACTIVE, C_DATETIME
    FROM MST_ROLE WITH (NOLOCK) WHERE ROLE_ID = @RoleId AND IS_DELETED = 0;
END
GO

-- ═══ ROLE: SEARCH ═══
CREATE OR ALTER PROCEDURE [dbo].[usp_Role_SEARCH]
    @RoleName NVARCHAR(100) = NULL, @IsActive BIT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ROLE_ID, ROLE_NAME, DESCRIPTION, IS_ACTIVE, C_DATETIME
    FROM MST_ROLE WITH (NOLOCK)
    WHERE IS_DELETED = 0
      AND (@RoleName IS NULL OR ROLE_NAME LIKE '%' + @RoleName + '%')
      AND (@IsActive IS NULL OR IS_ACTIVE = @IsActive)
    ORDER BY C_DATETIME DESC;
END
GO

-- ═══ PERMISSION: SAVE MATRIX ═══
CREATE OR ALTER PROCEDURE [dbo].[usp_RolePermission_SAVE]
    @RoleId     INT,
    @PermJson   NVARCHAR(MAX),
    @CUserId    INT = NULL,
    @Result     INT OUTPUT,
    @ErrorMessage NVARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON; SET @Result = 0; SET @ErrorMessage = '';
    BEGIN TRY
        BEGIN TRANSACTION;
        DELETE FROM MST_ROLE_PERMISSION WHERE ROLE_ID = @RoleId;
        INSERT INTO MST_ROLE_PERMISSION (ROLE_ID, MODULE_ID, CAN_CREATE, CAN_DISABLE, CAN_VIEW, CAN_UPDATE, CAN_DOWNLOAD, C_USER_ID)
        SELECT @RoleId, j.ModuleId, j.CanCreate, j.CanDisable, j.CanView, j.CanUpdate, j.CanDownload, @CUserId
        FROM OPENJSON(@PermJson) WITH (
            ModuleId    INT '$.moduleId',
            CanCreate   BIT '$.canCreate',
            CanDisable  BIT '$.canDisable',
            CanView     BIT '$.canView',
            CanUpdate   BIT '$.canUpdate',
            CanDownload BIT '$.canDownload'
        ) j;
        COMMIT TRANSACTION;
        SET @Result = 1;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        SET @Result = 0; SET @ErrorMessage = ERROR_MESSAGE();
    END CATCH
END
GO

-- ═══ PERMISSION: GET BY ROLE ═══
CREATE OR ALTER PROCEDURE [dbo].[usp_RolePermission_GETBYROLE]
    @RoleId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT rp.ROLE_PERMISSION_ID, rp.MODULE_ID, m.MODULE_CODE, m.MODULE_NAME,
           m.PARENT_MODULE_ID, pm.MODULE_NAME AS PARENT_MODULE_NAME,
           rp.CAN_CREATE, rp.CAN_DISABLE, rp.CAN_VIEW, rp.CAN_UPDATE, rp.CAN_DOWNLOAD
    FROM MST_MODULE m WITH (NOLOCK)
    LEFT JOIN MST_ROLE_PERMISSION rp WITH (NOLOCK) ON rp.MODULE_ID = m.MODULE_ID AND rp.ROLE_ID = @RoleId
    LEFT JOIN MST_MODULE pm WITH (NOLOCK) ON pm.MODULE_ID = m.PARENT_MODULE_ID
    WHERE m.IS_ACTIVE = 1
    ORDER BY m.PARENT_MODULE_ID, m.DISPLAY_ORDER;
END
GO

-- ═══ PERMISSION: COPY FROM ROLE ═══
CREATE OR ALTER PROCEDURE [dbo].[usp_RolePermission_COPYFROMROLE]
    @SourceRoleId   INT,
    @TargetRoleId   INT,
    @CUserId        INT = NULL,
    @Result         INT OUTPUT,
    @ErrorMessage   NVARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON; SET @Result = 0; SET @ErrorMessage = '';
    BEGIN TRY
        BEGIN TRANSACTION;
        DELETE FROM MST_ROLE_PERMISSION WHERE ROLE_ID = @TargetRoleId;
        INSERT INTO MST_ROLE_PERMISSION (ROLE_ID, MODULE_ID, CAN_CREATE, CAN_DISABLE, CAN_VIEW, CAN_UPDATE, CAN_DOWNLOAD, C_USER_ID)
        SELECT @TargetRoleId, MODULE_ID, CAN_CREATE, CAN_DISABLE, CAN_VIEW, CAN_UPDATE, CAN_DOWNLOAD, @CUserId
        FROM MST_ROLE_PERMISSION WITH (NOLOCK) WHERE ROLE_ID = @SourceRoleId;
        COMMIT TRANSACTION;
        SET @Result = 1;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        SET @Result = 0; SET @ErrorMessage = ERROR_MESSAGE();
    END CATCH
END
GO

-- ═══ PERMISSION: COPY FROM USER ═══
CREATE OR ALTER PROCEDURE [dbo].[usp_RolePermission_COPYFROMUSER]
    @SourceUserId   INT,
    @TargetRoleId   INT,
    @CUserId        INT = NULL,
    @Result         INT OUTPUT,
    @ErrorMessage   NVARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON; SET @Result = 0; SET @ErrorMessage = '';
    BEGIN TRY
        DECLARE @SourceRoleId INT;
        SELECT @SourceRoleId = ROLE_ID FROM MST_USER WITH (NOLOCK)
        WHERE USER_ID = @SourceUserId AND IS_DELETED = 0;
        IF @SourceRoleId IS NULL
        BEGIN SET @ErrorMessage = 'Source user not found.'; RETURN; END
        BEGIN TRANSACTION;
        DELETE FROM MST_ROLE_PERMISSION WHERE ROLE_ID = @TargetRoleId;
        INSERT INTO MST_ROLE_PERMISSION (ROLE_ID, MODULE_ID, CAN_CREATE, CAN_DISABLE, CAN_VIEW, CAN_UPDATE, CAN_DOWNLOAD, C_USER_ID)
        SELECT @TargetRoleId, MODULE_ID, CAN_CREATE, CAN_DISABLE, CAN_VIEW, CAN_UPDATE, CAN_DOWNLOAD, @CUserId
        FROM MST_ROLE_PERMISSION WITH (NOLOCK) WHERE ROLE_ID = @SourceRoleId;
        COMMIT TRANSACTION;
        SET @Result = 1;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        SET @Result = 0; SET @ErrorMessage = ERROR_MESSAGE();
    END CATCH
END
GO

-- ═══ LOOKUPS ═══
CREATE OR ALTER PROCEDURE [dbo].[usp_Lookup_Role]
AS BEGIN SET NOCOUNT ON;
    SELECT ROLE_ID AS [Value], ROLE_NAME AS [Text] FROM MST_ROLE WITH (NOLOCK) WHERE IS_ACTIVE = 1 AND IS_DELETED = 0 ORDER BY ROLE_NAME;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[usp_Lookup_Module]
AS BEGIN SET NOCOUNT ON;
    SELECT MODULE_ID, MODULE_CODE, MODULE_NAME, PARENT_MODULE_ID, ICON_CLASS, URL_PATH, DISPLAY_ORDER
    FROM MST_MODULE WITH (NOLOCK) WHERE IS_ACTIVE = 1 ORDER BY PARENT_MODULE_ID, DISPLAY_ORDER;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[usp_Lookup_User]
AS BEGIN SET NOCOUNT ON;
    SELECT USER_ID AS [Value], FULL_NAME + ' (' + EMAIL + ')' AS [Text]
    FROM MST_USER WITH (NOLOCK) WHERE IS_ACTIVE = 1 AND IS_DELETED = 0 ORDER BY FULL_NAME;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[usp_Lookup_Department]
AS BEGIN SET NOCOUNT ON;
    SELECT DEPARTMENT_ID AS [Value], DEPARTMENT_NAME AS [Text]
    FROM MST_DEPARTMENT WITH (NOLOCK) WHERE IS_ACTIVE = 1 AND IS_DELETED = 0 ORDER BY DEPARTMENT_NAME;
END
GO

-- ═══ USER PERMISSIONS FOR SESSION (returns permission strings) ═══
CREATE OR ALTER PROCEDURE [dbo].[usp_Auth_GetUserPermissions]
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @RoleId INT;
    SELECT @RoleId = ROLE_ID FROM MST_USER WITH (NOLOCK) WHERE USER_ID = @UserId AND IS_DELETED = 0;

    SELECT m.MODULE_CODE, rp.CAN_CREATE, rp.CAN_DISABLE, rp.CAN_VIEW, rp.CAN_UPDATE, rp.CAN_DOWNLOAD
    FROM MST_ROLE_PERMISSION rp WITH (NOLOCK)
    INNER JOIN MST_MODULE m WITH (NOLOCK) ON m.MODULE_ID = rp.MODULE_ID
    WHERE rp.ROLE_ID = @RoleId AND m.IS_ACTIVE = 1;
END
GO

PRINT '✅ Auth stored procedures created successfully.';
GO
