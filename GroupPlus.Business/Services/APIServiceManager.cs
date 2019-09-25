using GroupPlus.Business.DataManager;
using GroupPlus.Business.Repository.Common;
using GroupPlus.Business.Repository.CompanyManagement;
using GroupPlus.Business.Repository.StaffManagement;
using GroupPlus.BusinessContract.CommonAPIs;


namespace GroupPlus.Business.Services
{
    public class APIServiceManager
    {
        #region Migration

        //public static bool Migrate(out string msg)
        //{
        //    return MigrationService.Migrate(out msg);
        //}

        #endregion

        #region Common LoookUps

     

      
        #region Bank

        public static SettingRegRespObj AddBank(RegBankObj regObj)
        {
            return new BankRepository().AddBank(regObj);
        }

        public static SettingRegRespObj UpdateBank(EditBankObj regObj)
        {
            return new BankRepository().UpdateBank(regObj);
        }

        public static SettingRegRespObj DeleteBank(DeleteBankObj regObj)
        {
            return new BankRepository().DeleteBank(regObj);
        }

        public static BankRespObj LoadBanks(CommonSettingSearchObj regObj)
        {
            return new BankRepository().LoadBanks(regObj);
        }

        #endregion

        #region WorkFlowOrder

        public static SettingRegRespObj AddWorkflowOrder(RegWorkFlowOrderObj regObj)
        {
            return new WorkFlowOrderRepository().AddWorkflowOrder(regObj);
        }

        public static SettingRegRespObj UpdateWorkflowOrder(EditWorkFlowOrderObj regObj)
        {
            return new WorkFlowOrderRepository().UpdateWorkflowOrder(regObj);
        }

        public static SettingRegRespObj DeleteWorkflowOrder(DeleteWorkFlowOrderObj regObj)
        {
            return new WorkFlowOrderRepository().DeleteWorkflowOrder(regObj);
        }

        public static WorkFlowOrderRespObj LoadWorkflowOrders(CommonSettingSearchObj regObj)
        {
            return new WorkFlowOrderRepository().LoadWorkflowOrders(regObj);
        }

        #endregion

        #region WorkFlowOrderItem

        public static SettingRegRespObj AddWorkflowOrderItem(RegWorkFlowOrderItemObj regObj)
        {
            return new WorkflowOrderItemRepository().AddWorkflowOrderItem(regObj);
        }

        public static SettingRegRespObj UpdateWorkflowOrderItem(EditWorkFlowOrderItemObj regObj)
        {
            return new WorkflowOrderItemRepository().UpdateWorkflowOrderItem(regObj);
        }

        public static SettingRegRespObj DeleteWorkflowOrderItem(DeleteWorkFlowOrderItemObj regObj)
        {
            return new WorkflowOrderItemRepository().DeleteWorkflowOrderItem(regObj);
        }

        public static WorkFlowOrderItemRespObj LoadWorkflowOrderItems(CommonSettingSearchObj regObj)
        {
            return new WorkflowOrderItemRepository().LoadWorkflowOrderItems(regObj);
        }

        #endregion

        #region Discipline

        public static SettingRegRespObj AddDiscipline(RegDisciplineObj regObj)
        {
            return new DisciplineRepository().AddDiscipline(regObj);
        }

        public static SettingRegRespObj UpdateDiscipline(EditDisciplineObj regObj)
        {
            return new DisciplineRepository().UpdateDiscipline(regObj);
        }

        public static SettingRegRespObj DeleteDiscipline(DeleteDisciplineObj regObj)
        {
            return new DisciplineRepository().DeleteDiscipline(regObj);
        }

        public static DisciplineRespObj LoadDisciplines(CommonSettingSearchObj regObj)
        {
            return new DisciplineRepository().LoadDisciplines(regObj);
        }

        #endregion

        #region Class Of Award

        public static SettingRegRespObj AddClassOfAward(RegClassOfAwardObj regObj)
        {
            return new ClassOfAwardRepository().AddClassOfAward(regObj);
        }

        public static SettingRegRespObj UpdateClassOfAward(EditClassOfAwardObj regObj)
        {
            return new ClassOfAwardRepository().UpdateClassOfAward(regObj);
        }

