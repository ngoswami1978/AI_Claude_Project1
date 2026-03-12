-- ============================================================
-- MANPOWER CONTRACT MANAGEMENT - STORED PROCEDURES
-- ============================================================

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ── INSERT ───────────────────────────────────────────────────
CREATE OR ALTER PROCEDURE usp_ManpowerContract_INSERT
    @YEAR                   INT,
    @MONTH                  NVARCHAR(20),
    @RCO                    NVARCHAR(100),
    @DIVISION               NVARCHAR(200),
    @GROUP_COMPANY_ID       INT,
    @PLANT_ID               INT,
    @PLANT_COUNTRY          NVARCHAR(100),
    @SUPPLIER_ID            INT,
    @GLOBAL_SUPPLIER_NAME   NVARCHAR(200),
    @SUPPLIER_NAME_REMARKS  NVARCHAR(500),
    @SUPPLIER_COUNTRY       NVARCHAR(100),
    @ANNUAL_SUPPLIER_SPEND  DECIMAL(18,2),
    @CURRENCY_ID            INT,
    @CONTRACTED             NVARCHAR(5),
    @WORKER_TYPE            NVARCHAR(100),
    @CONTRACTED_START_DATE  DATE,
    @CONTRACTED_END_DATE    DATE,
    @C_USER_ID              INT,
    @Result                 INT OUTPUT,
    @ErrorMessage           NVARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @Result = 0;
    SET @ErrorMessage = '';

    BEGIN TRY
        BEGIN TRANSACTION;

        INSERT INTO MANPOWER_CONTRACT_MST (
            YEAR, MONTH, RCO, DIVISION, GROUP_COMPANY_ID, PLANT_ID,
            PLANT_COUNTRY, SUPPLIER_ID, GLOBAL_SUPPLIER_NAME,
            SUPPLIER_NAME_REMARKS, SUPPLIER_COUNTRY, ANNUAL_SUPPLIER_SPEND,
            CURRENCY_ID, CONTRACTED, WORKER_TYPE,
            CONTRACTED_START_DATE, CONTRACTED_END_DATE,
            IS_DELETED, C_USER_ID, C_DATETIME
        )
        VALUES (
            @YEAR, @MONTH, @RCO, @DIVISION, @GROUP_COMPANY_ID, @PLANT_ID,
            @PLANT_COUNTRY, @SUPPLIER_ID, @GLOBAL_SUPPLIER_NAME,
            @SUPPLIER_NAME_REMARKS, @SUPPLIER_COUNTRY, @ANNUAL_SUPPLIER_SPEND,
            @CURRENCY_ID, @CONTRACTED, @WORKER_TYPE,
            @CONTRACTED_START_DATE, @CONTRACTED_END_DATE,
            0, @C_USER_ID, GETDATE()
        );

        COMMIT TRANSACTION;
        SET @Result = 1;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        SET @Result = 0;
        SET @ErrorMessage = ERROR_MESSAGE();
    END CATCH
END
GO

