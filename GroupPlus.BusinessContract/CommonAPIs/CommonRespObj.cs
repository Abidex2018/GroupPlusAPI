using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GroupPlus.BusinessContract.CommonAPIs;
using GroupPlus.Common;

namespace GroupPlus.BusinessContract.CommonAPIs
{
    public class SettingRegRespObj
    {
        public long SettingId;
        public APIResponseStatus Status;
    }

    #region Bank
    public class BankRespObj
    {
        public List<BankObj> Banks;
        public APIResponseStatus Status;
    }

    public class BankObj
    {
        public int BankId;

        public string Name;

        public int Status;

        public string StatusLabel;
    }



    #endregion

    #region WorkFlowOrder
    public class WorkFlowOrderRespObj
    {
        public List<WorkFlowOrderObj> WorkFlowOrders;
        public APIResponseStatus Status;
    }

    public class WorkFlowOrderObj
    {
        public int WorkflowOrderId;

        public string Title;
        public string TimeStampRegistered;

        public int Status;
        public string StatusLabel;
    }



    #endregion

    #region WorkFlowOrderItem
    public class WorkFlowOrderItemRespObj
    {
        public List<WorkFlowOrderItemObj> WorkFlowOrderItems;
        public APIResponseStatus Status;
    }

    public class WorkFlowOrderItemObj
    {

        public int WorkflowOrderItemId;

        public int WorkflowOrderId;
        public string WorkflowOrderTitle;
        public int Order;

        public string Name;
        
        public int Status;
        public string StatusLabel;
    }



    #endregion


    #region Pension Administrator

    public class PensionAdministratorRespObj
    {
        public List<PensionAdministratorObj> PensionAdministrators;
        public APIResponseStatus Status;
    }

    public class PensionAdministratorObj
    {
        public int PensionAdministratorId;

        public string Name;

        public int Status;

        public string StatusLabel;
    }

    #endregion

    #region Country

    public class CountryRespObj
    {
        public List<CountryObj> Countries;
        public APIResponseStatus Status;
    }

    public class CountryObj
    {
        public int CountryId;

        public string Name;

        public int Status;

        public string StatusLabel;
    }

    #endregion

    #region State

    public class StateRespObj
    {
        public List<StateObj> States;
        public APIResponseStatus Status;
    }

    public class StateObj
    {
        public int StateId;

        public string Name;

        public int CountryId;

        public string CountryLabel;

        public int Status;

        public string StatusLabel;
    }

    #endregion

    #region LocalArea

    public class LocalAreaRespObj
    {
        public List<LocalAreaObj> LocalAreas;
        public APIResponseStatus Status;
    }

    public class LocalAreaObj
    {
        public int LocalAreaId;

        public string Name;

        public int StateId;

        public string StateLabel;

        public int Status;

        public string StatusLabel;
    }

    #endregion

    #region JobType

    public class JobTypeRespObj
    {
        public List<JobTypeObj> JobTypes;
        public APIResponseStatus Status;
    }

    public class JobTypeObj
    {
        public int JobTypeId;

        public string Name;

        public int Status;

        public string StatusLabel;


    }

    #endregion

    #region LeaveType

    public class LeaveTypeRespObj
    {
        public List<LeaveTypeObj> LeaveTypes;
        public APIResponseStatus Status;
    }

    public class LeaveTypeObj
    {
        public int LeaveTypeId;

        public string Name;

        public int MinDays;

        public int MaxDays;

        public int Status;

        public string StatusLabel;


    }

    #endregion


    #region JobLevel

    public class JobLevelRespObj
    {
        public List<JobLevelObj> JobLevels;
        public APIResponseStatus Status;
    }

    public class JobLevelObj
    {
        public int JobLevelId;

        public string Name;

        public int Status;

        public string StatusLabel;


    }

    #endregion

    #region Discipline

    public class DisciplineRespObj
    {
        public List<DisciplineObj> Disciplines;
        public APIResponseStatus Status;
    }

    public class DisciplineObj
    {
        public int DisciplineId;