        public static SettingRegRespObj DeleteClassOfAward(DeleteClassOfAwardObj regObj)
        {
            return new ClassOfAwardRepository().DeleteClassOfAward(regObj);
        }

        public static ClassOfAwardRespObj LoadClassOfAwards(CommonSettingSearchObj regObj)
        {
            return new ClassOfAwardRepository().LoadClassOfAwards(regObj);
        }

        #endregion

        #region Course of Study

        public static SettingRegRespObj AddCourseOfStudy(RegCourseOfStudyObj regObj)
        {
            return new CourseOfStudyRepository().AddCourseOfStudy(regObj);
        }

        public static SettingRegRespObj UpdateCourseOfStudy(EditCourseOfStudyObj regObj)
        {
            return new CourseOfStudyRepository().UpdateCourseOfStudy(regObj);
        }

        public static SettingRegRespObj DeleteCourseOfStudy(DeleteCourseOfStudyObj regObj)
        {
            return new CourseOfStudyRepository().DeleteCourseOfStudy(regObj);
        }

        public static CourseOfStudyRespObj LoadCourseOfStudys(CommonSettingSearchObj regObj)
        {
            return new CourseOfStudyRepository().LoadCourseOfStudys(regObj);
        }

        #endregion

        #region Institution

        public static SettingRegRespObj AddInstitution(RegInstitutionObj regObj)
        {
            return new InstitutionRepository().AddInstitution(regObj);
        }

        public static SettingRegRespObj UpdateInstitution(EditInstitutionObj regObj)
        {
            return new InstitutionRepository().UpdateInstitution(regObj);
        }

        public static SettingRegRespObj DeleteInstitution(DeleteInstitutionObj regObj)
        {
            return new InstitutionRepository().DeleteInstitution(regObj);
        }

        public static InstitutionRespObj LoadInstitutions(CommonSettingSearchObj regObj)
        {
            return new InstitutionRepository().LoadInstitutions(regObj);
        }

        #endregion

        #region Insuance Policy

        public static SettingRegRespObj AddInsurancePolicyType(RegInsurancePolicyTypeObj regObj)
        {
            return new InsurancePolicyTypeRepository().AddInsurancePolicyType(regObj);
        }

        public static SettingRegRespObj UpdateInsurancePolicyType(EditInsurancePolicyTypeObj regObj)
        {
            return new InsurancePolicyTypeRepository().UpdateInsurancePolicyType(regObj);
        }

        public static SettingRegRespObj DeleteInsurancePolicyType(DeleteInsurancePolicyTypeObj regObj)
        {
            return new InsurancePolicyTypeRepository().DeleteInsurancePolicyType(regObj);
        }

        public static InsurancePolicyTypeRespObj LoadInsurancePolicyTypes(CommonSettingSearchObj regObj)
        {
            return new InsurancePolicyTypeRepository().LoadInsurancePolicyTypes(regObj);
        }

        #endregion

        #region Job Position

        public static SettingRegRespObj AddJobPosition(RegJobPositionObj regObj)
        {
            return new JobPositionRepository().AddJobPosition(regObj);
        }

        public static SettingRegRespObj UpdateJobPosition(EditJobPositionObj regObj)
        {
            return new JobPositionRepository().UpdateJobPosition(regObj);
        }

        public static SettingRegRespObj DeleteJobPosition(DeleteJobPositionObj regObj)
        {
            return new JobPositionRepository().DeleteJobPosition(regObj);
        }

        public static JobPositionRespObj LoadJobPositions(CommonSettingSearchObj regObj)
        {
            return new JobPositionRepository().LoadJobPositions(regObj);
        }

        #endregion

        #region Job Specialization

        public static SettingRegRespObj AddJobSpecialization(RegJobSpecializationObj regObj)
        {
            return new JobSpecializationRepository().AddJobSpecialization(regObj);
        }

        public static SettingRegRespObj UpdateJobSpecialization(EditJobSpecializationObj regObj)
        {
            return new JobSpecializationRepository().UpdateJobSpecialization(regObj);
        }

        public static SettingRegRespObj DeleteJobSpecialization(DeleteJobSpecializationObj regObj)
        {
            return new JobSpecializationRepository().DeleteJobSpecialization(regObj);
        }

