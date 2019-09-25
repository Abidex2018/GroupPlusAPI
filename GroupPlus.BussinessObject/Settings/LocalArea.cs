using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GroupPlus.BussinessObject.StaffManagement;
using GroupPlus.Common;

namespace GroupPlus.BussinessObject.Settings
{
    [Table("GPlus.LocalArea")]
    public class LocalArea
    {
        public LocalArea()
        {
            Staffs = new HashSet<Staff>();
        }
        public int LocalAreaId { get; set; }

        public int StateId { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Local Area is required")]
        [StringLength(150, MinimumLength = 2, ErrorMessage = "Local Area must be between 2 and 150 characters")]
        public string Name { get; set; }

        public ItemStatus Status { get; set; }

        public virtual State State { get; set; }

        public ICollection<Staff> Staffs { get; set; }
    }
}
