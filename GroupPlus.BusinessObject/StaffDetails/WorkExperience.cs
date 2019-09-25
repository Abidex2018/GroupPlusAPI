using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GroupPlus.Common;

namespace GroupPlus.BusinessObject.StaffDetails
{
    [Table("GPlus.WorkExperience")]
    public class WorkExperience
    {
        public long WorkExperienceId { get; set; }

        [CheckNumber(0, ErrorMessage = "Invalid Staff Information")]
        public int StaffId { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Job Title is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Job Title is too short or too long")]
        public string JobTitle { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Company Name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Company Name is too short or too long")]
        public string CompanyName { get; set; }

        [CheckNumber(1979, ErrorMessage = "Invalid Start Year")]
        public int StartYear { get; set; }

        [CheckNumber(1979, ErrorMessage = "Invalid Stop Year")]
        public int StopYear { get; set; }

        public MonthOfCert StartMonth { get; set; }
        public MonthOfCert StopMonth { get; set; }

        [CheckNumber(0, ErrorMessage = "Job Level is required")]
        public int JobLeveId { get; set; }

        [CheckNumber(0, ErrorMessage = "Job Position is required")]
        public int JobPositionId { get; set; }

        [CheckNumber(0, ErrorMessage = "Job Specialization is required")]
        public int JobSpecializationId { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = true, ErrorMessage = "Job Level is required")]
        [StringLength(50)]
        public string JobLevelSpecified { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = true, ErrorMessage = "Job Position is required")]
        [StringLength(50)]
        public string JobPositionSpecified { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = true, ErrorMessage = "Job Specialization is required")]
        [StringLength(50)]
        public string JobSpecializationSpecified { get; set; }
        public int JobTypeId { get; set; }
        public bool IsCurrentJob { get; set; }

        [Column(TypeName = "text")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Responsibility(ies) is(are) required")]
        [StringLength(1000, MinimumLength = 2, ErrorMessage = "Responsibility(ies) is(are) too short or too long")]
        public string Responsibility { get; set; }

        [Column(TypeName = "text")]
        [Required(AllowEmptyStrings = true, ErrorMessage = "Achievement(s) is(are) required")]
        [StringLength(1000)]
        public string Achievement { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Time Registered is required")]
        [StringLength(35, MinimumLength = 15, ErrorMessage = "Invalid Time Registered")]
        public string TimeStampRegistered { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Time Last Edited is required")]
        [StringLength(35, MinimumLength = 15, ErrorMessage = "Invalid Time Edited")]
        public string TimeStampLastEdited { get; set; }
    }
}
