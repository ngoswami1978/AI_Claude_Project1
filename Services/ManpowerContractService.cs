using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ManpowerContract.Models;
using ManpowerContract.Repositories;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManpowerContract.Services
{
    // ── INTERFACE ───────────────────────────────────────────────
    public interface IManpowerContractService
    {
        Task<(bool Success, string Message)> SaveAsync(ManpowerContractModel model);
        Task<(bool Success, string Message)> DeleteAsync(int contractId, int userId);
        Task<ManpowerContractModel> GetByIdAsync(int contractId);
        Task<(List<ManpowerContractModel> Data, int TotalRecords, int FilteredRecords)> SearchAsync(
            int? groupCompanyId, int? plantId, int? supplierId,
            string supplierCountry, string contracted, string workerType,
            string searchValue, string orderColumn, string orderDir,
            int pageStart, int pageSize);
        Task<List<SelectListItem>> GetGroupCompanyDropdownAsync();
        Task<List<SelectListItem>> GetPlantDropdownAsync(int? groupCompanyId);
        Task<Dictionary<string, string>> GetPlantCountryMapAsync(int? groupCompanyId);
        Task<List<SelectListItem>> GetSupplierDropdownAsync();
        Task<List<SelectListItem>> GetCurrencyDropdownAsync();
    }

    // ── IMPLEMENTATION ──────────────────────────────────────────
    public class ManpowerContractService : IManpowerContractService
    {
        private readonly IManpowerContractRepository _repo;

        public ManpowerContractService(IManpowerContractRepository repo)
        {
            _repo = repo;
        }

        public async Task<(bool Success, string Message)> SaveAsync(ManpowerContractModel model)
        {
            // Business validation
            if (model.ContractedEndDate <= model.ContractedStartDate)
                return (false, "Contracted End Date must be after Start Date.");

            if (model.AnnualSupplierSpend < 0)
                return (false, "Annual Supplier Spend cannot be negative.");

            return model.ContractId == 0
                ? await _repo.InsertAsync(model)
                : await _repo.UpdateAsync(model);
        }

        public Task<(bool Success, string Message)> DeleteAsync(int contractId, int userId)
            => _repo.DeleteAsync(contractId, userId);

        public Task<ManpowerContractModel> GetByIdAsync(int contractId)
            => _repo.GetByIdAsync(contractId);

        public Task<(List<ManpowerContractModel> Data, int TotalRecords, int FilteredRecords)> SearchAsync(
            int? groupCompanyId, int? plantId, int? supplierId,
            string supplierCountry, string contracted, string workerType,
            string searchValue, string orderColumn, string orderDir,
            int pageStart, int pageSize)
            => _repo.SearchAsync(groupCompanyId, plantId, supplierId, supplierCountry,
                                 contracted, workerType, searchValue, orderColumn,
                                 orderDir, pageStart, pageSize);

        public async Task<List<SelectListItem>> GetGroupCompanyDropdownAsync()
        {
            var items = await _repo.GetGroupCompaniesAsync();
            var list  = new List<SelectListItem> { new SelectListItem { Value = "", Text = "Select" } };
            foreach (var i in items) list.Add(new SelectListItem { Value = i.Id.ToString(), Text = i.Name });
            return list;
        }

        public async Task<List<SelectListItem>> GetPlantDropdownAsync(int? groupCompanyId)
        {
            var items = await _repo.GetPlantsAsync(groupCompanyId);
            var list  = new List<SelectListItem> { new SelectListItem { Value = "", Text = "Select" } };
            foreach (var i in items)
                list.Add(new SelectListItem { Value = i.Id.ToString(), Text = i.Name });
            return list;
        }

        // Returns a PlantId → Country map so the controller can expose it to JS
        public async Task<Dictionary<string, string>> GetPlantCountryMapAsync(int? groupCompanyId)
        {
            var items = await _repo.GetPlantsAsync(groupCompanyId);
            var map   = new Dictionary<string, string>();
            foreach (var i in items)
                map[i.Id.ToString()] = i.Country ?? "";
            return map;
        }

        public async Task<List<SelectListItem>> GetSupplierDropdownAsync()
        {
            var items = await _repo.GetSuppliersAsync();
            var list  = new List<SelectListItem> { new SelectListItem { Value = "", Text = "Select" } };
            foreach (var i in items) list.Add(new SelectListItem { Value = i.Id.ToString(), Text = i.Name });
            return list;
        }

        public async Task<List<SelectListItem>> GetCurrencyDropdownAsync()
        {
            var items = await _repo.GetCurrenciesAsync();
            var list  = new List<SelectListItem> { new SelectListItem { Value = "", Text = "Select" } };
            foreach (var i in items) list.Add(new SelectListItem { Value = i.Id.ToString(), Text = i.Name });
            return list;
        }
    }
}
