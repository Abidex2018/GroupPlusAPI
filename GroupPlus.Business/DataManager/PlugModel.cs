using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using GroupPlus.BusinessObject.CompanyManagement;
using GroupPlus.BusinessObject.Settings;
using GroupPlus.BusinessObject.StaffDetail;
using GroupPlus.BusinessObject.StaffManagement;
using GroupPlus.BussinessObject.Workflow;


namespace GroupPlus.Business.DataManager
{
    internal partial class PlugModel : DbContext
    {
        public PlugModel()
            : base("name=GroupPlusEntities")
        {
            Configuration.ProxyCreationEnabled = false;
            Configuration.AutoDetectChangesEnabled = false;
            ChangeTracker.DetectChanges();
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("GPlus");
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
        public DbSet<SerialNumberKeeper> SerialNumberKeepers { get; set; }
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<WorkflowLog> WorkflowLogs { get; set; }
        public DbSet<WorkflowSetup> WorkflowSetups { get; set; }

        public DbSet<WorkflowOrder> WorkflowOrders { get; set; }
        public DbSet<WorkflowOrderItem> WorkflowOrderItems { get; set; }
        public DbSet<Bank> Banks { get; set; }
        public DbSet<ClassOfAward> ClassOfAwards { get; set; }
        public DbSet<Discipline> Disciplines { get; set; }
        public DbSet<Institution> Institutions { get; set; }

        public DbSet<InsurancePolicyType> InsurancePolicyTypes { get; set; }
        public DbSet<JobLevel> JobLevels { get; set; }
        public DbSet<JobPosition> JobPositions { get; set; }
        public DbSet<JobSpecialization> JobSpecializations { get; set; }
        public DbSet<JobType> JobTypes { get; set; }
        public DbSet<KPIndex> KpIndices { get; set; }

        public DbSet<LeaveType> LeaveTypes { get; set; }

        public DbSet<PensionAdministrator> pensionAdministrators { get; set; }
        public DbSet<ProfessionalBody> ProfessionalBodies { get; set; }

        public DbSet<ProfessionalMembershipType> ProfessionalMembershipTypes { get; set; }

        public DbSet<QualificationClassOfAward> QualificationClassOfAwards { get; set; }

        public DbSet<SalaryGrade> SalaryGrades { get; set; }
        public DbSet<SalaryLevel> SalaryLevels { get; set; }

        public DbSet<TerminationReason> TerminationReasons { get; set; }

        public DbSet<CourseOfStudy> CourseOfStudies { get; set; }

        public DbSet<EmergencyContact> EmergencyContacts { get; set; }
        public DbSet<HigherEducation> HigherEducations { get; set; }

        public DbSet<ProfessionalMembership> ProfessionalMemberships { get; set; }

        public DbSet<Qualification> Qualifications { get; set; }
        public DbSet<WorkExperience> WorkExperiences { get; set; }

        public DbSet<Comment> Comments { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
        public DbSet<StaffAccess> StaffAccesses { get; set; }

        public DbSet<StaffAccessRole> StaffAccessRoles { get; set; }

        public DbSet<StaffBankAccount> StaffBankAccounts { get; set; }
        public DbSet<StaffContact> StaffContacts { get; set; }

        public DbSet<StaffInsurance> StaffInsurances { get; set; }
        public DbSet<StaffJobInfo> StaffJobInfos { get; set; }
        public DbSet<StaffKPIndex> StaffKpIndices { get; set; }
        public DbSet<StaffLeave> StaffLeaves { get; set; }
        public DbSet<StaffLoginActivity> StaffLoginActivities { get; set; }
        public DbSet<StaffMedical> StaffMedicals { get; set; }

        public DbSet<StaffMemo> StaffMemos { get; set; }
        public DbSet<StaffMemoResponse> StaffMemoResponses { get; set; }

        public DbSet<StaffNextOfKin> StaffNextOfKins { get; set; }

        public DbSet<StaffPension> StaffPensions { get; set; }

        public DbSet<StaffRole> StaffRoles { get; set; }

        public DbSet<StaffSalary> StaffSalaries { get; set; }

        public DbSet<Department> Departments { get; set; }

        public DbSet<CompanyDepartment> CompanyDepartments { get; set; }

    }
}