        public string Name;

        public int Status;

        public string StatusLabel;


    }

    #endregion

    #region Institution

    public class InstitutionRespObj
    {
        public List<InstitutionObj> Institutions;
        public APIResponseStatus Status;
    }

    public class InstitutionObj
    {
        public int InstitutionId;

        public string Name;

        public int Status;

        public string StatusLabel;


    }

    #endregion

    #region ClassOfAward

    public class ClassOfAwardRespObj
    {
        public List<ClassOfAwardObj> ClassOfAwards;
        public APIResponseStatus Status;
    }

    public class ClassOfAwardObj
    {
        public int ClassOfAwardId;

        public string Name;

        public decimal LowerGradePoint;

        public decimal UpperGradePoint;

        public int Status;

        public string StatusLabel;


    }

    #endregion


    #region CourseOfStudy

    public class CourseOfStudyRespObj
    {
        public List<CourseOfStudyObj> CourseOfStudys;
        public APIResponseStatus Status;
    }

    public class CourseOfStudyObj
    {
        public int CourseOfStudyId;

        public string Name;
        public int DisciplineId;

        public string DisciplineLabel;

        public int Status;

        public string StatusLabel;


    }

    #endregion
 
    #region InsurancePolicyType

    public class InsurancePolicyTypeRespObj
    {
        public List<InsurancePolicyTypeObj> InsurancePolicyTypes;
        public APIResponseStatus Status;
    }

    public class InsurancePolicyTypeObj
    {
        public int InsurancePolicyTypeId;

        public string Name;

        public int Status;

        public string StatusLabel;


    }



    #endregion

    #region JobPosition

    public class JobPositionRespObj
    {
        public List<JobPositionObj> JobPositions;
        public APIResponseStatus Status;
    }

    public class JobPositionObj
    {
        public int JobPositionId;
        public string Name;
        public int Status;
        public string StatusLabel;


    }

    #endregion

    #region Staff Role

    public class StaffRoleRespObj
    {
        public List<StaffRoleObj> StaffRoles;
        public APIResponseStatus Status;
    }

    public class StaffRoleObj
    {
        public int StaffRoleId;
        public string Name;
        public int Status;
        public string StatusLabel;


    }


    #endregion

    #region SalaryGrade

    public class SalaryGradeRespObj
    {
        public List<SalaryGradeObj> SalaryGrades;
        public APIResponseStatus Status;
    }

    public class SalaryGradeObj
    {
        public int SalaryGradeId;
        public string Name;
        public int Status;
        public string StatusLabel;


    }

    #endregion

    #region SalaryLevel

    public class SalaryLevelRespObj
    {
        public List<SalaryLevelObj> SalaryLevels;
        public APIResponseStatus Status;
    }

    public class SalaryLevelObj
    {
        public int SalaryLevelId;
        public string Name;
        public int Status;
        public string StatusLabel;


    }


    #endregion

    #region ProfessionalMembershipType

    public class ProfessionalMembershipTypeRespObj
    {
        public List<ProfessionalMembershipTypeObj> ProfessionalMembershipTypes;
        public APIResponseStatus Status;
    }

    public class ProfessionalMembershipTypeObj
    {
        public int ProfessionalMembershipTypeId;
        public string Name;
        public int Status;
        public string StatusLabel;


    }

    #endregion

    #region JobSpecialization

    public class JobSpecializationRespObj
    {
        public List<JobSpecializationObj> JobSpecializations;
        public APIResponseStatus Status;
    }

    public class JobSpecializationObj
    {
        public int JobSpecializationId;
        public string Name;
        public int Status;
        public string StatusLabel;


    }

    #endregion

    #region KPIndex

    public class KPIndexRespObj
    {
        public List<KPIndexObj> KPIndexs;
        public APIResponseStatus Status;
    }

    public class KPIndexObj
    {
        public int KPIndexId;
        public string Name;
        public string Indicator;
        public decimal MinRating;
        public decimal MaxRating;
        public int Status;
        public string StatusLabel;


    }


    #endregion

