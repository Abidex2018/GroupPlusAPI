using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GroupPlus.BusinessObject.Settings;
using GroupPlus.Common;

namespace GroupPlus.BusinessObject.StaffDetails
{
    [Table("GPlus.Qualification")]
    public  class Qualification
    {
        public Qualification()
        {
            HigherEducations = new HashSet<HigherEducation>();
            QualificationClassOfAwards = new HashSet<QualificationClassOfAward>();
        }
        public int QualificationId { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Qualification is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Qualification must be between 2 and 50 characters")]
        public string Name { get; set; }

        public int Rank { get; set; }

        public ItemStatus Status { get; set; }

        public ICollection<HigherEducation> HigherEducations { get; set; }
        public ICollection<QualificationClassOfAward> QualificationClassOfAwards { get; set; }
    }
}