        public static JobSpecializationRespObj LoadJobSpecializations(CommonSettingSearchObj regObj)
        {
            return new JobSpecializationRepository().LoadJobSpecializations(regObj);
        }

        #endregion

        #region Job Type

        public static SettingRegRespObj AddJobType(RegJobTypeObj regObj)
        {
            return new JobTypeRepository().AddJobType(regObj);
        }

        public static SettingRegRespObj UpdateJobType(EditJobTypeObj regObj)
        {
            return new JobTypeRepository().UpdateJobType(regObj);
        }

        public static SettingRegRespObj DeleteJobType(DeleteJobTypeObj regObj)
        {
            return new JobTypeRepository().DeleteJobType(regObj);
        }

        public static JobTypeRespObj LoadJobTypes(CommonSettingSearchObj regObj)
        {
            return new JobTypeRepository().LoadJobTypes(regObj);
        }

        #endregion

        #region Job Level

        public static SettingRegRespObj AddJobLevel(RegJobLevelObj regObj)
        {
            return new JobLevelRepository().AddJobLevel(regObj);
        }

        public static SettingRegRespObj UpdateJobLevel(EditJobLevelObj regObj)
        {
            return new JobLevelRepository().UpdateJobLevel(regObj);
        }

        public static SettingRegRespObj DeleteJobLevel(DeleteJobLevelObj regObj)
        {
            return new JobLevelRepository().DeleteJobLevel(regObj);
        }

        public static JobLevelRespObj LoadJobLevels(CommonSettingSearchObj regObj)
        {
            return new JobLevelRepository().LoadJobLevels(regObj);
        }

        #endregion

        #region Key Performance Indicator

        public static SettingRegRespObj AddkPIndex(RegKPIndexObj regObj)
        {
            return new KPIndexRepository().AddkPIndex(regObj);
        }

        public static SettingRegRespObj UpdatekPIndex(EditKPIndexObj regObj)
        {
            return new KPIndexRepository().UpdatekPIndex(regObj);
        }

        public static SettingRegRespObj DeletekPIndex(DeleteKPIndexObj regObj)
        {
            return new KPIndexRepository().DeletekPIndex(regObj);
        }

        public static KPIndexRespObj LoadkPIndexs(CommonSettingSearchObj regObj)
        {
            return new KPIndexRepository().LoadkPIndexs(regObj);
        }

        #endregion

        #region Pension Administrator

        public static SettingRegRespObj AddPensionAdministrator(RegPensionAdministratorObj regObj)
        {
            return new PensionAdministratorRepository().AddPensionAdministrator(regObj);
        }

        public static SettingRegRespObj UpdatePensionAdministrator(EditPensionAdministratorObj regObj)
        {
            return new PensionAdministratorRepository().UpdatePensionAdministrator(regObj);
        }

        public static SettingRegRespObj DeletePensionAdministrator(DeletePensionAdministratorObj regObj)
        {
            return new PensionAdministratorRepository().DeletePensionAdministrator(regObj);
        }

        public static PensionAdministratorRespObj LoadPensionAdministrators(CommonSettingSearchObj regObj)
        {
            return new PensionAdministratorRepository().LoadPensionAdministrators(regObj);
        }

        #endregion

        #region Professional Body

        public static SettingRegRespObj AddProfessionalBody(RegProfessionalBodyObj regObj)
        {
            return new ProfessionalBodyRepository().AddProfessionalBody(regObj);
        }

        public static SettingRegRespObj UpdateProfessionalBody(EditProfessionalBodyObj regObj)
        {
            return new ProfessionalBodyRepository().UpdateProfessionalBody(regObj);
        }

        public static SettingRegRespObj DeleteProfessionalBody(DeleteProfessionalBodyObj regObj)
        {
            return new ProfessionalBodyRepository().DeleteProfessionalBody(regObj);
        }

        public static ProfessionalBodyRespObj LoadProfessionalBodys(CommonSettingSearchObj regObj)
        {
            return new ProfessionalBodyRepository().LoadProfessionalBodys(regObj);
        }

        #endregion

        #region Professional Membership Type

        public static SettingRegRespObj AddProfessionalMembershipType(RegProfessionalMembershipTypeObj regObj)
        {
            return new ProfessionalMemshipTypeRepository().AddProfessionalMembershipType(regObj);
        }

