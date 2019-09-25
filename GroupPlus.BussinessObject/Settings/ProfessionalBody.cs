using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GroupPlus.BussinessObject.StaffDetail;
using GroupPlus.Common;

namespace GroupPlus.BussinessObject.Settings
{
    [Table("GPlus.ProfessionalBody")]
    public class ProfessionalBody
    {
        public ProfessionalBody()
        {
            ProfessionalMemberships = new HashSet<ProfessionalMembership>();
        }

        public int ProfessionalBodyId { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Professional Body's Name is required")]
        [StringLength(500, MinimumLength = 2, ErrorMessage = "Professional Body's Name must be between 2 and 500 characters")]
        public string Name { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Professional Body's Acronym is required")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Professional Body's Acronym must be between 1 and 50 characters")]
        public string Acronym { get; set; }

        public ItemStatus Status { get; set; }

        public ICollection<ProfessionalMembership> ProfessionalMemberships { get; set; }
    }
}
