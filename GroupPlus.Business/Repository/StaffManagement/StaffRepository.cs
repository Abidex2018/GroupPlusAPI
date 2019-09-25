using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Web.Helpers;
using GroupPlus.Business.Core;
using GroupPlus.Business.Core.ItemComparers;
using GroupPlus.Business.DataManager;
using GroupPlus.Business.Infrastructure;
using GroupPlus.Business.Infrastructure.Contract;
using GroupPlus.Business.Repository.Common;
using GroupPlus.Business.Repository.CompanyManagement;
using GroupPlus.BusinessContract.CommonAPIs;
using GroupPlus.BusinessObject.StaffDetail;
using GroupPlus.BusinessObject.StaffManagement;
using GroupPlus.BussinessObject.Workflow;
using GroupPlus.Common;
using XPLUG.WEBTOOLS;
using XPLUG.WEBTOOLS.Security;

namespace GroupPlus.Business.Repository.StaffManagement
{
    internal partial class StaffRepository
    {
        private readonly IPlugRepository<StaffAccess> _accessRepository;
        private readonly IPlugRepository<Comment> _adminCommmentRepository;
        private readonly IPlugRepository<EmergencyContact> _emergencyContactRepository;
        private readonly IPlugRepository<StaffLoginActivity> _loginRepository;

        private readonly IPlugRepository<Staff> _repository;
        private readonly IPlugRepository<StaffBankAccount> _staffBankAccRepository;
        private readonly IPlugRepository<StaffContact> _staffContactRepository;
        private readonly IPlugRepository<HigherEducation> _staffHigherEducationRepository;
        private readonly IPlugRepository<StaffInsurance> _staffInsuranceRepository;
        private readonly IPlugRepository<StaffJobInfo> _staffJobInfoRepository;
        private readonly IPlugRepository<StaffKPIndex> _staffKPIndexRepository;
        private readonly IPlugRepository<StaffLeave> _staffLeaveRepository;
        private readonly IPlugRepository<LeaveRequest> _staffLeaveRequestRepository;
        private readonly IPlugRepository<StaffMedical> _staffMedicalRepository;
        private readonly IPlugRepository<StaffMemo> _staffMemoRepository;
        private readonly IPlugRepository<StaffMemoResponse> _staffMemoResponseRepository;
        private readonly IPlugRepository<StaffNextOfKin> _staffNextOfKinRepository;
        private readonly IPlugRepository<StaffPension> _staffPensionRepository;
        private readonly IPlugRepository<ProfessionalMembership> _staffProfessionalMembershipRepository;
        private readonly IPlugRepository<StaffSalary> _staffSalaryRepository;
        private readonly PlugUoWork _uoWork;
        private readonly IPlugRepository<WorkflowLog> _workflowLogRepository;
        private readonly IPlugRepository<WorkflowSetup> _workflowSetupRepository;
        private static string previousCode;
        public StaffRepository()
        {
            _uoWork = new PlugUoWork();
            _repository = new PlugRepository<Staff>(_uoWork);
            _staffContactRepository = new PlugRepository<StaffContact>(_uoWork);
            _emergencyContactRepository = new PlugRepository<EmergencyContact>(_uoWork);
            _staffBankAccRepository = new PlugRepository<StaffBankAccount>(_uoWork);
            _staffNextOfKinRepository = new PlugRepository<StaffNextOfKin>(_uoWork);
            _staffHigherEducationRepository = new PlugRepository<HigherEducation>(_uoWork);
            _staffProfessionalMembershipRepository = new PlugRepository<ProfessionalMembership>(_uoWork);
            _staffLeaveRequestRepository = new PlugRepository<LeaveRequest>(_uoWork);
            _staffLeaveRepository = new PlugRepository<StaffLeave>(_uoWork);
            _staffInsuranceRepository = new PlugRepository<StaffInsurance>(_uoWork);
            _staffJobInfoRepository = new PlugRepository<StaffJobInfo>(_uoWork);
            _staffMedicalRepository = new PlugRepository<StaffMedical>(_uoWork);
            _staffKPIndexRepository = new PlugRepository<StaffKPIndex>(_uoWork);
            _staffPensionRepository = new PlugRepository<StaffPension>(_uoWork);
            _staffMemoRepository = new PlugRepository<StaffMemo>(_uoWork);
            _staffMemoResponseRepository = new PlugRepository<StaffMemoResponse>(_uoWork);
            _staffSalaryRepository = new PlugRepository<StaffSalary>(_uoWork);
            _loginRepository = new PlugRepository<StaffLoginActivity>(_uoWork);
            _accessRepository = new PlugRepository<StaffAccess>(_uoWork);
            _workflowLogRepository = new PlugRepository<WorkflowLog>(_uoWork);
            _workflowSetupRepository = new PlugRepository<WorkflowSetup>(_uoWork);

           
        }

        #region Staff Logic

        public SettingRegRespObj AddStaff(RegStaffObj regObj)
        {
            var response = new SettingRegRespObj
            {
                Status = new APIResponseStatus
                {
                    IsSuccessful = false,
                    Message = new APIResponseMessage()
                }
            };

            try
            {
                if (regObj.Equals(null))
                {
                    response.Status.Message.FriendlyMessage = "Error Occurred! Unable to proceed with your request";
                    response.Status.Message.TechnicalMessage = "Registration Object is empty / invalid";
                    return response;
                }

                if (!EntityValidatorHelper.Validate(regObj, out var valResults))
                {
                    var errorDetail = new StringBuilder();
                    if (!valResults.IsNullOrEmpty())
                    {
                        errorDetail.AppendLine("Following error occurred:");
                        valResults.ForEachx(m => errorDetail.AppendLine(m.ErrorMessage));
                    }

                    else
                    {
                        errorDetail.AppendLine(
                            "Validation error occurred! Please check all supplied parameters and try again");
                    }
                    response.Status.Message.FriendlyMessage = errorDetail.ToString();
                    response.Status.Message.TechnicalMessage = errorDetail.ToString();
                    response.Status.IsSuccessful = false;
                    return response;
                }

                if (!HelperMethods.IsUserValid(regObj.AdminUserId, regObj.SysPathCode, HelperMethods.getAdminRoles(),
                    ref response.Status.Message))
                    return response;

                if (IsStaffDuplicate(regObj.CompanyId, regObj.LastName, regObj.FirstName, regObj.MiddleName,
                    regObj.Email, regObj.MobileNumber, regObj.Username, 1, ref response))
                    return response;

                var msg = "";
                var candidateAge =
                    DateMap.CalCulateAge(regObj.DateOfBirth, DateTime.Now.ToString("yyyy/MM/dd"), ref msg);

                if (!string.IsNullOrEmpty(msg))
                {
                    response.Status.Message.FriendlyMessage = "Age Verification Error: " + msg;
                    response.Status.Message.TechnicalMessage = "Age Verification Error: " + msg;
                    return response;
                }

                if (candidateAge < 18)
                {
                    response.Status.Message.FriendlyMessage = "Sorry, this applicant is not qualified for employment";
                    response.Status.Message.TechnicalMessage = "Sorry, this applicant is not qualified for employment";
                    return response;
                }

               

                var staff = new Staff
                {
                    FirstName = regObj.FirstName,
                    LastName = regObj.LastName,
                    MiddleName = string.IsNullOrEmpty(regObj.MiddleName) ? "" : regObj.MiddleName,
                    Gender = (Gender) regObj.Gender,
                    DateOfBirth = regObj.DateOfBirth,
                    Email = regObj.Email,
                    PermanentHomeAddress = "",
                    EmploymentType = (EmploymentType) regObj.EmploymentType,
                    MobileNumber = regObj.MobileNumber,
                    MaritalStatus = (MaritalStatus) regObj.MaritalStatus,
                    CountryOfOriginId = regObj.CountryOfOriginId,
                    LocalAreaId = regObj.LocalAreaId,
                    StateOfOriginId = regObj.StateOfOriginId,
                    EmploymentDate = regObj.EmploymentDate,
                    CompanyId = regObj.CompanyId,
                    TimeStamRegistered = DateMap.CurrentTimeStamp(),

                    Status = StaffStatus.Active,
                    StaffAccess = new StaffAccess
                    {
                        
                        CompanyId = regObj.CompanyId,
                        DateLockedOut = "",
                        DepartmentId = regObj.DepartmentId,
                        Email = regObj.Email,
                        FailedPasswordAttemptCount = 0,
                        IsApproved = true,
                        IsLockedOut = false,
                        LastLockedOutTimeStamp = "",
                        LastPasswordChangedTimeStamp = "",
                        LastReleasedTimeStamp = "",
                        TimeLockedOut = "",
                        MobileNumber = regObj.MobileNumber,
                        Username = regObj.Username,
                        Password = Crypto.GenerateSalt(),
                        UserCode = EncryptionHelper.GenerateSalt(30, 50),
                        AccessCode = Crypto.HashPassword(regObj.Password)
                    }
                };

                using (var db = _uoWork.BeginTransaction())
                {
                    try
                    {
                        var added = _repository.Add(staff);
                        _uoWork.SaveChanges();

                        if (added == null || added.StaffId < 1)
                        {
                            db.Rollback();
                            response.Status.Message.FriendlyMessage =
                                "Error Occurred! Unable to complete your request. Please try again later";
                            response.Status.Message.TechnicalMessage = "Unable to save to database";
                            return response;
                        }

                        added.StaffAccess.StaffId = added.StaffId;

                        var accessRet = _accessRepository.Update(added.StaffAccess);
                        _uoWork.SaveChanges();

                        if (accessRet == null || accessRet.StaffId < 1)
                        {
                            db.Rollback();
                            response.Status.Message.FriendlyMessage =
                                "Error Occurred! Unable to complete your request. Please try again later";
                            response.Status.Message.TechnicalMessage = "Unable to save to database";
                            return response;
                        }

                        db.Commit();

                        response.Status.IsSuccessful = true;
                        response.SettingId = added.StaffId;
                    }
                    catch (DbEntityValidationException ex)
                    {
                        db.Rollback();
                        ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                        response.Status.Message.FriendlyMessage = "Error Occurred! Please try again later";
                        response.Status.Message.TechnicalMessage = "Error: " + ex.GetBaseException().Message;
                        response.Status.IsSuccessful = false;
                        return response;
                    }
                    catch (Exception ex)
                    {
                        db.Rollback();
                        ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                        response.Status.Message.FriendlyMessage = "Error Occurred! Please try again later";
                        response.Status.Message.TechnicalMessage = "Error: " + ex.GetBaseException().Message;
                        response.Status.IsSuccessful = false;
                        return response;
                    }
                }
            }
            catch (DbEntityValidationException ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                response.Status.Message.FriendlyMessage = "Error Occurred! Please try again later";
                response.Status.Message.TechnicalMessage = "Error: " + ex.GetBaseException().Message;
                response.Status.IsSuccessful = false;
                return response;
            }
            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                response.Status.Message.FriendlyMessage = "Error Occurred! Please try again later";
                response.Status.Message.TechnicalMessage = "Error: " + ex.GetBaseException().Message;
                response.Status.IsSuccessful = false;
                return response;
            }
            return response;
        }