        public static SettingRegRespObj UpdateProfessionalMembershipType(EditProfessionalMembershipTypeObj regObj)
        {
            return new ProfessionalMemshipTypeRepository().UpdateProfessionalMembershipType(regObj);
        }

        public static SettingRegRespObj DeleteProfessionalMembershipType(DeleteProfessionalMembershipTypeObj regObj)
        {
            return new ProfessionalMemshipTypeRepository().DeleteProfessionalMembershipType(regObj);
        }

        public static ProfessionalMembershipTypeRespObj LoadProfessionalMembershipTypes(CommonSettingSearchObj regObj)
        {
            return new ProfessionalMemshipTypeRepository().LoadProfessionalMembershipTypes(regObj);
        }

        #endregion

        #region Qualification

        public static SettingRegRespObj AddQualification(RegQualificationObj regObj)
        {
            return new QualificationRepository().AddQualification(regObj);
        }

        public static SettingRegRespObj UpdateQualification(EditQualificationObj regObj)
        {
            return new QualificationRepository().UpdateQualification(regObj);
        }

        public static SettingRegRespObj DeleteQualification(DeleteQualificationObj regObj)
        {
            return new QualificationRepository().DeleteQualification(regObj);
        }

        public static QualificationRespObj LoadQualifications(CommonSettingSearchObj regObj)
        {
            return new QualificationRepository().LoadQualifications(regObj);
        }

        #endregion

        #region Salary Grade

        public static SettingRegRespObj AddSalaryGrade(RegSalaryGradeObj regObj)
        {
            return new SalaryGradeRepository().AddSalaryGrade(regObj);
        }

        public static SettingRegRespObj UpdateSalaryGrade(EditSalaryGradeObj regObj)
        {
            return new SalaryGradeRepository().UpdateSalaryGrade(regObj);
        }

        public static SettingRegRespObj DeleteSalaryGrade(DeleteSalaryGradeObj regObj)
        {
            return new SalaryGradeRepository().DeleteSalaryGrade(regObj);
        }

        public static SalaryGradeRespObj LoadSalaryGrades(CommonSettingSearchObj regObj)
        {
            return new SalaryGradeRepository().LoadSalaryGrades(regObj);
        }

        #endregion

        #region Salary Level

        public static SettingRegRespObj AddSalaryLevel(RegSalaryLevelObj regObj)
        {
            return new SalaryLevelRepository().AddSalaryLevel(regObj);
        }

        public static SettingRegRespObj UpdateSalaryLevel(EditSalaryLevelObj regObj)
        {
            return new SalaryLevelRepository().UpdateSalaryLevel(regObj);
        }

        public static SettingRegRespObj DeleteSalaryLevel(DeleteSalaryLevelObj regObj)
        {
            return new SalaryLevelRepository().DeleteSalaryLevel(regObj);
        }

        public static SalaryLevelRespObj LoadSalaryLevels(CommonSettingSearchObj regObj)
        {
            return new SalaryLevelRepository().LoadSalaryLevels(regObj);
        }

        #endregion

        #region Termination Reason

        public static SettingRegRespObj AddTerminationReason(RegTerminationReasonObj regObj)
        {
            return new TerminationReasonRepository().AddTerminationReason(regObj);
        }

        public static SettingRegRespObj UpdateTerminationReason(EditTerminationReasonObj regObj)
        {
            return new TerminationReasonRepository().UpdateTerminationReason(regObj);
        }

        public static SettingRegRespObj DeleteTerminationReason(DeleteTerminationReasonObj regObj)
        {
            return new TerminationReasonRepository().DeleteTerminationReason(regObj);
        }

        public static TerminationReasonRespObj LoadTerminationReasons(CommonSettingSearchObj regObj)
        {
            return new TerminationReasonRepository().LoadTerminationReasons(regObj);
        }

        #endregion

        #region Company

        public static SettingRegRespObj AddCompany(RegCompanyObj regObj)
        {
            return new CompanyRepository().AddCompany(regObj);
        }

        public static SettingRegRespObj UpdateCompany(EditCompanyObj regObj)
        {
            return new CompanyRepository().UpdateCompany(regObj);
        }

