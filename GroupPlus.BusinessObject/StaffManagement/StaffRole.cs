using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GroupPlus.Common;

namespace GroupPlus.BusinessObject.StaffManagement
{
    [Table("GPlus.StaffRole")]
    public class StaffRole
    {
        public int StaffRoleId { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Name is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 50 characters")]
        public string Name { get; set; }

        public ItemStatus Status { get; set; }
    }
}
