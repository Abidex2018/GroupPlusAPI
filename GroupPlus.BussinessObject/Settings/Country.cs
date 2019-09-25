using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GroupPlus.Common;

namespace GroupPlus.BussinessObject.Settings
{
    [Table("GPlus.Country")]
    public class Country
    {
        public Country()
        {
            States = new HashSet<State>();
        }
        
        public int CountryId { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Country Name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Country Name must be between 2 and 50 characters")]
        public string Name { get; set; }

        public ItemStatus Status { get; set; }
        public ICollection<State> States { get; set; }
    }
}