-- ── UPDATE ───────────────────────────────────────────────────
CREATE OR ALTER PROCEDURE usp_ManpowerContract_UPDATE
    @CONTRACT_ID            INT,
    @YEAR                   INT,
    @MONTH                  NVARCHAR(20),
    @RCO                    NVARCHAR(100),
    @DIVISION               NVARCHAR(200),
    @GROUP_COMPANY_ID       INT,
    @PLANT_ID               INT,
    @PLANT_COUNTRY          NVARCHAR(100),
    @SUPPLIER_ID            INT,
    @GLOBAL_SUPPLIER_NAME   NVARCHAR(200),
    @SUPPLIER_NAME_REMARKS  NVARCHAR(500),
    @SUPPLIER_COUNTRY       NVARCHAR(100),
    @ANNUAL_SUPPLIER_SPEND  DECIMAL(18,2),
    @CURRENCY_ID            INT,
    @CONTRACTED             NVARCHAR(5),
    @WORKER_TYPE            NVARCHAR(100),
    @CONTRACTED_START_DATE  DATE,
    @CONTRACTED_END_DATE    DATE,
    @M_USER_ID              INT,
    @Result                 INT OUTPUT,
    @ErrorMessage           NVARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @Result = 0;
    SET @ErrorMessage = '';

    BEGIN TRY
        BEGIN TRANSACTION;

        UPDATE MANPOWER_CONTRACT_MST SET
            YEAR                    = @YEAR,
            MONTH                   = @MONTH,
            RCO                     = @RCO,
            DIVISION                = @DIVISION,
            GROUP_COMPANY_ID        = @GROUP_COMPANY_ID,
            PLANT_ID                = @PLANT_ID,
            PLANT_COUNTRY           = @PLANT_COUNTRY,
            SUPPLIER_ID             = @SUPPLIER_ID,
            GLOBAL_SUPPLIER_NAME    = @GLOBAL_SUPPLIER_NAME,
            SUPPLIER_NAME_REMARKS   = @SUPPLIER_NAME_REMARKS,
            SUPPLIER_COUNTRY        = @SUPPLIER_COUNTRY,
            ANNUAL_SUPPLIER_SPEND   = @ANNUAL_SUPPLIER_SPEND,
            CURRENCY_ID             = @CURRENCY_ID,
            CONTRACTED              = @CONTRACTED,
            WORKER_TYPE             = @WORKER_TYPE,
            CONTRACTED_START_DATE   = @CONTRACTED_START_DATE,
            CONTRACTED_END_DATE     = @CONTRACTED_END_DATE,
            M_USER_ID               = @M_USER_ID,
            M_DATETIME              = GETDATE()
        WHERE CONTRACT_ID = @CONTRACT_ID AND IS_DELETED = 0;

        COMMIT TRANSACTION;
        SET @Result = 1;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        SET @Result = 0;
        SET @ErrorMessage = ERROR_MESSAGE();
    END CATCH
END
GO

-- ── SOFT DELETE ──────────────────────────────────────────────
CREATE OR ALTER PROCEDURE usp_ManpowerContract_DELETE
    @CONTRACT_ID    INT,
    @M_USER_ID      INT,
    @Result         INT OUTPUT,
    @ErrorMessage   NVARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @Result = 0;
    SET @ErrorMessage = '';

    BEGIN TRY
        BEGIN TRANSACTION;

        UPDATE MANPOWER_CONTRACT_MST SET
            IS_DELETED  = 1,
            M_USER_ID   = @M_USER_ID,
            M_DATETIME  = GETDATE()
        WHERE CONTRACT_ID = @CONTRACT_ID;

        COMMIT TRANSACTION;
        SET @Result = 1;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        SET @Result = 0;
        SET @ErrorMessage = ERROR_MESSAGE();
    END CATCH
END
GO

-- ── GET BY ID ────────────────────────────────────────────────
CREATE OR ALTER PROCEDURE usp_ManpowerContract_GETBYID
    @CONTRACT_ID INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        mc.CONTRACT_ID,
        mc.YEAR,
        mc.MONTH,
        mc.RCO,
        mc.DIVISION,
        mc.GROUP_COMPANY_ID,
        gc.GROUP_COMPANY_NAME,
        mc.PLANT_ID,
        pm.PLANT_NAME,
        mc.PLANT_COUNTRY,
        mc.SUPPLIER_ID,
        sm.ERP_SUPPLIER_NAME,
        mc.GLOBAL_SUPPLIER_NAME,
        mc.SUPPLIER_NAME_REMARKS,
        mc.SUPPLIER_COUNTRY,
        mc.ANNUAL_SUPPLIER_SPEND,
        mc.CURRENCY_ID,
        cu.CURRENCY_CODE,
        mc.CONTRACTED,
        mc.WORKER_TYPE,
        mc.CONTRACTED_START_DATE,
        mc.CONTRACTED_END_DATE,
        mc.C_USER_ID,
        mc.C_DATETIME
    FROM MANPOWER_CONTRACT_MST mc WITH (NOLOCK)
    INNER JOIN GROUP_COMPANY_MST gc WITH (NOLOCK) ON gc.GROUP_COMPANY_ID = mc.GROUP_COMPANY_ID
    INNER JOIN PLANT_MST         pm WITH (NOLOCK) ON pm.PLANT_ID         = mc.PLANT_ID
    INNER JOIN SUPPLIER_MST      sm WITH (NOLOCK) ON sm.SUPPLIER_ID      = mc.SUPPLIER_ID
    INNER JOIN CURRENCY_MST      cu WITH (NOLOCK) ON cu.CURRENCY_ID      = mc.CURRENCY_ID
    WHERE mc.CONTRACT_ID = @CONTRACT_ID AND mc.IS_DELETED = 0
