using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GroupPlus.BussinessObject.StaffDetail;
using GroupPlus.Common;

namespace GroupPlus.BussinessObject.Settings
{
    [Table("GPlus.CourseOfStudy")]
    public  class CourseOfStudy
    {
        public CourseOfStudy()
        {
            HigherEducations = new HashSet<HigherEducation>();
        }
        public int CourseOfStudyId { get; set; }

        public int DisciplineId { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Course of Study Name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Course of Study Name must be between 2 and 50 characters")]
        public string Name { get; set; }

        public ItemStatus Status { get; set; }
        public ICollection<HigherEducation> HigherEducations { get; set; }
        public virtual Discipline Discipline { get; set; }
    }
}
