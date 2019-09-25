namespace GroupPlus.Business.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MigrationX1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "GPlus.Bank",
                c => new
                    {
                        BankId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50, storeType: "varchar"),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.BankId);
            
            CreateTable(
                "GPlus.ClassOfAward",
                c => new
                    {
                        ClassOfAwardId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50, storeType: "varchar"),
                        LowerGradePoint = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UpperGradePoint = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ClassOfAwardId);
            
            CreateTable(
                "GPlus.HigherEducation",
                c => new
                    {
                        HigherEducationId = c.Long(nullable: false, identity: true),
                        StaffId = c.Int(nullable: false),
                        DisciplineId = c.Int(nullable: false),
                        CourseOfStudyId = c.Int(nullable: false),
                        QualificationId = c.Int(nullable: false),
                        ClassOfAwardId = c.Int(nullable: false),
                        InstitutionId = c.Int(nullable: false),
                        StartYear = c.Int(nullable: false),
                        EndYear = c.Int(nullable: false),
                        SpecifiedInstitution = c.String(nullable: false, maxLength: 200, storeType: "varchar"),
                        SpecifiedDiscipline = c.String(nullable: false, maxLength: 100, storeType: "varchar"),
                        SpecifiedCourseOfStudy = c.String(nullable: false, maxLength: 100, storeType: "varchar"),
                        CGPA = c.String(nullable: false, maxLength: 5, storeType: "varchar"),
                        GradeScale = c.Int(nullable: false),
                        TimeStampRegistered = c.String(nullable: false, maxLength: 35, storeType: "varchar"),
                        TimeStampLastEdited = c.String(nullable: false, maxLength: 35, storeType: "varchar"),
                    })
                .PrimaryKey(t => t.HigherEducationId)
                .ForeignKey("GPlus.Staff", t => t.StaffId, cascadeDelete: true)
                .ForeignKey("GPlus.ClassOfAward", t => t.ClassOfAwardId, cascadeDelete: true)
                .ForeignKey("GPlus.CourseOfStudy", t => t.CourseOfStudyId, cascadeDelete: true)
                .ForeignKey("GPlus.Institution", t => t.InstitutionId, cascadeDelete: true)
                .ForeignKey("GPlus.Qualification", t => t.QualificationId, cascadeDelete: true)
                .Index(t => t.StaffId)
                .Index(t => t.CourseOfStudyId)
                .Index(t => t.QualificationId)
                .Index(t => t.ClassOfAwardId)
                .Index(t => t.InstitutionId);
            
            CreateTable(
                "GPlus.Staff",
                c => new
                    {
                        StaffId = c.Int(nullable: false, identity: true),
                        LastName = c.String(nullable: false, maxLength: 80, storeType: "varchar"),
                        FirstName = c.String(nullable: false, maxLength: 80, storeType: "varchar"),
                        MiddleName = c.String(nullable: false, maxLength: 80, storeType: "varchar"),
                        Gender = c.Int(nullable: false),
                        DateOfBirth = c.String(nullable: false, maxLength: 10, storeType: "varchar"),
                        Email = c.String(nullable: false, maxLength: 50, storeType: "varchar"),
                        PermanentHomeAddress = c.String(nullable: false, maxLength: 200, storeType: "varchar"),
                        EmploymentType = c.Int(nullable: false),
                        MobileNumber = c.String(nullable: false, maxLength: 11, storeType: "varchar"),
                        CountryOfOriginId = c.Int(nullable: false),
                        StateOfOriginId = c.Int(nullable: false),
                        LocalAreaId = c.Int(nullable: false),
                        MaritalStatus = c.Int(nullable: false),
                        TimeStamRegistered = c.String(nullable: false, maxLength: 35, storeType: "varchar"),
                        EmploymentDate = c.String(nullable: false, maxLength: 10, storeType: "varchar"),
                        CompanyId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        StaffAccessId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.StaffId)
                .ForeignKey("GPlus.Company", t => t.CompanyId, cascadeDelete: true)
                .ForeignKey("GPlus.StaffAccess", t => t.StaffAccessId, cascadeDelete: true)
                .Index(t => t.CompanyId)
                .Index(t => t.StaffAccessId);
            
            CreateTable(
                "GPlus.Comment",
                c => new
                    {
                        CommentId = c.Int(nullable: false, identity: true),
                        StaffId = c.Int(nullable: false),
                        CommentDetails = c.String(nullable: false, maxLength: 500, storeType: "varchar"),
                        TimeStampCommented = c.String(nullable: false, maxLength: 35, storeType: "varchar"),
                        CommentType = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CommentId)
                .ForeignKey("GPlus.Staff", t => t.StaffId, cascadeDelete: true)
                .Index(t => t.StaffId);
            
            CreateTable(
                "GPlus.Company",
                c => new
                    {
                        CompanyId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 200, storeType: "varchar"),
                        BusinessDescription = c.String(nullable: false, maxLength: 300, storeType: "varchar"),
                        Email = c.String(nullable: false, maxLength: 50, storeType: "varchar"),
                        Address = c.String(nullable: false, maxLength: 300, storeType: "varchar"),
                        CompanyType = c.Int(nullable: false),
                        TimeStampRegister = c.String(nullable: false, maxLength: 35, storeType: "varchar"),
                        RegisteredBy = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CompanyId)
                .Index(t => t.Name, unique: true, name: "IX_CompKey");
            
            CreateTable(
                "GPlus.EmergencyContact",
                c => new
                    {
                        EmergencyContactId = c.Int(nullable: false, identity: true),
                        StaffId = c.Int(nullable: false),
                        LastName = c.String(nullable: false, maxLength: 200, storeType: "varchar"),
                        FirstName = c.String(nullable: false, maxLength: 200, storeType: "varchar"),
                        MiddleName = c.String(nullable: false, maxLength: 200, storeType: "varchar"),
                        Gender = c.Int(nullable: false),
                        ResidentialAddress = c.String(nullable: false, maxLength: 200, storeType: "varchar"),
                        MobileNumber = c.String(nullable: false, maxLength: 11, storeType: "varchar"),
                        IsDefault = c.Boolean(nullable: false),
                        StateOfLocationId = c.Int(nullable: false),
                        LocalAreaOfLocationId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.EmergencyContactId)
                .ForeignKey("GPlus.Staff", t => t.StaffId, cascadeDelete: true)
                .Index(t => t.StaffId);
            
            CreateTable(
                "GPlus.LeaveRequest",
                c => new
                    {
                        LeaveRequestId = c.Long(nullable: false, identity: true),
                        StaffId = c.Int(nullable: false),
                        DepartmentId = c.Int(nullable: false),
                        CompanyId = c.Int(nullable: false),
                        LeaveTitle = c.String(nullable: false, maxLength: 300, storeType: "varchar"),
                        LeaveType = c.Int(nullable: false),
                        TimeStampRegistered = c.String(nullable: false, maxLength: 35, storeType: "varchar"),
                        ProposedStartDate = c.String(nullable: false, maxLength: 10, storeType: "varchar"),
                        ProposedEndDate = c.String(nullable: false, maxLength: 10, storeType: "varchar"),
                        Purpose = c.String(nullable: false, maxLength: 500, storeType: "varchar"),
                        OtherRemarks = c.String(maxLength: 300, storeType: "varchar"),
                        Status = c.Int(nullable: false),
                        LeaveType_LeaveTypeId = c.Int(),
                    })
                .PrimaryKey(t => t.LeaveRequestId)
                .ForeignKey("GPlus.Staff", t => t.StaffId, cascadeDelete: true)
                .ForeignKey("GPlus.LeaveType", t => t.LeaveType_LeaveTypeId)
                .Index(t => t.StaffId)
                .Index(t => t.LeaveType_LeaveTypeId);
            
            CreateTable(
                "GPlus.ProfessionalMembership",
                c => new
                    {
                        ProfessionalMembershipId = c.Long(nullable: false, identity: true),
                        StaffId = c.Int(nullable: false),
                        ProfessionalBodyId = c.Int(nullable: false),
                        YearJoined = c.Int(nullable: false),
                        ProfessionalMembershipTypeId = c.Int(nullable: false),
                        TimeStampRegistered = c.String(nullable: false, maxLength: 35, storeType: "varchar"),
                    })
                .PrimaryKey(t => t.ProfessionalMembershipId)
                .ForeignKey("GPlus.ProfessionalMembershipType", t => t.ProfessionalMembershipTypeId, cascadeDelete: true)
                .ForeignKey("GPlus.Staff", t => t.StaffId, cascadeDelete: true)
                .ForeignKey("GPlus.ProfessionalBody", t => t.ProfessionalBodyId, cascadeDelete: true)
                .Index(t => t.StaffId)
                .Index(t => t.ProfessionalBodyId)
                .Index(t => t.ProfessionalMembershipTypeId);
            
            CreateTable(
                "GPlus.ProfessionalMembershipType",
                c => new
                    {
                        ProfessionalMembershipTypeId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50, storeType: "varchar"),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ProfessionalMembershipTypeId);
            
            CreateTable(
                "GPlus.StaffAccess",
                c => new
                    {
                        StaffAccessId = c.Int(nullable: false, identity: true),
                        StaffId = c.Int(nullable: false),
                        Email = c.String(nullable: false, maxLength: 50, storeType: "varchar"),
                        Username = c.String(nullable: false, maxLength: 50, storeType: "varchar"),
                        MobileNumber = c.String(nullable: false, maxLength: 11, storeType: "varchar"),
                        IsApproved = c.Boolean(nullable: false),
                        IsLockedOut = c.Boolean(nullable: false),
                        FailedPasswordAttemptCount = c.Int(nullable: false),
                        DateLockedOut = c.String(nullable: false, maxLength: 10, storeType: "varchar"),
                        TimeLockedOut = c.String(nullable: false, maxLength: 5, storeType: "varchar"),
                        LastLockedOutTimeStamp = c.String(nullable: false, maxLength: 35, storeType: "varchar"),
                        LastReleasedTimeStamp = c.String(nullable: false, maxLength: 35, storeType: "varchar"),
                        LastPasswordChangedTimeStamp = c.String(nullable: false, maxLength: 35, storeType: "varchar"),
                        UserCode = c.String(nullable: false, maxLength: 200, storeType: "varchar"),
                        AccessCode = c.String(nullable: false, maxLength: 200, storeType: "varchar"),
                        Password = c.String(nullable: false, maxLength: 200, storeType: "varchar"),
                        CompanyId = c.Int(nullable: false),
                        DepartmentId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.StaffAccessId)
                .Index(t => t.Email, unique: true, name: "UQ_User_Email")
                .Index(t => t.Username, unique: true, name: "UQ_User_Username")
                .Index(t => t.MobileNumber, unique: true, name: "UQ_Stake_MobileNo");
            
            CreateTable(
                "GPlus.StaffBankAccount",
                c => new
                    {
                        StaffBankAccountId = c.Int(nullable: false, identity: true),
                        StaffId = c.Int(nullable: false),
                        BankId = c.Int(nullable: false),
                        AccountName = c.String(nullable: false, maxLength: 100, storeType: "varchar"),
                        AccountNumber = c.String(nullable: false, maxLength: 10, storeType: "varchar"),
                        IsDefault = c.Boolean(nullable: false),
                        Status = c.Int(nullable: false),
                        TimeStamRegistered = c.String(nullable: false, maxLength: 35, storeType: "varchar"),
                    })
                .PrimaryKey(t => t.StaffBankAccountId)
                .ForeignKey("GPlus.Staff", t => t.StaffId, cascadeDelete: true)
                .Index(t => t.StaffId)
                .Index(t => t.AccountNumber, unique: true, name: "UQ_Acc_No");
            
            CreateTable(
                "GPlus.StaffContact",
                c => new
                    {
                        StaffContactId = c.Int(nullable: false, identity: true),
                        StaffId = c.Int(nullable: false),
                        ResidentialAddress = c.String(nullable: false, maxLength: 200, storeType: "varchar"),
                        TownOfResidence = c.String(nullable: false, maxLength: 200, storeType: "varchar"),
                        LocationLandmark = c.String(nullable: false, maxLength: 200, storeType: "varchar"),
                        StateOfResidenceId = c.Int(nullable: false),
                        LocalAreaOfResidenceId = c.Int(nullable: false),
                        IsDefault = c.Boolean(nullable: false),
                        Status = c.Int(nullable: false),
                        TimeStamRegistered = c.String(nullable: false, maxLength: 35, storeType: "varchar"),
                    })
                .PrimaryKey(t => t.StaffContactId)
                .ForeignKey("GPlus.Staff", t => t.StaffId, cascadeDelete: true)
                .Index(t => t.StaffId);
            
            CreateTable(
                "GPlus.StaffKPIndex",
                c => new
                    {
                        StaffKPIndexId = c.Int(nullable: false, identity: true),
                        StaffId = c.Int(nullable: false),
                        KPIndexId = c.Int(nullable: false),
                        Description = c.String(nullable: false, storeType: "text"),
                        StartDate = c.String(nullable: false, maxLength: 10, storeType: "varchar"),
                        EndDate = c.String(nullable: false, maxLength: 10, storeType: "varchar"),
                        Rating = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Comment = c.String(nullable: false, storeType: "text"),
                        SupervisorRemarks = c.String(nullable: false, storeType: "text"),
                        SupervisorId = c.Int(nullable: false),
                        RemarkTimeStamp = c.String(nullable: false, maxLength: 35, storeType: "varchar"),
                        TimeStampRegistered = c.String(nullable: false, maxLength: 35, storeType: "varchar"),
                    })
                .PrimaryKey(t => t.StaffKPIndexId)
                .ForeignKey("GPlus.KPIndex", t => t.KPIndexId, cascadeDelete: true)
                .ForeignKey("GPlus.Staff", t => t.StaffId, cascadeDelete: true)
                .Index(t => t.StaffId)
                .Index(t => t.KPIndexId);
            
            CreateTable(
                "GPlus.KPIndex",
                c => new
                    {
                        KPIndexId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100, storeType: "varchar"),
                        Indicator = c.String(nullable: false, maxLength: 500, storeType: "varchar"),
                        MinRating = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MaxRating = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.KPIndexId);
            
            CreateTable(
                "GPlus.StaffLoginActivity",
                c => new
                    {
                        StaffLoginActivityId = c.Long(nullable: false, identity: true),
                        StaffId = c.Int(nullable: false),
                        IsLoggedIn = c.Boolean(nullable: false),
                        LoginAddress = c.String(maxLength: 50, storeType: "varchar"),
                        LoginToken = c.String(maxLength: 50, storeType: "varchar"),
                        TokenExpiryDate = c.String(maxLength: 10, storeType: "varchar"),
                        IsTokenExpired = c.Boolean(nullable: false),
                        LoginTimeStamp = c.String(maxLength: 35, storeType: "varchar"),
                    })
                .PrimaryKey(t => t.StaffLoginActivityId)
                .ForeignKey("GPlus.Staff", t => t.StaffId, cascadeDelete: true)
                .Index(t => t.StaffId);
            
            CreateTable(
                "GPlus.StaffMemo",
                c => new
                    {
                        StaffMemoId = c.Int(nullable: false, identity: true),
                        StaffId = c.Int(nullable: false),
                        MemoType = c.Int(nullable: false),
                        Title = c.String(nullable: false, maxLength: 200, storeType: "varchar"),
                        RegisterBy = c.Int(nullable: false),
                        ApprovedBy = c.Int(nullable: false),
                        MemoDetail = c.String(nullable: false, storeType: "text"),
                        TimeStampRegister = c.String(nullable: false, maxLength: 35, storeType: "varchar"),
                        IsReplied = c.Boolean(nullable: false),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.StaffMemoId)
                .ForeignKey("GPlus.Staff", t => t.StaffId, cascadeDelete: true)
                .Index(t => t.StaffId);
            
            CreateTable(
                "GPlus.StaffMemoResponse",
                c => new
                    {
                        StaffMemoResponseId = c.Int(nullable: false, identity: true),
                        StaffMemoId = c.Int(nullable: false),
                        StaffId = c.Int(nullable: false),
                        MemoResponse = c.String(nullable: false, maxLength: 1000, storeType: "varchar"),
                        TimeStampRegister = c.String(nullable: false, maxLength: 35, storeType: "varchar"),
                        IssuerId = c.Int(nullable: false),
                        IssuerRemarks = c.String(nullable: false, maxLength: 500, storeType: "varchar"),
                        IssuerRemarkTimeStamp = c.String(nullable: false, maxLength: 35, storeType: "varchar"),
                        ManagementRemarks = c.String(nullable: false, maxLength: 500, storeType: "varchar"),
                        ManagementRemarkTimeStamp = c.String(nullable: false, maxLength: 35, storeType: "varchar"),
                        ManagementRemarksBy = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.StaffMemoResponseId)
                .ForeignKey("GPlus.StaffMemo", t => t.StaffMemoId, cascadeDelete: true)
                .Index(t => t.StaffMemoId);
            
            CreateTable(
                "GPlus.WorkExperience",
                c => new
                    {
                        WorkExperienceId = c.Long(nullable: false, identity: true),
                        StaffId = c.Int(nullable: false),
                        JobTitle = c.String(nullable: false, maxLength: 100, storeType: "varchar"),
                        CompanyName = c.String(nullable: false, maxLength: 100, storeType: "varchar"),
                        StartYear = c.Int(nullable: false),
                        StopYear = c.Int(nullable: false),
                        StartMonth = c.Int(nullable: false),
                        StopMonth = c.Int(nullable: false),
                        JobLeveId = c.Int(nullable: false),
                        JobPositionId = c.Int(nullable: false),
                        JobSpecializationId = c.Int(nullable: false),
                        JobLevelSpecified = c.String(nullable: false, maxLength: 50, storeType: "varchar"),
                        JobPositionSpecified = c.String(nullable: false, maxLength: 50, storeType: "varchar"),
                        JobSpecializationSpecified = c.String(nullable: false, maxLength: 50, storeType: "varchar"),
                        JobTypeId = c.Int(nullable: false),
                        IsCurrentJob = c.Boolean(nullable: false),
                        Responsibility = c.String(nullable: false, storeType: "text"),
                        Achievement = c.String(nullable: false, storeType: "text"),
                        TimeStampRegistered = c.String(nullable: false, maxLength: 35, storeType: "varchar"),
                        TimeStampLastEdited = c.String(nullable: false, maxLength: 35, storeType: "varchar"),
                    })
                .PrimaryKey(t => t.WorkExperienceId)
                .ForeignKey("GPlus.Staff", t => t.StaffId, cascadeDelete: true)
                .Index(t => t.StaffId);
            
            CreateTable(
                "GPlus.CompanyDepartment",
                c => new
                    {
                        CompanyDepartmentId = c.Int(nullable: false, identity: true),
                        CompanyId = c.Int(nullable: false),
                        DepartmentId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CompanyDepartmentId)
                .ForeignKey("GPlus.Company", t => t.CompanyId, cascadeDelete: true)
                .ForeignKey("GPlus.Department", t => t.DepartmentId, cascadeDelete: true)
                .Index(t => t.CompanyId)
                .Index(t => t.DepartmentId);
            
            CreateTable(
                "GPlus.Department",
                c => new
                    {
                        DepartmentId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 200, storeType: "varchar"),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.DepartmentId);
            
            CreateTable(
                "GPlus.CourseOfStudy",
                c => new
                    {
                        CourseOfStudyId = c.Int(nullable: false, identity: true),
                        DisciplineId = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 50, storeType: "varchar"),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CourseOfStudyId)
                .ForeignKey("GPlus.Discipline", t => t.DisciplineId, cascadeDelete: true)
                .Index(t => t.DisciplineId);
            
            CreateTable(
                "GPlus.Discipline",
                c => new
                    {
                        DisciplineId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50, storeType: "varchar"),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.DisciplineId);
            
            CreateTable(
                "GPlus.Institution",
                c => new
                    {
                        InstitutionId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 200, storeType: "varchar"),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.InstitutionId);
            
            CreateTable(
                "GPlus.InsurancePolicyType",
                c => new
                    {
                        InsurancePolicyTypeId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50, storeType: "varchar"),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.InsurancePolicyTypeId);
            
            CreateTable(
                "GPlus.StaffInsurance",
                c => new
                    {
                        StaffInsuranceId = c.Int(nullable: false, identity: true),
                        InsurancePolicyTypeId = c.Int(nullable: false),
                        StaffId = c.Int(nullable: false),
                        PolicyNumber = c.String(nullable: false, maxLength: 200, storeType: "varchar"),
                        Insurer = c.String(nullable: false, maxLength: 200, storeType: "varchar"),
                        CommencementDate = c.String(nullable: false, maxLength: 10, storeType: "varchar"),
                        TerminationDate = c.String(nullable: false, maxLength: 10, storeType: "varchar"),
                        PersonalContibution = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CompanyContibution = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Status = c.Int(nullable: false),
                        TimeStamRegistered = c.String(nullable: false, maxLength: 35, storeType: "varchar"),
                    })
                .PrimaryKey(t => t.StaffInsuranceId)
                .ForeignKey("GPlus.InsurancePolicyType", t => t.InsurancePolicyTypeId, cascadeDelete: true)
                .ForeignKey("GPlus.Staff", t => t.StaffId, cascadeDelete: true)
                .Index(t => t.InsurancePolicyTypeId)
                .Index(t => t.StaffId)
                .Index(t => t.PolicyNumber, unique: true, name: "UQ_Ins_No");
            
            CreateTable(
                "GPlus.JobLevel",
                c => new
                    {
                        JobLevelId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50, storeType: "varchar"),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.JobLevelId);
            
            CreateTable(
                "GPlus.StaffJobInfo",
                c => new
                    {
                        StaffJobInfoId = c.Int(nullable: false, identity: true),
                        StaffId = c.Int(nullable: false),
                        EntranceCompanyId = c.Int(nullable: false),
                        CurrentCompanyId = c.Int(nullable: false),
                        EntranceDepartmentId = c.Int(nullable: false),
                        CurrentDepartmentId = c.Int(nullable: false),
                        JobTypeId = c.Int(nullable: false),
                        JobPositionId = c.Int(nullable: false),
                        JobLevelId = c.Int(nullable: false),
                        JobSpecializationId = c.Int(nullable: false),
                        JobTitle = c.String(nullable: false, maxLength: 100, storeType: "varchar"),
                        JobDescription = c.String(nullable: false, storeType: "text"),
                        TimeStampRegistered = c.String(nullable: false, maxLength: 35, storeType: "varchar"),
                        SalaryGradeId = c.Int(nullable: false),
                        SalaryLevelId = c.Int(nullable: false),
                        TeamLeadId = c.Int(nullable: false),
                        LineManagerId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.StaffJobInfoId)
                .ForeignKey("GPlus.JobLevel", t => t.JobLevelId, cascadeDelete: true)
                .ForeignKey("GPlus.JobPosition", t => t.JobPositionId, cascadeDelete: true)
                .ForeignKey("GPlus.JobSpecialization", t => t.JobSpecializationId, cascadeDelete: true)
                .ForeignKey("GPlus.JobType", t => t.JobTypeId, cascadeDelete: true)
                .ForeignKey("GPlus.SalaryGrade", t => t.SalaryGradeId, cascadeDelete: true)
                .ForeignKey("GPlus.SalaryLevel", t => t.SalaryLevelId, cascadeDelete: true)
                .ForeignKey("GPlus.Staff", t => t.StaffId, cascadeDelete: true)
                .Index(t => t.StaffId)
                .Index(t => t.JobTypeId)
                .Index(t => t.JobPositionId)
                .Index(t => t.JobLevelId)
                .Index(t => t.JobSpecializationId)
                .Index(t => t.SalaryGradeId)
                .Index(t => t.SalaryLevelId);
            
            CreateTable(
                "GPlus.JobPosition",
                c => new
                    {
                        JobPositionId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100, storeType: "varchar"),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.JobPositionId);
            
            CreateTable(
                "GPlus.JobSpecialization",
                c => new
                    {
                        JobSpecializationId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 350, storeType: "varchar"),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.JobSpecializationId);
            
            CreateTable(
                "GPlus.JobType",
                c => new
                    {
                        JobTypeId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50, storeType: "varchar"),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.JobTypeId);
            
            CreateTable(
                "GPlus.SalaryGrade",
                c => new
                    {
                        SalaryGradeId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50, storeType: "varchar"),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.SalaryGradeId);
            
            CreateTable(
                "GPlus.SalaryLevel",
                c => new
                    {
                        SalaryLevelId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50, storeType: "varchar"),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.SalaryLevelId);
            
            CreateTable(
                "GPlus.LeaveType",
                c => new
                    {
                        LeaveTypeId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50, storeType: "varchar"),
                        MinDays = c.Int(nullable: false),
                        MaxDays = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.LeaveTypeId);
            
            CreateTable(
                "GPlus.PensionAdministrator",
                c => new
                    {
                        PensionAdministratorId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 200, storeType: "varchar"),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PensionAdministratorId);
            
            CreateTable(
                "GPlus.StaffPension",
                c => new
                    {
                        StaffPensionId = c.Int(nullable: false, identity: true),
                        StaffId = c.Int(nullable: false),
                        PensionNumber = c.String(nullable: false, maxLength: 15, storeType: "varchar"),
                        CompanyContribution = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PersonalContribution = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PensionAdministratorId = c.Int(nullable: false),
                        TimeStampRegister = c.String(nullable: false, maxLength: 35, storeType: "varchar"),
                    })
                .PrimaryKey(t => t.StaffPensionId)
                .ForeignKey("GPlus.PensionAdministrator", t => t.PensionAdministratorId, cascadeDelete: true)
                .ForeignKey("GPlus.Staff", t => t.StaffId, cascadeDelete: true)
                .Index(t => t.StaffId)
                .Index(t => t.PensionNumber, unique: true, name: "IX_PenKey")
                .Index(t => t.PensionAdministratorId);
            
            CreateTable(
                "GPlus.ProfessionalBody",
                c => new
                    {
                        ProfessionalBodyId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 500, storeType: "varchar"),
                        Acronym = c.String(nullable: false, maxLength: 50, storeType: "varchar"),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ProfessionalBodyId);
            
            CreateTable(
                "GPlus.QualificationClassOfAward",
                c => new
                    {
                        QualificationClassOfAwardId = c.Int(nullable: false, identity: true),
                        QualificationId = c.Int(nullable: false),
                        ClassOfAwardId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.QualificationClassOfAwardId)
                .ForeignKey("GPlus.Qualification", t => t.QualificationId, cascadeDelete: true)
                .Index(t => t.QualificationId);
            
            CreateTable(
                "GPlus.Qualification",
                c => new
                    {
                        QualificationId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50, storeType: "varchar"),
                        Rank = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.QualificationId);
            
            CreateTable(
                "GPlus.SerialNumberKeeper",
                c => new
                    {
                        SerialNumberKeeperId = c.Long(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.SerialNumberKeeperId);
            
            CreateTable(
                "GPlus.StaffAccessRole",
                c => new
                    {
                        StaffAccessRoleId = c.Int(nullable: false, identity: true),
                        StaffAccessId = c.Int(nullable: false),
                        StaffRoleId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.StaffAccessRoleId)
                .ForeignKey("GPlus.StaffAccess", t => t.StaffAccessId, cascadeDelete: true)
                .ForeignKey("GPlus.StaffRole", t => t.StaffRoleId, cascadeDelete: true)
                .Index(t => t.StaffAccessId)
                .Index(t => t.StaffRoleId);
            
            CreateTable(
                "GPlus.StaffRole",
                c => new
                    {
                        StaffRoleId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50, storeType: "varchar"),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.StaffRoleId);
            
            CreateTable(
                "GPlus.StaffLeave",
                c => new
                    {
                        StaffLeaveId = c.Long(nullable: false, identity: true),
                        StaffId = c.Int(nullable: false),
                        LeaveRequestId = c.Long(nullable: false),
                        LeaveTypeId = c.Int(nullable: false),
                        TimeStampRequested = c.String(nullable: false, maxLength: 35, storeType: "varchar"),
                        ProposedStartDate = c.String(nullable: false, maxLength: 35, storeType: "varchar"),
                        ProposedEndDate = c.String(nullable: false, maxLength: 35, storeType: "varchar"),
                        HODApprovedBy = c.Int(nullable: false),
                        LMApprovedBy = c.Int(nullable: false),
                        HRApprovedBy = c.Int(nullable: false),
                        HODComment = c.String(nullable: false, maxLength: 500, storeType: "varchar"),
                        LMComment = c.String(nullable: false, maxLength: 500, storeType: "varchar"),
                        HRComment = c.String(nullable: false, maxLength: 500, storeType: "varchar"),
                        HODApprovedStartDate = c.String(nullable: false, maxLength: 10, storeType: "varchar"),
                        HODApprovedEndDate = c.String(nullable: false, maxLength: 10, storeType: "varchar"),
                        LMApprovedStartDate = c.String(nullable: false, maxLength: 10, storeType: "varchar"),
                        LMApprovedEndDate = c.String(nullable: false, maxLength: 10, storeType: "varchar"),
                        HRApprovedStartDate = c.String(nullable: false, maxLength: 10, storeType: "varchar"),
                        HRApprovedEndDate = c.String(nullable: false, maxLength: 10, storeType: "varchar"),
                        LMApprovedTimeStamp = c.String(nullable: false, maxLength: 35, storeType: "varchar"),
                        HODApprovedTimeStamp = c.String(nullable: false, maxLength: 35, storeType: "varchar"),
                        HRApprovedTimeStamp = c.String(nullable: false, maxLength: 35, storeType: "varchar"),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.StaffLeaveId)
                .ForeignKey("GPlus.LeaveRequest", t => t.LeaveRequestId, cascadeDelete: true)
                .Index(t => t.LeaveRequestId);
            
            CreateTable(
                "GPlus.StaffMedical",
                c => new
                    {
                        StaffMedicalId = c.Int(nullable: false, identity: true),
                        StaffId = c.Int(nullable: false),
                        BloodGroup = c.Int(nullable: false),
                        Genotype = c.Int(nullable: false),
                        MedicalFitnessReport = c.String(nullable: false, maxLength: 2000, storeType: "varchar"),
                        KnownAilment = c.String(nullable: false, maxLength: 2000, storeType: "varchar"),
                        TimeStampRegistered = c.String(nullable: false, maxLength: 35, storeType: "varchar"),
                    })
                .PrimaryKey(t => t.StaffMedicalId)
                .ForeignKey("GPlus.Staff", t => t.StaffId, cascadeDelete: true)
                .Index(t => t.StaffId);
            
            CreateTable(
                "GPlus.StaffNextOfKin",
                c => new
                    {
                        StaffNextOfKinId = c.Int(nullable: false, identity: true),
                        StaffId = c.Int(nullable: false),
                        StateOfLocationId = c.Int(nullable: false),
                        LocalAreaOfLocationId = c.Int(nullable: false),
                        LastName = c.String(nullable: false, maxLength: 50, storeType: "varchar"),
                        FirstName = c.String(nullable: false, maxLength: 2050, storeType: "varchar"),
                        MiddleName = c.String(nullable: false, maxLength: 50, storeType: "varchar"),
                        Gender = c.Int(nullable: false),
                        ResidentialAddress = c.String(nullable: false, maxLength: 200, storeType: "varchar"),
                        MobileNumber = c.String(nullable: false, maxLength: 11, storeType: "varchar"),
                        Email = c.String(nullable: false, maxLength: 50, storeType: "varchar"),
                        Relationship = c.Int(nullable: false),
                        Landphone = c.String(nullable: false, maxLength: 11, storeType: "varchar"),
                        MaritalStatus = c.Int(nullable: false),
                        TimeStampRegister = c.String(nullable: false, maxLength: 35, storeType: "varchar"),
                    })
                .PrimaryKey(t => t.StaffNextOfKinId)
                .ForeignKey("GPlus.Staff", t => t.StaffId, cascadeDelete: true)
                .Index(t => t.StaffId);
            
            CreateTable(
                "GPlus.StaffSalary",
                c => new
                    {
                        StaffSalaryId = c.Int(nullable: false, identity: true),
                        StaffId = c.Int(nullable: false),
                        StaffJobInfoId = c.Int(nullable: false),
                        Currency = c.Int(nullable: false),
                        BasicAllowance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        HousingAllowance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EducationAllowance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FurnitureAllowance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WardrobeAllowance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TransportAllowance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LeaveAllowance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EntertainmentAllowance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PensionDeduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PayeDeduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        InsuranceDeduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalPayment = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalDeduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TimeStamRegistered = c.String(nullable: false, maxLength: 35, storeType: "varchar"),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.StaffSalaryId)
                .ForeignKey("GPlus.Staff", t => t.StaffId, cascadeDelete: true)
                .Index(t => t.StaffId);
            
            CreateTable(
                "GPlus.TerminationReason",
                c => new
                    {
                        TerminationReasonId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50, storeType: "varchar"),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TerminationReasonId);
            
            CreateTable(
                "GPlus.WorkflowLog",
                c => new
                    {
                        WorkflowLogId = c.Long(nullable: false, identity: true),
                        WorkflowSetupId = c.Int(nullable: false),
                        ApprovalType = c.Int(nullable: false),
                        StaffId = c.Int(nullable: false),
                        ProcessorId = c.Int(nullable: false),
                        WorkflowOrderItemId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Comment = c.String(nullable: false, maxLength: 500, storeType: "varchar"),
                        LogTimeStamp = c.String(nullable: false, maxLength: 35, storeType: "varchar"),
                    })
                .PrimaryKey(t => t.WorkflowLogId)
                .ForeignKey("GPlus.WorkflowOrderItem", t => t.WorkflowOrderItemId, cascadeDelete: true)
                .Index(t => t.WorkflowOrderItemId);
            
            CreateTable(
                "GPlus.WorkflowOrderItem",
                c => new
                    {
                        WorkflowOrderItemId = c.Int(nullable: false, identity: true),
                        WorkflowOrderId = c.Int(nullable: false),
                        Order = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 100, storeType: "varchar"),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.WorkflowOrderItemId)
                .ForeignKey("GPlus.WorkflowOrder", t => t.WorkflowOrderId, cascadeDelete: true)
                .Index(t => t.WorkflowOrderId);
            
            CreateTable(
                "GPlus.WorkflowOrder",
                c => new
                    {
                        WorkflowOrderId = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 100, storeType: "varchar"),
                        TimeStampRegistered = c.String(nullable: false, maxLength: 35, storeType: "varchar"),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.WorkflowOrderId);
            
            CreateTable(
                "GPlus.WorkflowSetup",
                c => new
                    {
                        WorkflowSetupId = c.Int(nullable: false, identity: true),
                        Item = c.Int(nullable: false),
                        StaffId = c.Int(nullable: false),
                        InitiatorId = c.Int(nullable: false),
                        InitiatorType = c.Int(nullable: false),
                        WorkflowOrderId = c.Int(nullable: false),
                        WorkflowSourceId = c.Long(nullable: false),
                        Description = c.String(nullable: false, maxLength: 150, storeType: "varchar"),
                        TimeStampInitiated = c.String(nullable: false, maxLength: 35, storeType: "varchar"),
                        LastTimeStampModified = c.String(nullable: false, maxLength: 35, storeType: "varchar"),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.WorkflowSetupId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("GPlus.WorkflowOrderItem", "WorkflowOrderId", "GPlus.WorkflowOrder");
            DropForeignKey("GPlus.WorkflowLog", "WorkflowOrderItemId", "GPlus.WorkflowOrderItem");
            DropForeignKey("GPlus.StaffSalary", "StaffId", "GPlus.Staff");
            DropForeignKey("GPlus.StaffNextOfKin", "StaffId", "GPlus.Staff");
            DropForeignKey("GPlus.StaffMedical", "StaffId", "GPlus.Staff");
            DropForeignKey("GPlus.StaffLeave", "LeaveRequestId", "GPlus.LeaveRequest");
            DropForeignKey("GPlus.StaffAccessRole", "StaffRoleId", "GPlus.StaffRole");
            DropForeignKey("GPlus.StaffAccessRole", "StaffAccessId", "GPlus.StaffAccess");
            DropForeignKey("GPlus.QualificationClassOfAward", "QualificationId", "GPlus.Qualification");
            DropForeignKey("GPlus.HigherEducation", "QualificationId", "GPlus.Qualification");
            DropForeignKey("GPlus.ProfessionalMembership", "ProfessionalBodyId", "GPlus.ProfessionalBody");
            DropForeignKey("GPlus.StaffPension", "StaffId", "GPlus.Staff");
            DropForeignKey("GPlus.StaffPension", "PensionAdministratorId", "GPlus.PensionAdministrator");
            DropForeignKey("GPlus.LeaveRequest", "LeaveType_LeaveTypeId", "GPlus.LeaveType");
            DropForeignKey("GPlus.StaffJobInfo", "StaffId", "GPlus.Staff");
            DropForeignKey("GPlus.StaffJobInfo", "SalaryLevelId", "GPlus.SalaryLevel");
            DropForeignKey("GPlus.StaffJobInfo", "SalaryGradeId", "GPlus.SalaryGrade");
            DropForeignKey("GPlus.StaffJobInfo", "JobTypeId", "GPlus.JobType");
            DropForeignKey("GPlus.StaffJobInfo", "JobSpecializationId", "GPlus.JobSpecialization");
            DropForeignKey("GPlus.StaffJobInfo", "JobPositionId", "GPlus.JobPosition");
            DropForeignKey("GPlus.StaffJobInfo", "JobLevelId", "GPlus.JobLevel");
            DropForeignKey("GPlus.StaffInsurance", "StaffId", "GPlus.Staff");
            DropForeignKey("GPlus.StaffInsurance", "InsurancePolicyTypeId", "GPlus.InsurancePolicyType");
            DropForeignKey("GPlus.HigherEducation", "InstitutionId", "GPlus.Institution");
            DropForeignKey("GPlus.HigherEducation", "CourseOfStudyId", "GPlus.CourseOfStudy");
            DropForeignKey("GPlus.CourseOfStudy", "DisciplineId", "GPlus.Discipline");
            DropForeignKey("GPlus.CompanyDepartment", "DepartmentId", "GPlus.Department");
            DropForeignKey("GPlus.CompanyDepartment", "CompanyId", "GPlus.Company");
            DropForeignKey("GPlus.HigherEducation", "ClassOfAwardId", "GPlus.ClassOfAward");
            DropForeignKey("GPlus.WorkExperience", "StaffId", "GPlus.Staff");
            DropForeignKey("GPlus.StaffMemoResponse", "StaffMemoId", "GPlus.StaffMemo");
            DropForeignKey("GPlus.StaffMemo", "StaffId", "GPlus.Staff");
            DropForeignKey("GPlus.StaffLoginActivity", "StaffId", "GPlus.Staff");
            DropForeignKey("GPlus.StaffKPIndex", "StaffId", "GPlus.Staff");
            DropForeignKey("GPlus.StaffKPIndex", "KPIndexId", "GPlus.KPIndex");
            DropForeignKey("GPlus.StaffContact", "StaffId", "GPlus.Staff");
            DropForeignKey("GPlus.StaffBankAccount", "StaffId", "GPlus.Staff");
            DropForeignKey("GPlus.Staff", "StaffAccessId", "GPlus.StaffAccess");
            DropForeignKey("GPlus.ProfessionalMembership", "StaffId", "GPlus.Staff");
            DropForeignKey("GPlus.ProfessionalMembership", "ProfessionalMembershipTypeId", "GPlus.ProfessionalMembershipType");
            DropForeignKey("GPlus.LeaveRequest", "StaffId", "GPlus.Staff");
            DropForeignKey("GPlus.HigherEducation", "StaffId", "GPlus.Staff");
            DropForeignKey("GPlus.EmergencyContact", "StaffId", "GPlus.Staff");
            DropForeignKey("GPlus.Staff", "CompanyId", "GPlus.Company");
            DropForeignKey("GPlus.Comment", "StaffId", "GPlus.Staff");
            DropIndex("GPlus.WorkflowOrderItem", new[] { "WorkflowOrderId" });
            DropIndex("GPlus.WorkflowLog", new[] { "WorkflowOrderItemId" });
            DropIndex("GPlus.StaffSalary", new[] { "StaffId" });
            DropIndex("GPlus.StaffNextOfKin", new[] { "StaffId" });
            DropIndex("GPlus.StaffMedical", new[] { "StaffId" });
            DropIndex("GPlus.StaffLeave", new[] { "LeaveRequestId" });
            DropIndex("GPlus.StaffAccessRole", new[] { "StaffRoleId" });
            DropIndex("GPlus.StaffAccessRole", new[] { "StaffAccessId" });
            DropIndex("GPlus.QualificationClassOfAward", new[] { "QualificationId" });
            DropIndex("GPlus.StaffPension", new[] { "PensionAdministratorId" });
            DropIndex("GPlus.StaffPension", "IX_PenKey");
            DropIndex("GPlus.StaffPension", new[] { "StaffId" });
            DropIndex("GPlus.StaffJobInfo", new[] { "SalaryLevelId" });
            DropIndex("GPlus.StaffJobInfo", new[] { "SalaryGradeId" });
            DropIndex("GPlus.StaffJobInfo", new[] { "JobSpecializationId" });
            DropIndex("GPlus.StaffJobInfo", new[] { "JobLevelId" });
            DropIndex("GPlus.StaffJobInfo", new[] { "JobPositionId" });
            DropIndex("GPlus.StaffJobInfo", new[] { "JobTypeId" });
            DropIndex("GPlus.StaffJobInfo", new[] { "StaffId" });
            DropIndex("GPlus.StaffInsurance", "UQ_Ins_No");
            DropIndex("GPlus.StaffInsurance", new[] { "StaffId" });
            DropIndex("GPlus.StaffInsurance", new[] { "InsurancePolicyTypeId" });
            DropIndex("GPlus.CourseOfStudy", new[] { "DisciplineId" });
            DropIndex("GPlus.CompanyDepartment", new[] { "DepartmentId" });
            DropIndex("GPlus.CompanyDepartment", new[] { "CompanyId" });
            DropIndex("GPlus.WorkExperience", new[] { "StaffId" });
            DropIndex("GPlus.StaffMemoResponse", new[] { "StaffMemoId" });
            DropIndex("GPlus.StaffMemo", new[] { "StaffId" });
            DropIndex("GPlus.StaffLoginActivity", new[] { "StaffId" });
            DropIndex("GPlus.StaffKPIndex", new[] { "KPIndexId" });
            DropIndex("GPlus.StaffKPIndex", new[] { "StaffId" });
            DropIndex("GPlus.StaffContact", new[] { "StaffId" });
            DropIndex("GPlus.StaffBankAccount", "UQ_Acc_No");
            DropIndex("GPlus.StaffBankAccount", new[] { "StaffId" });
            DropIndex("GPlus.StaffAccess", "UQ_Stake_MobileNo");
            DropIndex("GPlus.StaffAccess", "UQ_User_Username");
            DropIndex("GPlus.StaffAccess", "UQ_User_Email");
            DropIndex("GPlus.ProfessionalMembership", new[] { "ProfessionalMembershipTypeId" });
            DropIndex("GPlus.ProfessionalMembership", new[] { "ProfessionalBodyId" });
            DropIndex("GPlus.ProfessionalMembership", new[] { "StaffId" });
            DropIndex("GPlus.LeaveRequest", new[] { "LeaveType_LeaveTypeId" });
            DropIndex("GPlus.LeaveRequest", new[] { "StaffId" });
            DropIndex("GPlus.EmergencyContact", new[] { "StaffId" });
            DropIndex("GPlus.Company", "IX_CompKey");
            DropIndex("GPlus.Comment", new[] { "StaffId" });
            DropIndex("GPlus.Staff", new[] { "StaffAccessId" });
            DropIndex("GPlus.Staff", new[] { "CompanyId" });
            DropIndex("GPlus.HigherEducation", new[] { "InstitutionId" });
            DropIndex("GPlus.HigherEducation", new[] { "ClassOfAwardId" });
            DropIndex("GPlus.HigherEducation", new[] { "QualificationId" });
            DropIndex("GPlus.HigherEducation", new[] { "CourseOfStudyId" });
            DropIndex("GPlus.HigherEducation", new[] { "StaffId" });
            DropTable("GPlus.WorkflowSetup");
            DropTable("GPlus.WorkflowOrder");
            DropTable("GPlus.WorkflowOrderItem");
            DropTable("GPlus.WorkflowLog");
            DropTable("GPlus.TerminationReason");
            DropTable("GPlus.StaffSalary");
            DropTable("GPlus.StaffNextOfKin");
            DropTable("GPlus.StaffMedical");
            DropTable("GPlus.StaffLeave");
            DropTable("GPlus.StaffRole");
            DropTable("GPlus.StaffAccessRole");
            DropTable("GPlus.SerialNumberKeeper");
            DropTable("GPlus.Qualification");
            DropTable("GPlus.QualificationClassOfAward");
            DropTable("GPlus.ProfessionalBody");
            DropTable("GPlus.StaffPension");
            DropTable("GPlus.PensionAdministrator");
            DropTable("GPlus.LeaveType");
            DropTable("GPlus.SalaryLevel");
            DropTable("GPlus.SalaryGrade");
            DropTable("GPlus.JobType");
            DropTable("GPlus.JobSpecialization");
            DropTable("GPlus.JobPosition");
            DropTable("GPlus.StaffJobInfo");
            DropTable("GPlus.JobLevel");
            DropTable("GPlus.StaffInsurance");
            DropTable("GPlus.InsurancePolicyType");
            DropTable("GPlus.Institution");
            DropTable("GPlus.Discipline");
            DropTable("GPlus.CourseOfStudy");
            DropTable("GPlus.Department");
            DropTable("GPlus.CompanyDepartment");
            DropTable("GPlus.WorkExperience");
            DropTable("GPlus.StaffMemoResponse");
            DropTable("GPlus.StaffMemo");
            DropTable("GPlus.StaffLoginActivity");
            DropTable("GPlus.KPIndex");
            DropTable("GPlus.StaffKPIndex");
            DropTable("GPlus.StaffContact");
            DropTable("GPlus.StaffBankAccount");
            DropTable("GPlus.StaffAccess");
            DropTable("GPlus.ProfessionalMembershipType");
            DropTable("GPlus.ProfessionalMembership");
            DropTable("GPlus.LeaveRequest");
            DropTable("GPlus.EmergencyContact");
            DropTable("GPlus.Company");
            DropTable("GPlus.Comment");
            DropTable("GPlus.Staff");
            DropTable("GPlus.HigherEducation");
            DropTable("GPlus.ClassOfAward");
            DropTable("GPlus.Bank");
        }
    }
}
