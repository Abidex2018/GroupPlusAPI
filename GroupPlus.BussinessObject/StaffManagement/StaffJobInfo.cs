using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GroupPlus.BussinessObject.Settings;
using GroupPlus.Common;

namespace GroupPlus.BussinessObject.StaffManagement
{
    [Table("GPlus.StaffJobInfo")]
    public class StaffJobInfo
    {
        public int StaffJobInfoId { get; set; }

        [CheckNumber(0, ErrorMessage = "Staff Id is required")]
        public int StaffId { get; set; }

        [CheckNumber(0, ErrorMessage = "Entrance Company Id is required")]
        public int EntranceCompanyId { get; set; }

        [CheckNumber(0, ErrorMessage = "Current Company Id is required")]
        public int CurrentCompanyId { get; set; }

        [CheckNumber(0, ErrorMessage = "Entrance Department Id is required")]
        public int EntranceDepartmentId { get; set; }

        [CheckNumber(0, ErrorMessage = "Current Department Id is required")]
        public int CurrentDepartmentId { get; set; }

        [CheckNumber(0, ErrorMessage = "Job Type Id is required")]
        public int JobTypeId { get; set; }

        [CheckNumber(0, ErrorMessage = "Job Position Id is required")]
        public int JobPositionId { get; set; }

        [CheckNumber(0, ErrorMessage = "Job Position Id is required")]
        public int JobLevelId { get; set; }

        [CheckNumber(0, ErrorMessage = "Job Specialization Id is required")]
        public int JobSpecializationId { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Job Title is required")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Job Title must be between 5 and 100 characters")]
        public string JobTitle { get; set; }

        [Column(TypeName = "text")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Date Time Registered is required")]
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "Date Time Registered is required")]
        public string JobDescription { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Date Time Registered is required")]
        [StringLength(35, MinimumLength = 10, ErrorMessage = "Date Time Registered is required")]
        public string TimeStampRegistered { get; set; }

        public int SalaryGradeId { get; set; }
        public int SalaryLevelId { get; set; }

        public int TeamLeadId { get; set; }
        public int LineManagerId { get; set; }
        public virtual JobPosition JobPosition { get; set; }

        public virtual JobLevel JobLevel { get; set; }

        public virtual JobType JobType { get; set; }
        public virtual JobSpecialization JobSpecialization { get; set; }
        public virtual SalaryLevel SalaryLevel { get; set; }
        public virtual SalaryGrade SalaryGrade { get; set; }

        public virtual Staff Staff { get; set; }
    }
}