        public SettingRegRespObj UpdateStaff(EditStaffObj regObj)
        {
            var response = new SettingRegRespObj
            {
                Status = new APIResponseStatus
                {
                    IsSuccessful = false,
                    Message = new APIResponseMessage()
                }
            };

            try
            {
                if (regObj.Equals(null))
                {
                    response.Status.Message.FriendlyMessage = "Error Occurred! Unable to proceed with your request";
                    response.Status.Message.TechnicalMessage = "Registration Object is empty / invalid";
                    return response;
                }

                if (!EntityValidatorHelper.Validate(regObj, out var valResults))
                {
                    var errorDetail = new StringBuilder();
                    if (!valResults.IsNullOrEmpty())
                    {
                        errorDetail.AppendLine("Following error occurred:");
                        valResults.ForEachx(m => errorDetail.AppendLine(m.ErrorMessage));
                    }

                    else
                    {
                        errorDetail.AppendLine(
                            "Validation error occurred! Please check all supplied parameters and try again");
                    }
                    response.Status.Message.FriendlyMessage = errorDetail.ToString();
                    response.Status.Message.TechnicalMessage = errorDetail.ToString();
                    response.Status.IsSuccessful = false;
                    return response;
                }

                if (!HelperMethods.IsStaffUserValid(regObj.AdminUserId, regObj.SysPathCode, regObj.LoginIP, false,
                    out var staffInfo, ref response.Status.Message))
                    return response;

                //var staffInfo = getStaffInfo(regObj.AdminUserId);
                if (staffInfo == null || staffInfo.StaffId < 1)
                {
                    response.Status.Message.FriendlyMessage = "Invalid Staff Information";
                    response.Status.Message.TechnicalMessage = "Invalid Staff Information";
                    return response;
                }
                var thisStaff = getStaffInfo(staffInfo.StaffId);

                if (thisStaff == null || thisStaff.StaffId < 1)
                {
                    response.Status.Message.FriendlyMessage = "No Staff Information found for the supplied information";
                    response.Status.Message.TechnicalMessage = "No Staff Information found!";
                    return response;
                }


                thisStaff.MobileNumber = regObj.MobileNumber;
                thisStaff.Email = regObj.Email;
                thisStaff.PermanentHomeAddress = regObj.PermanentHomeAddress;
                thisStaff.MaritalStatus = (MaritalStatus) regObj.MaritalStatus;

                var added = _repository.Update(thisStaff);

                _uoWork.SaveChanges();

                if (added.StaffId < 1)
                {
                    response.Status.Message.FriendlyMessage =
                        "Error Occurred! Unable to complete your request. Please try again later";
                    response.Status.Message.TechnicalMessage = "Unable to save to database";
                    return response;
                }

                response.Status.IsSuccessful = true;
                response.SettingId = added.StaffId;
            }
            catch (DbEntityValidationException ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                response.Status.Message.FriendlyMessage = "Error Occurred! Please try again later";
                response.Status.Message.TechnicalMessage = "Error: " + ex.GetBaseException().Message;
                response.Status.IsSuccessful = false;
                return response;
            }
            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                response.Status.Message.FriendlyMessage = "Error Occurred! Please try again later";
                response.Status.Message.TechnicalMessage = "Error: " + ex.GetBaseException().Message;
                response.Status.IsSuccessful = false;
                return response;
            }
            return response;
        }

        public SettingRegRespObj ChangePassword(ChangePasswordObj regObj)
        {
            var response = new SettingRegRespObj
            {
                Status = new APIResponseStatus
                {
                    IsSuccessful = false,
                    Message = new APIResponseMessage()
                }
            };

            try
            {
                if (regObj.Equals(null))
                {
                    response.Status.Message.FriendlyMessage = "Error Occurred! Unable to proceed with your request";
                    response.Status.Message.TechnicalMessage = "Registration Object is empty / invalid";
                    return response;
                }

                if (!EntityValidatorHelper.Validate(regObj, out var valResults))
                {
                    var errorDetail = new StringBuilder();
                    if (!valResults.IsNullOrEmpty())
                    {
                        errorDetail.AppendLine("Following error occurred:");
                        valResults.ForEachx(m => errorDetail.AppendLine(m.ErrorMessage));
                    }

                    else
                    {
                        errorDetail.AppendLine(
                            "Validation error occurred! Please check all supplied parameters and try again");
                    }
                    response.Status.Message.FriendlyMessage = errorDetail.ToString();
                    response.Status.Message.TechnicalMessage = errorDetail.ToString();
                    response.Status.IsSuccessful = false;
                    return response;
                }

                if (!HelperMethods.IsStaffUserValid(regObj.AdminUserId, regObj.SysPathCode, regObj.LoginIP, false,
                    out var staffInfo, ref response.Status.Message))
                    return response;

                //var staffInfo = getStaffInfo(regObj.AdminUserId);
                if (staffInfo == null || staffInfo.StaffId < 1)
                {
                    response.Status.Message.FriendlyMessage = "Invalid Staff Information";
                    response.Status.Message.TechnicalMessage = "Invalid Staff Information";
                    return response;
                }
                var thisStaffAccess = getStaffAccessInfo(staffInfo.StaffId);

                if (thisStaffAccess == null || thisStaffAccess.StaffId < 1)
                {
                    response.Status.Message.FriendlyMessage = "No Staff Information found for the supplied information";
                    response.Status.Message.TechnicalMessage = "No Staff Information found!";
                    return response;
                }

                previousCode = thisStaffAccess.Password;

                if (regObj.OldPassword != previousCode)
                {
                    response.Status.Message.FriendlyMessage = "Old Password must be same with Previous Password";
                    response.Status.Message.TechnicalMessage = "Invalid Staff Password";
                    return response;
                }

                thisStaffAccess.Password = regObj.NewPassword;
                thisStaffAccess.UserCode = EncryptionHelper.GenerateSalt(30, 50);
                thisStaffAccess.AccessCode = Crypto.HashPassword(regObj.NewPassword);
                thisStaffAccess.LastPasswordChangedTimeStamp = DateMap.CurrentTimeStamp();

                var added = _accessRepository.Update(thisStaffAccess);

                _uoWork.SaveChanges();

                if (added.StaffAccessId < 1)
                {
                    response.Status.Message.FriendlyMessage =
                        "Error Occurred! Unable to complete your request. Please try again later";
                    response.Status.Message.TechnicalMessage = "Unable to save to database";
                    return response;
                }

                response.Status.IsSuccessful = true;
                response.SettingId = added.StaffAccessId;
            }
            catch (DbEntityValidationException ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                response.Status.Message.FriendlyMessage = "Error Occurred! Please try again later";
                response.Status.Message.TechnicalMessage = "Error: " + ex.GetBaseException().Message;
                response.Status.IsSuccessful = false;
                return response;
            }
            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                response.Status.Message.FriendlyMessage = "Error Occurred! Please try again later";
                response.Status.Message.TechnicalMessage = "Error: " + ex.GetBaseException().Message;
                response.Status.IsSuccessful = false;
                return response;
            }
            return response;
        }

        public SettingRegRespObj UpdateStaffByAdmin(EditStaffByAdminObj regObj)
        {
            var response = new SettingRegRespObj
            {
                Status = new APIResponseStatus
                {
                    IsSuccessful = false,
                    Message = new APIResponseMessage()
                }
            };

            try
            {
                if (regObj.Equals(null))
                {
                    response.Status.Message.FriendlyMessage = "Error Occurred! Unable to proceed with your request";
                    response.Status.Message.TechnicalMessage = "Registration Object is empty / invalid";
                    return response;
                }

                if (!EntityValidatorHelper.Validate(regObj, out var valResults))
                {
                    var errorDetail = new StringBuilder();
                    if (!valResults.IsNullOrEmpty())
                    {
                        errorDetail.AppendLine("Following error occurred:");
                        valResults.ForEachx(m => errorDetail.AppendLine(m.ErrorMessage));
                    }

                    else
                    {
                        errorDetail.AppendLine(
                            "Validation error occurred! Please check all supplied parameters and try again");
                    }
                    response.Status.Message.FriendlyMessage = errorDetail.ToString();
                    response.Status.Message.TechnicalMessage = errorDetail.ToString();
                    response.Status.IsSuccessful = false;
                    return response;
                }

                if (!HelperMethods.IsUserValid(regObj.AdminUserId, regObj.SysPathCode, HelperMethods.getAdminRoles(),
                    ref response.Status.Message))
                    return response;

                var thisStaff = getStaffInfo(regObj.StaffId);

                if (thisStaff == null || thisStaff.StaffId < 1)
                {
                    response.Status.Message.FriendlyMessage = "No Staff Information found for the supplied information";
                    response.Status.Message.TechnicalMessage = "No Staff Information found!";
                    return response;
                }


                var msg = "";
                var candidateAge =
                    DateMap.CalCulateAge(regObj.DateOfBirth, DateTime.Now.ToString("yyyy/MM/dd"), ref msg);

                if (!string.IsNullOrEmpty(msg))
                {
                    response.Status.Message.FriendlyMessage = "Age Verification Error: " + msg;
                    response.Status.Message.TechnicalMessage = "Age Verification Error: " + msg;
                    return response;
                }

                if (candidateAge < 18)
                {
                    response.Status.Message.FriendlyMessage = "Sorry, this applicant is not qualified for employment";
                    response.Status.Message.TechnicalMessage = "Sorry, this applicant is not qualified for employment";
                    return response;
                }


                thisStaff.FirstName = regObj.FirstName;
                thisStaff.LastName = regObj.LastName;
                thisStaff.MiddleName = string.IsNullOrEmpty(regObj.MiddleName) ? "" : regObj.MiddleName;
                thisStaff.Gender = (Gender) regObj.Gender;
                thisStaff.EmploymentType = (EmploymentType) regObj.EmploymentType;
                thisStaff.EmploymentDate = regObj.EmploymentDate;
                thisStaff.MaritalStatus = (MaritalStatus) regObj.MaritalStatus;
                thisStaff.DateOfBirth = regObj.DateOfBirth;
                thisStaff.CompanyId = regObj.CompanyId;
                thisStaff.CountryOfOriginId = regObj.CountryOfOriginId;
                thisStaff.LocalAreaId = regObj.LocalAreaId;
                thisStaff.StateOfOriginId = regObj.StateOfOriginId;
                thisStaff.Status = (StaffStatus) regObj.Status;

                var added = _repository.Update(thisStaff);

                _uoWork.SaveChanges();

                if (added.StaffId < 1)
                {
                    response.Status.Message.FriendlyMessage =
                        "Error Occurred! Unable to complete your request. Please try again later";
                    response.Status.Message.TechnicalMessage = "Unable to save to database";
                    return response;
                }

                response.Status.IsSuccessful = true;
                response.SettingId = added.StaffId;
            }
            catch (DbEntityValidationException ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                response.Status.Message.FriendlyMessage = "Error Occurred! Please try again later";
                response.Status.Message.TechnicalMessage = "Error: " + ex.GetBaseException().Message;
                response.Status.IsSuccessful = false;
                return response;
            }
            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                response.Status.Message.FriendlyMessage = "Error Occurred! Please try again later";
                response.Status.Message.TechnicalMessage = "Error: " + ex.GetBaseException().Message;
                response.Status.IsSuccessful = false;
                return response;
            }
            return response;
        }

        public StaffRespObj LoadStaffs(StaffSearchObj searchObj)
        {
            var response = new StaffRespObj
            {
                Status = new APIResponseStatus
                {
                    IsSuccessful = false,
                    Message = new APIResponseMessage()
                }
            };

            try
            {
                if (searchObj.Equals(null))
                {
                    response.Status.Message.FriendlyMessage = "Error Occurred! Unable to proceed with your request";
                    response.Status.Message.TechnicalMessage = "Registration Object is empty / invalid";
                    return response;
                }

                if (!EntityValidatorHelper.Validate(searchObj, out var valResults))
                {
                    var errorDetail = new StringBuilder();
                    if (!valResults.IsNullOrEmpty())
                    {
                        errorDetail.AppendLine("Following error occurred:");
                        valResults.ForEachx(m => errorDetail.AppendLine(m.ErrorMessage));
                    }

                    else
                    {
                        errorDetail.AppendLine(
                            "Validation error occurred! Please check all supplied parameters and try again");
                    }
                    response.Status.Message.FriendlyMessage = errorDetail.ToString();
                    response.Status.Message.TechnicalMessage = errorDetail.ToString();
                    response.Status.IsSuccessful = false;
                    return response;
                }

                if (!HelperMethods.IsUserValid(searchObj.AdminUserId, searchObj.SysPathCode,HelperMethods.getStaffRoles(), ref response.Status.Message))
                    return response;

                var thisStaffs = getStaffInfos(searchObj);
                if (thisStaffs == null || !thisStaffs.Any())
                {
                    response.Status.Message.FriendlyMessage = "No Staff Information found!";
                    response.Status.Message.TechnicalMessage = "No Staff  Information found!";
                    return response;
                }


                var staffItems = new List<StaffObj>();
                thisStaffs.ForEachx(m =>
                {
                    staffItems.Add(new StaffObj
                    {
                        StaffId = m.StaffId,
                        FirstName = m.FirstName,
                        LastName = m.LastName,
                        MiddleName = m.MiddleName,
                        Gender = (int) m.Gender,
                        GenderLabel = m.Gender.ToString().Replace("_", " "),
                        MobileNumber = m.MobileNumber,
                        Email = m.Email,
                        PermanentHomeAddress = m.PermanentHomeAddress,
                        EmploymentType = (int) m.EmploymentType,
                        EmploymentTypeLabel = m.EmploymentType.ToString().Replace("_", " "),
                        EmploymentDate = m.EmploymentDate,
                        MaritalStatus = (int) m.MaritalStatus,
                        MaritalStatusLabel = m.MaritalStatus.ToString().Replace("_", " "),
                        DateOfBirth = m.DateOfBirth,
                        CompanyId = m.CompanyId,
                        CompanyLabel = new CompanyRepository().GetCompany(m.CompanyId).Name,
                        CountryOfOriginId = m.CountryOfOriginId,
                        LocalAreaId = m.LocalAreaId,
                        StateOfOriginId = m.StateOfOriginId,
                        Status = (int) m.Status,
                        StatusLabel = m.Status.ToString().Replace("_", " "),
                        TimeStamRegistered = m.TimeStamRegistered
                    });
                });

                response.Status.IsSuccessful = true;
                response.Staffs = staffItems;
                return response;
            }

            catch (DbEntityValidationException ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                response.Status.Message.FriendlyMessage = "Error Occurred! Please try again later";
                response.Status.Message.TechnicalMessage = "Error: " + ex.GetBaseException().Message;
                response.Status.IsSuccessful = false;
                return response;
            }

            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                response.Status.Message.FriendlyMessage = "Error Occurred! Please try again later";
                response.Status.Message.TechnicalMessage = "Error: " + ex.GetBaseException().Message;
                response.Status.IsSuccessful = false;
                return response;
            }
        }
        public StaffRespObj LoadStaffs()
        {
            var response = new StaffRespObj
            {
                Status = new APIResponseStatus
                {
                    IsSuccessful = false,
                    Message = new APIResponseMessage()
                }
            };

            try
            {
                var thisStaffs = getStaffInfos();
                if (thisStaffs == null || !thisStaffs.Any())
                {
                    response.Status.Message.FriendlyMessage = "No Staff Information found!";
                    response.Status.Message.TechnicalMessage = "No Staff  Information found!";
                    return response;
                }


                var staffItems = new List<StaffObj>();
                thisStaffs.ForEachx(m =>
                {
                    staffItems.Add(new StaffObj
                    {
                        StaffId = m.StaffId,
                        FirstName = m.FirstName,
                        LastName = m.LastName,
                        MiddleName = m.MiddleName,
                        Gender = (int)m.Gender,
                        GenderLabel = m.Gender.ToString().Replace("_", " "),
                        MobileNumber = m.MobileNumber,
                        Email = m.Email,
                        PermanentHomeAddress = m.PermanentHomeAddress,
                        EmploymentType = (int)m.EmploymentType,
                        EmploymentTypeLabel = m.EmploymentType.ToString().Replace("_", " "),
                        EmploymentDate = m.EmploymentDate,
                        MaritalStatus = (int)m.MaritalStatus,
                        MaritalStatusLabel = m.MaritalStatus.ToString().Replace("_", " "),
                        DateOfBirth = m.DateOfBirth,
                        CompanyId = m.CompanyId,
                        CompanyLabel = new CompanyRepository().GetCompany(m.CompanyId).Name,
                        CountryOfOriginId = m.CountryOfOriginId,
                        LocalAreaId = m.LocalAreaId,
                        StateOfOriginId = m.StateOfOriginId,
                        Status = (int)m.Status,
                        StatusLabel = m.Status.ToString().Replace("_", " "),
                        TimeStamRegistered = m.TimeStamRegistered
                    });
                });

                response.Status.IsSuccessful = true;
                response.Staffs = staffItems;
                return response;
            }

            catch (DbEntityValidationException ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                response.Status.Message.FriendlyMessage = "Error Occurred! Please try again later";
                response.Status.Message.TechnicalMessage = "Error: " + ex.GetBaseException().Message;
                response.Status.IsSuccessful = false;
                return response;
            }

            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                response.Status.Message.FriendlyMessage = "Error Occurred! Please try again later";
                response.Status.Message.TechnicalMessage = "Error: " + ex.GetBaseException().Message;
                response.Status.IsSuccessful = false;
                return response;
            }
        }
        public StaffLoginResp LoginStaff(StaffLoginObj regObj)
        {
            var response = new StaffLoginResp
            {
                Status = new APIResponseStatus
                {
                    IsSuccessful = false,
                    Message = new APIResponseMessage()
                }
            };


            try
            {
                if (regObj.Equals(null))
                {
                    response.Status.Message.FriendlyMessage = "Error Occurred! Unable to proceed with your request";
                    response.Status.Message.TechnicalMessage = "Registration Object is empty / invalid";
                    return response;
                }

                if (!EntityValidatorHelper.Validate(regObj, out var valResults))
                {
                    var errorDetail = new StringBuilder();
                    if (!valResults.IsNullOrEmpty())
                    {
                        errorDetail.AppendLine("Following error occurred:");
                        valResults.ForEachx(m => errorDetail.AppendLine(m.ErrorMessage));
                    }
                    else
                    {
                        errorDetail.AppendLine(
                            "Validation error occurred! Please check all supplied parameters and try again");
                    }
                    response.Status.Message.FriendlyMessage = errorDetail.ToString();
                    response.Status.Message.TechnicalMessage = errorDetail.ToString();
                    response.Status.IsSuccessful = false;
                    return response;
                }

                var staffAccess = getStaffAccessInfo(regObj.Username, out var msg);
                if (staffAccess == null || staffAccess.StaffId < 1)
                {
                    response.Status.Message.FriendlyMessage = string.IsNullOrEmpty(msg) ? "Invalid Staff Access " : msg;
                    response.Status.Message.TechnicalMessage = string.IsNullOrEmpty(msg) ? "Invalid Staff Access" : msg;
                    return response;
                }

                var validated = Crypto.VerifyHashedPassword(staffAccess.AccessCode, regObj.Password.Trim());
                var token = RegisterLoginEvent(staffAccess.StaffId, regObj.LoginSourceIP,
                    validated && staffAccess.IsApproved, ref response.Status.Message);
                if (string.IsNullOrEmpty(token))
                    return response;

                if (!validated)
                {
                    response.Status.Message.FriendlyMessage = "Login Not Successful! Please try again";
                    response.Status.Message.TechnicalMessage = "Login Not Successful! Please try again";
                    return response;
                }


                var staffInfo = getStaffInfo(staffAccess.StaffId);
                if (staffInfo == null || staffInfo.StaffId < 1)
                {
                    response.Status.Message.FriendlyMessage = "No Staff Information Found!";
                    response.Status.Message.TechnicalMessage = "No Staff Information Found!";
                    return response;
                }

                if (staffInfo.Status != StaffStatus.Active)
                {
                    response.Status.Message.FriendlyMessage = "Inactive Staff Login!";
                    response.Status.Message.TechnicalMessage = "Inactive Staff Login!";
                    return response;
                }


                var candidateItem = new StaffObj
                {
                    StaffId = staffInfo.StaffId,
                    Gender = (int) staffInfo.Gender,
                    GenderLabel = staffInfo.Gender.ToString().Replace("_", " "),
                    CountryOfOriginId = staffInfo.CountryOfOriginId,
                    StateOfOriginId = staffInfo.StateOfOriginId,
                    LocalAreaId = staffInfo.LocalAreaId,
                    CountryOfOriginLabel = "",
                    StateOfOriginLabel = "",
                    LocalAreaLabel = "",
                    FirstName = staffInfo.FirstName,
                    MiddleName = staffInfo.MiddleName,
                    LastName = staffInfo.LastName,
                    DateOfBirth = staffInfo.DateOfBirth,
                    MobileNumber = staffInfo.MobileNumber,
                    Email = staffInfo.Email,
                    PermanentHomeAddress = staffInfo.PermanentHomeAddress,
                    MaritalStatus = (int) staffInfo.MaritalStatus,
                    MaritalStatusLabel = staffInfo.MaritalStatus.ToString().Replace("_", " "),
                    EmploymentType = (int) staffInfo.EmploymentType,
                    EmploymentTypeLabel = staffInfo.EmploymentType.ToString().Replace("_", " "),
                    Status = (int) staffInfo.Status,
                    StatusLabel = staffInfo.Status.ToString().Replace("_", " "),
                    EmploymentDate = staffInfo.EmploymentDate,
                    CompanyId = staffInfo.CompanyId,
                    TimeStamRegistered = staffInfo.TimeStamRegistered
                };

                response.AuthToken = token;
                response.Username = staffAccess.Username;
                response.AccessRoles = new List<string> {"Staff"};
                response.Status.IsSuccessful = true;
                response.UserId = staffAccess.StaffId;
                response.StaffInfo = candidateItem;
            }
            catch (DbEntityValidationException ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                response.Status.Message.FriendlyMessage = "Error Occurred! Please try again later";
                response.Status.Message.TechnicalMessage = "Error: " + ex.GetBaseException().Message;
                response.Status.IsSuccessful = false;
                return response;
            }
            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                response.Status.Message.FriendlyMessage = "Error Occurred! Please try again later";
                response.Status.Message.TechnicalMessage = "Error: " + ex.GetBaseException().Message;
                response.Status.IsSuccessful = false;
                return response;
            }

            return response;
        }

        #endregion

        #region StaffKPIIndex

        public SettingRegRespObj AddStaffKpiIndex(RegStaffKPIIndexObj regObj)
        {
            var response = new SettingRegRespObj
            {
                Status = new APIResponseStatus
                {
                    IsSuccessful = false,
                    Message = new APIResponseMessage()
                }
            };

            try
            {
                if (regObj.Equals(null))
                {
                    response.Status.Message.FriendlyMessage = "Error Occurred! Unable to proceed with your request";
                    response.Status.Message.TechnicalMessage = "Registration Object is empty / invalid";
                    return response;
                }

                if (!EntityValidatorHelper.Validate(regObj, out var valResults))
                {
                    var errorDetail = new StringBuilder();
                    if (!valResults.IsNullOrEmpty())
                    {
                        errorDetail.AppendLine("Following error occurred:");
                        valResults.ForEachx(m => errorDetail.AppendLine(m.ErrorMessage));
                    }

                    else
                    {
                        errorDetail.AppendLine(
                            "Validation error occurred! Please check all supplied parameters and try again");
                    }
                    response.Status.Message.FriendlyMessage = errorDetail.ToString();
                    response.Status.Message.TechnicalMessage = errorDetail.ToString();
                    response.Status.IsSuccessful = false;
                    return response;
                }

                if (!HelperMethods.IsUserValid(regObj.AdminUserId, regObj.SysPathCode,
                    HelperMethods.getStaffRoles(), ref response.Status.Message))
                    return response;


                if (DateTime.Parse(regObj.StartDate) > DateTime.Parse(regObj.EndDate))
                {
                    response.Status.Message.FriendlyMessage =
                        "Staff Key performance indicator Start Date cannot be greater than End Date";
                    response.Status.Message.TechnicalMessage =
                        "Staff Key performance indicator Start Date cannot be greater than End Date";
                    return response;
                }


                var staffKpIndex = new StaffKPIndex
                {
                    StaffId = regObj.StaffId,
                    KPIndexId = regObj.KPIndexId,
                    Description = regObj.Description,
                    StartDate = regObj.StartDate,
                    EndDate = regObj.EndDate,
                    Comment = regObj.Comment,
                    Rating = regObj.Rating,
                    SupervisorId = regObj.AdminUserId,
                    SupervisorRemarks = regObj.SupervisorRemarks,
                    RemarkTimeStamp = DateMap.CurrentTimeStamp(),
                    TimeStampRegistered = DateMap.CurrentTimeStamp()
                };
                var added = _staffKPIndexRepository.Add(staffKpIndex);
                _uoWork.SaveChanges();
                if (added.StaffKPIndexId < 1)
                {
                    response.Status.Message.FriendlyMessage =
                        "Error Occurred! Unable to complete your request. Please try again later";
                    response.Status.Message.TechnicalMessage = "Unable to save to database";
                    return response;
                }

                response.Status.IsSuccessful = true;
                response.SettingId = added.StaffKPIndexId;
            }
            catch (DbEntityValidationException ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                response.Status.Message.FriendlyMessage = "Error Occurred! Please try again later";
                response.Status.Message.TechnicalMessage = "Error: " + ex.GetBaseException().Message;
                response.Status.IsSuccessful = false;
                return response;
            }
            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                response.Status.Message.FriendlyMessage = "Error Occurred! Please try again later";
                response.Status.Message.TechnicalMessage = "Error: " + ex.GetBaseException().Message;
                response.Status.IsSuccessful = false;
                return response;
            }
            return response;
        }

        public SettingRegRespObj UpdateStaffKpiIndex(EditStaffKPIIndexObj regObj)
        {
            var response = new SettingRegRespObj
            {
                Status = new APIResponseStatus
                {
                    IsSuccessful = false,
                    Message = new APIResponseMessage()
                }
            };

            try
            {
                if (regObj.Equals(null))
                {
                    response.Status.Message.FriendlyMessage = "Error Occurred! Unable to proceed with your request";
                    response.Status.Message.TechnicalMessage = "Registration Object is empty / invalid";
                    return response;
                }

                if (!EntityValidatorHelper.Validate(regObj, out var valResults))
                {
                    var errorDetail = new StringBuilder();
                    if (!valResults.IsNullOrEmpty())
                    {
                        errorDetail.AppendLine("Following error occurred:");
                        valResults.ForEachx(m => errorDetail.AppendLine(m.ErrorMessage));
                    }

                    else
                    {
                        errorDetail.AppendLine(
                            "Validation error occurred! Please check all supplied parameters and try again");
                    }
                    response.Status.Message.FriendlyMessage = errorDetail.ToString();
                    response.Status.Message.TechnicalMessage = errorDetail.ToString();
                    response.Status.IsSuccessful = false;
                    return response;
                }

                if (!HelperMethods.IsUserValid(regObj.AdminUserId, regObj.SysPathCode,
                    HelperMethods.getStaffRoles(), ref response.Status.Message))
                    return response;
                var searchObj = new StaffKPIIndexSearchObj
                {
                    AdminUserId = 0,
                    StaffId = 0,
                    StaffKPIndexId = 0,
                    SysPathCode = ""
                };

                //retrieve previous contacts and validate with the new one
                var staffKPIndexIdList = getStaffKPIndexs(searchObj);

                if (staffKPIndexIdList == null || !staffKPIndexIdList.Any())
                {
                    response.Status.Message.FriendlyMessage = "No Staff Key performance Indicator  Information found";
                    response.Status.Message.TechnicalMessage = "No Staff Key performance Indicator  Information found";
                    return response;
                }
                var thisStaffKpIndex = staffKPIndexIdList.Find(m => m.StaffKPIndexId == searchObj.StaffKPIndexId);
                if (thisStaffKpIndex.StaffKPIndexId < 1)
                {
                    response.Status.Message.FriendlyMessage =
                        "This Staff Key performance Indicator  Information not found";
                    response.Status.Message.TechnicalMessage =
                        "This Staff Key performance Indicator  Information not found";
                    return response;
                }

                thisStaffKpIndex.StaffId = regObj.StaffId;
                thisStaffKpIndex.KPIndexId = regObj.KPIndexId;
                thisStaffKpIndex.Description = regObj.Description;
                thisStaffKpIndex.StartDate = regObj.StartDate;
                thisStaffKpIndex.EndDate = regObj.EndDate;
                thisStaffKpIndex.Comment = regObj.Comment;
                thisStaffKpIndex.Rating = regObj.Rating;
                thisStaffKpIndex.SupervisorRemarks = regObj.SupervisorRemarks;

                var updateStaffKpiIndex = _staffKPIndexRepository.Update(thisStaffKpIndex);
                _uoWork.SaveChanges();

                if (updateStaffKpiIndex.StaffKPIndexId < 1)
                {
                    response.Status.Message.FriendlyMessage =
                        "Error Occurred! Unable to complete your request. Please try again later";
                    response.Status.Message.TechnicalMessage = "Unable to save to database";
                    return response;
                }
                response.Status.IsSuccessful = true;
                response.SettingId = updateStaffKpiIndex.StaffKPIndexId;
            }
            catch (DbEntityValidationException ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                response.Status.Message.FriendlyMessage = "Error Occurred! Please try again later";
                response.Status.Message.TechnicalMessage = "Error: " + ex.GetBaseException().Message;
                response.Status.IsSuccessful = false;
                return response;
            }
            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                response.Status.Message.FriendlyMessage = "Error Occurred! Please try again later";
                response.Status.Message.TechnicalMessage = "Error: " + ex.GetBaseException().Message;
                response.Status.IsSuccessful = false;
                return response;
            }
            return response;
        }

        public StaffKPIIndexRespObj LoadStaffKpiIndex(StaffKPIIndexSearchObj searchObj)
        {
            var response = new StaffKPIIndexRespObj
            {
                Status = new APIResponseStatus
                {
                    IsSuccessful = false,
                    Message = new APIResponseMessage()
                }
            };

            try
            {
                if (searchObj.Equals(null))
                {
                    response.Status.Message.FriendlyMessage = "Error Occurred! Unable to proceed with your request";
                    response.Status.Message.TechnicalMessage = "Registration Object is empty / invalid";
                    return response;
                }

                if (!EntityValidatorHelper.Validate(searchObj, out var valResults))
                {
                    var errorDetail = new StringBuilder();
                    if (!valResults.IsNullOrEmpty())
                    {
                        errorDetail.AppendLine("Following error occurred:");
                        valResults.ForEachx(m => errorDetail.AppendLine(m.ErrorMessage));
                    }

                    else
                    {
                        errorDetail.AppendLine(
                            "Validation error occurred! Please check all supplied parameters and try again");
                    }
                    response.Status.Message.FriendlyMessage = errorDetail.ToString();
                    response.Status.Message.TechnicalMessage = errorDetail.ToString();
                    response.Status.IsSuccessful = false;
                    return response;
                }

                if (!HelperMethods.IsUserValid(searchObj.AdminUserId, searchObj.SysPathCode,
                    HelperMethods.getStaffRoles(), ref response.Status.Message))
                    return response;
                var thisKpiIndex = getStaffKPIndexs(searchObj);

                if (thisKpiIndex == null || !thisKpiIndex.Any())
                {
                    response.Status.Message.FriendlyMessage = "No Staff Key performance Indicator Information found!";
                    response.Status.Message.TechnicalMessage = "No Staff Key performance Indicator  Information found!";
                    return response;
                }


                var staffKpiIndexList = new List<StaffKPIIndexObj>();

                thisKpiIndex.ForEachx(m =>
                {
                    staffKpiIndexList.Add(new StaffKPIIndexObj
                    {
                        StaffKPIndexId = m.StaffKPIndexId,
                        StaffId = m.StaffId,
                        FirstName = new StaffRepository().getStaffInfo(m.StaffId).FirstName,
                        LastName = new StaffRepository().getStaffInfo(m.StaffId).LastName,
                        MiddleName = new StaffRepository().getStaffInfo(m.StaffId).MiddleName,
                        KPIndexId = m.KPIndexId,
                        KPIndexLabel = new KPIndexRepository().GetkPIndex(m.KPIndexId).Name,
                        Description = m.Description,
                        StartDate = m.StartDate,
                        EndDate = m.EndDate,
                        Comment = m.Comment,
                        Rating = m.Rating,
                        SupervisorId = m.SupervisorId,
                        SupervisorName = new StaffRepository().getStaffInfo(m.SupervisorId).FirstName,
                        SupervisorRemarks = m.SupervisorRemarks,
                        RemarkTimeStamp = m.RemarkTimeStamp,
                        TimeStampRegistered = m.TimeStampRegistered
                    });
                });

                response.Status.IsSuccessful = true;
                response.StaffKPIIndexs = staffKpiIndexList;
                return response;
            }

            catch (DbEntityValidationException ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                response.Status.Message.FriendlyMessage = "Error Occurred! Please try again later";
                response.Status.Message.TechnicalMessage = "Error: " + ex.GetBaseException().Message;
                response.Status.IsSuccessful = false;
                return response;
            }

            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                response.Status.Message.FriendlyMessage = "Error Occurred! Please try again later";
                response.Status.Message.TechnicalMessage = "Error: " + ex.GetBaseException().Message;
                response.Status.IsSuccessful = false;
                return response;
            }
        }

        #endregion


        #region StaffSalary

        public SettingRegRespObj AddStaffSalary(RegStaffSalaryObj regObj)
        {
            var response = new SettingRegRespObj
            {
                Status = new APIResponseStatus
                {
                    IsSuccessful = false,
                    Message = new APIResponseMessage()
                }
            };

            try
            {
                if (regObj.Equals(null))
                {
                    response.Status.Message.FriendlyMessage = "Error Occurred! Unable to proceed with your request";
                    response.Status.Message.TechnicalMessage = "Registration Object is empty / invalid";
                    return response;
                }

                if (!EntityValidatorHelper.Validate(regObj, out var valResults))
                {
                    var errorDetail = new StringBuilder();
                    if (!valResults.IsNullOrEmpty())
                    {
                        errorDetail.AppendLine("Following error occurred:");
                        valResults.ForEachx(m => errorDetail.AppendLine(m.ErrorMessage));
                    }

                    else
                    {
                        errorDetail.AppendLine(
                            "Validation error occurred! Please check all supplied parameters and try again");
                    }
                    response.Status.Message.FriendlyMessage = errorDetail.ToString();
                    response.Status.Message.TechnicalMessage = errorDetail.ToString();
                    response.Status.IsSuccessful = false;
                    return response;
                }

                if (!HelperMethods.IsUserValid(regObj.AdminUserId, regObj.SysPathCode,
                    HelperMethods.getStaffRoles(), ref response.Status.Message))
                    return response;
                var staffId = regObj.StaffId;
                if(IsStaffSalaryDuplicate(regObj.StaffId,1,ref response))
                {
                  return  response;
                }
                var staffSalaryList = getStaffSalaryInfo(staffId);

                if (staffSalaryList != null && staffSalaryList.Any())
                {
                    var defCount =
                        staffSalaryList.Count(m => m.StaffId == regObj.StaffId);
                    if (defCount > 1)
                    {
                        response.Status.Message.FriendlyMessage = "Staff Insurance Infomation already exist";
                        response.Status.Message.TechnicalMessage = "Staff Insurance Infomation already exist";
                        return response;
                    }
                }


                var totalDeduction = regObj.PayeDeduction + regObj.PensionDeduction + regObj.InsuranceDeduction;

                var staffTotalPayment = regObj.EducationAllowance + regObj.EntertainmentAllowance +
                                        regObj.FurnitureAllowance + regObj.HousingAllowance + regObj.LeaveAllowance +
                                        regObj.BasicAllowance + regObj.TransportAllowance + regObj.WardrobeAllowance;

                var staffJobInfoId = new StaffRepository().getStaffJobInfo(regObj.StaffId).StaffJobInfoId;

                if (staffJobInfoId < 1)
                {
                    response.Status.Message.FriendlyMessage = "Invalid Staff Job Id!";
                    response.Status.Message.TechnicalMessage = "Invalid Staff Job Id!";
                    return response;
                }

                var companyPensionDeduction = new StaffRepository().getStaffPension(regObj.StaffId).CompanyContribution;
                var personalPensionDeduction =
                    new StaffRepository().getStaffPension(regObj.StaffId).PersonalContribution;

                var totalPensionDeduction = companyPensionDeduction + personalPensionDeduction;

               

                if (regObj.PensionDeduction != totalPensionDeduction)
                {
                    response.Status.Message.FriendlyMessage = "Invalid Staff Pension Deduction!";
                    response.Status.Message.TechnicalMessage = "Invalid Staff Pension Deduction!";
                    return response;
                }
                if (regObj.TotalDeduction != totalDeduction)
                {
                    response.Status.Message.FriendlyMessage = "Invalid Staff Total Deduction!";
                    response.Status.Message.TechnicalMessage = "Invalid Staff Total Deduction!";
                    return response;
                }
                if (staffTotalPayment != regObj.TotalPayment)
                {
                    response.Status.Message.FriendlyMessage =
                        "Staff Total Payment is Incorrect,Please Check Calculation!";
                    response.Status.Message.TechnicalMessage =
                        "Staff Total Payment is Incorrect,Please Check Calculation!";
                    return response;
                }
                var staffSalary = new StaffSalary
                {
                    StaffId = regObj.StaffId,
                    StaffJobInfoId = staffJobInfoId,
                    BasicAllowance = regObj.BasicAllowance,
                    Currency = (Currency) regObj.Currency,
                    EducationAllowance = regObj.EducationAllowance,
                    EntertainmentAllowance = regObj.EntertainmentAllowance,
                    FurnitureAllowance = regObj.FurnitureAllowance,
                    HousingAllowance = regObj.HousingAllowance,
                    TransportAllowance = regObj.TransportAllowance,
                    WardrobeAllowance = regObj.WardrobeAllowance,
                    InsuranceDeduction = regObj.InsuranceDeduction,
                    LeaveAllowance = regObj.LeaveAllowance,
                    PayeDeduction = regObj.PayeDeduction,
                    PensionDeduction = regObj.PensionDeduction,
                    TotalDeduction = regObj.TotalDeduction,
                    TotalPayment = regObj.TotalPayment,
                    TimeStamRegistered = DateMap.CurrentTimeStamp(),
                    Status = (ItemStatus) regObj.Status
                };

                var added = _staffSalaryRepository.Add(staffSalary);
                _uoWork.SaveChanges();
                if (added.StaffSalaryId < 1)
                {
                    response.Status.Message.FriendlyMessage =
                        "Error Occurred! Unable to complete your request. Please try again later";
                    response.Status.Message.TechnicalMessage = "Unable to save to database";
                    return response;
                }
                response.Status.IsSuccessful = true;
                response.SettingId = added.StaffSalaryId;
            }
            catch (DbEntityValidationException ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                response.Status.Message.FriendlyMessage = "Error Occurred! Please try again later";
                response.Status.Message.TechnicalMessage = "Error: " + ex.GetBaseException().Message;
                response.Status.IsSuccessful = false;
                return response;
            }
            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                response.Status.Message.FriendlyMessage = "Error Occurred! Please try again later";
                response.Status.Message.TechnicalMessage = "Error: " + ex.GetBaseException().Message;
                response.Status.IsSuccessful = false;
                return response;
            }
            return response;
        }

        public SettingRegRespObj UpdateStaffSalary(UpdateStaffSalaryObj regObj)
        {
            var response = new SettingRegRespObj
            {
                Status = new APIResponseStatus
                {
                    IsSuccessful = false,
                    Message = new APIResponseMessage()
                }
            };

            try
            {
                if (regObj.Equals(null))
                {
                    response.Status.Message.FriendlyMessage = "Error Occurred! Unable to proceed with your request";
                    response.Status.Message.TechnicalMessage = "Registration Object is empty / invalid";
                    return response;
                }

                if (!EntityValidatorHelper.Validate(regObj, out var valResults))
                {
                    var errorDetail = new StringBuilder();
                    if (!valResults.IsNullOrEmpty())
                    {
                        errorDetail.AppendLine("Following error occurred:");
                        valResults.ForEachx(m => errorDetail.AppendLine(m.ErrorMessage));
                    }

                    else
                    {
                        errorDetail.AppendLine(
                            "Validation error occurred! Please check all supplied parameters and try again");
                    }
                    response.Status.Message.FriendlyMessage = errorDetail.ToString();
                    response.Status.Message.TechnicalMessage = errorDetail.ToString();
                    response.Status.IsSuccessful = false;
                    return response;
                }


                if (!HelperMethods.IsUserValid(regObj.AdminUserId, regObj.SysPathCode,
                    HelperMethods.getStaffRoles(), ref response.Status.Message))
                    return response;


                //var searchObj = new StaffSalarySearchObj
                //{
                //    AdminUserId = 0,
                //    StaffId = regObj.StaffId,
                //    StaffSalaryId = 0,
                //    Status = 0,
                //    SysPathCode = ""
                //};

                //retrieve previous contacts and validate with the new one

                if (IsStaffSalaryDuplicate(regObj.StaffId, 2, ref response))
                {
                    return response;
                }
                var staffSalaries = getStaffSalaryInfo(regObj.StaffId);
                if (staffSalaries == null || !staffSalaries.Any())
                {
                    response.Status.Message.FriendlyMessage = "No Staff Salary Information found";
                    response.Status.Message.TechnicalMessage = "No Staff Salary Information found";
                    return response;
                }
                var thisStaffSalaryFind = staffSalaries.Find(m => m.StaffSalaryId == regObj.StaffSalaryId);
                if (thisStaffSalaryFind.StaffSalaryId < 1)
                {
                    response.Status.Message.FriendlyMessage = "This Staff Salary Information not found";
                    response.Status.Message.TechnicalMessage = "This Staff Salary Information not found";
                    return response;
                }
                var totalDeduction = regObj.PayeDeduction + regObj.PensionDeduction + regObj.InsuranceDeduction;
                var staffTotalPayment = regObj.EducationAllowance + regObj.EntertainmentAllowance +
                                        regObj.FurnitureAllowance + regObj.HousingAllowance + regObj.LeaveAllowance +
                                        regObj.BasicAllowance + regObj.TransportAllowance + regObj.WardrobeAllowance;

               

                var staffJobInfoId = new StaffRepository().getStaffJobInfo(regObj.StaffId).StaffJobInfoId;
                if (staffJobInfoId < 1)
                {
                    response.Status.Message.FriendlyMessage = "Invalid Staff Job Id!";
                    response.Status.Message.TechnicalMessage = "Invalid Staff Job Id!";
                    return response;
                }
                var companyPensionDeduction = new StaffRepository().getStaffPension(regObj.StaffId).CompanyContribution;
                var personalPensionDeduction =
                    new StaffRepository().getStaffPension(regObj.StaffId).PersonalContribution;
                var totalPensionDeduction = companyPensionDeduction + personalPensionDeduction;
                if (regObj.PensionDeduction != totalPensionDeduction)
                {
                    response.Status.Message.FriendlyMessage = "Invalid Staff Pension Deduction!";
                    response.Status.Message.TechnicalMessage = "Invalid Staff Pension Deduction!";
                    return response;
                }
                if (regObj.TotalDeduction != totalDeduction)
                {
                    response.Status.Message.FriendlyMessage = "Invalid Staff Total Deduction!";
                    response.Status.Message.TechnicalMessage = "Invalid Staff Total Deduction!";
                    return response;
                }
                if (staffTotalPayment != regObj.TotalPayment)
                {
                    response.Status.Message.FriendlyMessage =
                        "Staff Total Payment is Incorrect,Please Check Calculation!";
                    response.Status.Message.TechnicalMessage =
                        "Staff Total Payment is Incorrect,Please Check Calculation!";
                    return response;
                }
                thisStaffSalaryFind.StaffJobInfoId = staffJobInfoId;
                thisStaffSalaryFind.BasicAllowance = regObj.BasicAllowance;
                thisStaffSalaryFind.Currency = (Currency) regObj.Currency;
                thisStaffSalaryFind.EducationAllowance = regObj.EducationAllowance;
                thisStaffSalaryFind.EntertainmentAllowance = regObj.EntertainmentAllowance;
                thisStaffSalaryFind.FurnitureAllowance = regObj.FurnitureAllowance;
                thisStaffSalaryFind.HousingAllowance = regObj.HousingAllowance;
                thisStaffSalaryFind.TransportAllowance = regObj.TransportAllowance;
                thisStaffSalaryFind.WardrobeAllowance = regObj.WardrobeAllowance;
                thisStaffSalaryFind.InsuranceDeduction = regObj.InsuranceDeduction;
                thisStaffSalaryFind.LeaveAllowance = regObj.LeaveAllowance;
                thisStaffSalaryFind.PayeDeduction = regObj.PayeDeduction;
                thisStaffSalaryFind.PensionDeduction = regObj.PensionDeduction;
                thisStaffSalaryFind.TotalDeduction = totalDeduction;
                thisStaffSalaryFind.TotalPayment = regObj.TotalPayment;
                thisStaffSalaryFind.Status = (ItemStatus) regObj.Status;

               
                var retVal = _staffSalaryRepository.Update(thisStaffSalaryFind);
                _uoWork.SaveChanges();
                if (retVal.StaffSalaryId < 1)
                {
                    response.Status.Message.FriendlyMessage =
                        "Error Occurred! Unable to complete your request. Please try again later";
                    response.Status.Message.TechnicalMessage = "Unable to save to database";
                    return response;
                }
                response.Status.IsSuccessful = true;
                response.SettingId = retVal.StaffSalaryId;
            }
            catch (DbEntityValidationException ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                response.Status.Message.FriendlyMessage = "Error Occurred! Please try again later";
                response.Status.Message.TechnicalMessage = "Error: " + ex.GetBaseException().Message;
                response.Status.IsSuccessful = false;
                return response;
            }
            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                response.Status.Message.FriendlyMessage = "Error Occurred! Please try again later";
                response.Status.Message.TechnicalMessage = "Error: " + ex.GetBaseException().Message;
                response.Status.IsSuccessful = false;
                return response;
            }
            return response;
        }

        public StaffSalaryRespObj LoadStaffSalary(StaffSalarySearchObj searchObj)
        {
            var response = new StaffSalaryRespObj
            {
                Status = new APIResponseStatus
                {
                    IsSuccessful = false,
                    Message = new APIResponseMessage()
                }
            };

            try
            {
                if (searchObj.Equals(null))
                {
                    response.Status.Message.FriendlyMessage = "Error Occurred! Unable to proceed with your request";
                    response.Status.Message.TechnicalMessage = "Registration Object is empty / invalid";
                    return response;
                }

                if (!EntityValidatorHelper.Validate(searchObj, out var valResults))
                {
                    var errorDetail = new StringBuilder();
                    if (!valResults.IsNullOrEmpty())
                    {
                        errorDetail.AppendLine("Following error occurred:");
                        valResults.ForEachx(m => errorDetail.AppendLine(m.ErrorMessage));
                    }

                    else
                    {
                        errorDetail.AppendLine(
                            "Validation error occurred! Please check all supplied parameters and try again");
                    }
                    response.Status.Message.FriendlyMessage = errorDetail.ToString();
                    response.Status.Message.TechnicalMessage = errorDetail.ToString();
                    response.Status.IsSuccessful = false;
                    return response;
                }

                if (!HelperMethods.IsUserValid(searchObj.AdminUserId, searchObj.SysPathCode,
                    HelperMethods.getStaffRoles(), ref response.Status.Message))
                    return response;
                var thisStaffSalaryInfos = GetStaffSalaryInfos(searchObj);

                if (thisStaffSalaryInfos == null || !thisStaffSalaryInfos.Any())
                {
                    response.Status.Message.FriendlyMessage = "No Staff Salary Information found!";
                    response.Status.Message.TechnicalMessage = "No Staff Leave Request  Information found!";
                    return response;
                }


                var staffSalaryList = new List<StaffSalaryObj>();

                thisStaffSalaryInfos.ForEachx(m =>
                {
                    staffSalaryList.Add(new StaffSalaryObj
                    {
                        StaffSalaryId = m.StaffSalaryId,
                        StaffId = m.StaffId,
                        FirstName = new StaffRepository().getStaffInfo(m.StaffId).FirstName,
                        LastName = new StaffRepository().getStaffInfo(m.StaffId).LastName,
                        MiddleName = new StaffRepository().getStaffInfo(m.StaffId).MiddleName,
                        StaffJobInfoId = m.StaffJobInfoId,
                        StaffJobInfoTitle = new StaffRepository().getStaffJobInfo(m.StaffId).JobTitle,
                        Currency = (int) m.Currency,
                        CurrencyName = m.Currency.ToString().Replace("_", " "),
                        BasicAllowance = m.BasicAllowance,
                        HousingAllowance = m.HousingAllowance,
                        EducationAllowance = m.EducationAllowance,
                        EntertainmentAllowance = m.EntertainmentAllowance,
                        LeaveAllowance = m.LeaveAllowance,
                        FurnitureAllowance = m.FurnitureAllowance,
                        WardrobeAllowance = m.WardrobeAllowance,
                        TransportAllowance = m.TransportAllowance,
                        PensionDeduction = m.PensionDeduction,
                        PayeDeduction = m.PayeDeduction,
                        TotalDeduction = m.TotalDeduction,
                        TotalPayment = m.TotalPayment,
                        TimeStamRegistered = m.TimeStamRegistered,
                        Status = (int) m.Status,
                        StatusLabel = m.Status.ToString().Replace("_", " ")
                    });
                });

                response.Status.IsSuccessful = true;
                response.StaffSalaries = staffSalaryList;
                return response;
            }

            catch (DbEntityValidationException ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                response.Status.Message.FriendlyMessage = "Error Occurred! Please try again later";
                response.Status.Message.TechnicalMessage = "Error: " + ex.GetBaseException().Message;
                response.Status.IsSuccessful = false;
                return response;
            }

            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                response.Status.Message.FriendlyMessage = "Error Occurred! Please try again later";
                response.Status.Message.TechnicalMessage = "Error: " + ex.GetBaseException().Message;
                response.Status.IsSuccessful = false;
                return response;
            }
        }

        #endregion

        #region StaffPension

        public SettingRegRespObj AddStaffPension(RegStaffPensionObj regObj)
        {
            var response = new SettingRegRespObj
            {
                Status = new APIResponseStatus
                {
                    IsSuccessful = false,
                    Message = new APIResponseMessage()
                }
            };

            try
            {
                if (regObj.Equals(null))
                {
                    response.Status.Message.FriendlyMessage = "Error Occurred! Unable to proceed with your request";
                    response.Status.Message.TechnicalMessage = "Registration Object is empty / invalid";
                    return response;
                }

                if (!EntityValidatorHelper.Validate(regObj, out var valResults))
                {
                    var errorDetail = new StringBuilder();
                    if (!valResults.IsNullOrEmpty())
                    {
                        errorDetail.AppendLine("Following error occurred:");
                        valResults.ForEachx(m => errorDetail.AppendLine(m.ErrorMessage));
                    }

                    else
                    {
                        errorDetail.AppendLine(
                            "Validation error occurred! Please check all supplied parameters and try again");
                    }
                    response.Status.Message.FriendlyMessage = errorDetail.ToString();
                    response.Status.Message.TechnicalMessage = errorDetail.ToString();
                    response.Status.IsSuccessful = false;
                    return response;
                }

                if (!HelperMethods.IsUserValid(regObj.AdminUserId, regObj.SysPathCode,
                    HelperMethods.getStaffRoles(), ref response.Status.Message))
                    return response;
                var search = new StaffPensionSearchObj
                {
                    AdminUserId = 0,
                    StaffId = regObj.AdminUserId,
                    StaffPensionId = 0,
                    PensionNumber = "",
                    SysPathCode = ""
                };

                var staffPensionInfo = getStaffPensions(search);
                if (staffPensionInfo != null && staffPensionInfo.Any())
                {
                    var myList = new List<PensionNumberHelper>();

                    staffPensionInfo.ForEachx(m =>
                    {
                        myList.Add(new PensionNumberHelper
                        {
                            PensionNumber = m.PensionNumber,
                            Id = m.StaffPensionId
                        });
                    });

                    myList.Add(new PensionNumberHelper {PensionNumber = regObj.PensionNumber, Id = myList.Count + 1});


                    var distinctAdd = myList.Distinct(new PensionNumberComparer());


                    if (staffPensionInfo.Count + 1 != distinctAdd.Count())

                    {
                        response.Status.Message.FriendlyMessage =
                            "Duplicate Error!  The Pension Number already exist";
                        response.Status.Message.TechnicalMessage =
                            "Duplicate Error!  The Pension Number already exist";
                        return response;
                    }
                }

                //retrieve previous contacts and validate with the new one

                var staffPension = new StaffPension
                {
                    StaffId = regObj.StaffId,
                    PensionNumber = regObj.PensionNumber,
                    CompanyContribution = regObj.CompanyContribution,
                    PersonalContribution = regObj.PersonalContribution,
                    PensionAdministratorId = regObj.PensionAdministratorId,
                    TimeStampRegister = DateMap.CurrentTimeStamp()
                };

                var added = _staffPensionRepository.Add(staffPension);
                _uoWork.SaveChanges();
                if (added.StaffPensionId < 1)
                {
                    response.Status.Message.FriendlyMessage =
                        "Error Occurred! Unable to complete your request. Please try again later";
                    response.Status.Message.TechnicalMessage = "Unable to save to database";
                    return response;
                }
                response.Status.IsSuccessful = true;
                response.SettingId = added.StaffPensionId;
            }
            catch (DbEntityValidationException ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                response.Status.Message.FriendlyMessage = "Error Occurred! Please try again later";
                response.Status.Message.TechnicalMessage = "Error: " + ex.GetBaseException().Message;
                response.Status.IsSuccessful = false;
                return response;
            }
            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                response.Status.Message.FriendlyMessage = "Error Occurred! Please try again later";
                response.Status.Message.TechnicalMessage = "Error: " + ex.GetBaseException().Message;
                response.Status.IsSuccessful = false;
                return response;
            }
            return response;
        }

        public SettingRegRespObj UpdateStaffPension(EditStaffPensionObj regObj)
        {
            var response = new SettingRegRespObj
            {
                Status = new APIResponseStatus
                {
                    IsSuccessful = false,
                    Message = new APIResponseMessage()
                }
            };

            try
            {
                if (regObj.Equals(null))
                {
                    response.Status.Message.FriendlyMessage = "Error Occurred! Unable to proceed with your request";
                    response.Status.Message.TechnicalMessage = "Registration Object is empty / invalid";
                    return response;
                }

                if (!EntityValidatorHelper.Validate(regObj, out var valResults))
                {
                    var errorDetail = new StringBuilder();
                    if (!valResults.IsNullOrEmpty())
                    {
                        errorDetail.AppendLine("Following error occurred:");
                        valResults.ForEachx(m => errorDetail.AppendLine(m.ErrorMessage));
                    }

                    else
                    {
                        errorDetail.AppendLine(
                            "Validation error occurred! Please check all supplied parameters and try again");
                    }
                    response.Status.Message.FriendlyMessage = errorDetail.ToString();
                    response.Status.Message.TechnicalMessage = errorDetail.ToString();
                    response.Status.IsSuccessful = false;
                    return response;
                }


                if (!HelperMethods.IsUserValid(regObj.AdminUserId, regObj.SysPathCode,
                    HelperMethods.getStaffRoles(), ref response.Status.Message))
                    return response;


                var search = new StaffPensionSearchObj
                {
                    AdminUserId = 0,
                    StaffId = regObj.StaffId,
                    StaffPensionId = 0,
                    PensionNumber = "",
                    SysPathCode = ""
                };

                var staffPensionInfo = getStaffPensions(search);
                if (staffPensionInfo == null || !staffPensionInfo.Any())
                {
                    response.Status.Message.FriendlyMessage = " Staff Pension Information found";
                    response.Status.Message.TechnicalMessage = " Staff Pension Information found";
                    return response;
                }
                var thisCurrentStaffPension =
                    staffPensionInfo.Find(m => m.StaffId == regObj.StaffId);

                if (thisCurrentStaffPension.StaffId < 1)
                {
                    response.Status.Message.FriendlyMessage = "This Staff Pension Information not found";
                    response.Status.Message.TechnicalMessage = "This Staff Pension Contact Information not found";
                    return response;
                }

                var myList = new List<PensionNumberHelper>();

                staffPensionInfo.ForEachx(m =>
                {
                    myList.Add(new PensionNumberHelper {PensionNumber = m.PensionNumber, Id = m.StaffPensionId});
                });

                myList.Add(new PensionNumberHelper {PensionNumber = regObj.PensionNumber, Id = myList.Count + 1});


                var distinctAdd = myList.Distinct(new PensionNumberComparer());


                if (staffPensionInfo.Count + 1 != distinctAdd.Count())
                {
                    response.Status.Message.FriendlyMessage =
                        "Duplicate Error!  The Pension Number already exist";
                    response.Status.Message.TechnicalMessage =
                        "Duplicate Error!  The Pension Number already exist";
                    return response;
                }

                thisCurrentStaffPension.PensionNumber = regObj.PensionNumber;
                thisCurrentStaffPension.CompanyContribution = regObj.CompanyContribution;
                thisCurrentStaffPension.PensionAdministratorId = regObj.PensionAdministratorId;


                var retVal = _staffPensionRepository.Update(thisCurrentStaffPension);
                _uoWork.SaveChanges();
                if (retVal.StaffPensionId < 1)
                {
                    response.Status.Message.FriendlyMessage =
                        "Error Occurred! Unable to complete your request. Please try again later";
                    response.Status.Message.TechnicalMessage = "Unable to save to database";
                    return response;
                }
                response.Status.IsSuccessful = true;
                response.SettingId = retVal.StaffPensionId;
            }
            catch (DbEntityValidationException ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                response.Status.Message.FriendlyMessage = "Error Occurred! Please try again later";
                response.Status.Message.TechnicalMessage = "Error: " + ex.GetBaseException().Message;
                response.Status.IsSuccessful = false;
                return response;
            }
            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                response.Status.Message.FriendlyMessage = "Error Occurred! Please try again later";
                response.Status.Message.TechnicalMessage = "Error: " + ex.GetBaseException().Message;
                response.Status.IsSuccessful = false;
                return response;
            }
            return response;
        }

        public StaffPensionRespObj LoadStaffPension(StaffPensionSearchObj searchObj)
        {
            var response = new StaffPensionRespObj
            {
                Status = new APIResponseStatus
                {
                    IsSuccessful = false,
                    Message = new APIResponseMessage()
                }
            };

            try
            {
                if (searchObj.Equals(null))
                {
                    response.Status.Message.FriendlyMessage = "Error Occurred! Unable to proceed with your request";
                    response.Status.Message.TechnicalMessage = "Registration Object is empty / invalid";
                    return response;
                }

                if (!EntityValidatorHelper.Validate(searchObj, out var valResults))
                {
                    var errorDetail = new StringBuilder();
                    if (!valResults.IsNullOrEmpty())
                    {
                        errorDetail.AppendLine("Following error occurred:");
                        valResults.ForEachx(m => errorDetail.AppendLine(m.ErrorMessage));
                    }

                    else
                    {
                        errorDetail.AppendLine(
                            "Validation error occurred! Please check all supplied parameters and try again");
                    }
                    response.Status.Message.FriendlyMessage = errorDetail.ToString();
                    response.Status.Message.TechnicalMessage = errorDetail.ToString();
                    response.Status.IsSuccessful = false;
                    return response;
                }

                if (!HelperMethods.IsUserValid(searchObj.AdminUserId, searchObj.SysPathCode,
                    HelperMethods.getStaffRoles(), ref response.Status.Message))
                    return response;

                var thisStaffPensions = getStaffPensions(searchObj);

                if (thisStaffPensions == null || !thisStaffPensions.Any())
                {
                    response.Status.Message.FriendlyMessage = "No Staff Pension Information found!";
                    response.Status.Message.TechnicalMessage = "No Staff Pension  Information found!";
                    return response;
                }


                var staffPensionsItems = new List<StaffPensionObj>();

                thisStaffPensions.ForEachx(m =>
                {
                    staffPensionsItems.Add(new StaffPensionObj
                    {
                        StaffPensionId = m.StaffPensionId,
                        StaffId = m.StaffId,
                        FirstName = new StaffRepository().getStaffInfo(m.StaffId).FirstName,
                        LastName = new StaffRepository().getStaffInfo(m.StaffId).LastName,
                        PensionNumber = m.PensionNumber,
                        PensionAdministratorId = m.PensionAdministratorId,
                        PensionAdministrator = new PensionAdministratorRepository()
                            .GetPensionAdministrator(m.PensionAdministratorId).Name,
                        CompanyContribution = m.CompanyContribution,
                        PersonalContribution = m.PersonalContribution,
                        TimeStampRegister = m.TimeStampRegister
                    });
                });

                response.Status.IsSuccessful = true;
                response.StaffPensions = staffPensionsItems;
                return response;
            }

            catch (DbEntityValidationException ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                response.Status.Message.FriendlyMessage = "Error Occurred! Please try again later";
                response.Status.Message.TechnicalMessage = "Error: " + ex.GetBaseException().Message;
                response.Status.IsSuccessful = false;
                return response;
            }

            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                response.Status.Message.FriendlyMessage = "Error Occurred! Please try again later";
                response.Status.Message.TechnicalMessage = "Error: " + ex.GetBaseException().Message;
                response.Status.IsSuccessful = false;
                return response;
            }
        }

        #endregion

        #region Staff Login

        private string RegisterLoginEvent(int staffId, string loginIP, bool success, ref APIResponseMessage msg)
        {
            try
            {
                var token = TransactionRefGenerator.GenerateAccessToken().ToLower();
                if (string.IsNullOrEmpty(token))
                    token = TransactionRefGenerator.GenerateAccessToken().ToLower();
                if (string.IsNullOrEmpty(token))
                {
                    msg.FriendlyMessage = "Unable to complete your login due to error";
                    msg.TechnicalMessage = "Unable to generate Token";
                    return "";
                }

                try
                {
                    var userActivity = new StaffLoginActivity
                    {
                        IsLoggedIn = success,
                        LoginTimeStamp = DateTime.Now.ToString("yyyy/MM/dd - hh:mm:ss tt"),
                        LoginAddress = loginIP,
                        LoginToken = success ? token : "",
                        IsTokenExpired = !success,
                        TokenExpiryDate = success ? DateTime.Now.AddHours(6).ToString("yyyy/MM/dd") : "",
                        StaffId = staffId
                    };

                    var retVal = _loginRepository.Add(userActivity);
                    _uoWork.SaveChanges();

                    if (retVal.StaffLoginActivityId < 1)
                    {
                        msg.FriendlyMessage = "Unable to complete login due to error";
                        msg.TechnicalMessage = "Unable to save login activity";
                        return "";
                    }
                }
                catch (DbEntityValidationException ex)
                {
                    msg.TechnicalMessage = "Error: " + ex.GetBaseException().Message;
                    msg.FriendlyMessage = "Unable to complete your request due to error! Please try again later";
                    ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                    return "";
                }
                catch (Exception ex)
                {
                    msg.TechnicalMessage = "Error: " + ex.GetBaseException().Message;
                    msg.FriendlyMessage = "Unable to complete your request due to error! Please try again later";
                    ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                    return "";
                }


                return token;
            }
            catch (DbEntityValidationException ex)
            {
                msg.TechnicalMessage = "Error: " + ex.GetBaseException().Message;
                msg.FriendlyMessage = "Unable to complete your request due to error! Please try again later";
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                return "";
            }
            catch (Exception ex)
            {
                msg.TechnicalMessage = "Error: " + ex.GetBaseException().Message;
                msg.FriendlyMessage = "Unable to complete your request due to error! Please try again later";
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                return "";
            }
        }

        private StaffLoginActivity GetStaffLogin(int staffId, string token)
        {
            try
            {
                var sql3 =
                    $"Select * FROM  \"GPlus\".\"StaffLoginActivity\"  WHERE \"StaffId\" = {staffId} AND lower(\"LoginToken\") = lower('{token}')";
                var check3 = _repository.RepositoryContext().Database.SqlQuery<StaffLoginActivity>(sql3).ToList();

                if (check3.IsNullOrEmpty() || check3.Count != 1)
                    return null;

                return check3[0];
            }
            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }

        internal bool IsTokenValid(int staffId, string token, string currentIP, bool check, ref APIResponseMessage msg)
        {
            try
            {
                if (string.IsNullOrEmpty(token) || token.Length != 32)
                {
                    msg.TechnicalMessage = msg.FriendlyMessage = "Empty / Invalid Token";
                    return false;
                }
                var loginActivity = GetStaffLogin(staffId, token);
                if (loginActivity == null || loginActivity.StaffLoginActivityId < 1)
                {
                    msg.TechnicalMessage = msg.FriendlyMessage = "Unable to login due to invalid Access Token";
                    return false;
                }
                if (!loginActivity.IsLoggedIn)
                {
                    msg.FriendlyMessage = "Invalid Access! Please re-login to your account";
                    msg.TechnicalMessage = "Login Failed with the supplied token";
                    return false;
                }
                if (loginActivity.IsTokenExpired)
                {
                    msg.FriendlyMessage = "Your Login Access has expired! Please re-login";
                    msg.TechnicalMessage = "Token has expired! Please re-login";
                    return false;
                }
                if (string.IsNullOrEmpty(loginActivity.TokenExpiryDate) || loginActivity.TokenExpiryDate.Length != 10)
                {
                    msg.FriendlyMessage = "Invalid Access! Please re-login to your account";
                    msg.TechnicalMessage = "Invalid Token Expiry Date";
                    return false;
                }
                if (
                    (DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd")) -
                     DateTime.Parse(loginActivity.TokenExpiryDate)).Days > 0)
                {
                    msg.FriendlyMessage = "Your Login Access has expired! Please re-login";
                    msg.TechnicalMessage = "Token has expired! Please re-login";
                    return false;
                }

                if (!check) return true;
                if (string.Compare(loginActivity.LoginAddress.Trim(), currentIP.Trim(),
                        StringComparison.CurrentCultureIgnoreCase) == 0) return true;

                msg.FriendlyMessage = "Your login already exist on a different computer";
                msg.TechnicalMessage = "Your login already exist on a different computer";
                return false;
            }
            catch (DbEntityValidationException ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.GetBaseException().Message);
                msg.TechnicalMessage = "Error: " + ex.GetBaseException().Message;
                msg.FriendlyMessage = "Unable to complete your request due to error! Please try again later";
                return false;
            }
            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                msg.TechnicalMessage = "Error: " + ex.GetBaseException().Message;
                msg.FriendlyMessage = "Unable to complete your request due to error! Please try again later";
                return false;
            }
        }

        #endregion

        #region Comment

        public SettingRegRespObj AddComment(RegCommentObj regObj)
        {
            var response = new SettingRegRespObj
            {
                Status = new APIResponseStatus
                {
                    IsSuccessful = false,
                    Message = new APIResponseMessage()
                }
            };

            try
            {
                if (regObj.Equals(null))
                {
                    response.Status.Message.FriendlyMessage = "Error Occurred! Unable to proceed with your request";
                    response.Status.Message.TechnicalMessage = "Registration Object is empty / invalid";
                    return response;
                }

                if (!EntityValidatorHelper.Validate(regObj, out var valResults))
                {
                    var errorDetail = new StringBuilder();
                    if (!valResults.IsNullOrEmpty())
                    {
                        errorDetail.AppendLine("Following error occurred:");
                        valResults.ForEachx(m => errorDetail.AppendLine(m.ErrorMessage));
                    }

                    else
                    {
                        errorDetail.AppendLine(
                            "Validation error occurred! Please check all supplied parameters and try again");
                    }
                    response.Status.Message.FriendlyMessage = errorDetail.ToString();
                    response.Status.Message.TechnicalMessage = errorDetail.ToString();
                    response.Status.IsSuccessful = false;
                    return response;
                }

                if (!HelperMethods.IsUserValid(regObj.AdminUserId, regObj.SysPathCode,
                    HelperMethods.getStaffRoles(), ref response.Status.Message))
                    return response;



                var comment = new Comment
                {
                    StaffId = regObj.StaffId,
                    CommentType = (CommentType)regObj.CommentType,
                    CommentDetails = regObj.CommentDetails,
                    TimeStampCommented = DateMap.CurrentTimeStamp()
                };
                var added = _adminCommmentRepository.Add(comment);
                _uoWork.SaveChanges();
                if (added.CommentId < 1)
                {
                    response.Status.Message.FriendlyMessage =
                        "Error Occurred! Unable to complete your request. Please try again later";
                    response.Status.Message.TechnicalMessage = "Unable to save to database";
                    return response;
                }

                response.Status.IsSuccessful = true;
                response.SettingId = added.CommentId;
            }
            catch (DbEntityValidationException ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                response.Status.Message.FriendlyMessage = "Error Occurred! Please try again later";
                response.Status.Message.TechnicalMessage = "Error: " + ex.GetBaseException().Message;
                response.Status.IsSuccessful = false;
                return response;
            }
            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                response.Status.Message.FriendlyMessage = "Error Occurred! Please try again later";
                response.Status.Message.TechnicalMessage = "Error: " + ex.GetBaseException().Message;
                response.Status.IsSuccessful = false;
                return response;
            }
            return response;
        }

        public CommentRespObj LoadComment(CommentSearchObj searchObj)
        {
            var response = new CommentRespObj
            {
                Status = new APIResponseStatus
                {
                    IsSuccessful = false,
                    Message = new APIResponseMessage()
                }
            };

            try
            {
                if (searchObj.Equals(null))
                {
                    response.Status.Message.FriendlyMessage = "Error Occurred! Unable to proceed with your request";
                    response.Status.Message.TechnicalMessage = "Registration Object is empty / invalid";
                    return response;
                }

                if (!EntityValidatorHelper.Validate(searchObj, out var valResults))
                {
                    var errorDetail = new StringBuilder();
                    if (!valResults.IsNullOrEmpty())
                    {
                        errorDetail.AppendLine("Following error occurred:");
                        valResults.ForEachx(m => errorDetail.AppendLine(m.ErrorMessage));
                    }

                    else
                    {
                        errorDetail.AppendLine(
                            "Validation error occurred! Please check all supplied parameters and try again");
                    }
                    response.Status.Message.FriendlyMessage = errorDetail.ToString();
                    response.Status.Message.TechnicalMessage = errorDetail.ToString();
                    response.Status.IsSuccessful = false;
                    return response;
                }


                if (!HelperMethods.IsStaffUserValid(searchObj.AdminUserId, searchObj.SysPathCode, searchObj.LoginIP,
                    false, out var staffInfo, ref response.Status.Message))
                    return response;
                if (staffInfo == null || staffInfo.StaffId < 1)
                {
                    response.Status.Message.FriendlyMessage = "Invalid Staff Information";
                    response.Status.Message.TechnicalMessage = "Invalid Staff Information";
                    return response;
                }

                var thisComments = GetCommentInfos(searchObj);

                if (thisComments == null || !thisComments.Any())
                {
                    response.Status.Message.FriendlyMessage = "No Staff Contact Information found!";
                    response.Status.Message.TechnicalMessage = "No Staff Contact  Information found!";
                    return response;
                }


                var comments = new List<CommentObj>();

                thisComments.ForEachx(m =>
                {
                    comments.Add(new CommentObj
                    {
                        CommentId = m.CommentId,
                        StaffId = m.StaffId,
                        FirstName = new StaffRepository().getStaffInfo(m.StaffId).FirstName,
                        LastName = new StaffRepository().getStaffInfo(m.StaffId).LastName,
                        CommentType = (int) m.CommentType,
                        CommentTypeLabel = m.CommentDetails.ToString().Replace("_", " "),
                        TimeStampCommented = m.TimeStampCommented,
                    });
                });

                response.Status.IsSuccessful = true;
                response.Comments = comments;
                return response;
            }

            catch (DbEntityValidationException ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                response.Status.Message.FriendlyMessage = "Error Occurred! Please try again later";
                response.Status.Message.TechnicalMessage = "Error: " + ex.GetBaseException().Message;
                response.Status.IsSuccessful = false;
                return response;
            }

            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                response.Status.Message.FriendlyMessage = "Error Occurred! Please try again later";
                response.Status.Message.TechnicalMessage = "Error: " + ex.GetBaseException().Message;
                response.Status.IsSuccessful = false;
                return response;
            }
        }

        #endregion
    }
}