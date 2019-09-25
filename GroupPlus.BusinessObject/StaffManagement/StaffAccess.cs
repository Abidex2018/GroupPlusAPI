using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GroupPlus.Common;

namespace GroupPlus.BusinessObject.StaffManagement
{

    [Table("GPlus.StaffAccess")]
    public class StaffAccess
    {
        public int StaffAccessId { get; set; }

        public int StaffId { get; set; }
        

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email address is required")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Email address is required")]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Invalid Email Address")]
        [Index("UQ_User_Email", IsUnique = true)]
        public string Email { get; set; }


        [Column(TypeName = "varchar")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "User name is required")]
        [Index("UQ_User_Username", IsUnique = true)]
        public string Username { get; set; }

        [Column(TypeName = "varchar")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "Sub Agent's Mobile Number must be 11 digits")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Sub Agent's Mobile Number is required")]
        [CheckMobileNumber(ErrorMessage = "Invalid Mobile Number")]
        [Index("UQ_Stake_MobileNo", IsUnique = true)]
        public string MobileNumber { get; set; }


        public bool IsApproved { get; set; }
        public bool IsLockedOut { get; set; }
        public int FailedPasswordAttemptCount { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = true)]
        [StringLength(10)]
        public string DateLockedOut { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = true)]
        [StringLength(5)]
        public string TimeLockedOut { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = true)]
        [StringLength(35)]
        public string LastLockedOutTimeStamp { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = true)]
        [StringLength(35)]
        public string LastReleasedTimeStamp { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = true)]
        [StringLength(35)]
        public string LastPasswordChangedTimeStamp { get; set; }

        [Column(TypeName = "varchar")]
        [StringLength(200)]
        [Required(ErrorMessage = "User Code is required")]
        public string UserCode { get; set; }

        [Column(TypeName = "varchar")]
        [StringLength(200)]
        [Required(ErrorMessage = "Access Code is required")]
        public string AccessCode { get; set; }

        [Column(TypeName = "varchar")]
        [StringLength(200)]
        [Required(ErrorMessage = "Login Passcode is required")]
        public string Password { get; set; }

        [CheckNumber(0, ErrorMessage = "Company Id is required")]
        public int CompanyId { get; set; }

        [CheckNumber(0, ErrorMessage = "Department Id is required")]
        public int DepartmentId { get; set; }


    }
}
