using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GroupPlus.BussinessObject.StaffManagement;
using GroupPlus.Common;

namespace GroupPlus.BussinessObject.Settings
{
    [Table("GPlus.KPIndex")]
    public class KPIndex
    {
        public KPIndex()
        {
            StaffKPIndexes = new HashSet<StaffKPIndex>();
        }
        public int KPIndexId { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "KPI Name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "KPI Name must be between 2 and 100 characters")]
        public string Name { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Indicator is required")]
        [StringLength(500, MinimumLength = 2, ErrorMessage = "Indicator must be between 2 and 500 characters")]
        public string Indicator { get; set; }

        public decimal MinRating { get; set; }

        public decimal MaxRating { get; set; }

        public ItemStatus Status { get; set; }

        public ICollection<StaffKPIndex> StaffKPIndexes { get; set; }
    }
}