        public static SettingRegRespObj DeleteCompany(DeleteCompanyObj regObj)
        {
            return new CompanyRepository().DeleteCompany(regObj);
        }

        public static CompanyRespObj LoadCompanys(CommonSettingSearchObj regObj)
        {
            return new CompanyRepository().LoadCompanies(regObj);
        }

        #endregion

        #region Department

        public static SettingRegRespObj AddDepartment(RegDepartmentObj regObj)
        {
            return new DepartmentRepository().AddDepartment(regObj);
        }

        public static SettingRegRespObj UpdateDepartment(EditDepartmentObj regObj)
        {
            return new DepartmentRepository().UpdateDepartment(regObj);
        }

        public static SettingRegRespObj DeleteDepartment(DeleteDepartmentObj regObj)
        {
            return new DepartmentRepository().DeleteDepartment(regObj);
        }

        public static DepartmentRespObj LoadDepartments(CommonSettingSearchObj regObj)
        {
            return new DepartmentRepository().LoadDepartments(regObj);
        }

        #endregion

        #region Role

        public static SettingRegRespObj AddStaffRole(RegStaffRoleObj regObj)
        {
            return new StaffRoleRepository().AddStaffRole(regObj);
        }

        public static SettingRegRespObj UpdateStaffRole(EditStaffRoleObj regObj)
        {
            return new StaffRoleRepository().UpdateStaffRole(regObj);
        }

        public static SettingRegRespObj DeleteStaffRole(DeleteStaffRoleObj regObj)
        {
            return new StaffRoleRepository().DeleteStaffRole(regObj);
        }

        public static StaffRoleRespObj LoadStaffRoles(SettingSearchObj regObj)
        {
            return new StaffRoleRepository().LoadStaffRole(regObj);
        }

        #endregion

        #region Leave Type

        public static SettingRegRespObj AddLeaveType(RegLeaveTypeObj regObj)
        {
            return new LeaveTypeRepository().AddLeaveType(regObj);
        }

        public static SettingRegRespObj UpdateLeaveType(EditLeaveTypeObj regObj)
        {
            return new LeaveTypeRepository().UpdateLeaveType(regObj);
        }

        public static SettingRegRespObj DeleteLeaveType(DeleteLeaveTypeObj regObj)
        {
            return new LeaveTypeRepository().DeleteLeaveType(regObj);
        }

        public static LeaveTypeRespObj LoadLeaveTypes(CommonSettingSearchObj regObj)
        {
            return new LeaveTypeRepository().LoadLeaveTypes(regObj);
        }

        #endregion

        #endregion

        #region Staff Management

        public static SettingRegRespObj UpdateStaff(EditStaffObj regObj)
        {
            return new StaffRepository().UpdateStaff(regObj);
        }

        public static SettingRegRespObj ChangePassword(ChangePasswordObj regObj)
        {
            return new StaffRepository().ChangePassword(regObj);
        }

        public static StaffRespObj LoadStaffs()
        {
            return new StaffRepository().LoadStaffs();
        }

        #region Staff Contact

        public static SettingRegRespObj AddStaffContact(RegStaffContactObj regObj)
        {
            return new StaffRepository().AddStaffContact(regObj);
        }

        public static SettingRegRespObj UpdateStaffContact(EditStaffContactObj regObj)
        {
            return new StaffRepository().UpdateStaffContact(regObj);
        }

        public static StaffContactRespObj LoadStaffContact(StaffContactSearchObj regObj)
        {
            return new StaffRepository().LoadStaffContact(regObj);
        }

        #endregion


        #region Staff Emergency Contact

        public static SettingRegRespObj AddEmergencyContact(RegEmergencyContactObj regObj)
        {
            return new StaffRepository().AddEmergencyContact(regObj);
        }

        public static SettingRegRespObj UpdateEmergencyContact(EditEmergencyContactObj regObj)
        {
            return new StaffRepository().UpdateEmergencyContact(regObj);
        }

        public static EmergencyContactRespObj LoadEmergencyContact(EmergencyContactSearchObj regObj)
        {
            return new StaffRepository().LoadEmergencyContact(regObj);
        }

        #endregion

        #region Staff Bank Account