    #region Qualification

    public class QualificationRespObj
    {
        public List<QualificationObj> Qualifications;
        public APIResponseStatus Status;
    }

    public class QualificationObj
    {
        public int QualificationId;
        public string Name;
        public int Rank;
        public int Status;
        public string StatusLabel;


    }


    #endregion

    #region ProfessionalBody

    public class ProfessionalBodyRespObj
    {
        public List<ProfessionalBodyObj> ProfessionalBodys;
        public APIResponseStatus Status;
    }

    public class ProfessionalBodyObj
    {
        public int ProfessionalBodyId;
        public string Name;
        public string Acronym;
        public int Status;
        public string StatusLabel;


    }

    #endregion

    #region TerminationReason

    public class TerminationReasonRespObj
    {
        public List<TerminationReasonObj> TerminationReasons;
        public APIResponseStatus Status;
    }

    public class TerminationReasonObj
    {
        public int TerminationReasonId;
        public string Name;
        public int Status;
        public string StatusLabel;

    }


    #endregion

    #region Company

    public class CompanyRespObj
    {
        public List<CompanyObj> Companies;
        public APIResponseStatus Status;
    }

    public class CompanyObj
    {
        public int CompanyId;
        public string Name;
        public string BusinessDescription;
        public string Email;
        public string Address;
        public int CompanyType;
        public string CompanyTypeLabel;
        public int Status;
        public int RegisteredBy;
        public string RegisteredByName;
        public string TimeStampRegister;
        public string StatusLabel;

    }

    #endregion

    #region Department

    public class DepartmentRespObj
    {
        public List<DepartmentObj> Departments;
        public APIResponseStatus Status;
    }

    public class DepartmentObj
    {
        public int DepartmentId;
        public string Name;
        public int Status;
        public string StatusLabel;

    }


    #endregion

    #region Staff
    public class StaffLoginResp
    {
        public StaffObj StaffInfo;
        public string AuthToken;
        public string Username;
        public string CustomSetting;
        public List<string> AccessRoles;
        public int UserId;
        public APIResponseStatus Status;
    }

    public class StaffRespObj
    {
        public List<StaffObj> Staffs;
        public APIResponseStatus Status;
    }

    public class StaffObj
    {

        public int StaffId;
        public string LastName;
        public string FirstName;
        public string MiddleName;

        public int Gender;
        public string GenderLabel;

        public string DateOfBirth;
        public string Email;
        public string PermanentHomeAddress;

        public int EmploymentType;
        public string EmploymentTypeLabel;

        public string MobileNumber;

        public int CountryOfOriginId;
        public string CountryOfOriginLabel;

        public int StateOfOriginId;
        public string StateOfOriginLabel;

        public int LocalAreaId;
        public string LocalAreaLabel;

        public int MaritalStatus;
        public string MaritalStatusLabel;

        public string EmploymentDate;

        public int CompanyId;
        public string CompanyLabel;

        public int Status;
        public string StatusLabel;

        public string TimeStamRegistered;
    }

}

  #endregion


    #region StaffContact


public class StaffContactRespObj
{
    public List<StaffContactObj> Staffs;
    public APIResponseStatus Status;
}

public class StaffContactObj
{
    public int StaffContactId;

    public int StaffId;
    public string FirstName;
    public string LastName;

    public string ResidentialAddress;
    public string TownOfResidence;
    public string LocationLandmark;

    public int StateOfResidenceId;
    public string StateOfResidenceLabel;

    public int LocalAreaOfResidenceId;
    public string LocalAreaOfResidenceLabel;

    public bool IsDefault;

    public int Status;
    public string StatusLabel;



    public string TimeStamRegistered;
}

#endregion

    #region StaffEmergencyContact

public class EmergencyContactRespObj
{
    public List<EmergencyContactObj> EmergencyContacts;
    public APIResponseStatus Status;
}

public class EmergencyContactObj
{
    public int EmergencyContactId;
    public int StaffId;
    
    public string LastName;
    public string FirstName;
    public string MiddleName;

