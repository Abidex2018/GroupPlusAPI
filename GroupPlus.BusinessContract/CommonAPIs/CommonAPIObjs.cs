using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GroupPlus.Common;

namespace GroupPlus.BusinessContract.CommonAPIs
{
    public class SettingSearchObj : AdminObj
    {
        public int SettingId { get; set; }
        public int Status { get; set; }
    }
    public class CommonSettingSearchObj 
    {
        public int SettingId { get; set; }
        public int Status { get; set; }
    }
    public class StaffSearchObj : AdminObj
    {
        public int StaffId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string MobileNumber { get; set; }
        public string Email { get; set; }
        public int CompanyId { get; set; }
        public int Status { get; set; }
    }

    #region Common Settings

    #region Bank

    public class RegBankObj : AdminObj
    {

        [Required(AllowEmptyStrings = false, ErrorMessage = "Bank Name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Bank Name must be between 2 and 50 characters")]
        public string Name { get; set; }

        public int Status { get; set; }

    }

    public class EditBankObj : AdminObj
    {
        [CheckNumber(0, ErrorMessage = "Bank Id is required")]
        public int BankId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Bank Name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Bank Name must be between 2 and 50 characters")]
        public string Name { get; set; }

        public int Status { get; set; }

    }
    public class DeleteBankObj : AdminObj
    {
        [CheckNumber(0, ErrorMessage = "Bank Id is required")]
        public int BankId { get; set; }
    }
    #endregion


    #region WorkFlowOrder

    public class RegWorkFlowOrderObj : AdminObj
    {

        [Required(AllowEmptyStrings = false, ErrorMessage = "Title is required")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Title is too short or too long (5 - 100)")]
        public string Title { get; set; }

        public int Status { get; set; }

    }

    public class EditWorkFlowOrderObj : AdminObj
    {
        [CheckNumber(0, ErrorMessage = "WorkflowOrder Id is required")]
        public int WorkflowOrderId { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Title is required")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Title is too short or too long (5 - 100)")]
        public string Title { get; set; }

        public int Status { get; set; }

    }
    public class DeleteWorkFlowOrderObj : AdminObj
    {
        [CheckNumber(0, ErrorMessage = "WorkFlowOrder Id is required")]
        public int WorkflowOrderId { get; set; }
    }
    #endregion

    #region WorkFlowOrderItem

    public class RegWorkFlowOrderItemObj : AdminObj
    {
        [CheckNumber(0, ErrorMessage = "WorkflowOrder Id is required")]
        public int WorkflowOrderId { get; set; }

        public int Order { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Name is required")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Name is too short or too long (5 - 100)")]
        public string Name { get; set; }