        public static SettingRegRespObj AddStaffBankAccount(RegStaffBankAccountObj regObj)
        {
            return new StaffRepository().AddStaffBankAccount(regObj);
        }

        public static SettingRegRespObj UpdateStaffBackAccount(EditStaffBankAccountObj regObj)
        {
            return new StaffRepository().UpdateStaffBankAccount(regObj);
        }

        public static StaffBankAccountRespObj LoadStaffBackAccount(StaffBankAccountSearchObj regObj)
        {
            return new StaffRepository().LoadStaffBankAccount(regObj);
        }

        #endregion

        #region Staff Insurance

        public static SettingRegRespObj AddStaffInsurance(RegStaffInsuranceObj regObj)
        {
            return new StaffRepository().AddStaffInsurance(regObj);
        }

        public static SettingRegRespObj UpdateStaffInsurance(EditStaffInsuranceObj regObj)
        {
            return new StaffRepository().UpdateStaffInsurance(regObj);
        }

        public static StaffInsuranceRespObj LoadStaffInsurances(StaffInsuranceSearchObj regObj)
        {
            return new StaffRepository().LoadStaffInsurances(regObj);
        }

        #endregion


        #region Staff Next Of Kin

        public static SettingRegRespObj AddStaffNextOfKin(RegStaffNextOfKinObj regObj)
        {
            return new StaffRepository().AddStaffNextOfKin(regObj);
        }

        public static SettingRegRespObj UpdateStaffNextOfKin(EditStaffNextOfKinObj regObj)
        {
            return new StaffRepository().UpdateStaffNextOfKin(regObj);
        }

        public static StaffNextOfKinRespObj LoadStaffNextOfKin(StaffNextOfKinSearchObj regObj)
        {
            return new StaffRepository().LoadStaffNextOfKin(regObj);
        }

        #endregion

        #region Educational Qualification

        public static SettingRegRespObj AddEducationalQualification(RegEducationalQualificationObj regObj)
        {
            return new StaffRepository().AddEducationalQualification(regObj);
        }

        public static SettingRegRespObj UpdateEducationalQualification(EditEducationalQualificationObj regObj)
        {
            return new StaffRepository().UpdateEducationalQualification(regObj);
        }

        public static EducationalQualificationRespObj LoadEducationalQualification(EducationalQualificationSearchObj regObj)
        {
            return new StaffRepository().LoadEducationalQualification(regObj);
        }

        #endregion

        #region Professional Membership

        public static SettingRegRespObj AddProfessionalMembership(RegProfessionalMemberShipObj regObj)
        {
            return new StaffRepository().AddProfessionalMembership(regObj);
        }

        public static SettingRegRespObj UpdateProfessionalMembership(EditProfessionalMemberShipObj regObj)
        {
            return new StaffRepository().UpdateProfessionalMembership(regObj);
        }

        public static ProfessionalMembershipRespObj LoadProfessionalMembership(ProfessionalMemberShipSearchObj regObj)
        {
            return new StaffRepository().LoadProfessionalMembership(regObj);
        }

        #endregion

        #region Staff Leave Request

        public static SettingRegRespObj AddLeaveRequest(RegLeaveRequestObj regObj)
        {
            return new StaffRepository().AddLeaveRequest(regObj);
        }

        public static SettingRegRespObj UpdateLeaveRequest(EditLeaveRequestObj regObj)
        {
            return new StaffRepository().UpdateLeaveRequest(regObj);
        }

        public static LeaveRequestRespObj LoadStaffLeaveRequest(LeaveRequestSearchObj regObj)
        {
            return new StaffRepository().LoadStaffLeaveRequest(regObj);
        }
        #endregion

        #region Staff Medical

        public static SettingRegRespObj AddStaffMedical(RegStaffMedicalObj regObj)
        {
            return new StaffRepository().AddStaffMedical(regObj);
        }

        public static SettingRegRespObj UpdateStaffMedical(EditStaffMedicalObj regObj)
        {
            return new StaffRepository().UpdateStaffMedical(regObj);
        }

        public static StaffMedicalRespObj LoadStaffMedical(StaffMedicalSearchObj regObj)
        {
            return new StaffRepository().LoadStaffMedical(regObj);
        }

        #endregion


        #region Staff Job Info

