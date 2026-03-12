-- ============================================================
-- SEED DATA
-- ============================================================

INSERT INTO GROUP_COMPANY_MST (GROUP_COMPANY_NAME, IS_DELETED, C_USER_ID) VALUES
('MTS', 0, 1), ('SMRC', 0, 1), ('Aerospace Group', 0, 1);

INSERT INTO PLANT_MST (PLANT_NAME, GROUP_COMPANY_ID, PLANT_COUNTRY, IS_DELETED, C_USER_ID) VALUES
('MTS Noida',    1, 'India',  0, 1),
('Rougegoutte',  2, 'France', 0, 1),
('Igualada',     2, 'Spain',  0, 1);

INSERT INTO SUPPLIER_MST (ERP_SUPPLIER_NAME, GLOBAL_SUPPLIER_NAME, SUPPLIER_COUNTRY, IS_DELETED, C_USER_ID) VALUES
('Autovision', 'MTS Autovision', 'India',  0, 1),
('CRIT',       'SMRC CRIT',      'France', 0, 1),
('ADECCO',     'SMRC ADECCO',    'France', 0, 1);

INSERT INTO CURRENCY_MST (CURRENCY_CODE, CURRENCY_NAME) VALUES
('INR', 'Indian Rupee'),
('AED', 'UAE Dirham'),
('CZK', 'Czech Koruna'),
('EUR', 'Euro'),
('USD', 'US Dollar'),
('GBP', 'British Pound');
GO
