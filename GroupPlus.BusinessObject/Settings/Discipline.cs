using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GroupPlus.Common;

namespace GroupPlus.BusinessObject.Settings
{
    [Table("GPlus.Discipline")]
    public class Discipline
    {
        public Discipline()
        {
            CourseOfStudies = new HashSet<CourseOfStudy>();
        }
        public int DisciplineId { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Discipline is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Discipline must be between 2 and 50 characters")]
        public string Name { get; set; }

        public ItemStatus Status { get; set; }
        public ICollection<CourseOfStudy> CourseOfStudies { get; set; }
    }
}