        public static SettingRegRespObj AddStaffJobInfo(RegStaffJobInfoObj regObj)
        {
            return new StaffRepository().AddStaffJobInfo(regObj);
        }

        public static SettingRegRespObj UpdateStaffJobInfo(EditStaffJobInfoObj regObj)
        {
            return new StaffRepository().UpdateStaffJobInfo(regObj);
        }

        public static StaffJobInfoRespObj LoadStaffJobInfos(StaffJobInfoSearchObj regObj)
        {
            return new StaffRepository().LoadStaffJobInfos(regObj);
        }

        #endregion

        #region Staff Memo Response

        public static SettingRegRespObj AddStaffMemoResponse(RegStaffMemoResponseObj regObj)
        {
            return new StaffRepository().AddStaffMemoResponse(regObj);
        }

        public static SettingRegRespObj ApproveStaffMemoResponse(ApprovaStaffMemoResponseObj regObj)
        {
            return new StaffRepository().ApproveStaffMemoResponse(regObj);
        }

        public static StaffMemoResponseRespObj LoadStaffMemoResponse(StaffMemoResponseSearchObj regObj)
        {
            return new StaffRepository().LoadStaffMemoResponse(regObj);
        }

        #endregion

        #endregion

        #region Admin Management

        #region Staff Pension

        public static SettingRegRespObj AddStaffPension(RegStaffPensionObj regObj)
        {
            return new StaffRepository().AddStaffPension(regObj);
        }

        public static SettingRegRespObj UpdateStaffPension(EditStaffPensionObj regObj)
        {
            return new StaffRepository().UpdateStaffPension(regObj);
        }

        public static StaffPensionRespObj LoadStaffPension(StaffPensionSearchObj regObj)
        {
            return new StaffRepository().LoadStaffPension(regObj);
        }

        #endregion


        #region Staff Leave

        public static SettingRegRespObj AddStaffLeave(RegStaffLeaveObj regObj)
        {
            return new StaffRepository().AddStaffLeave(regObj);
        }

        public static SettingRegRespObj ApproveStaffLeave(ApproveStaffLeaveObj regObj)
        {
            return new StaffRepository().ApproveStaffLeave(regObj);
        }

        public static StaffLeaveRespObj LoadStaffLeave(StaffLeaveSearchObj regObj)
        {
            return new StaffRepository().LoadStaffLeave(regObj);
        }

        public static StaffLeaveRespObj LoadStaffLeaveByAdmin(StaffLeaveSearchObj regObj)
        {
            return new StaffRepository().LoadStaffLeaveByAdmin(regObj);
        }

       

        #endregion

        #region Staff Memo

        public static SettingRegRespObj AddStaffMemo(RegStaffMemoObj regObj)
        {
            return new StaffRepository().AddStaffMemo(regObj);
        }

        public static SettingRegRespObj AddBulkStaffMemo(RegBulkStaffMemoObj regObj)
        {
            return new StaffRepository().AddBulkStaffMemo(regObj);
        }

        public static SettingRegRespObj ApproveStaffMemo(ApprovaStaffMemoObj regObj)
        {
            return new StaffRepository().ApproveStaffMemo(regObj);
        }

        public static StaffMemoRespObj LoadStaffMemo(StaffMemoSearchObj regObj)
        {
            return new StaffRepository().LoadStaffMemo(regObj);
        }

        public static StaffMemoRespObj LoadStaffMemoByAdmin(StaffMemoSearchObj regObj)
        {
            return new StaffRepository().LoadStaffMemoByAdmin(regObj);
        }

        #endregion

        #region Staff Key Performance Indicator

        public static SettingRegRespObj AddStaffKpiIndex(RegStaffKPIIndexObj regObj)
        {
            return new StaffRepository().AddStaffKpiIndex(regObj);
        }

        public static SettingRegRespObj UpdateStaffKpiIndex(EditStaffKPIIndexObj regObj)
        {
            return new StaffRepository().UpdateStaffKpiIndex(regObj);
        }

        public static StaffKPIIndexRespObj LoadStaffKpiIndex(StaffKPIIndexSearchObj regObj)
        {
            return new StaffRepository().LoadStaffKpiIndex(regObj);
        }

        #endregion

