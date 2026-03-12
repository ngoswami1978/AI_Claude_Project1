using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using ManpowerContract.Models;

namespace ManpowerContract.ViewModels
{
    public class ManpowerContractViewModel
    {
        public ManpowerContractModel Contract { get; set; } = new ManpowerContractModel();

        // Filter fields
        public int? FilterGroupCompanyId { get; set; }
        public int? FilterPlantId { get; set; }
        public int? FilterSupplierId { get; set; }
        public string FilterSupplierCountry { get; set; }
        public string FilterContracted { get; set; }
        public string FilterWorkerType { get; set; }

        // Dropdowns
        public List<SelectListItem> GroupCompanyList { get; set; } = new();
        public List<SelectListItem> PlantList { get; set; } = new();
        public List<SelectListItem> SupplierList { get; set; } = new();
        public List<SelectListItem> CurrencyList { get; set; } = new();

        public List<SelectListItem> ContractedList { get; set; } = new()
        {
            new SelectListItem { Value = "Yes", Text = "Yes" },
            new SelectListItem { Value = "No",  Text = "No"  }
        };

        public List<SelectListItem> WorkerTypeList { get; set; } = new()
        {
            new SelectListItem { Value = "Permanent",   Text = "Permanent"   },
            new SelectListItem { Value = "Contract",    Text = "Contract"    },
            new SelectListItem { Value = "Temporary",   Text = "Temporary"   },
            new SelectListItem { Value = "Part-Time",   Text = "Part-Time"   }
        };

        public List<SelectListItem> MonthList { get; set; } = new()
        {
            new SelectListItem { Value = "January",   Text = "January"   },
            new SelectListItem { Value = "February",  Text = "February"  },
            new SelectListItem { Value = "March",     Text = "March"     },
            new SelectListItem { Value = "April",     Text = "April"     },
            new SelectListItem { Value = "May",       Text = "May"       },
            new SelectListItem { Value = "June",      Text = "June"      },
            new SelectListItem { Value = "July",      Text = "July"      },
            new SelectListItem { Value = "August",    Text = "August"    },
            new SelectListItem { Value = "September", Text = "September" },
            new SelectListItem { Value = "October",   Text = "October"   },
            new SelectListItem { Value = "November",  Text = "November"  },
            new SelectListItem { Value = "December",  Text = "December"  }
        };
    }
}