        public int Status { get; set; }

    }

    public class EditWorkFlowOrderItemObj : AdminObj
    {
        [CheckNumber(0, ErrorMessage = "WorkflowOrderItem Id is required")]
        public int WorkflowOrderItemId { get; set; }

        public int WorkflowOrderId { get; set; }

        public int Order { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Name is required")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Name is too short or too long (5 - 100)")]
        public string Name { get; set; }

        public int Status { get; set; }

    }
    public class DeleteWorkFlowOrderItemObj : AdminObj
    {
        [CheckNumber(0, ErrorMessage = "WorkFlowOrderItem Id is required")]
        public int WorkflowOrderItemId { get; set; }
    }
    #endregion

    #region Pension Administrator

    public class RegPensionAdministratorObj : AdminObj
    {

        [Required(AllowEmptyStrings = false, ErrorMessage = "Pension Administrator's Name is required")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Pension Administrator's Name must be between 2 and 200 characters")]
        public string Name { get; set; }

        public int Status { get; set; }

    }

    public class EditPensionAdministratorObj : AdminObj
    {
        [CheckNumber(0, ErrorMessage = "Pension Administrator Id is required")]
        public int PensionAdministratorId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Pension Administrator's Name is required")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Pension Administrator's Name must be between 2 and 200 characters")]
        public string Name { get; set; }

        public int Status { get; set; }

    }
    public class DeletePensionAdministratorObj : AdminObj
    {
        [CheckNumber(0, ErrorMessage = "Bank Id is required")]
        public int PensionAdministratorId { get; set; }
    }
    #endregion

    #region Country

    public class RegCountryObj : AdminObj
    {

        [Required(AllowEmptyStrings = false, ErrorMessage = "Country Name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Country Name must be between 2 and 50 characters")]
        public string Name { get; set; }

        public int Status { get; set; }

    }

    public class EditCountryObj : AdminObj
    {
        [CheckNumber(0, ErrorMessage = "Country Id is required")]
        public int CountryId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Country Name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Country Name must be between 2 and 50 characters")]
        public string Name { get; set; }

        public int Status { get; set; }

    }
    public class DeleteCountryObj : AdminObj
    {
        [CheckNumber(0, ErrorMessage = "Country Id is required")]
        public int CountryId { get; set; }
    }
    #endregion

    #region State

    public class RegStateObj : AdminObj
    {

        [CheckNumber(0, ErrorMessage = "Country Id is required")]
        public int CountryId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "State Name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "State Name must be between 2 and 50 characters")]
        public string Name { get; set; }

        public int Status { get; set; }

    }

    public class EditStateObj : AdminObj
    {
        [CheckNumber(0, ErrorMessage = "State Id is required")]
        public int StateId { get; set; }

        [CheckNumber(0, ErrorMessage = "Country Id is required")]
        public int CountryId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "State Name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "State Name must be between 2 and 50 characters")]
        public string Name { get; set; }

        public int Status { get; set; }

    }
    public class DeleteStateObj : AdminObj
    {
        [CheckNumber(0, ErrorMessage = "State Id is required")]
        public int StateId { get; set; }
    }
    #endregion

    #region Local Area

    public class RegLocalAreaObj : AdminObj
    {

        [CheckNumber(0, ErrorMessage = "State Id is required")]
        public int StateId { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Local Area Name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Local Area Name must be between 2 and 50 characters")]
        public string Name { get; set; }

        public int Status { get; set; }

    }

    public class EditLocalAreaObj : AdminObj
    {
        [CheckNumber(0, ErrorMessage = "Local Area Id is required")]
        public int LocalAreaId { get; set; }

        [CheckNumber(0, ErrorMessage = "State Id is required")]
        public int StateId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Local Area Name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Local Area must be between 2 and 50 characters")]
        public string Name { get; set; }

        public int Status { get; set; }

    }
    public class DeleteLocalAreaObj : AdminObj
    {
        [CheckNumber(0, ErrorMessage = "Local Area Id is required")]
        public int LocalAreaId { get; set; }
    }
    #endregion



    #region JobType

    public class RegJobTypeObj : AdminObj
    {

        [Required(AllowEmptyStrings = false, ErrorMessage = "Job Type is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Job Type must be between 2 and 50 characters")]
        public string Name { get; set; }

        public int Status { get; set; }

    }

    public class EditJobTypeObj : AdminObj
    {
        [CheckNumber(0, ErrorMessage = "Job Type is required")]
        public int JobTypeId { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Job Type is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Job Type must be between 2 and 50 characters")]
        public string Name { get; set; }

        public int Status { get; set; }

    }
    public class DeleteJobTypeObj : AdminObj
    {
        [CheckNumber(0, ErrorMessage = "Job Type is required")]
        public int JobTypeId { get; set; }
    }

    #endregion

    #region Role

    public class RegStaffRoleObj : AdminObj
    {


        [Required(AllowEmptyStrings = false, ErrorMessage = "Staff Role Name is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Staff Role Name must be between 3 and 50 characters")]
        public string Name { get; set; }

        public int Status { get; set; }

    }

    public class EditStaffRoleObj : AdminObj
    {
        [CheckNumber(0, ErrorMessage = "Staff Role Id is required")]
        public int StaffRoleId { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Staff Role Name is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Staff Role Name must be between 3 and 50 characters")]
        public string Name { get; set; }

        public int Status { get; set; }

    }

    public class DeleteStaffRoleObj : AdminObj
    {
        [CheckNumber(0, ErrorMessage = "Staff Role Id is required")]
        public int StaffRoleId { get; set; }
    }


    #endregion

    #region LeaveType

    public class RegLeaveTypeObj : AdminObj
    {

        [Required(AllowEmptyStrings = false, ErrorMessage = "Leave Type is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Leave Type must be between 2 and 50 characters")]
        public string Name { get; set; }

        [CheckNumber(0, ErrorMessage = "Invalid Minimum Days")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Minimum Days is required")]
        public int MinDays { get; set; }

        [CheckNumber(0, ErrorMessage = "Invalid Maximum Days")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Maximum Days is required")]
        public int MaxDays { get; set; }

        public int Status { get; set; }

    }

    public class EditLeaveTypeObj : AdminObj
    {
        [CheckNumber(0, ErrorMessage = "Leave Type is required")]
        public int LeaveTypeId { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Leave Type is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Leave Type must be between 2 and 50 characters")]
        public string Name { get; set; }

        [CheckNumber(0, ErrorMessage = "Invalid Minimum Days")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Minimum Days is required")]
        public int MinDays { get; set; }
        [CheckNumber(0, ErrorMessage = "Invalid Maximum Days")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Maximum Days is required")]
        public int MaxDays { get; set; }

        [CheckNumber(0, ErrorMessage = "Invalid Maximum Days")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Maximum Days is required")]
        public int Status { get; set; }

    }
    public class DeleteLeaveTypeObj : AdminObj
    {
        [CheckNumber(0, ErrorMessage = "Leave Type is required")]
        public int LeaveTypeId { get; set; }
    }


    #endregion


    #region JobLevel
    public class RegJobLevelObj : AdminObj
    {

        public int JobLevelId { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Job Level Name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Job Level Name must be between 2 and 50 characters")]
        public string Name { get; set; }

        public int Status { get; set; }

    }

    public class EditJobLevelObj : AdminObj
    {
        [CheckNumber(0, ErrorMessage = "Job Level Id is required")]
        public int JobLevelId { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Job Level Name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Job Level Name must be between 2 and 50 characters")]
        public string Name { get; set; }

        public int Status { get; set; }
    }
    public class DeleteJobLevelObj : AdminObj
    {
        [CheckNumber(0, ErrorMessage = "Job Level Id is required")]
        public int JobLevelId { get; set; }
    }

    #endregion

    #region Discipline
    public class RegDisciplineObj : AdminObj
    {

        [Required(AllowEmptyStrings = false, ErrorMessage = "Discipline is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Discipline must be between 2 and 50 characters")]
        public string Name { get; set; }

        public int Status { get; set; }

    }

    public class EditDisciplineObj : AdminObj
    {
        [CheckNumber(0, ErrorMessage = "Discipline Id is required")]
        public int DisciplineId { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Discipline is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Discipline must be between 2 and 50 characters")]
        public string Name { get; set; }

        public int Status { get; set; }
    }
    public class DeleteDisciplineObj : AdminObj
    {
        [CheckNumber(0, ErrorMessage = "Discipline Id is required")]
        public int DisciplineId { get; set; }
    }

    #endregion

    #region Institution

    public class RegInstitutionObj : AdminObj
    {


        [Required(AllowEmptyStrings = false, ErrorMessage = "Institution's Name is required")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Institution's Name must be between 2 and 200 characters")]
        public string Name { get; set; }

        public int Status { get; set; }

    }

    public class EditInstitutionObj : AdminObj
    {
        [CheckNumber(0, ErrorMessage = "Institution Id is required")]
        public int InstitutionId { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Institution's Name is required")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Institution's Name must be between 2 and 200 characters")]
        public string Name { get; set; }

        public int Status { get; set; }
    }
    public class DeleteInstitutionObj : AdminObj
    {
        [CheckNumber(0, ErrorMessage = "Institution Id is required")]
        public int InstitutionId { get; set; }
    }



    #endregion

    #region ClassOfAward

    public class RegClassOfAwardObj : AdminObj
    {

        [Required(AllowEmptyStrings = false, ErrorMessage = "Class of Certificate is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Class of Certificate must be between 2 and 50 characters")]
        public string Name { get; set; }

        public decimal LowerGradePoint { get; set; }
        public decimal UpperGradePoint { get; set; }
        public int Status { get; set; }

    }

    public class EditClassOfAwardObj : AdminObj
    {
        [CheckNumber(0, ErrorMessage = "Class Award Id is required")]
        public int ClassOfAwardId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Class of Certificate is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Class of Certificate must be between 2 and 50 characters")]
        public string Name { get; set; }

        public decimal LowerGradePoint { get; set; }
        public decimal UpperGradePoint { get; set; }
        public int Status { get; set; }
    }
    public class DeleteClassOfAwardObj : AdminObj
    {
        [CheckNumber(0, ErrorMessage = "Class Award Id is required")]
        public int ClassOfAwardId { get; set; }
    }

    #endregion

    #region InsurancePolicy

    public class RegInsurancePolicyTypeObj : AdminObj
    {


        [Required(AllowEmptyStrings = false, ErrorMessage = "Policy Name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Policy Name must be between 2 and 50 characters")]
        public string Name { get; set; }

        public int Status { get; set; }

    }

    public class EditInsurancePolicyTypeObj : AdminObj
    {
        [CheckNumber(0, ErrorMessage = "Insurance Policy Type Id is required")]
        public int InsurancePolicyTypeId { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Policy Name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Policy Name must be between 2 and 50 characters")]
        public string Name { get; set; }

        public int Status { get; set; }
    }
    public class DeleteInsurancePolicyTypeObj : AdminObj
    {
        [CheckNumber(0, ErrorMessage = "Insurance Policy Type Id is required")]
        public int InsurancePolicyTypeId { get; set; }
    }

    #endregion

    #region JobPosition

    public class RegJobPositionObj : AdminObj
    {




        [Required(AllowEmptyStrings = false, ErrorMessage = "Job Position is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Job Position must be between 2 and 100 characters")]
        public string Name { get; set; }

        public int Status { get; set; }

    }

    public class EditJobPositionObj : AdminObj
    {
        [CheckNumber(0, ErrorMessage = "Job Position Id is required")]
        public int JobPositionId { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Job Position is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Job Position must be between 2 and 100 characters")]
        public string Name { get; set; }

        public int Status { get; set; }
    }
    public class DeleteJobPositionObj : AdminObj
    {
        [CheckNumber(0, ErrorMessage = "Job Position Type Id is required")]
        public int JobPositionId { get; set; }
    }


    #endregion

    #region SalaryGrade

    public class RegSalaryGradeObj : AdminObj
    {




        [Required(AllowEmptyStrings = false, ErrorMessage = "Salary Grade is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Salary Grade must be between 2 and 50 characters")]
        public string Name { get; set; }

        public int Status { get; set; }

    }

    public class EditSalaryGradeObj : AdminObj
    {
        [CheckNumber(0, ErrorMessage = "Salary Grade Id is required")]
        public int SalaryGradeId { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Salary Grade is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Salary Grade must be between 2 and 50 characters")]
        public string Name { get; set; }

        public int Status { get; set; }
    }
    public class DeleteSalaryGradeObj : AdminObj
    {
        [CheckNumber(0, ErrorMessage = "Salary Grade Id is required")]
        public int SalaryGradeId { get; set; }
    }


    #endregion


    #region SalaryLevel

    public class RegSalaryLevelObj : AdminObj
    {




        [Required(AllowEmptyStrings = false, ErrorMessage = "Salary Level is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Salary Level must be between 2 and 50 characters")]
        public string Name { get; set; }

        public int Status { get; set; }

    }

    public class EditSalaryLevelObj : AdminObj
    {
        [CheckNumber(0, ErrorMessage = "Salary Level Id is required")]
        public int SalaryLevelId { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Salary Level is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Salary Level must be between 2 and 50 characters")]
        public string Name { get; set; }

        public int Status { get; set; }
    }
    public class DeleteSalaryLevelObj : AdminObj
    {
        [CheckNumber(0, ErrorMessage = "Salary Level Id is required")]
        public int SalaryLevelId { get; set; }
    }


    #endregion

    #region CourseOfStudy

    public class RegCourseOfStudyObj : AdminObj
    {


        [CheckNumber(0, ErrorMessage = "Discipline Id is required")]
        public int DisciplineId { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Course of Study Name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Course of Study Name must be between 2 and 50 characters")]
        public string Name { get; set; }

        public int Status { get; set; }

    }

    public class EditCourseOfStudyObj : AdminObj
    {
        [CheckNumber(0, ErrorMessage = "Course Of Study Id is required")]
        public int CourseOfStudyId { get; set; }

        [CheckNumber(0, ErrorMessage = "Discipline Id is required")]
        public int DisciplineId { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Course of Study Name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Course of Study Name must be between 2 and 50 characters")]
        public string Name { get; set; }

        public int Status { get; set; }
    }
    public class DeleteCourseOfStudyObj : AdminObj
    {
        [CheckNumber(0, ErrorMessage = "Course Of Study Id is required")]
        public int CourseOfStudyId { get; set; }
    }

    #endregion

    #region ProfessionalMembershipType

    public class RegProfessionalMembershipTypeObj : AdminObj
    {




        [Required(AllowEmptyStrings = false, ErrorMessage = "Professional Membership Type Name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Professional Membership Type Name must be between 2 and 50 characters")]
        public string Name { get; set; }

        public int Status { get; set; }

    }

    public class EditProfessionalMembershipTypeObj : AdminObj
    {
        [CheckNumber(0, ErrorMessage = "Professional Membership Type Id is required")]
        public int ProfessionalMembershipTypeId { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Professional Membership Type Name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Professional Membership Type Name must be between 2 and 50 characters")]
        public string Name { get; set; }

        public int Status { get; set; }
    }
    public class DeleteProfessionalMembershipTypeObj : AdminObj
    {
        [CheckNumber(0, ErrorMessage = "Professional Membership Type Id is required")]
        public int ProfessionalMembershipTypeId { get; set; }
    }


    #endregion

    #region JobSpecialization

    public class RegJobSpecializationObj : AdminObj
    {


        [Required(AllowEmptyStrings = false, ErrorMessage = "Specialization Name is required")]
        [StringLength(350, MinimumLength = 2, ErrorMessage = "Specialization Name must be between 2 and 350 characters")]
        public string Name { get; set; }

        public int Status { get; set; }

    }

    public class EditJobSpecializationObj : AdminObj
    {
        [CheckNumber(0, ErrorMessage = "Specialization Id is required")]
        public int JobSpecializationId { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Specialization Name is required")]
        [StringLength(350, MinimumLength = 2, ErrorMessage = "Specialization Name must be between 2 and 350 characters")]
        public string Name { get; set; }

        public int Status { get; set; }
    }
    public class DeleteJobSpecializationObj : AdminObj
    {
        [CheckNumber(0, ErrorMessage = "Specialization Id is required")]
        public int JobSpecializationId { get; set; }
    }

    #endregion

    #region KPIndex
    public class RegKPIndexObj : AdminObj
    {



        [Required(AllowEmptyStrings = false, ErrorMessage = "KPI Name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "KPI Name must be between 2 and 100 characters")]
        public string Name { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Indicator is required")]
        [StringLength(500, MinimumLength = 2, ErrorMessage = "Indicator must be between 2 and 500 characters")]
        public string Indicator { get; set; }

        public decimal MinRating { get; set; }

        public decimal MaxRating { get; set; }


        public int Status { get; set; }

    }

    public class EditKPIndexObj : AdminObj
    {
        [CheckNumber(0, ErrorMessage = "KPIndex Id is required")]
        public int KPIndexId { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "KPI Name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "KPI Name must be between 2 and 100 characters")]
        public string Name { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Indicator is required")]
        [StringLength(500, MinimumLength = 2, ErrorMessage = "Indicator must be between 2 and 500 characters")]
        public string Indicator { get; set; }

        public decimal MinRating { get; set; }

        public decimal MaxRating { get; set; }


        public int Status { get; set; }
    }
    public class DeleteKPIndexObj : AdminObj
    {
        [CheckNumber(0, ErrorMessage = "KPIndex Id is required")]
        public int KPIndexId { get; set; }
    }




    #endregion

    #region Qualification

    public class RegQualificationObj : AdminObj
    {



        [Required(AllowEmptyStrings = false, ErrorMessage = "Qualification is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Qualification must be between 2 and 50 characters")]
        public string Name { get; set; }

        public int Rank { get; set; }
        public int Status { get; set; }

    }

    public class EditQualificationObj : AdminObj
    {
        [CheckNumber(0, ErrorMessage = "Qualification Id is required")]
        public int QualificationId { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Qualification is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Qualification must be between 2 and 50 characters")]
        public string Name { get; set; }

        public int Rank { get; set; }
        public int Status { get; set; }
    }
    public class DeleteQualificationObj : AdminObj
    {
        [CheckNumber(0, ErrorMessage = "Qualification Id is required")]
        public int QualificationId { get; set; }
    }


    #endregion

    #region ProfessionalBody

    public class RegProfessionalBodyObj : AdminObj
    {


        [Required(AllowEmptyStrings = false, ErrorMessage = "Professional Body's Name is required")]
        [StringLength(500, MinimumLength = 2, ErrorMessage = "Professional Body's Name must be between 2 and 500 characters")]
        public string Name { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Professional Body's Acronym is required")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Professional Body's Acronym must be between 1 and 50 characters")]
        public string Acronym { get; set; }
        public int Status { get; set; }

    }

    public class EditProfessionalBodyObj : AdminObj
    {
        [CheckNumber(0, ErrorMessage = "Professional Body Id is required")]
        public int ProfessionalBodyId { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Professional Body's Name is required")]
        [StringLength(500, MinimumLength = 2, ErrorMessage = "Professional Body's Name must be between 2 and 500 characters")]
        public string Name { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Professional Body's Acronym is required")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Professional Body's Acronym must be between 1 and 50 characters")]
        public string Acronym { get; set; }
        public int Status { get; set; }
    }
    public class DeleteProfessionalBodyObj : AdminObj
    {
        [CheckNumber(0, ErrorMessage = "Professional Body Id is required")]
        public int ProfessionalBodyId { get; set; }
    }


    #endregion

    #region TerminationReason
    public class RegTerminationReasonObj : AdminObj
    {

        [Required(AllowEmptyStrings = false, ErrorMessage = "Termination Reason Name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Termination Reason Name must be between 2 and 50 characters")]
        public string Name { get; set; }
        public int Status { get; set; }

    }

    public class EditTerminationReasonObj : AdminObj
    {
        [CheckNumber(0, ErrorMessage = "Termination is required")]
        public int TerminationReasonId { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Termination Reason Name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Termination Reason Name must be between 2 and 50 characters")]
        public string Name { get; set; }
        public int Status { get; set; }
    }
    public class DeleteTerminationReasonObj : AdminObj
    {
        [CheckNumber(0, ErrorMessage = "Termination Id is required")]
        public int TerminationReasonId { get; set; }
    }



    #endregion

    #region Company

    public class RegCompanyObj : AdminObj
    {


        [Required(AllowEmptyStrings = false, ErrorMessage = "Company Name is required")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Company Name  must be between 3 and 200 characters")]
        public string Name { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Business Description is required")]
        [StringLength(300, MinimumLength = 15, ErrorMessage = "Business Description  must be between 15 and 300 characters")]
        public string BusinessDescription { get; set; }


        [Required(AllowEmptyStrings = true, ErrorMessage = "Company Email is required")]
        [StringLength(50)]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Company Address is required")]
        [StringLength(300)]
        public string Address { get; set; }
        public int CompanyType { get; set; }
        public int Status { get; set; }

    }

    public class EditCompanyObj : AdminObj
    {
        [CheckNumber(0, ErrorMessage = "Company is required")]
        public int CompanyId { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Company Name is required")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Company Name  must be between 3 and 200 characters")]
        public string Name { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Business Description is required")]
        [StringLength(300, MinimumLength = 15, ErrorMessage = "Business Description  must be between 15 and 300 characters")]
        public string BusinessDescription { get; set; }


        [Required(AllowEmptyStrings = true, ErrorMessage = "Company Email is required")]
        [StringLength(50)]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Company Address is required")]
        [StringLength(300)]
        public string Address { get; set; }
        public int CompanyType { get; set; }
        public int Status { get; set; }
    }
    public class DeleteCompanyObj : AdminObj
    {
        [CheckNumber(0, ErrorMessage = "Company Id is required")]
        public int CompanyId { get; set; }
    }


    #endregion


    #region Department


    public class RegDepartmentObj : AdminObj
    {

        [Required(AllowEmptyStrings = false, ErrorMessage = "Department Name is required")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Department Name  must be between 3 and 200 characters")]
        public string Name { get; set; }
        public int Status { get; set; }

    }

    public class EditDepartmentObj : AdminObj
    {
        [CheckNumber(0, ErrorMessage = "Department is required")]
        public int DepartmentId { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Department Name is required")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Department Name  must be between 3 and 200 characters")]
        public string Name { get; set; }
        public int Status { get; set; }
    }
    public class DeleteDepartmentObj : AdminObj
    {
        [CheckNumber(0, ErrorMessage = "Department Id is required")]
        public int DepartmentId { get; set; }
    }

    #endregion

    #endregion

    #region Staff Management

    #region Staff
    public class RegStaffObj : AdminObj
    {


        [Required(AllowEmptyStrings = false, ErrorMessage = "Last Name is required")]
        [StringLength(80, MinimumLength = 3, ErrorMessage = "Last Name  must be between 3 and 80 characters")]
        public string LastName { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "First Name is required")]
        [StringLength(80, MinimumLength = 3, ErrorMessage = "First Name  must be between 3 and 80 characters")]
        public string FirstName { get; set; }


        [Required(AllowEmptyStrings = true)]
        [StringLength(80)]
        public string MiddleName { get; set; }

        [CheckNumber(0, ErrorMessage = "Invalid Gender")]
        [Range(minimum: 1, maximum: 2, ErrorMessage = "Invalid Gender")]
        public int Gender { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Date of Birth is required")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Invalid Date of Birth")]
        public string DateOfBirth { get; set; }

      

        [Required(AllowEmptyStrings = false, ErrorMessage = "Company Email is required")]
        [StringLength(50)]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }


        [CheckNumber(0, ErrorMessage = "Invalid Employment Type")]
        public int EmploymentType { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Staff Mobile Number is required")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "Invalid Staff Mobile Number")]
        [CheckMobileNumber(ErrorMessage = "Invalid Mobile Number")]
        public string MobileNumber { get; set; }

        [CheckNumber(0, ErrorMessage = "Country of Origin Id is required")]
        public int CountryOfOriginId { get; set; }

        [CheckNumber(0, ErrorMessage = "State of Origin Id is required")]
        public int StateOfOriginId { get; set; }

        [CheckNumber(0, ErrorMessage = "Local Area of Origin Id is required")]
        public int LocalAreaId { get; set; }

        [CheckNumber(0, ErrorMessage = "Invalid Marital Status")]
        public int MaritalStatus { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Employment Date is required")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Invalid Employment Date")]
        public string EmploymentDate { get; set; }

        [CheckNumber(0, ErrorMessage = "Company Id is required")]
        public int CompanyId { get; set; }

        [CheckNumber(0, ErrorMessage = "Department Id is required")]
        public int DepartmentId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Username is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
        public string Username { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is required")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Password must be between 3 and 30 characters")]
        public string Password { get; set; }

    }

    public class EditStaffObj : AdminObj
    {



        [Required(AllowEmptyStrings = true, ErrorMessage = "Permanent Home Address is required")]
        [StringLength(200)]
        public string PermanentHomeAddress { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Company Email is required")]
        [StringLength(50)]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Staff Mobile Number is required")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "Invalid Staff Mobile Number")]
        public string MobileNumber { get; set; }

     

        [Required(AllowEmptyStrings = false, ErrorMessage = "Employment Type is required")]
        public int MaritalStatus { get; set; }

    }

    public class ChangePasswordObj : AdminObj
    {
        [StringLength(200)]
        [Required(ErrorMessage = "OldPasscode is required")]
        public string OldPassword { get; set; }

        [StringLength(200)]
        [Required(ErrorMessage = "Login Passcode is required")]
        public string NewPassword { get; set; }

    }
    public class EditStaffByAdminObj : AdminObj
    {
        [CheckNumber(0, ErrorMessage = "Staff is required")]
        public int StaffId { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Last Name is required")]
        [StringLength(80, MinimumLength = 3, ErrorMessage = "Last Name  must be between 3 and 80 characters")]
        public string LastName { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "First Name is required")]
        [StringLength(80, MinimumLength = 3, ErrorMessage = "First Name  must be between 3 and 80 characters")]
        public string FirstName { get; set; }


        [Required(AllowEmptyStrings = true)]
        [StringLength(80)]
        public string MiddleName { get; set; }

        [CheckNumber(0, ErrorMessage = "Invalid Gender")]
        [Range(1, 2, ErrorMessage = "Invalid Gender")]
        public int Gender { get; set; }




        [Required(AllowEmptyStrings = false, ErrorMessage = "Date of Birth is required")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Invalid Date of Birth")]
        public string DateOfBirth { get; set; }



        [CheckNumber(0, ErrorMessage = "Invalid Employment Type")]
        public int EmploymentType { get; set; }



        [CheckNumber(0, ErrorMessage = "Country of Origin Id is required")]
        public int CountryOfOriginId { get; set; }

        [CheckNumber(0, ErrorMessage = "State of Origin Id is required")]
        public int StateOfOriginId { get; set; }

        [CheckNumber(0, ErrorMessage = "Local Area of Origin Id is required")]
        public int LocalAreaId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Employment Type is required")]
        public int MaritalStatus { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Employment Date is required")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Invalid Employment Date")]
        public string EmploymentDate { get; set; }

        [CheckNumber(0, ErrorMessage = "Company Id is required")]
        public int CompanyId { get; set; }

        public int Status { get; set; }
    }

    public class EditStaffAccessObj : AdminObj
    {
        public int StaffAccessId { get; set; }

        public int StaffId { get; set; }


        [StringLength(11, MinimumLength = 11, ErrorMessage = "Sub Staff's Mobile Number must be 11 digits")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Sub Staff's Mobile Number is required")]
        [CheckMobileNumber(ErrorMessage = "Invalid Mobile Number")]
        [Index("UQ_Stake_MobileNo", IsUnique = true)]
        public string MobileNumber { get; set; }


        public bool IsApproved { get; set; }
        public bool IsLockedOut { get; set; }
        public int FailedPasswordAttemptCount { get; set; }


        [Required(AllowEmptyStrings = true)]
        [StringLength(10)]
        public string DateLockedOut { get; set; }


        [Required(AllowEmptyStrings = true)]
        [StringLength(5)]
        public string TimeLockedOut { get; set; }


        [Required(AllowEmptyStrings = true)]
        [StringLength(35)]
        public string LastLockedOutTimeStamp { get; set; }


        [Required(AllowEmptyStrings = true)]
        [StringLength(35)]
        public string LastReleasedTimeStamp { get; set; }


        [Required(AllowEmptyStrings = true)]
        [StringLength(35)]
        public string LastPasswordChangedTimeStamp { get; set; }


        [StringLength(200)]
        [Required(ErrorMessage = "User Code is required")]
        public string UserCode { get; set; }


        [StringLength(200)]
        [Required(ErrorMessage = "Access Code is required")]
        public string AccessCode { get; set; }


        [StringLength(200)]
        [Required(ErrorMessage = "Login Passcode is required")]
        public string Password { get; set; }

        [CheckNumber(0, ErrorMessage = "Company Id is required")]
        public int CompanyId { get; set; }

        [CheckNumber(0, ErrorMessage = "Department Id is required")]
        public int DepartmentId { get; set; }
    }

    #endregion

    #region StaffInsurance
    public class RegStaffInsuranceObj : AdminObj
    {



        [CheckNumber(0, ErrorMessage = "Insurance Policy Type is required")]
        public int InsurancePolicyTypeId { get; set; }

        [CheckNumber(0, ErrorMessage = "Staff is required")]
        public int StaffId { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Policy Number is required")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Policy Number  must be between 3 and 200 characters")]
        [Index("UQ_Ins_No", IsUnique = true)]
        public string PolicyNumber { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Insurer's Name is required")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Insurer's Name  must be between 2 and 200 characters")]
        public string Insurer { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Commencement Date is required")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Invalid Commencement Date")]
        public string CommencementDate { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Termination Date is required")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Invalid Termination Date")]
        public string TerminationDate { get; set; }
        public decimal PersonalContibution { get; set; }
        public decimal CompanyContibution { get; set; }

        public int Status { get; set; }


    }

    public class EditStaffInsuranceObj : AdminObj
    {
        public int StaffInsuranceId { get; set; }

        [CheckNumber(0, ErrorMessage = "Insurance Policy Type is required")]
        public int InsurancePolicyTypeId { get; set; }

        [CheckNumber(0, ErrorMessage = "Staff is required")]
        public int StaffId { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Policy Number is required")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Policy Number  must be between 3 and 200 characters")]
        [Index("UQ_Ins_No", IsUnique = true)]
        public string PolicyNumber { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Insurer's Name is required")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Insurer's Name  must be between 2 and 200 characters")]
        public string Insurer { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Commencement Date is required")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Invalid Commencement Date")]
        public string CommencementDate { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Termination Date is required")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Invalid Termination Date")]
        public string TerminationDate { get; set; }
        public decimal PersonalContibution { get; set; }
        public decimal CompanyContibution { get; set; }

        public int Status { get; set; }
    }

    public class DeleteStaffInsuranceObj : AdminObj
    {
        [CheckNumber(0, ErrorMessage = "Staff Insurance Id is required")]
        public int StaffInsuranceId { get; set; }
    }
    public class StaffInsuranceSearchObj : AdminObj
    {
        public int StaffId { get; set; }

        public int Status { get; set; }

        public string PolicyNumber { get; set; }
        public int StaffInsuranceId { get; set; }

    }
    #endregion

    #region StaffJobInfo
    public class RegStaffJobInfoObj : AdminObj
    {


        [CheckNumber(0, ErrorMessage = "Entrance Company Id is required")]
        public int EntranceCompanyId { get; set; }

        [CheckNumber(0, ErrorMessage = "Current Company Id is required")]
        public int CurrentCompanyId { get; set; }

        [CheckNumber(0, ErrorMessage = "Entrance Department Id is required")]
        public int EntranceDepartmentId { get; set; }

        [CheckNumber(0, ErrorMessage = "Current Department Id is required")]
        public int CurrentDepartmentId { get; set; }

        [CheckNumber(0, ErrorMessage = "Job Type Id is required")]
        public int JobTypeId { get; set; }

        [CheckNumber(0, ErrorMessage = "Job Position Id is required")]
        public int JobPositionId { get; set; }

        [CheckNumber(0, ErrorMessage = "Job Position Id is required")]
        public int JobLevelId { get; set; }

        [CheckNumber(0, ErrorMessage = "Job Specialization Id is required")]
        public int JobSpecializationId { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Job Title is required")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Job Title must be between 5 and 100 characters")]
        public string JobTitle { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Date Time Registered is required")]
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "Date Time Registered is required")]
        public string JobDescription { get; set; }


        public int SalaryGradeId { get; set; }
        public int SalaryLevelId { get; set; }

        public int TeamLeadId { get; set; }
        public int LineManagerId { get; set; }
    }

    public class EditStaffJobInfoObj : AdminObj
    {


        [CheckNumber(0, ErrorMessage = "Staff Id is required")]
        public int StaffId { get; set; }

        [CheckNumber(0, ErrorMessage = "Entrance Company Id is required")]
        public int EntranceCompanyId { get; set; }

        [CheckNumber(0, ErrorMessage = "Current Company Id is required")]
        public int CurrentCompanyId { get; set; }

        [CheckNumber(0, ErrorMessage = "Entrance Department Id is required")]
        public int EntranceDepartmentId { get; set; }

        [CheckNumber(0, ErrorMessage = "Current Department Id is required")]
        public int CurrentDepartmentId { get; set; }

        [CheckNumber(0, ErrorMessage = "Job Type Id is required")]
        public int JobTypeId { get; set; }

        [CheckNumber(0, ErrorMessage = "Job Position Id is required")]
        public int JobPositionId { get; set; }

        [CheckNumber(0, ErrorMessage = "Job Position Id is required")]
        public int JobLevelId { get; set; }

        [CheckNumber(0, ErrorMessage = "Job Specialization Id is required")]
        public int JobSpecializationId { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Job Title is required")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Job Title must be between 5 and 100 characters")]
        public string JobTitle { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Date Time Registered is required")]
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "Date Time Registered is required")]
        public string JobDescription { get; set; }


        public int SalaryGradeId { get; set; }
        public int SalaryLevelId { get; set; }

        public int TeamLeadId { get; set; }
        public int LineManagerId { get; set; }
    }

    public class DeleteStaffJobInfoObj : AdminObj
    {
        [CheckNumber(0, ErrorMessage = "Staff JobInfo Id is required")]
        public int StaffJobInfoId { get; set; }
    }
    public class StaffJobInfoSearchObj : AdminObj
    {
        public int StaffId { get; set; }

        public string JobTitle { get; set; }
        public int StaffJobInfoId { get; set; }

    }
    #endregion


    #region StaffMedical
    public class RegStaffMedicalObj : AdminObj
    {

        public int BloodGroup { get; set; }
        public int Genotype { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Medical Fitness Report is required")]
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "Medical Fitness Report must be between 10 and 2000 characters")]
        public string MedicalFitnessReport { get; set; }


        [Required(AllowEmptyStrings = true, ErrorMessage = "Ailment Information is required")]
        [StringLength(2000)]
        public string KnownAilment { get; set; }

    }

    public class EditStaffMedicalObj : AdminObj
    {
        [CheckNumber(0, ErrorMessage = "Staff Medical Id is required")]
        public int StaffMedicalId { get; set; }

        [CheckNumber(0, ErrorMessage = "Staff Id is required")]
        public int StaffId { get; set; }

        public int BloodGroup { get; set; }
        public int Genotype { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Medical Fitness Report is required")]
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "Medical Fitness Report must be between 10 and 2000 characters")]
        public string MedicalFitnessReport { get; set; }


        [Required(AllowEmptyStrings = true, ErrorMessage = "Ailment Information is required")]
        [StringLength(2000)]
        public string KnownAilment { get; set; }

    }

    public class DeleteStaffMedicalObj : AdminObj
    {
        [CheckNumber(0, ErrorMessage = "Staff Id is required")]
        public int staffId { get; set; }
    }
    public class StaffMedicalSearchObj : AdminObj
    {
        public int StaffId { get; set; }
        public int StaffMedicalId { get; set; }

    }

    #endregion

    #region Staff Key Performance Indicator
    public class RegStaffKPIIndexObj : AdminObj
    {

        public int StaffId { get; set; }
        public int KPIndexId { get; set; }
        

        [Required(AllowEmptyStrings = false, ErrorMessage = "Description is required")]
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "Description is too short or too long")]
        public string Description { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Start Date is required")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Start Date is required")]
        public string StartDate { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "End Date is required")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "End Date is required")]
        public string EndDate { get; set; }

        public decimal Rating { get; set; }


        [Required(AllowEmptyStrings = true, ErrorMessage = "Comment is required")]
        [StringLength(2000)]
        public string Comment { get; set; }


        [Required(AllowEmptyStrings = true, ErrorMessage = "Supervisor's is required")]
        [StringLength(2000)]
        public string SupervisorRemarks { get; set; }

        public int SupervisorId { get; set; }




    }

    public class EditStaffKPIIndexObj : AdminObj
    {

        [CheckNumber(0, ErrorMessage = "Staff Key Performance Indicator Id is required")]
        public int StaffKPIndexId { get; set; }

        [CheckNumber(0, ErrorMessage = "Staff Id is required")]
        public int StaffId { get; set; }
        public int KPIndexId { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Description is required")]
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "Description is too short or too long")]
        public string Description { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Start Date is required")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Start Date is required")]
        public string StartDate { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "End Date is required")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "End Date is required")]
        public string EndDate { get; set; }

        public decimal Rating { get; set; }


        [Required(AllowEmptyStrings = true, ErrorMessage = "Comment is required")]
        [StringLength(2000)]
        public string Comment { get; set; }


        [Required(AllowEmptyStrings = true, ErrorMessage = "Supervisor's is required")]
        [StringLength(2000)]
        public string SupervisorRemarks { get; set; }

        public int SupervisorId { get; set; }



    }

    public class DeleteStaffKPIIndexObj : AdminObj
    {
        [CheckNumber(0, ErrorMessage = "Staff Key Performance Indicator Id is required")]
        public int StaffKPIndexId { get; set; }
    }
    public class StaffKPIIndexSearchObj : AdminObj
    {
        public int StaffId { get; set; }
        public int StaffKPIndexId { get; set; }
        public int KPIndexId { get; set; }

        public string StartDate { get; set; }

        public string EndDate { get; set; }
    }

    #endregion

    #region StaffContact

    public class RegStaffContactObj : AdminObj
    {
        [CheckNumber(0, ErrorMessage = "Staff Id is required")]
        public int StaffId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Residential Address is required")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Residential Address  must be between 3 and 200 characters")]
        public string ResidentialAddress { get; set; }


        [Required(AllowEmptyStrings = true, ErrorMessage = "Town of Residence is required")]
        [StringLength(200)]
        public string TownOfResidence { get; set; }


        [Required(AllowEmptyStrings = true, ErrorMessage = "Location Landmark is required")]
        [StringLength(200)]
        public string LocationLandmark { get; set; }

        [CheckNumber(0, ErrorMessage = "State Of Origin Id  is required")]
        public int StateOfResidenceId { get; set; }

        [CheckNumber(0, ErrorMessage = "LGA Of Origin Id is required")]
        public int LocalAreaOfResidenceId { get; set; }

        public bool IsDefault { get; set; }

        public int Status { get; set; }

    }


    public class EditStaffContactObj : AdminObj
    {

        [CheckNumber(0, ErrorMessage = "Staff Id is required")]
        public int StaffId { get; set; }

        [CheckNumber(0, ErrorMessage = "Staff Contact Id  is required")]
        public int StaffContactId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Residential Address is required")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Residential Address  must be between 3 and 200 characters")]
        public string ResidentialAddress { get; set; }


        [Required(AllowEmptyStrings = true, ErrorMessage = "Town of Residence is required")]
        [StringLength(200)]
        public string TownOfResidence { get; set; }


        [Required(AllowEmptyStrings = true, ErrorMessage = "Location Landmark is required")]
        [StringLength(200)]
        public string LocationLandmark { get; set; }

        [CheckNumber(0, ErrorMessage = "State Of Origin Id  is required")]
        public int StateOfResidenceId { get; set; }

        [CheckNumber(0, ErrorMessage = "LGA Of Origin Id is required")]
        public int LocalAreaOfResidenceId { get; set; }

        public bool IsDefault { get; set; }

        public int Status { get; set; }

    }

    public class DeleteStaffContactObj : AdminObj
    {
        [CheckNumber(0, ErrorMessage = "Staff Contact Id is required")]
        public int StaffContactId { get; set; }
    }

    public class StaffContactSearchObj : AdminObj
    {
        public int StaffId { get; set; }
        public int StaffContactId { get; set; }
        public int Status { get; set; }
        public string ResidentialAddress { get; set; }
        public string TownOfResidence { get; set; }
        public string LocationLandmark { get; set; }
        public int StateOfResidenceId { get; set; }
        public int LocalAreaOfResidenceId { get; set; }
    }

    #endregion

    #region EmergencyContact


    public class RegEmergencyContactObj : AdminObj
    {


        [Required(AllowEmptyStrings = false, ErrorMessage = "Last Name is required")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Last Name  must be between 3 and 200 characters")]
        public string LastName { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "First Name is required")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "First Name  must be between 3 and 200 characters")]
        public string FirstName { get; set; }


        [Required(AllowEmptyStrings = true)]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Middle Name  must be between 3 and 200 characters")]
        public string MiddleName { get; set; }


        [CheckNumber(0, ErrorMessage = "Invalid Gender")]
        [Range(minimum: 1, maximum: 2, ErrorMessage = "Invalid Gender")]

        public int Gender { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Residential Address is required")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Residential Address  must be between 3 and 200 characters")]
        public string ResidentialAddress { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Mobile Number is required")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "Mobile Number  must be between 3 and 200 characters")]
        public string MobileNumber { get; set; }

        public bool IsDefault { get; set; }

        public int StateOfLocationId { get; set; }
        public int LocalAreaOfLocationId { get; set; }


    }

    public class EditEmergencyContactObj : AdminObj
    {
        [CheckNumber(0, ErrorMessage = "staff Id is required")]
        public int StaffId { get; set; }

        [CheckNumber(0, ErrorMessage = "Emergency Contact Id is required")]
        public int EmergencyContactId { get; set; }
        
        [Required(AllowEmptyStrings = false, ErrorMessage = "Last Name is required")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Last Name  must be between 3 and 200 characters")]
        public string LastName { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "First Name is required")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "First Name  must be between 3 and 200 characters")]
        public string FirstName { get; set; }


        [Required(AllowEmptyStrings = true)]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Middle Name  must be between 3 and 200 characters")]
        public string MiddleName { get; set; }


        [CheckNumber(0, ErrorMessage = "Invalid Gender")]
        [Range(minimum: 1, maximum: 2, ErrorMessage = "Invalid Gender")]

        public int Gender { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Residential Address is required")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Residential Address  must be between 3 and 200 characters")]
        public string ResidentialAddress { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Mobile Number is required")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "Mobile Number  must be between 3 and 200 characters")]
        public string MobileNumber { get; set; }

        public bool IsDefault { get; set; }

        public int StateOfLocationId { get; set; }
        public int LocalAreaOfLocationId { get; set; }



    }


    public class DeleteEmergencyContactObj : AdminObj
    {
        [CheckNumber(0, ErrorMessage = "Emergency Contact Id is required")]
        public int EmergencyContactId { get; set; }
    }

    public class EmergencyContactSearchObj : AdminObj
    {
        public int EmergencyContactId { get; set; }
        public int StaffId { get; set; }
        public int Status { get; set; }

        public string MobileNumber { get; set; }

        public string ResidentialAddress { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public int StateOfLocationId { get; set; }
        public int LocalAreaOfLocationId { get; set; }
        public bool IsDefault { get; set; }
    }

    #endregion


    #region Staff Bank Account


    public class RegStaffBankAccountObj : AdminObj
    {
        [CheckNumber(0, ErrorMessage = "Staff Id is required")]
        public int StaffId { get; set; }

        [CheckNumber(0, ErrorMessage = "Bank Id is required")]
        public int BankId { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Account Name is required")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Account Name must be between 5 and 100 characters")]
        public string AccountName { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Account Number is required")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Invalid Account Number")]

        public string AccountNumber { get; set; }

        public bool IsDefault { get; set; }

        public int Status { get; set; }


    }


    public class EditStaffBankAccountObj : AdminObj
    {
        [CheckNumber(0, ErrorMessage = "Bank Account Id is required")]
        public int StaffBankAccountId { get; set; }

        [CheckNumber(0, ErrorMessage = "Staff Id is required")]
        public int StaffId { get; set; }

        [CheckNumber(0, ErrorMessage = "Bank Id is required")]
        public int BankId { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Account Name is required")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Account Name must be between 5 and 100 characters")]
        public string AccountName { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Account Number is required")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Invalid Account Number")]

        public string AccountNumber { get; set; }

        public bool IsDefault { get; set; }

        public int Status { get; set; }
    }


    public class DeleteStaffBankAccountObj : AdminObj
    {
        [CheckNumber(0, ErrorMessage = "Bank Account Id is required")]
        public int StaffBankAccountId { get; set; }
    }



    public class StaffBankAccountSearchObj : AdminObj
    {

        public int StaffBankAccountId { get; set; }
        public int StaffId { get; set; }
        public int BankId { get; set; }
        public int Status { get; set; }
        public string AccountName { get; set; }

        public string AccountNumber { get; set; }
    }

    #endregion


    #region StaffPension


    public class RegStaffPensionObj : AdminObj
    {


        [CheckNumber(0, ErrorMessage = "Staff Id is required")]
        public int StaffId { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Pension Number is required")]
        [StringLength(15, ErrorMessage = "Pension Number must not exceed 15 characters")]
        [Index("IX_PenKey", IsUnique = true)]
        public string PensionNumber { get; set; }

        public decimal CompanyContribution { get; set; }

        public decimal PersonalContribution { get; set; }

        public int PensionAdministratorId { get; set; }


    }


    public class EditStaffPensionObj : AdminObj
    {
        [CheckNumber(0, ErrorMessage = "Staff Pension Id is required")]
        public int StaffPensionId { get; set; }

        [CheckNumber(0, ErrorMessage = "Staff Id is required")]
        public int StaffId { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Pension Number is required")]
        [StringLength(15, ErrorMessage = "Pension Number must not exceed 15 characters")]
        [Index("IX_PenKey", IsUnique = true)]
        public string PensionNumber { get; set; }

        public decimal CompanyContribution { get; set; }

        public decimal PersonalContribution { get; set; }

        public int PensionAdministratorId { get; set; }



    }


    public class DeleteStaffPensionObj : AdminObj
    {
        [CheckNumber(0, ErrorMessage = "Staff Pension Id is required")]
        public int StaffPensionId { get; set; }
    }

    public class StaffPensionSearchObj : AdminObj
    {

        public int StaffPensionId { get; set; }
        public int StaffId { get; set; }
        public string PensionNumber { get; set; }
    }

    #endregion

    #region Next Of Kin

    public class RegStaffNextOfKinObj : AdminObj
    {


        public int StateOfLocationId { get; set; }
        public int LocalAreaOfLocationId { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Last Name is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Last Name  must be between 3 and 50 characters")]
        public string LastName { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "First Name is required")]
        [StringLength(2050, MinimumLength = 3, ErrorMessage = "First Name  must be between 3 and 50 characters")]
        public string FirstName { get; set; }


        [Required(AllowEmptyStrings = true)]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Middle Name  must be between 3 and 50 characters")]
        public string MiddleName { get; set; }

        public int Gender { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Residential Address is required")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Residential Address  must be between 3 and 200 characters")]
        public string ResidentialAddress { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Mobile Number is required")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "Mobile Number  must be between 3 and 200 characters")]
        public string MobileNumber { get; set; }



        [Required(AllowEmptyStrings = false, ErrorMessage = "Next of Kin Email Address is required")]
        [StringLength(50)]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        public int Relationship { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Land phone Number is required")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "Land phone Number  must be between 3 and 200 characters")]
        public string Landphone { get; set; }

        public int MaritalStatus { get; set; }




    }

    public class EditStaffNextOfKinObj : AdminObj
    {

        [CheckNumber(0, ErrorMessage = "Staff Next Of Kin Id is required")]
        public int StaffNextOfKinId { get; set; }
        [CheckNumber(0, ErrorMessage = "Staff Id is required")]
        public int StaffId { get; set; }

        public int StateOfLocationId { get; set; }
        public int LocalAreaOfLocationId { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Last Name is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Last Name  must be between 3 and 50 characters")]
        public string LastName { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "First Name is required")]
        [StringLength(2050, MinimumLength = 3, ErrorMessage = "First Name  must be between 3 and 50 characters")]
        public string FirstName { get; set; }


        [Required(AllowEmptyStrings = true)]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Middle Name  must be between 3 and 50 characters")]
        public string MiddleName { get; set; }

        public int Gender { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Residential Address is required")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Residential Address  must be between 3 and 200 characters")]
        public string ResidentialAddress { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Mobile Number is required")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "Mobile Number  must be between 3 and 200 characters")]
        public string MobileNumber { get; set; }



        [Required(AllowEmptyStrings = false, ErrorMessage = "Next of Kin Email Address is required")]
        [StringLength(50)]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        public int Relationship { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Mobile Number is required")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "Mobile Number  must be between 3 and 200 characters")]
        public string Landphone { get; set; }

        public int MaritalStatus { get; set; }



    }


    public class DeleteStaffNextOfKinObj : AdminObj
    {
        [CheckNumber(0, ErrorMessage = "Staff Next Of Kin Id is required")]
        public int StaffNextOfKinId { get; set; }
    }

    public class StaffNextOfKinSearchObj : AdminObj
    {
        public int StaffNextOfKinId { get; set; }
        public int StaffId { get; set; }
        public int Status { get; set; }
        public string MobileNumber { get; set; }
        public string ResidentialAddress { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public int StateOfLocationId { get; set; }
        public int LocalAreaOfLocationId { get; set; }

    }

    #endregion

    #region Educational Qualification


    public class RegEducationalQualificationObj : AdminObj
    {


        [CheckNumber(0, ErrorMessage = "Invalid Discipline")]
        public int DisciplineId { get; set; }

        [CheckNumber(0, ErrorMessage = "Invalid Course of Study")]
        public int CourseOfStudyId { get; set; }

        [CheckNumber(0, ErrorMessage = "Invalid Qualification")]
        public int QualificationId { get; set; }

        [CheckNumber(0, ErrorMessage = "Invalid Class of Award")]
        public int ClassOfAwardId { get; set; }

        [CheckNumber(0, ErrorMessage = "Invalid Institution")]
        public int InstitutionId { get; set; }


        [CheckNumber(1979, ErrorMessage = "Invalid Start Year")]
        public int StartYear { get; set; }

        [CheckNumber(1979, ErrorMessage = "Invalid End Year")]
        public int EndYear { get; set; }

        [Required(AllowEmptyStrings = true, ErrorMessage = "CGPA is required")]
        [StringLength(5)]
        public string CGPA { get; set; }
        public int GradeScale { get; set; }



    }

    public class EditEducationalQualificationObj : AdminObj
    {

        [CheckNumber(0, ErrorMessage = "Higher Education Id is required")]
        public long HigherEducationId { get; set; }



        [CheckNumber(0, ErrorMessage = "Invalid Discipline")]
        public int DisciplineId { get; set; }

        [CheckNumber(0, ErrorMessage = "Invalid Course of Study")]
        public int CourseOfStudyId { get; set; }

        [CheckNumber(0, ErrorMessage = "Invalid Qualification")]
        public int QualificationId { get; set; }

        [CheckNumber(0, ErrorMessage = "Invalid Class of Award")]
        public int ClassOfAwardId { get; set; }

        [CheckNumber(0, ErrorMessage = "Invalid Institution")]
        public int InstitutionId { get; set; }


        [CheckNumber(1979, ErrorMessage = "Invalid Start Year")]
        public int StartYear { get; set; }

        [CheckNumber(1979, ErrorMessage = "Invalid End Year")]
        public int EndYear { get; set; }


        [Required(AllowEmptyStrings = true, ErrorMessage = "Specified Institution is required")]
        [StringLength(200)]
        public string SpecifiedInstitution { get; set; }


        [Required(AllowEmptyStrings = true, ErrorMessage = "Specified Discipline is required")]
        [StringLength(100)]
        public string SpecifiedDiscipline { get; set; }


        [Required(AllowEmptyStrings = true, ErrorMessage = "Specified Course of Study is required")]
        [StringLength(100)]
        public string SpecifiedCourseOfStudy { get; set; }


        [Required(AllowEmptyStrings = true, ErrorMessage = "CGPA is required")]
        [StringLength(5)]
        public string CGPA { get; set; }

        [CheckNumber(0, ErrorMessage = "Grade Scale is Required")]
        public int GradeScale { get; set; }




    }


    public class DeleteEducationalQualificationObj : AdminObj
    {
        [CheckNumber(0, ErrorMessage = "Higher EducationId is required")]
        public long HigherEducationId { get; set; }
    }

    public class EducationalQualificationSearchObj : AdminObj
    {
        public int HigherEducationId { get; set; }
        public int StaffId { get; set; }
        public int DisciplineId { get; set; }
        public int CourseOfStudyId { get; set; }
        public int QualificationId { get; set; }
        public int ClassOfAwardId { get; set; }
        public int InstitutionId { get; set; }

    }



    #endregion

    #region ProfessionalMemberShip Body

    public class RegProfessionalMemberShipObj : AdminObj
    {


        [CheckNumber(0, ErrorMessage = "Invalid Professional Body Information")]
        public int ProfessionalBodyId { get; set; }

        [CheckNumber(1979, ErrorMessage = "Invalid Year Joined")]
        public int YearJoined { get; set; }

        public int ProfessionalMembershipTypeId { get; set; }



    }

    public class EditProfessionalMemberShipObj : AdminObj
    {
        [CheckNumber(0, ErrorMessage = "Invalid Professional Membership Id Information")]
        public long ProfessionalMembershipId { get; set; }

        [CheckNumber(0, ErrorMessage = "Invalid Professional Body Information")]
        public int ProfessionalBodyId { get; set; }

        [CheckNumber(1979, ErrorMessage = "Invalid Year Joined")]
        public int YearJoined { get; set; }

        public int ProfessionalMembershipTypeId { get; set; }
    }
    public class DeleteProfessionalMemberShipObj : AdminObj
    {
        [CheckNumber(0, ErrorMessage = "Professional MemberShip Id is required")]
        public long ProfessionalMembershipId { get; set; }
    }
    public class ProfessionalMemberShipSearchObj : AdminObj
    {
        public int ProfessionalMembershipId { get; set; }
        public int StaffId { get; set; }
        public int ProfessionalBodyId { get; set; }

        public int ProfessionalMembershipTypeId { get; set; }
    }
    #endregion

    #region LeaveRequest

    public class RegLeaveRequestObj : AdminObj
    {


        [CheckNumber(0, ErrorMessage = "Staff Id is required")]
        public int StaffId { get; set; }

        [CheckNumber(0, ErrorMessage = "Work flow Order Id is required")]
        public int WorkflowOrderId { get; set; }
        

        [CheckNumber(0, ErrorMessage = "Department Id is required")]
        public int DepartmentId { get; set; }

        [CheckNumber(0, ErrorMessage = "Company Id is required")]
        public int CompanyId { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Leave Title is required")]
        [StringLength(300, MinimumLength = 8, ErrorMessage = "Leave Title  must be between 8 and 300 characters")]
        public string LeaveTitle { get; set; }

        public int LeaveTypeId { get; set; }



        [Required(AllowEmptyStrings = false, ErrorMessage = "Proposed Start Date is required")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Invalid Proposed Start Date")]
        public string ProposedStartDate { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Proposed End Date is required")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Invalid Proposed End Date")]
        public string ProposedEndDate { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Leave Purpose is required")]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "Purpose of leave is too short or too long")]
        public string Purpose { get; set; }


        [StringLength(300)]
        public string OtherRemarks { get; set; }

     

    }

    public class EditLeaveRequestObj : AdminObj
    {
        [CheckNumber(0, ErrorMessage = "Leave Request Id is required")]
        public long LeaveRequestId { get; set; }

        [CheckNumber(0, ErrorMessage = "Staff Id is required")]
        public int StaffId { get; set; }


        [CheckNumber(0, ErrorMessage = "Department Id is required")]
        public int DepartmentId { get; set; }

        [CheckNumber(0, ErrorMessage = "Company Id is required")]
        public int CompanyId { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Leave Title is required")]
        [StringLength(300, MinimumLength = 8, ErrorMessage = "Leave Title  must be between 8 and 300 characters")]
        public string LeaveTitle { get; set; }

        public int LeaveTypeId { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Proposed Start Date is required")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Invalid Proposed Start Date")]
        public string ProposedStartDate { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Proposed End Date is required")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Invalid Proposed End Date")]
        public string ProposedEndDate { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Leave Purpose is required")]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "Purpose of leave is too short or too long")]
        public string Purpose { get; set; }


        [StringLength(300)]
        public string OtherRemarks { get; set; }

    }

    public class DeleteLeaveRequestObj : AdminObj
    {
        [CheckNumber(0, ErrorMessage = "Leave Request Id is required")]
        public long LeaveRequestId { get; set; }
    }
    public class LeaveRequestSearchObj : AdminObj
    {
        public string LeaveTitle { get; set; }
        public int LeaveType { get; set; }

        public int Status { get; set; }
        public int StaffId { get; set; }
        public long LeaveRequestId { get; set; }


    }
    #endregion

    #region StaffLeave

    public class RegStaffLeaveObj : AdminObj
    {
        [CheckNumber(0, ErrorMessage = "Staff Id is required")]
        public int StaffId { get; set; }

        [CheckNumber(0, ErrorMessage = "Staff Leave Request Id is required")]
        public int LeaveRequestId { get; set; }
        public int Status { get; set; }


    }

    public class EditStaffLeaveObj : AdminObj
    {
        [CheckNumber(0, ErrorMessage = "Staff Leave Id is required")]
        public long StaffLeaveId { get; set; }

        [CheckNumber(0, ErrorMessage = "Staff Id is required")]
        public int StaffId { get; set; }

        [Required(AllowEmptyStrings = true, ErrorMessage = "HOD's Comment is required")]
        [StringLength(500)]
        public string HODComment { get; set; }


        [Required(AllowEmptyStrings = true, ErrorMessage = "Line Manager's Comment is required")]
        [StringLength(500)]
        public string LMComment { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "HR's Comment is required")]
        [StringLength(500)]
        public string HRComment { get; set; }


        [Required(AllowEmptyStrings = true, ErrorMessage = "HOD's Approved Start Date is required")]
        [StringLength(10)]
        public string HODApprovedStartDate { get; set; }


        [Required(AllowEmptyStrings = true, ErrorMessage = "HOD's Approved End Date is required")]
        [StringLength(10)]
        public string HODApprovedEndDate { get; set; }



        [Required(AllowEmptyStrings = true, ErrorMessage = "Line Manager's Approved Start Date is required")]
        [StringLength(10)]
        public string LMApprovedStartDate { get; set; }


        [Required(AllowEmptyStrings = true, ErrorMessage = "Line Manager's Approved End Date is required")]
        [StringLength(10)]
        public string LMApprovedEndDate { get; set; }



        [Required(AllowEmptyStrings = true, ErrorMessage = "HR's Approved Start Date is required")]
        [StringLength(10)]
        public string HRApprovedStartDate { get; set; }


        [Required(AllowEmptyStrings = true, ErrorMessage = "HR's Approved End Date is required")]
        [StringLength(10)]
        public string HRApprovedEndDate { get; set; }
        public int Status { get; set; }

    }
    public class ApproveStaffLeaveObj : AdminObj
    {

        [CheckNumber(0, ErrorMessage = "Staff Leave Id is required")]
        public long StaffLeaveId { get; set; }

        public int HODApprovedBy { get; set; }

        public int LMApprovedBy { get; set; }


        [Required(AllowEmptyStrings = true, ErrorMessage = "HOD's Comment is required")]
        [StringLength(500)]
        public string HODComment { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "HR's Comment is required")]
        [StringLength(500)]
        public string HRComment { get; set; }

        [Required(AllowEmptyStrings = true, ErrorMessage = "HR's Approved Start Date is required")]
        [StringLength(10)]
        public string HRApprovedStartDate { get; set; }


        [Required(AllowEmptyStrings = true, ErrorMessage = "HR's Approved End Date is required")]
        [StringLength(10)]
        public string HRApprovedEndDate { get; set; }



        [Required(AllowEmptyStrings = true, ErrorMessage = "HOD's Approved Start Date is required")]
        [StringLength(10)]
        public string HODApprovedStartDate { get; set; }


        [Required(AllowEmptyStrings = true, ErrorMessage = "HOD's Approved End Date is required")]
        [StringLength(10)]
        public string HODApprovedEndDate { get; set; }

        [Required(AllowEmptyStrings = true, ErrorMessage = "Line Manager's Comment is required")]
        [StringLength(500)]
        public string LMComment { get; set; }

        [Required(AllowEmptyStrings = true, ErrorMessage = "Line Manager's Approved Start Date is required")]
        [StringLength(10)]
        public string LMApprovedStartDate { get; set; }


        [Required(AllowEmptyStrings = true, ErrorMessage = "Line Manager's Approved End Date is required")]
        [StringLength(10)]
        public string LMApprovedEndDate { get; set; }

        [CheckNumber(0, ErrorMessage = "Workflow Order Item is required")]
        public int WorkflowOrderItemId { get; set; }
        public bool IsHODApproval { get; set; }
        public bool IsHRApproval { get; set; }
        public bool IsLMApproval { get; set; }
        public int Status { get; set; }

    }
    public class DeleteStaffLeaveObj : AdminObj
    {
        [CheckNumber(0, ErrorMessage = "Staff Leave Id is required")]
        public long StaffLeaveId { get; set; }
    }
    public class StaffLeaveSearchObj : AdminObj
    {
        public int LeaveRequestId { get; set; }
        public int LeaveTypeId { get; set; }
        public int StaffId { get; set; }
        public long StaffLeaveId { get; set; }
        public int Status { get; set; }

    }

    public class WorkFlowLogSearchObj : AdminObj
    {

        public long WorkflowLogId { get; set; }
        public int ApprovalType { get; set; }
        public int StaffId { get; set; }
        public int ProcessorId { get; set; }
        public int Status { get; set; }
     
    }
    #endregion

    #region StaffMemo

    public class RegStaffMemoObj : AdminObj
    {
        [CheckNumber(0, ErrorMessage = "Staff Id is required")]
        public int StaffId { get; set; }

        [CheckNumber(0, ErrorMessage = "Work flow Order Id is required")]
        public int WorkflowOrderId { get; set; }

        public int MemoType { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Leave Purpose is required")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Leave Purpose  must be between 5 and 200 characters")]
        public string Title { get; set; }

        public int RegisterBy { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Memo Detail is required")]
        [StringLength(2000, MinimumLength = 20, ErrorMessage = "Memo Detail  must be between 20 and 2000 characters")]
        public string MemoDetail { get; set; }


    }
    public class RegBulkStaffMemoObj : AdminObj
    {
       
        public List<RegBulkStaffMemoItemObj> StaffMemoItem { get; set; }

    }
    public class RegBulkStaffMemoItemObj 
    {
       

        [CheckNumber(0, ErrorMessage = "Staff Id is required")]
        public int StaffId { get; set; }

        [CheckNumber(0, ErrorMessage = "Work flow Order Id is required")]
        public int WorkflowOrderId { get; set; }

        [CheckNumber(0, ErrorMessage = "Work flow Source Id is required")]
        public long WorkflowSourceId { get; set; }
        public int MemoType { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Leave Purpose is required")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Leave Purpose  must be between 5 and 200 characters")]
        public string Title { get; set; }

        public int RegisterBy { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Memo Detail is required")]
        [StringLength(2000, MinimumLength = 20, ErrorMessage = "Memo Detail  must be between 20 and 2000 characters")]
        public string MemoDetail { get; set; }


    }
    public class ApprovaStaffMemoObj : AdminObj
    {
        [CheckNumber(0, ErrorMessage = "Staff Memo Id is required")]
        public int StaffMemoId { get; set; }

        [CheckNumber(0, ErrorMessage = "Workflow Order Item is required")]
        public int WorkflowOrderItemId { get; set; }

        [CheckNumber(0, ErrorMessage = "Staff Id is required")]
        public int StaffId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Memo Detail is required")]
        [StringLength(2000, MinimumLength = 20, ErrorMessage = "Memo Detail  must be between 20 and 2000 characters")]
        public string MemoDetail { get; set; }

        public int ApprovedBy { get; set; }
        public int Status { get; set; }

        public int MemoType { get; set; }


    }

    public class DeleteStaffMemoObj : AdminObj
    {
        [CheckNumber(0, ErrorMessage = "Staff Memo Id is required")]
        public int StaffMemoId { get; set; }
    }
    public class StaffMemoSearchObj : AdminObj
    {

        public int MemoType { get; set; }
        public int StaffId { get; set; }
        public int StaffMemoId { get; set; }
        public int Status { get; set; }

    }
    #endregion

    #region StaffSalary

    public class RegStaffSalaryObj : AdminObj
    {

        public int StaffId { get; set; }

        public int Currency { get; set; }

       
        public decimal BasicAllowance { get; set; }
        public decimal HousingAllowance { get; set; }
        public decimal EducationAllowance { get; set; }
        public decimal FurnitureAllowance { get; set; }
        public decimal WardrobeAllowance { get; set; }
        public decimal TransportAllowance { get; set; }
        public decimal LeaveAllowance { get; set; }
        public decimal EntertainmentAllowance { get; set; }

        public decimal PensionDeduction { get; set; }
        public decimal PayeDeduction { get; set; }
        public decimal InsuranceDeduction { get; set; }

        public decimal TotalPayment { get; set; }
        public decimal TotalDeduction { get; set; }
        public int Status { get; set; }
    }

    public class UpdateStaffSalaryObj : AdminObj
    {
        public int StaffSalaryId { get; set; }
        public int StaffId { get; set; }
        public int StaffJobInfoId { get; set; }
        public int Currency { get; set; }
        public decimal BasicAllowance { get; set; }
        public decimal HousingAllowance { get; set; }
        public decimal EducationAllowance { get; set; }
        public decimal FurnitureAllowance { get; set; }
        public decimal WardrobeAllowance { get; set; }
        public decimal TransportAllowance { get; set; }
        public decimal LeaveAllowance { get; set; }
        public decimal EntertainmentAllowance { get; set; }

        public decimal PensionDeduction { get; set; }
        public decimal PayeDeduction { get; set; }
        public decimal InsuranceDeduction { get; set; }

        public decimal TotalPayment { get; set; }
        public decimal TotalDeduction { get; set; }

        public int Status { get; set; }

    }

    public class StaffLoginObj
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Staff's Username (Email) is required")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Invalid Email Address")]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Invalid Email Address")]
        public string Username { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Staff's Login Password is required")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Invalid Login Password")]
        public string Password { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Staff's Login Source IP is required")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Invalid Login Source IP")]
        public string LoginSourceIP { get; set; }

    }

    public class StaffSalarySearchObj : AdminObj
    {


        public int StaffId { get; set; }
        public int StaffSalaryId { get; set; }
        public int Status { get; set; }

    }
    #endregion

    #region StaffMemoResponse

    public class RegStaffMemoResponseObj : AdminObj
    {
        [CheckNumber(0, ErrorMessage = "Staff Id is required")]
        public int StaffId { get; set; }

      

        [Required(AllowEmptyStrings = false, ErrorMessage = "Memo Responese is required")]
        [StringLength(1000, MinimumLength = 8, ErrorMessage = "Memo Responese  must be between 8 and 500 characters")]
        public string MemoResponse { get; set; }

        
    }

    public class ApprovaStaffMemoResponseObj : AdminObj
    {

        [CheckNumber(0, ErrorMessage = "Staff Id is required")]
        public int StaffId { get; set; }

        [CheckNumber(0, ErrorMessage = "Workflow Order Item is required")]
        public int WorkflowOrderItemId { get; set; }

        [Required(AllowEmptyStrings = true, ErrorMessage = "Management's Remarks is required")]
        [StringLength(500)]
        public string ManagementRemarks { get; set; }


        [Required(AllowEmptyStrings = true, ErrorMessage = "Management's Remarks Date - Time is required")]
        [StringLength(35)]
        public string ManagementRemarkTimeStamp { get; set; }

        public int ManagementRemarksBy { get; set; }

        public int Status { get; set; }
    }
    public class StaffMemoResponseSearchObj : AdminObj
    {


        public int StaffId { get; set; }
        public int StaffMemoId { get; set; }
        public int StaffMemoResponseId { get; set; }


    }
    #endregion

    #region Comment

    public class RegCommentObj : AdminObj
    {

        public int CommentId { get; set; }
        public int StaffId { get; set; }

      
        [Required(AllowEmptyStrings = false, ErrorMessage = "Comment Details is required")]
        [StringLength(500, MinimumLength = 2, ErrorMessage = "Comment Details must be between 2 and 500 characters")]
        public string CommentDetails { get; set; }

        public int CommentType { get; set; }

    }
    public class CommentSearchObj : AdminObj
    {
        public int StaffId { get; set; }
        public int CommentId { get; set; }
     
    }
    #endregion

    #endregion


}
