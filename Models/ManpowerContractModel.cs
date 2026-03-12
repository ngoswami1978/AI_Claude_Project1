using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ManpowerContract.Models
{
    public class ManpowerContractModel
    {
        public int ContractId { get; set; }

        [Required(ErrorMessage = "Year is required")]
        [Range(2000, 2100, ErrorMessage = "Enter a valid year")]
        [Display(Name = "Year")]
        public int Year { get; set; }

        [Required(ErrorMessage = "Month is required")]
        [Display(Name = "Month")]
        public string Month { get; set; }

        [Required(ErrorMessage = "RCO is required")]
        [StringLength(100)]
        [Display(Name = "RCO")]
        public string Rco { get; set; }

        [Required(ErrorMessage = "Division is required")]
        [StringLength(200)]
        [Display(Name = "Division")]
        public string Division { get; set; }

        [Required(ErrorMessage = "Group Company is required")]
        [Display(Name = "Group Company")]
        public int GroupCompanyId { get; set; }

        [ValidateNever]   // display-only — populated from DB join, not from form
        public string GroupCompanyName { get; set; }

        [Required(ErrorMessage = "Plant is required")]
        [Display(Name = "Plant Name")]
        public int PlantId { get; set; }

        [ValidateNever]   // display-only — populated from DB join, not from form
        public string PlantName { get; set; }

        [ValidateNever]   // auto-filled from plant selection
        [StringLength(100)]
        [Display(Name = "Plant Country")]
        public string PlantCountry { get; set; }

        [Required(ErrorMessage = "Supplier is required")]
        [Display(Name = "ERP Supplier Name")]
        public int SupplierId { get; set; }

        [ValidateNever]   // display-only — populated from DB join, not from form
        public string ErpSupplierName { get; set; }

        [ValidateNever]   // optional free-text field
        [StringLength(200)]
        [Display(Name = "Global Supplier Name")]
        public string GlobalSupplierName { get; set; }

        [ValidateNever]   // optional free-text field
        [StringLength(500)]
        [Display(Name = "Supplier Name Remarks")]
        public string SupplierNameRemarks { get; set; }

        [ValidateNever]   // optional free-text field
        [StringLength(100)]
        [Display(Name = "Supplier Country")]
        public string SupplierCountry { get; set; }

        [Required(ErrorMessage = "Annual Supplier Spend is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Spend must be positive")]
        [Display(Name = "Annual Supplier Spend")]
        public decimal AnnualSupplierSpend { get; set; }

        [Required(ErrorMessage = "Currency is required")]
        [Display(Name = "Currency")]
        public int CurrencyId { get; set; }

        [ValidateNever]   // display-only — populated from DB join, not from form
        public string CurrencyCode { get; set; }

        [ValidateNever]   // optional dropdown
        [Display(Name = "Contracted")]
        public string Contracted { get; set; }

        [ValidateNever]   // optional dropdown
        [StringLength(100)]
        [Display(Name = "Worker Type")]
        public string WorkerType { get; set; }

        [Required(ErrorMessage = "Contracted Start Date is required")]
        [Display(Name = "Contracted Start Date")]
        public DateTime ContractedStartDate { get; set; }

        [Required(ErrorMessage = "Contracted End Date is required")]
        [Display(Name = "Contracted End Date")]
        public DateTime ContractedEndDate { get; set; }

        public int CUserId { get; set; }

        [ValidateNever]
        public DateTime CDatetime { get; set; }
    }

    public class LookupModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
    }
}
