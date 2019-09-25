using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GroupPlus.BusinessObject.StaffManagement;
using GroupPlus.Common;

namespace GroupPlus.BusinessObject.StaffDetails
{
    [Table("GPlus.HigherEducation")]
    public class HigherEducation
    {
        public long HigherEducationId { get; set; }

        [CheckNumber(0, ErrorMessage = "Invalid Staff Information")]
        public int StaffId { get; set; }

        [CheckNumber(0, ErrorMessage = "Invalid Discipline")]
        public int DisciplineId { get; set; }

        [CheckNumber(0, ErrorMessage = "Invalid Course of Study")]
        public int CourseOfStudyId { get; set; }

        [CheckNumber(0, ErrorMessage = "Invalid Qualification")]
        public int QualificationId { get; set; }

        [CheckNumber(0, ErrorMessage = "Invalid Class of Award")]
        public int ClassOfAwardId { get; set; }

        [CheckNumber(0, ErrorMessage = "Invalid Institution")]
        public int InstitutionId { get; set; }


        [CheckNumber(1979, ErrorMessage = "Invalid Start Year")]
        public int StartYear { get; set; }

        [CheckNumber(1979, ErrorMessage = "Invalid End Year")]
        public int EndYear { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = true, ErrorMessage = "Specified Institution is required")]
        [StringLength(200)]
        public string SpecifiedInstitution { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = true, ErrorMessage = "Specified Discipline is required")]
        [StringLength(100)]
        public string SpecifiedDiscipline { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = true, ErrorMessage = "Specified Course of Study is required")]
        [StringLength(100)]
        public string SpecifiedCourseOfStudy { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = true, ErrorMessage = "CGPA is required")]
        [StringLength(5)]
        public string CGPA { get; set; }
        public int GradeScale { get; set; }
       

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Time Registered is required")]
        [StringLength(35, MinimumLength = 15, ErrorMessage = "Invalid Time Registered")]
        public string TimeStampRegistered { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Time Last Edited is required")]
        [StringLength(35, MinimumLength = 15, ErrorMessage = "Invalid Time Edited")]
        public string TimeStampLastEdited { get; set; }
        public virtual Staff Staff { get; set; }
    }
}