    public int Gender;
    public string GenderLabel;

    public string ResidentialAddress;
    public string MobileNumber;
    public bool IsDefault;

    public int StateOfOriginId;
    public string StateOfOriginLabel;

    public int LocalAreaId;
    public string LocalAreaLabel;
 
 }

#endregion
   
    #region StaffInsurance

public class StaffInsuranceRespObj
{
    public List<StaffInsuranceObj> StaffInsurances;
    public APIResponseStatus Status;
}

public class StaffInsuranceObj
{
    public int StaffInsuranceId;
    public int StaffId;

    public string LastName;
    public string FirstName;
    public string MiddleName;

    public int InsurancePolicyTypeId;
    public string InsurancePolicyType;

    public string PolicyNumber;
    public string Insurer;
    public string CommencementDate;
    public string TerminationDate;
    public decimal PersonalContibution;
    public decimal CompanyContibution;

    public int Status;
    public string StatusLabel;
}

#endregion

    #region StaffJobInfo

public class StaffJobInfoRespObj
{
    public List<StaffJobInfoObj> StaffJobInfos;
    public APIResponseStatus Status;
}

public class StaffJobInfoObj
{

    public int StaffJobInfoId;

    public int StaffId;
    public string LastName;
    public string FirstName;
    public string MiddleName;

    public int EntranceCompanyId;
    public string EntranceCompany;

    public int CurrentCompanyId;
    public string CurrentCompany;

    public int EntranceDepartmentId;
    public string EntranceDepartment;

    public int CurrentDepartmentId;
    public string CurrentDepartment;

    public int JobTypeId;
    public string JobType;

    public int JobLevelId;
    public string JobLevel;

    public int JobPositionId;
    public string JobPosition;

    public int JobSpecializationId;
    public string JobSpecialization;

    public string JobTitle;
    public string JobDescription;


    public int SalaryGradeId;
    public string SalaryGrade;

    public int SalaryLevelId;
    public string SalaryLevel;

    public int TeamLeadId;
    public string TeamLead;

    public int LineManagerId;
    public string LineManager;

    public int StaffInsuranceId;
    public string StaffInsurance;
    public string TimeStampRegistered;
}

#endregion

    #region StaffMedical

public class StaffMedicalRespObj
{
    public List<StaffMedicalObj> StaffMedicals;
    public APIResponseStatus Status;
}

public class StaffMedicalObj
{

    public int StaffId;
    public int StaffMedicalId;
    public int BloodGroup;
    public string BloodGroupLabel;

    public int Genotype;
    public string GenotypeLabel;

    public string MedicalFitnessReport;
    public string KnownAilment;
    public string TimeStampRegistered;
}



#endregion

    #region StaffBankAccount

public class StaffBankAccountRespObj
{
    public List<StaffBankAccountObj> StaffBankAccounts;
    public APIResponseStatus Status;
}

public class StaffBankAccountObj
{
    public int StaffBankAccountId;
    public int StaffId;
    public int StaffName;

    public int BankId;
    public string BankName;

    public string AccountName;
    public string AccountNumber;

    public bool IsDefault;

    public int Status;
    public string StatusLabel;

    public string TimeStamRegistered;


}

#endregion

    #region Staff_NextOf Kin
public class StaffNextOfKinRespObj
{
    public List<StaffNextOfKinObj> StaffNextOfKins;
    public APIResponseStatus Status;
}

public class StaffNextOfKinObj
{
    public int StaffNextOfKinId;

    public int StaffId;
    public string StaffName;

    public int StateOfLocationId;
    public string StateOfLocation;

    public int LocalAreaOfLocationId;
    public string LocalAreaOfLocation;

    public string LastName;
    public string FirstName;
    public string MiddleName;

    public int Gender;
    public string GenderLabel;

    public string ResidentialAddress;
    public string MobileNumber;
    public string Email;

    public int Relationship;
    public string RelationshipLabel;

    public string Landphone;

    public int MaritalStatus;
    public string MaritalStatusLabel;

    public string TimeStampRegister;
}


