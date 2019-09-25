using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GroupPlus.BusinessObject.StaffManagement;
using GroupPlus.Common;

namespace GroupPlus.BusinessObject.Settings
{
    [Table("GPlus.InsurancePolicyType")]
    public class InsurancePolicyType
    {
        public InsurancePolicyType()
        {
            StaffInsurances = new HashSet<StaffInsurance>();
        }
        public int InsurancePolicyTypeId { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Policy Name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Policy Name must be between 2 and 50 characters")]
        public string Name { get; set; }

        public ItemStatus Status { get; set; }
        public ICollection<StaffInsurance> StaffInsurances { get; set; }
    }
}
