using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GroupPlus.Common;

namespace GroupPlus.BussinessObject.StaffManagement
{
    [Table("GPlus.StaffMedical")]
    public class StaffMedical
    {
        public int StaffMedicalId { get; set; }
        public int StaffId { get; set; }
        public BloodGroup BloodGroup { get; set; }
        public Genotype Genotype { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Medical Fitness Report is required")]
        [StringLength(2000, MinimumLength = 10,ErrorMessage = "Medical Fitness Report must be between 10 and 2000 characters")]
        public string MedicalFitnessReport { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = true, ErrorMessage = "Ailment Information is required")]
        [StringLength(2000)]
        public string KnownAilment { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Registered Date - Time is required")]
        [StringLength(35, MinimumLength = 10, ErrorMessage = "Registered Date must be between 10 and 35 characters")]
        public string TimeStampRegistered { get; set; }
        public virtual Staff Staff { get; set; }
    }
}
