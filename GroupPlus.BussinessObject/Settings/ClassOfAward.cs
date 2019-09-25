using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GroupPlus.BussinessObject.StaffDetail;
using GroupPlus.Common;

namespace GroupPlus.BussinessObject.Settings
{
    [Table("GPlus.ClassOfAward")]
    public  class ClassOfAward
    {
        public ClassOfAward()
        {
            HigherEducations = new HashSet<HigherEducation>();
        }
        public int ClassOfAwardId { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Class of Certificate is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Class of Certificate must be between 2 and 50 characters")]
        public string Name { get; set; }

        public decimal LowerGradePoint { get; set; }
        public decimal UpperGradePoint { get; set; }
        public ItemStatus Status { get; set; }
        public ICollection<HigherEducation> HigherEducations { get; set; }
    }
}
