using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GroupPlus.BusinessObject.CompanyManagement;
using GroupPlus.BusinessObject.StaffDetail;
using GroupPlus.Common;
// ReSharper disable InconsistentNaming

namespace GroupPlus.BusinessObject.StaffManagement
{
    [Table("GPlus.Staff")]
    public class Staff
    {

        public Staff()
        {
            StaffBankAccounts = new HashSet<StaffBankAccount>();
            StaffContacts = new HashSet<StaffContact>();
            LeaveRequests = new HashSet<LeaveRequest>();
            StaffMemos = new HashSet<StaffMemo>();
            StaffKPIndexes = new HashSet<StaffKPIndex>(); 
            WorkExperiences = new HashSet<WorkExperience>();
            ProfessionalMemberships = new HashSet<ProfessionalMembership>();
            HigherEducations = new HashSet<HigherEducation>();
            EmergencyContacts = new HashSet<EmergencyContact>();
            StaffLoginActivities= new HashSet<StaffLoginActivity>();
            Comments = new HashSet<Comment>();
        }

        public int StaffId { get; set; }


        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Last Name is required")]
        [StringLength(80, MinimumLength = 3, ErrorMessage = "Last Name  must be between 3 and 80 characters")]
        public string LastName { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "First Name is required")]
        [StringLength(80, MinimumLength = 3, ErrorMessage = "First Name  must be between 3 and 80 characters")]
        public string FirstName { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = true, ErrorMessage = "Middle Name cannot be null")]
        [StringLength(80)]
        public string MiddleName { get; set; }

        public Gender Gender { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Date of Birth is required")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Invalid Date of Birth")]
        public string DateOfBirth { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Company Email is required")]
        [StringLength(50)]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = true, ErrorMessage = "Permanent Home Address is required")]
        [StringLength(200)]
        public string PermanentHomeAddress { get; set; }


        public EmploymentType EmploymentType { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Staff Mobile Number is required")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "Invalid Staff Mobile Number")]
        public string MobileNumber { get; set; }

        public int CountryOfOriginId { get; set; }

        public int StateOfOriginId { get; set; }

        public int LocalAreaId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Employment Type is required")]
        public MaritalStatus MaritalStatus { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Date - Time registered is required")]
        [StringLength(35, MinimumLength = 10, ErrorMessage = "Date  Time registered must be between 10 and 35 characters")]
        public string TimeStamRegistered { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Employment Date is required")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Invalid Employment Date")]
        public string EmploymentDate { get; set; }
        public int CompanyId { get; set; }
        public StaffStatus Status { get; set; }
        public ICollection<StaffContact> StaffContacts { get; set; }
        public ICollection<StaffBankAccount> StaffBankAccounts { get; set; }
        public ICollection<LeaveRequest> LeaveRequests { get; set; }
        public ICollection<StaffMemo> StaffMemos { get; set; }
        public ICollection<StaffKPIndex> StaffKPIndexes { get; set; }
        public ICollection<WorkExperience> WorkExperiences { get; set; }
        public ICollection<ProfessionalMembership> ProfessionalMemberships { get; set; }
        public ICollection<HigherEducation> HigherEducations { get; set; }
        public ICollection<EmergencyContact> EmergencyContacts { get; set; }
        public ICollection<StaffLoginActivity> StaffLoginActivities { get; set; }

        public ICollection<Comment> Comments { get; set; }
        public int StaffAccessId { get; set; }
        public virtual StaffAccess StaffAccess { get; set; }

        public virtual Company Company { get; set; }

    }
}
