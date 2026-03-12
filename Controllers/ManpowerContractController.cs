using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ManpowerContract.Models;
using ManpowerContract.Services;
using ManpowerContract.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ClosedXML.Excel;

namespace ManpowerContract.Controllers
{
    public class ManpowerContractController : Controller
    {
        private readonly IManpowerContractService _service;
        private readonly ILogger<ManpowerContractController> _logger;
        private int CurrentUserId => 1; // Replace with session/claims user ID

        public ManpowerContractController(IManpowerContractService service, ILogger<ManpowerContractController> logger)
        {
            _service = service;
            _logger = logger;
        }

        // ── INDEX ────────────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var vm = new ManpowerContractViewModel
            {
                GroupCompanyList = await _service.GetGroupCompanyDropdownAsync(),
                PlantList = await _service.GetPlantDropdownAsync(null),
                SupplierList = await _service.GetSupplierDropdownAsync(),
                CurrencyList = await _service.GetCurrencyDropdownAsync()
            };
            return View(vm);
        }

        // ── GET ALL — SERVER-SIDE PAGING (AJAX) ─────────────────
        [HttpGet]
        public async Task<IActionResult> GetAll(
            int draw, int start, int length,
            int? groupCompanyId, int? plantId, int? supplierId,
            string supplierCountry, string contracted, string workerType,
            // These come flattened from JS (not nested DataTables format)
            string searchValue, int? orderColumnIndex, string orderDir)
        {
            try
            {
                // Map DataTables column index → SP column name
                var columnMap = new Dictionary<int, string>
                {
                    { 1,  "year"               },
                    { 2,  "month"              },
                    { 3,  "rco"                },
                    { 4,  "division"           },
                    { 5,  "groupCompanyName"   },
                    { 6,  "plantName"          },
                    { 7,  "plantCountry"       },
                    { 8,  "globalSupplierName" },
                    { 9,  "erpSupplierName"    },
                    { 12, "annualSupplierSpend"},
                    { 14, "contracted"         },
                    { 15, "contractedStartDate"},
                    { 16, "contractedEndDate"  }
                };

                string orderColumn = columnMap.TryGetValue(orderColumnIndex ?? 0, out var col)
                    ? col : "mc.CONTRACT_ID";
                string dir = string.Equals(orderDir, "asc", StringComparison.OrdinalIgnoreCase)
                    ? "ASC" : "DESC";

                int pageLength = length > 0 ? length : 10;

                var (data, totalRecords, filteredRecords) = await _service.SearchAsync(
                    groupCompanyId, plantId, supplierId,
                    string.IsNullOrWhiteSpace(supplierCountry) ? null : supplierCountry,
                    string.IsNullOrWhiteSpace(contracted) ? null : contracted,
                    string.IsNullOrWhiteSpace(workerType) ? null : workerType,
                    string.IsNullOrWhiteSpace(searchValue) ? null : searchValue.Trim(),
                    orderColumn, dir,
                    start, pageLength
                );

                return Json(new
                {
                    draw = draw,
                    recordsTotal = totalRecords,
                    recordsFiltered = filteredRecords,
                    data = data.Select(r => new
                    {
                        contractId = r.ContractId,
                        year = r.Year,
                        month = r.Month,
                        rco = r.Rco,
                        division = r.Division,
                        groupCompanyName = r.GroupCompanyName,
                        plantName = r.PlantName,
                        plantCountry = r.PlantCountry ?? "",
                        globalSupplierName = r.GlobalSupplierName ?? "",
                        erpSupplierName = r.ErpSupplierName ?? "",
                        supplierNameRemarks = r.SupplierNameRemarks ?? "",
                        supplierCountry = r.SupplierCountry ?? "",
                        annualSupplierSpend = r.AnnualSupplierSpend,
                        currencyCode = r.CurrencyCode ?? "",
                        contracted = r.Contracted ?? "",
                        contractedStartDate = r.ContractedStartDate.ToString("dd-MM-yyyy"),
                        contractedEndDate = r.ContractedEndDate.ToString("dd-MM-yyyy")
                    })
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAll failed");
                return Json(new
                {
                    draw = draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<object>(),
                    error = "Failed to load data."
                });
            }
        }

        // ── GET BY ID (AJAX) ─────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var data = await _service.GetByIdAsync(id);
                if (data == null)
                    return Json(new { success = false, message = "Record not found." });

                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetById failed");
                return Json(new { success = false, message = "Failed to fetch record." });
            }
        }