END
GO

-- ── GET ALL / SEARCH WITH SERVER-SIDE PAGING ─────────────────
CREATE OR ALTER PROCEDURE usp_ManpowerContract_SEARCH
    @GROUP_COMPANY_ID   INT             = NULL,
    @PLANT_ID           INT             = NULL,
    @SUPPLIER_ID        INT             = NULL,
    @SUPPLIER_COUNTRY   NVARCHAR(100)   = NULL,
    @CONTRACTED         NVARCHAR(5)     = NULL,
    @WORKER_TYPE        NVARCHAR(100)   = NULL,
    @SEARCH_VALUE       NVARCHAR(200)   = NULL,   -- global search box
    @ORDER_COLUMN       NVARCHAR(100)   = 'mc.CONTRACT_ID',
    @ORDER_DIR          NVARCHAR(4)     = 'DESC',
    @PAGE_START         INT             = 0,      -- DataTables "start" (offset)
    @PAGE_SIZE          INT             = 10,     -- DataTables "length"
    @TOTAL_RECORDS      INT             OUTPUT,   -- total after filters (for DataTables)
    @FILTERED_RECORDS   INT             OUTPUT    -- same as total (for DataTables recordsFiltered)
AS
BEGIN
    SET NOCOUNT ON;

    -- ── Step 1: get filtered total count ──────────────────────
    SELECT @TOTAL_RECORDS = COUNT(1)
    FROM MANPOWER_CONTRACT_MST mc WITH (NOLOCK)
    INNER JOIN GROUP_COMPANY_MST gc WITH (NOLOCK) ON gc.GROUP_COMPANY_ID = mc.GROUP_COMPANY_ID
    INNER JOIN PLANT_MST         pm WITH (NOLOCK) ON pm.PLANT_ID         = mc.PLANT_ID
    INNER JOIN SUPPLIER_MST      sm WITH (NOLOCK) ON sm.SUPPLIER_ID      = mc.SUPPLIER_ID
    INNER JOIN CURRENCY_MST      cu WITH (NOLOCK) ON cu.CURRENCY_ID      = mc.CURRENCY_ID
    WHERE mc.IS_DELETED = 0
      AND (@GROUP_COMPANY_ID IS NULL OR mc.GROUP_COMPANY_ID = @GROUP_COMPANY_ID)
      AND (@PLANT_ID         IS NULL OR mc.PLANT_ID         = @PLANT_ID)
      AND (@SUPPLIER_ID      IS NULL OR mc.SUPPLIER_ID      = @SUPPLIER_ID)
      AND (@SUPPLIER_COUNTRY IS NULL OR mc.SUPPLIER_COUNTRY = @SUPPLIER_COUNTRY)
      AND (@CONTRACTED       IS NULL OR mc.CONTRACTED       = @CONTRACTED)
      AND (@WORKER_TYPE      IS NULL OR mc.WORKER_TYPE      = @WORKER_TYPE)
      AND (
            @SEARCH_VALUE IS NULL
            OR gc.GROUP_COMPANY_NAME    LIKE '%' + @SEARCH_VALUE + '%'
            OR pm.PLANT_NAME            LIKE '%' + @SEARCH_VALUE + '%'
            OR sm.ERP_SUPPLIER_NAME     LIKE '%' + @SEARCH_VALUE + '%'
            OR mc.GLOBAL_SUPPLIER_NAME  LIKE '%' + @SEARCH_VALUE + '%'
            OR mc.DIVISION              LIKE '%' + @SEARCH_VALUE + '%'
            OR mc.RCO                   LIKE '%' + @SEARCH_VALUE + '%'
            OR mc.SUPPLIER_COUNTRY      LIKE '%' + @SEARCH_VALUE + '%'
            OR cu.CURRENCY_CODE         LIKE '%' + @SEARCH_VALUE + '%'
            OR CAST(mc.YEAR AS NVARCHAR) LIKE '%' + @SEARCH_VALUE + '%'
          );

    SET @FILTERED_RECORDS = @TOTAL_RECORDS;

    -- ── Step 2: return paged data ─────────────────────────────
    SELECT
        mc.CONTRACT_ID,
        mc.YEAR,
        mc.MONTH,
        mc.RCO,
        mc.DIVISION,
        gc.GROUP_COMPANY_NAME,
        pm.PLANT_NAME,
        mc.PLANT_COUNTRY,
        mc.GLOBAL_SUPPLIER_NAME,
        sm.ERP_SUPPLIER_NAME,
        mc.SUPPLIER_NAME_REMARKS,
        mc.SUPPLIER_COUNTRY,
        mc.ANNUAL_SUPPLIER_SPEND,
        cu.CURRENCY_CODE,
        mc.CONTRACTED,
        mc.CONTRACTED_START_DATE,
        mc.CONTRACTED_END_DATE
    FROM MANPOWER_CONTRACT_MST mc WITH (NOLOCK)
    INNER JOIN GROUP_COMPANY_MST gc WITH (NOLOCK) ON gc.GROUP_COMPANY_ID = mc.GROUP_COMPANY_ID
    INNER JOIN PLANT_MST         pm WITH (NOLOCK) ON pm.PLANT_ID         = mc.PLANT_ID
    INNER JOIN SUPPLIER_MST      sm WITH (NOLOCK) ON sm.SUPPLIER_ID      = mc.SUPPLIER_ID
    INNER JOIN CURRENCY_MST      cu WITH (NOLOCK) ON cu.CURRENCY_ID      = mc.CURRENCY_ID
    WHERE mc.IS_DELETED = 0
      AND (@GROUP_COMPANY_ID IS NULL OR mc.GROUP_COMPANY_ID = @GROUP_COMPANY_ID)
      AND (@PLANT_ID         IS NULL OR mc.PLANT_ID         = @PLANT_ID)
      AND (@SUPPLIER_ID      IS NULL OR mc.SUPPLIER_ID      = @SUPPLIER_ID)
      AND (@SUPPLIER_COUNTRY IS NULL OR mc.SUPPLIER_COUNTRY = @SUPPLIER_COUNTRY)
      AND (@CONTRACTED       IS NULL OR mc.CONTRACTED       = @CONTRACTED)
      AND (@WORKER_TYPE      IS NULL OR mc.WORKER_TYPE      = @WORKER_TYPE)
      AND (
            @SEARCH_VALUE IS NULL
            OR gc.GROUP_COMPANY_NAME    LIKE '%' + @SEARCH_VALUE + '%'
            OR pm.PLANT_NAME            LIKE '%' + @SEARCH_VALUE + '%'
            OR sm.ERP_SUPPLIER_NAME     LIKE '%' + @SEARCH_VALUE + '%'
            OR mc.GLOBAL_SUPPLIER_NAME  LIKE '%' + @SEARCH_VALUE + '%'
            OR mc.DIVISION              LIKE '%' + @SEARCH_VALUE + '%'
            OR mc.RCO                   LIKE '%' + @SEARCH_VALUE + '%'
            OR mc.SUPPLIER_COUNTRY      LIKE '%' + @SEARCH_VALUE + '%'
            OR cu.CURRENCY_CODE         LIKE '%' + @SEARCH_VALUE + '%'
            OR CAST(mc.YEAR AS NVARCHAR) LIKE '%' + @SEARCH_VALUE + '%'
          )
    ORDER BY
        CASE WHEN @ORDER_COLUMN = 'year'              AND @ORDER_DIR = 'ASC'  THEN mc.YEAR               END ASC,
        CASE WHEN @ORDER_COLUMN = 'year'              AND @ORDER_DIR = 'DESC' THEN mc.YEAR               END DESC,
        CASE WHEN @ORDER_COLUMN = 'month'             AND @ORDER_DIR = 'ASC'  THEN mc.MONTH              END ASC,
        CASE WHEN @ORDER_COLUMN = 'month'             AND @ORDER_DIR = 'DESC' THEN mc.MONTH              END DESC,
        CASE WHEN @ORDER_COLUMN = 'groupCompanyName'  AND @ORDER_DIR = 'ASC'  THEN gc.GROUP_COMPANY_NAME END ASC,
        CASE WHEN @ORDER_COLUMN = 'groupCompanyName'  AND @ORDER_DIR = 'DESC' THEN gc.GROUP_COMPANY_NAME END DESC,
        CASE WHEN @ORDER_COLUMN = 'plantName'         AND @ORDER_DIR = 'ASC'  THEN pm.PLANT_NAME         END ASC,
        CASE WHEN @ORDER_COLUMN = 'plantName'         AND @ORDER_DIR = 'DESC' THEN pm.PLANT_NAME         END DESC,
        CASE WHEN @ORDER_COLUMN = 'erpSupplierName'   AND @ORDER_DIR = 'ASC'  THEN sm.ERP_SUPPLIER_NAME  END ASC,
        CASE WHEN @ORDER_COLUMN = 'erpSupplierName'   AND @ORDER_DIR = 'DESC' THEN sm.ERP_SUPPLIER_NAME  END DESC,
        mc.CONTRACT_ID DESC  -- default sort
    OFFSET @PAGE_START ROWS
    FETCH NEXT @PAGE_SIZE ROWS ONLY;
