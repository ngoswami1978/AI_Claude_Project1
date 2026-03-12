using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using ManpowerContract.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ManpowerContract.Repositories
{
    // ── INTERFACE ───────────────────────────────────────────────
    public interface IManpowerContractRepository
    {
        Task<(bool Success, string Message)> InsertAsync(ManpowerContractModel model);
        Task<(bool Success, string Message)> UpdateAsync(ManpowerContractModel model);
        Task<(bool Success, string Message)> DeleteAsync(int contractId, int userId);
        Task<ManpowerContractModel> GetByIdAsync(int contractId);
        Task<(List<ManpowerContractModel> Data, int TotalRecords, int FilteredRecords)> SearchAsync(
            int? groupCompanyId, int? plantId, int? supplierId,
            string supplierCountry, string contracted, string workerType,
            string searchValue, string orderColumn, string orderDir,
            int pageStart, int pageSize);
        Task<List<LookupModel>> GetGroupCompaniesAsync();
        Task<List<LookupModel>> GetPlantsAsync(int? groupCompanyId);
        Task<List<LookupModel>> GetSuppliersAsync();
        Task<List<LookupModel>> GetCurrenciesAsync();
    }

    // ── IMPLEMENTATION ──────────────────────────────────────────
    public class ManpowerContractRepository : IManpowerContractRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<ManpowerContractRepository> _logger;

        public ManpowerContractRepository(IConfiguration config, ILogger<ManpowerContractRepository> logger)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
            _logger = logger;
        }

        // ── INSERT ──────────────────────────────────────────────
        public async Task<(bool Success, string Message)> InsertAsync(ManpowerContractModel m)
        {
            try
            {
                using var con = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("usp_ManpowerContract_INSERT", con)
                { CommandType = CommandType.StoredProcedure };

                AddContractParams(cmd, m);
                cmd.Parameters.AddWithValue("@C_USER_ID", m.CUserId);
                var pResult  = cmd.Parameters.Add("@Result",       SqlDbType.Int);         pResult.Direction  = ParameterDirection.Output;
                var pMessage = cmd.Parameters.Add("@ErrorMessage", SqlDbType.NVarChar, 500); pMessage.Direction = ParameterDirection.Output;

                await con.OpenAsync();
                await cmd.ExecuteNonQueryAsync();

                int result = (int)pResult.Value;
                string msg = pMessage.Value?.ToString() ?? "";
                return (result == 1, result == 1 ? "Saved successfully." : msg);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "InsertAsync failed");
                return (false, "An error occurred while saving.");
            }
        }

        // ── UPDATE ──────────────────────────────────────────────
        public async Task<(bool Success, string Message)> UpdateAsync(ManpowerContractModel m)
        {
            try
            {
                using var con = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("usp_ManpowerContract_UPDATE", con)
                { CommandType = CommandType.StoredProcedure };

                cmd.Parameters.AddWithValue("@CONTRACT_ID", m.ContractId);
                AddContractParams(cmd, m);
                cmd.Parameters.AddWithValue("@M_USER_ID", m.CUserId);
                var pResult  = cmd.Parameters.Add("@Result",       SqlDbType.Int);          pResult.Direction  = ParameterDirection.Output;
                var pMessage = cmd.Parameters.Add("@ErrorMessage", SqlDbType.NVarChar, 500); pMessage.Direction = ParameterDirection.Output;

                await con.OpenAsync();
                await cmd.ExecuteNonQueryAsync();

                int result = (int)pResult.Value;
                string msg = pMessage.Value?.ToString() ?? "";
                return (result == 1, result == 1 ? "Updated successfully." : msg);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateAsync failed");
                return (false, "An error occurred while updating.");
            }
        }

        // ── DELETE ──────────────────────────────────────────────
        public async Task<(bool Success, string Message)> DeleteAsync(int contractId, int userId)
        {
            try
            {
                using var con = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("usp_ManpowerContract_DELETE", con)
                { CommandType = CommandType.StoredProcedure };

                cmd.Parameters.AddWithValue("@CONTRACT_ID", contractId);
                cmd.Parameters.AddWithValue("@M_USER_ID",   userId);
                var pResult  = cmd.Parameters.Add("@Result",       SqlDbType.Int);          pResult.Direction  = ParameterDirection.Output;
                var pMessage = cmd.Parameters.Add("@ErrorMessage", SqlDbType.NVarChar, 500); pMessage.Direction = ParameterDirection.Output;

                await con.OpenAsync();
                await cmd.ExecuteNonQueryAsync();

                int result = (int)pResult.Value;
                string msg = pMessage.Value?.ToString() ?? "";
                return (result == 1, result == 1 ? "Deleted successfully." : msg);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteAsync failed");
                return (false, "An error occurred while deleting.");
            }
        }

        // ── GET BY ID ───────────────────────────────────────────
        public async Task<ManpowerContractModel> GetByIdAsync(int contractId)
        {
            try
            {
                using var con = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("usp_ManpowerContract_GETBYID", con)
                { CommandType = CommandType.StoredProcedure };

                cmd.Parameters.AddWithValue("@CONTRACT_ID", contractId);
                await con.OpenAsync();

                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                    return MapContract(reader);

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByIdAsync failed");
                return null;
            }
        }

        // ── SEARCH WITH SERVER-SIDE PAGING ──────────────────────
        public async Task<(List<ManpowerContractModel> Data, int TotalRecords, int FilteredRecords)> SearchAsync(
            int? groupCompanyId, int? plantId, int? supplierId,
            string supplierCountry, string contracted, string workerType,
            string searchValue, string orderColumn, string orderDir,
            int pageStart, int pageSize)
        {
            var list = new List<ManpowerContractModel>();
            int totalRecords    = 0;
            int filteredRecords = 0;

            try
            {
                using var con = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("usp_ManpowerContract_SEARCH", con)
                { CommandType = CommandType.StoredProcedure };

                cmd.Parameters.AddWithValue("@GROUP_COMPANY_ID", (object)groupCompanyId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PLANT_ID",         (object)plantId        ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@SUPPLIER_ID",      (object)supplierId      ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@SUPPLIER_COUNTRY",
                    string.IsNullOrEmpty(supplierCountry) ? (object)DBNull.Value : supplierCountry);
                cmd.Parameters.AddWithValue("@CONTRACTED",
                    string.IsNullOrEmpty(contracted) ? (object)DBNull.Value : contracted);
                cmd.Parameters.AddWithValue("@WORKER_TYPE",
                    string.IsNullOrEmpty(workerType) ? (object)DBNull.Value : workerType);
                cmd.Parameters.AddWithValue("@SEARCH_VALUE",
                    string.IsNullOrEmpty(searchValue) ? (object)DBNull.Value : searchValue);
                cmd.Parameters.AddWithValue("@ORDER_COLUMN",
                    string.IsNullOrEmpty(orderColumn) ? "mc.CONTRACT_ID" : orderColumn);
                cmd.Parameters.AddWithValue("@ORDER_DIR",
                    string.IsNullOrEmpty(orderDir) ? "DESC" : orderDir.ToUpper());
                cmd.Parameters.AddWithValue("@PAGE_START", pageStart);
                cmd.Parameters.AddWithValue("@PAGE_SIZE",  pageSize);

                var pTotal    = cmd.Parameters.Add("@TOTAL_RECORDS",    SqlDbType.Int);
                var pFiltered = cmd.Parameters.Add("@FILTERED_RECORDS", SqlDbType.Int);
                pTotal.Direction    = ParameterDirection.Output;
                pFiltered.Direction = ParameterDirection.Output;

                await con.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                    list.Add(MapContractList(reader));

                // Read output params AFTER closing reader
                await reader.CloseAsync();
                totalRecords    = pTotal.Value    != DBNull.Value ? (int)pTotal.Value    : 0;
                filteredRecords = pFiltered.Value != DBNull.Value ? (int)pFiltered.Value : 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SearchAsync failed");
            }

            return (list, totalRecords, filteredRecords);
        }

        // ── LOOKUPS ─────────────────────────────────────────────
        public async Task<List<LookupModel>> GetGroupCompaniesAsync()
            => await GetLookupAsync("usp_Lookup_GroupCompany", null);

        public async Task<List<LookupModel>> GetPlantsAsync(int? groupCompanyId)
            => await GetLookupAsync("usp_Lookup_Plant", groupCompanyId);

        public async Task<List<LookupModel>> GetSuppliersAsync()
            => await GetLookupAsync("usp_Lookup_Supplier", null);

        public async Task<List<LookupModel>> GetCurrenciesAsync()
            => await GetLookupAsync("usp_Lookup_Currency", null);

        private async Task<List<LookupModel>> GetLookupAsync(string spName, int? paramValue)
        {
            var list = new List<LookupModel>();
            try
            {
                using var con = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(spName, con) { CommandType = CommandType.StoredProcedure };

                if (spName == "usp_Lookup_Plant")
                    cmd.Parameters.AddWithValue("@GROUP_COMPANY_ID", (object)paramValue ?? DBNull.Value);

                await con.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                    list.Add(new LookupModel
                    {
                        Id      = reader.GetInt32(reader.GetOrdinal("Id")),
                        Name    = reader["Name"]?.ToString(),
                        Country = reader.HasColumn("Country") ? reader["Country"]?.ToString() : null
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GetLookupAsync({spName}) failed");
            }
            return list;
        }

        // ── MAPPER HELPERS ──────────────────────────────────────
        private static ManpowerContractModel MapContract(SqlDataReader r) => new()
        {
            ContractId           = r.GetInt32(r.GetOrdinal("CONTRACT_ID")),
            Year                 = r.GetInt32(r.GetOrdinal("YEAR")),
            Month                = r["MONTH"]?.ToString(),
            Rco                  = r["RCO"]?.ToString(),
            Division             = r["DIVISION"]?.ToString(),
            GroupCompanyId       = r.GetInt32(r.GetOrdinal("GROUP_COMPANY_ID")),
            GroupCompanyName     = r["GROUP_COMPANY_NAME"]?.ToString(),
            PlantId              = r.GetInt32(r.GetOrdinal("PLANT_ID")),
            PlantName            = r["PLANT_NAME"]?.ToString(),
            PlantCountry         = r["PLANT_COUNTRY"]?.ToString(),
            SupplierId           = r.GetInt32(r.GetOrdinal("SUPPLIER_ID")),
            ErpSupplierName      = r["ERP_SUPPLIER_NAME"]?.ToString(),
            GlobalSupplierName   = r["GLOBAL_SUPPLIER_NAME"]?.ToString(),
            SupplierNameRemarks  = r["SUPPLIER_NAME_REMARKS"]?.ToString(),
            SupplierCountry      = r["SUPPLIER_COUNTRY"]?.ToString(),
            AnnualSupplierSpend  = r.GetDecimal(r.GetOrdinal("ANNUAL_SUPPLIER_SPEND")),
            CurrencyId           = r.GetInt32(r.GetOrdinal("CURRENCY_ID")),
            CurrencyCode         = r["CURRENCY_CODE"]?.ToString(),
            Contracted           = r["CONTRACTED"]?.ToString(),
            WorkerType           = r["WORKER_TYPE"]?.ToString(),
            ContractedStartDate  = r.GetDateTime(r.GetOrdinal("CONTRACTED_START_DATE")),
            ContractedEndDate    = r.GetDateTime(r.GetOrdinal("CONTRACTED_END_DATE"))
        };

        private static ManpowerContractModel MapContractList(SqlDataReader r) => new()
        {
            ContractId          = r.GetInt32(r.GetOrdinal("CONTRACT_ID")),
            Year                = r.GetInt32(r.GetOrdinal("YEAR")),
            Month               = r["MONTH"]?.ToString(),
            Rco                 = r["RCO"]?.ToString(),
            Division            = r["DIVISION"]?.ToString(),
            GroupCompanyName    = r["GROUP_COMPANY_NAME"]?.ToString(),
            PlantName           = r["PLANT_NAME"]?.ToString(),
            PlantCountry        = r["PLANT_COUNTRY"]?.ToString(),
            GlobalSupplierName  = r["GLOBAL_SUPPLIER_NAME"]?.ToString(),
            ErpSupplierName     = r["ERP_SUPPLIER_NAME"]?.ToString(),
            SupplierNameRemarks = r["SUPPLIER_NAME_REMARKS"]?.ToString(),
            SupplierCountry     = r["SUPPLIER_COUNTRY"]?.ToString(),
            AnnualSupplierSpend = r.GetDecimal(r.GetOrdinal("ANNUAL_SUPPLIER_SPEND")),
            CurrencyCode        = r["CURRENCY_CODE"]?.ToString(),
            Contracted          = r["CONTRACTED"]?.ToString(),
            ContractedStartDate = r.GetDateTime(r.GetOrdinal("CONTRACTED_START_DATE")),
            ContractedEndDate   = r.GetDateTime(r.GetOrdinal("CONTRACTED_END_DATE"))
        };

        private static void AddContractParams(SqlCommand cmd, ManpowerContractModel m)
        {
            cmd.Parameters.AddWithValue("@YEAR",                   m.Year);
            cmd.Parameters.AddWithValue("@MONTH",                  m.Month);
            cmd.Parameters.AddWithValue("@RCO",                    m.Rco);
            cmd.Parameters.AddWithValue("@DIVISION",               m.Division);
            cmd.Parameters.AddWithValue("@GROUP_COMPANY_ID",       m.GroupCompanyId);
            cmd.Parameters.AddWithValue("@PLANT_ID",               m.PlantId);
            cmd.Parameters.AddWithValue("@PLANT_COUNTRY",          (object)m.PlantCountry        ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@SUPPLIER_ID",            m.SupplierId);
            cmd.Parameters.AddWithValue("@GLOBAL_SUPPLIER_NAME",   (object)m.GlobalSupplierName  ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@SUPPLIER_NAME_REMARKS",  (object)m.SupplierNameRemarks ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@SUPPLIER_COUNTRY",       (object)m.SupplierCountry     ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ANNUAL_SUPPLIER_SPEND",  m.AnnualSupplierSpend);
            cmd.Parameters.AddWithValue("@CURRENCY_ID",            m.CurrencyId);
            cmd.Parameters.AddWithValue("@CONTRACTED",             m.Contracted);
            cmd.Parameters.AddWithValue("@WORKER_TYPE",            (object)m.WorkerType          ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CONTRACTED_START_DATE",  m.ContractedStartDate);
            cmd.Parameters.AddWithValue("@CONTRACTED_END_DATE",    m.ContractedEndDate);
        }
    }

    // ── EXTENSION HELPER ────────────────────────────────────────
    public static class DataReaderExtensions
    {
        public static bool HasColumn(this SqlDataReader reader, string columnName)
        {
            for (int i = 0; i < reader.FieldCount; i++)
                if (reader.GetName(i).Equals(columnName, StringComparison.OrdinalIgnoreCase))
                    return true;
            return false;
        }
    }
}
