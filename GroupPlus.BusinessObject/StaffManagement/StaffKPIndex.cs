using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GroupPlus.BusinessObject.Settings;

namespace GroupPlus.BusinessObject.StaffManagement
{
    [Table("GPlus.StaffKPIndex")]
    public class StaffKPIndex
    {
        public int StaffKPIndexId { get; set; }
        public int StaffId { get; set; }
        public int KPIndexId { get; set; }

        [Column(TypeName = "text")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Description is required")]
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "Description is too short or too long")]
        public string Description { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Start Date is required")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Start Date is required")]
        public string StartDate { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "End Date is required")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "End Date is required")]
        public string EndDate { get; set; }

        public decimal Rating { get; set; }

        [Column(TypeName = "text")]
        [Required(AllowEmptyStrings = true, ErrorMessage = "Comment is required")]
        [StringLength(2000)]
        public string Comment { get; set; }

        [Column(TypeName = "text")]
        [Required(AllowEmptyStrings = true, ErrorMessage = "Supervisor's is required")]
        [StringLength(2000)]
        public string SupervisorRemarks { get; set; }

        public int SupervisorId { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = true, ErrorMessage = "Supervisor's Remark Date Time is required")]
        [StringLength(35)]
        public string RemarkTimeStamp { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Date Time Registered is required")]
        [StringLength(35, MinimumLength = 10, ErrorMessage = "Date Time Registered is required")]
        public string TimeStampRegistered { get; set; }

        public virtual KPIndex KPIndex { get; set; }
    }
}