END
GO

-- ── DROPDOWN LOOKUPS ─────────────────────────────────────────
CREATE OR ALTER PROCEDURE usp_Lookup_GroupCompany
AS
BEGIN
    SET NOCOUNT ON;
    SELECT GROUP_COMPANY_ID AS Id, GROUP_COMPANY_NAME AS Name
    FROM GROUP_COMPANY_MST WITH (NOLOCK)
    WHERE IS_DELETED = 0
    ORDER BY GROUP_COMPANY_NAME
END
GO

CREATE OR ALTER PROCEDURE usp_Lookup_Plant
    @GROUP_COMPANY_ID INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT PLANT_ID AS Id, PLANT_NAME AS Name, PLANT_COUNTRY AS Country
    FROM PLANT_MST WITH (NOLOCK)
    WHERE IS_DELETED = 0
      AND (@GROUP_COMPANY_ID IS NULL OR GROUP_COMPANY_ID = @GROUP_COMPANY_ID)
    ORDER BY PLANT_NAME
END
GO

CREATE OR ALTER PROCEDURE usp_Lookup_Supplier
AS
BEGIN
    SET NOCOUNT ON;
    SELECT SUPPLIER_ID AS Id, ERP_SUPPLIER_NAME AS Name, SUPPLIER_COUNTRY AS Country
    FROM SUPPLIER_MST WITH (NOLOCK)
    WHERE IS_DELETED = 0
    ORDER BY ERP_SUPPLIER_NAME
END
GO

CREATE OR ALTER PROCEDURE usp_Lookup_Currency
AS
BEGIN
    SET NOCOUNT ON;
    SELECT CURRENCY_ID AS Id, CURRENCY_CODE AS Name
    FROM CURRENCY_MST WITH (NOLOCK)
    ORDER BY CURRENCY_CODE
END
GO