#endregion

    #region EducationalQualification
public class EducationalQualificationRespObj
{
    public List<EducationalQualificationObj> EducationalQualifications;
    public APIResponseStatus Status;
}

public class EducationalQualificationObj
{
    public long HigherEducationId;

    public int StaffId;
    public string FirstName;
    public string LastName;
    public string MiddleName;

    public int DisciplineId;
    public string SpecifiedDiscipline;

    public int CourseOfStudyId;
    public string SpecifiedCourseOfStudy;

    public int QualificationId;
    public string QualificationLabel;

    public int ClassOfAwardId;
    public string ClassOfAwardLabel;

    public int InstitutionId;
    public string SpecifiedInstitution;

    public int StartYear;
    public int EndYear;
  
    public string CGPA;
    public int GradeScale;
    public string TimeStampRegistered;
    public string TimeStampLastEdited;

}


#endregion

    #region Professional Membership
public class ProfessionalMembershipRespObj
{
    public List<ProfessionalMembershipObj> ProfessionalMemberships;
    public APIResponseStatus Status;
}

public class ProfessionalMembershipObj
{
    public long ProfessionalMembershipId;

    public int StaffId;
    public string FirstName;
    public string MiddleName;
    public string LastName;

    public int ProfessionalBodyId;
    public string ProfessionalBody;

    public int YearJoined;

    public int ProfessionalMembershipTypeId;
    public string ProfessionalMembershipType;

}



#endregion

    #region LeaveRequest

public class LeaveRequestRespObj
{
    public List<LeaveRequestObj> LeaveRequests;
    public APIResponseStatus Status;
}

public class LeaveRequestObj
{

    public long LeaveRequestId;

    public int StaffId;
    public string FirstName;
    public string LastName;
    public string MiddleName;

    public int DepartmentId;
    public string Department;
    public string LeaveTitle;
    

    public int CompanyId;
    public string Company;

    public int LeaveTypeId;
    public string LeaveType;

    public string TimeStampRegistered;

    public string ProposedStartDate;
    public string ProposedEndDate;

    public string Purpose;
    public  string OtherRemarks;

    public int Status;
    public string StatusLabel;
}


#endregion

    #region StaffLeave

public class StaffLeaveRespObj
{
    public List<StaffLeaveObj> StaffLeaves;
    public APIResponseStatus Status;
}

public class StaffLeaveObj
{
    public long StaffLeaveId;

    public long LeaveRequestId;
    public string LeaveTitle;

    public int LeaveTypeId;
    public string LeaveType;

    public int StaffId;
    public string FirstName;
    public string LastName;
    public string MiddleName;

    public int DepartmentId;
    public string Department;

    public int CompanyId;
    public string Company;

    public string ProposedStartDate;
    public string ProposedEndDate;



    public string TimeStampRegistered;
    public int HODApprovedBy;

    public int LMApprovedBy;

    public int HRApprovedBy;

 

    public string HODComment;
    public string LMComment;
    public string HRComment;
    public string HODApprovedStartDate;
    public string HODApprovedEndDate;
    public string LMApprovedStartDate;
    public string LMApprovedEndDate;
    public string HRApprovedStartDate;
    public string HRApprovedEndDate;
    public string LMApprovedTimeStamp;
    public string HODApprovedTimeStamp;
    public string HRApprovedTimeStamp ;

    public int Status;
    public string StatusLabel;



    public string Purpose;
    public string OtherRemarks;

}



#endregion

    #region WorkFlowLog

public class WorkFlowLogRespObj
{
    public List<WorkFlowLogObj> WorkFlowLogs;
    public APIResponseStatus Status;
}

public class WorkFlowLogObj
{
    public long WorkflowLogId;

    public int WorkflowSetupId;
    public WorkflowApprovalType ApprovalType;

    public int StaffId;
    public string StaffName;

    public int ProcessorId;

    public int WorkflowOrderItemId;
    public string WorkflowOrderItem;

    public int Status;
    public string StatusLabel;

    public string Comment;
    public string LogTimeStamp;

}