        #region Staff Salary

        public static SettingRegRespObj AddStaffSalary(RegStaffSalaryObj regObj)
        {
            return new StaffRepository().AddStaffSalary(regObj);
        }

        public static SettingRegRespObj UpdateStaffSalary(UpdateStaffSalaryObj regObj)
        {
            return new StaffRepository().UpdateStaffSalary(regObj);
        }

        public static StaffSalaryRespObj LoadStaffSalary(StaffSalarySearchObj regObj)
        {
            return new StaffRepository().LoadStaffSalary(regObj);
        }

        #endregion

        #region Staff 

        public static SettingRegRespObj AddStaff(RegStaffObj regObj)
        {
            return new StaffRepository().AddStaff(regObj);
        }

        public static SettingRegRespObj UpdateStaffByAdmin(EditStaffByAdminObj regObj)
        {
            return new StaffRepository().UpdateStaffByAdmin(regObj);
        }


        public static StaffLoginResp LoginStaff(StaffLoginObj regObj)
        {
            return new StaffRepository().LoginStaff(regObj);
        }

        public static StaffRespObj LoadStaffs(StaffSearchObj regObj)
        {
            return new StaffRepository().LoadStaffs(regObj);
        }
        public static WorkFlowLogRespObj LoadWorkFlow(WorkFlowLogSearchObj regObj)
        {
            return new StaffRepository().LoadWorkFlow(regObj);
        }
        #endregion


        public static StaffContactRespObj LoadStaffContactByAdmin(StaffContactSearchObj regObj)
        {
            return new StaffRepository().LoadStaffContactByAdmin(regObj);
        }
        public static SettingRegRespObj AddComment(RegCommentObj regObj)
        {
            return new StaffRepository().AddComment(regObj);
        }
        public static EmergencyContactRespObj LoadEmergencyContactByAdmin(EmergencyContactSearchObj regObj)
        {
            return new StaffRepository().LoadEmergencyContactByAdmin(regObj);
        }

        public static StaffMedicalRespObj LoadStaffMedicalByAdmin(StaffMedicalSearchObj regObj)
        {
            return new StaffRepository().LoadStaffMedicalByAdmin(regObj);
        }

        public static LeaveRequestRespObj LoadLeaveRequestByAdmin(LeaveRequestSearchObj regObj)
        {
            return new StaffRepository().LoadLeaveRequestByAdmin(regObj);
        }

        public static WorkFlowLogRespObj LoadWorkFlowByAdmin(WorkFlowLogSearchObj regObj)
        {
            return new StaffRepository().LoadWorkFlowByAdmin(regObj);
        }

        public static StaffMemoResponseRespObj LoadStaffMemoResponseByAdmin(StaffMemoResponseSearchObj regObj)
        {
            return new StaffRepository().LoadStaffMemoResponseByAdmin(regObj);
        }

        public static StaffBankAccountRespObj LoadStaffBankAccountByAdmin(StaffBankAccountSearchObj regObj)
        {
            return new StaffRepository().LoadStaffBankAccountByAdmin(regObj);
        }

        public static StaffNextOfKinRespObj LoadStaffNextOfKinByAdmin(StaffNextOfKinSearchObj regObj)
        {
            return new StaffRepository().LoadStaffNextOfKinByAdmin(regObj);
        }

        public static EducationalQualificationRespObj LoadEducationalQualificationByAdmin(EducationalQualificationSearchObj regObj)
        {
            return new StaffRepository().LoadEducationalQualificationByAdmin(regObj);
        }

        public static ProfessionalMembershipRespObj LoadProfessionalMembershipByAdmin(ProfessionalMemberShipSearchObj regObj)
        {
            return new StaffRepository().LoadProfessionalMembershipByAdmin(regObj);
        }


        public static StaffInsuranceRespObj LoadStaffInsurancesByAdmin(StaffInsuranceSearchObj regObj)
        {
            return new StaffRepository().LoadStaffInsurancesByAdmin(regObj);
        }

        public static StaffJobInfoRespObj LoadStaffJobInfosByAdmin(StaffJobInfoSearchObj regObj)
        {
            return new StaffRepository().LoadStaffJobInfosByAdmin(regObj);
        }

        #endregion
    }
}