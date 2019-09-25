using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GroupPlus.BussinessObject.StaffManagement;
using GroupPlus.Common;

namespace GroupPlus.BussinessObject.CompanyManagement
{

    [Table("GPlus.Company")]
    public class Company
    {
        public Company()
        {
            Staffs = new HashSet<Staff>();
        }
        public int CompanyId { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Company Name is required")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Company Name  must be between 3 and 200 characters")]
        [Index("IX_CompKey", IsUnique = true)]
        public string Name { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Business Description is required")]
        [StringLength(300, MinimumLength = 15, ErrorMessage = "Business Description  must be between 15 and 300 characters")]
        public string BusinessDescription { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = true, ErrorMessage = "Company Email is required")]
        [StringLength(50)]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Invalid Email Address")]

        public string Email { get; set; }
        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Company Address is required")]
        [StringLength(300)]
        public string Address { get; set; }
        public CompanyType CompanyType { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Registration Date - Time is required")]
        [StringLength(35, MinimumLength = 10, ErrorMessage = "Registration Date must be between 10 and 35 characters")]
        public string TimeStampRegister { get; set; }

        public int RegisteredBy { get; set; }

        public ItemStatus Status { get; set; }
        
        public ICollection<Staff> Staffs { get; set; }
    }
}