        // ── SAVE (INSERT / UPDATE) ───────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save([FromBody] ManpowerContractModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return Json(new { success = false, message = string.Join("; ", errors) });
                }

                model.CUserId = CurrentUserId;
                var (success, message) = await _service.SaveAsync(model);
                return Json(new { success, message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Save failed");
                return Json(new { success = false, message = "An error occurred while saving." });
            }
        }

        // ── DELETE ───────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromBody] int id)
        {
            try
            {
                var (success, message) = await _service.DeleteAsync(id, CurrentUserId);
                return Json(new { success, message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Delete failed");
                return Json(new { success = false, message = "An error occurred while deleting." });
            }
        }

        // ── GET PLANTS BY GROUP COMPANY (cascading dropdown) ─────
        [HttpGet]
        public async Task<IActionResult> GetPlants(int? groupCompanyId)
        {
            var plants = await _service.GetPlantDropdownAsync(groupCompanyId);
            var countryMap = await _service.GetPlantCountryMapAsync(groupCompanyId);

            var result = plants
                .Where(x => x.Value != "")
                .Select(x => new
                {
                    value = x.Value,
                    text = x.Text,
                    country = countryMap.TryGetValue(x.Value, out var c) ? c : ""
                });

            return Json(result);
        }

        // ── EXPORT TO EXCEL ──────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> ExportExcel(
            int? groupCompanyId, int? plantId, int? supplierId,
            string supplierCountry, string contracted, string workerType)
        {
            try
            {
                // Export = no paging (pageStart=0, pageSize=int.MaxValue = all rows)
                // No search value, no specific sort needed for export
                var (data, _, _) = await _service.SearchAsync(
                    groupCompanyId, plantId, supplierId,
                    supplierCountry, contracted, workerType,
                    searchValue: null,
                    orderColumn: "mc.CONTRACT_ID",
                    orderDir: "DESC",
                    pageStart: 0,
                    pageSize: int.MaxValue
                );

                using var wb = new XLWorkbook();
                var ws = wb.Worksheets.Add("Manpower Contracts");

                // Header row style
                var headerStyle = ws.Range("A1:Q1");
                string[] headers = {
                    "Year","Month","RCO","Division","Group Company","Plant Name",
                    "Plant Country","Global Supplier","ERP Supplier","Supplier Remarks",
                    "Supplier Country","Annual Spend","Currency","Contracted",
                    "Contracted Start","Contracted End"
                };
                for (int i = 0; i < headers.Length; i++)
                {
                    ws.Cell(1, i + 1).Value = headers[i];
                    ws.Cell(1, i + 1).Style.Font.Bold = true;
                    ws.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#009688");
                    ws.Cell(1, i + 1).Style.Font.FontColor = XLColor.White;
                }

                // Data rows
                int row = 2;
                foreach (var r in data)
                {
                    ws.Cell(row, 1).Value = r.Year;
                    ws.Cell(row, 2).Value = r.Month;
                    ws.Cell(row, 3).Value = r.Rco;
                    ws.Cell(row, 4).Value = r.Division;
                    ws.Cell(row, 5).Value = r.GroupCompanyName;
                    ws.Cell(row, 6).Value = r.PlantName;
                    ws.Cell(row, 7).Value = r.PlantCountry;
                    ws.Cell(row, 8).Value = r.GlobalSupplierName;
                    ws.Cell(row, 9).Value = r.ErpSupplierName;
                    ws.Cell(row, 10).Value = r.SupplierNameRemarks;
                    ws.Cell(row, 11).Value = r.SupplierCountry;
                    ws.Cell(row, 12).Value = (double)r.AnnualSupplierSpend;
                    ws.Cell(row, 13).Value = r.CurrencyCode;
                    ws.Cell(row, 14).Value = r.Contracted;
                    ws.Cell(row, 15).Value = r.ContractedStartDate.ToString("dd-MM-yyyy");
                    ws.Cell(row, 16).Value = r.ContractedEndDate.ToString("dd-MM-yyyy");
                    row++;
                }

                ws.Columns().AdjustToContents();

                using var ms = new MemoryStream();
                wb.SaveAs(ms);
                ms.Position = 0;

                return File(ms.ToArray(),
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"ManpowerContracts_{DateTime.Now:yyyyMMdd_HHmm}.xlsx");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ExportExcel failed");
                return Json(new { success = false, message = "Export failed." });
            }
        }
    }
}