#endregion

    #region StaffKPIIndex

public class StaffKPIIndexRespObj
{
    public List<StaffKPIIndexObj> StaffKPIIndexs;
    public APIResponseStatus Status;
}

public class StaffKPIIndexObj
{

    public int StaffKPIndexId;

    public int StaffId;
    public string FirstName;
    public string LastName;
    public string MiddleName;

    public int KPIndexId;
    public string KPIndexLabel;

    public string Description;
    public string StartDate;
    public string EndDate;

    public decimal Rating;

    public string Comment;
    public string SupervisorRemarks;
    public string RemarkTimeStamp;

    public int SupervisorId;
    public string SupervisorName;

    public string TimeStampRegistered;
}



#endregion

    #region StaffPension

public class StaffPensionRespObj
{
    public List<StaffPensionObj> StaffPensions;
    public APIResponseStatus Status;
}

public class StaffPensionObj
{


    public int StaffPensionId;

    public int StaffId;
    public string FirstName;
    public string LastName;
    public string MiddleName;

    public string PensionNumber;

    public decimal CompanyContribution;

    public decimal PersonalContribution;

    public int PensionAdministratorId;
    public string PensionAdministrator;

    public string TimeStampRegister;
}



#endregion

    #region StaffMemo

public class StaffMemoRespObj
{
    public List<StaffMemoObj> StaffMemos;
    public APIResponseStatus Status;
}

public class StaffMemoObj
{
    public int StaffMemoId;

    public int StaffId;
    public string FirstName;
    public string LastName;
    public string MiddleName;

    public int MemoTypeId;
    public string MemoType;

    public string Title;

    public int RegisterBy;
  

    public int ApprovedBy;
    public bool IsReplied;


    public string MemoDetail;
    public string TimeStampRegister;

    public int Status;
    public string StatusLabel;
}




#endregion

    #region StaffMemoResponse

public class StaffMemoResponseRespObj
{
    public List<StaffMemoResponseObj> StaffMemoResponses;
    public APIResponseStatus Status;
}

public class StaffMemoResponseObj
{
    public int StaffMemoResponseId;

    public int StaffMemoId;
    public string MemoTitle;

    public int StaffId;
    public string FirstName;
    public string LastName;
    public string MiddleName;

    public string MemoResponse;
  
    public string TimeStampRegister;

    public int IssuerId;
  

    public string IssuerRemarks;
    public string IssuerRemarkTimeStamp;
    public string ManagementRemarks;
    public string ManagementRemarkTimeStamp;

    public int ManagementRemarksBy;
   
}




#endregion

    #region StaffSalary

public class StaffSalaryRespObj
{
    public List<StaffSalaryObj> StaffSalaries;
    public APIResponseStatus Status;
}

public class StaffSalaryObj
{
    public int StaffSalaryId;

    public int StaffId;
    public string FirstName;
    public string LastName;
    public string MiddleName;

    public int StaffJobInfoId;
    public string StaffJobInfoTitle;

    public int Currency;
    public string CurrencyName;
    public decimal BasicAllowance;
    public decimal HousingAllowance;
    public decimal EducationAllowance;
    public decimal FurnitureAllowance;
    public decimal WardrobeAllowance;
    public decimal TransportAllowance;
    public decimal LeaveAllowance;
    public decimal EntertainmentAllowance;

    public decimal PensionDeduction;
    public decimal PayeDeduction;
    public decimal InsuranceDeduction;

    public decimal TotalPayment;
    public decimal TotalDeduction;
    public string TimeStamRegistered;

    public int Status;
    public string StatusLabel;
}




#endregion

#region Comment

public class CommentRespObj
{
    public List<CommentObj> Comments;
    public APIResponseStatus Status;
}

public class CommentObj
{
    public int CommentId;

    public int StaffId;
    public string FirstName;
    public string LastName;
    public string MiddleName;

  
    public string CommentDetails;

    public int CommentType;
    public string CommentTypeLabel;

    public string TimeStampCommented;
}

#endregion