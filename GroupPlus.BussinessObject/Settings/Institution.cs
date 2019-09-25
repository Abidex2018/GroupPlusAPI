using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GroupPlus.BussinessObject.StaffDetail;
using GroupPlus.Common;

namespace GroupPlus.BussinessObject.Settings
{
    [Table("GPlus.Institution")]
    public class Institution
    {
        public Institution()
        {
            HigherEducations = new HashSet<HigherEducation>();
        }
        public int InstitutionId { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Institution's Name is required")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Institution's Name must be between 2 and 200 characters")]
        public string Name { get; set; }

        public ItemStatus Status { get; set; }
        public ICollection<HigherEducation> HigherEducations { get; set; }
    }
}
