using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GroupPlus.BusinessObject.StaffManagement;
using GroupPlus.Common;

namespace GroupPlus.BusinessObject.Settings
{
    [Table("GPlus.PensionAdministrator")]
    public  class PensionAdministrator
    {
        public PensionAdministrator()
        {
            StaffPensions = new HashSet<StaffPension>();
        }
        public int PensionAdministratorId { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Pension Administrator's Name is required")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Pension Administrator's Name must be between 2 and 200 characters")]
        public string Name { get; set; }

        public ItemStatus Status { get; set; }

        public ICollection<StaffPension> StaffPensions { get; set; }
    }
}
