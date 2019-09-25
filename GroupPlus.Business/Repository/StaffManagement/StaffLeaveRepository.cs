using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using GroupPlus.Business.Core;
using GroupPlus.Business.DataManager;
using GroupPlus.Business.Repository.Common;
using GroupPlus.Business.Repository.CompanyManagement;
using GroupPlus.BusinessContract.CommonAPIs;
using GroupPlus.BusinessObject.StaffManagement;
using GroupPlus.BussinessObject.Workflow;
using GroupPlus.Common;
using XPLUG.WEBTOOLS;

namespace GroupPlus.Business.Repository.StaffManagement
{
    internal partial class StaffRepository
    {
        #region LeaveRequest

        public SettingRegRespObj AddLeaveRequest(RegLeaveRequestObj regObj)
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

                if (staffInfo == null || staffInfo.StaffId < 1)
                {
                    response.Status.Message.FriendlyMessage = "Invalid Staff Information";
                    response.Status.Message.TechnicalMessage = "Invalid Staff Information";
                    return response;
                }
                var staffId = staffInfo.StaffId;


                if (DateTime.Parse(regObj.ProposedStartDate) >= DateTime.Parse(regObj.ProposedEndDate))
                {
                    response.Status.Message.FriendlyMessage = "Proposed Start Date cannot be greater than End Date";
                    response.Status.Message.TechnicalMessage = "Proposed Start Date cannot be greater than End Date";
                    return response;
                }

                if (DateTime.Parse(regObj.ProposedStartDate) < DateTime.Now)
                {
                    response.Status.Message.FriendlyMessage = "Proposed Start Date cannot be less than Present Date";
                    response.Status.Message.TechnicalMessage = "Proposed Start Date cannot be less than present Date";
                    return response;
                }
                var staffLeaveRequestInfo = getStaffLeaveRequest(staffId);

                if (staffLeaveRequestInfo != null && staffLeaveRequestInfo.Any())
                {
                    var leaveTypeValid = new LeaveTypeRepository().GetLeaveType(regObj.LeaveTypeId);

                    var leaveDays = HelperMethods.NumberOfDays(regObj.ProposedStartDate, regObj.ProposedEndDate);


                    if (leaveTypeValid.MinDays > leaveDays && leaveTypeValid.MaxDays < leaveDays)
                    {
                        response.Status.Message.FriendlyMessage =
                            $"Staff Leave Request not in the Range of Minimum Days {leaveTypeValid.MinDays} and Maximum Days {leaveTypeValid.MaxDays} Prescibed";
                        response.Status.Message.TechnicalMessage =
                            $"Staff Leave Request not in the Range of Minimum Days {leaveTypeValid.MinDays} and Maximum Days {leaveTypeValid.MaxDays} Prescibed";
                        return response;
                    }

                    if (IsStaffLeaveRequestDuplicate(staffInfo.StaffId, regObj.LeaveTitle, regObj.LeaveTypeId,
                        regObj.ProposedStartDate, regObj.ProposedEndDate, 1, ref response)) return response;
                }


                var staffLeaveRequest = new LeaveRequest
                {
                    StaffId = staffInfo.StaffId,
                    CompanyId = regObj.CompanyId,
                    DepartmentId = regObj.DepartmentId,
                    LeaveTitle = regObj.LeaveTitle,
                    ProposedStartDate = regObj.ProposedStartDate,
                    ProposedEndDate = regObj.ProposedEndDate,
                    Purpose = regObj.Purpose,
                    OtherRemarks = regObj.OtherRemarks,
                    LeaveType = regObj.LeaveTypeId,
                    Status = LeaveRequestStatus.Registering,
                    TimeStampRegistered = DateMap.CurrentTimeStamp()
                };

              

                using (var db = _uoWork.BeginTransaction())
                {
                    try
                    {
                        var added = _staffLeaveRequestRepository.Add(staffLeaveRequest);
                        _uoWork.SaveChanges();
                        if (added.LeaveRequestId < 1)
                        {
                            db.Rollback();
                            response.Status.Message.FriendlyMessage =
                                "Error Occurred! Unable to complete your request. Please try again later";
                            response.Status.Message.TechnicalMessage = "Unable to save to database";
                            return response;
                        }

                        workFlowSourceId = (int)added.LeaveRequestId;
                        var workflow = new WorkflowSetup
                        {
                            Description = regObj.LeaveTitle,
                            InitiatorId = staffInfo.StaffId,
                            InitiatorType = WorkflowInitiatorType.Staff,
                            Item = WorkflowItem.Staff_Leave,
                            WorkflowOrderId = regObj.WorkflowOrderId,
                            WorkflowSourceId = staffLeaveRequest.LeaveRequestId,
                            LastTimeStampModified = DateMap.CurrentTimeStamp(),
                            StaffId = staffInfo.StaffId,
                            TimeStampInitiated = DateMap.CurrentTimeStamp(),
                            Status = WorkflowStatus.Initiated
                        };

                        var retVal = _workflowSetupRepository.Add(workflow);
                        _uoWork.SaveChanges();
                        if (retVal.WorkflowSetupId < 1)
                        {
                            db.Rollback();
                            response.Status.Message.FriendlyMessage =
                                "Error Occurred! Unable to complete your request. Please try again later";
                            response.Status.Message.TechnicalMessage = "Unable to save to database";
                            return response;
                        }
                        response.Status.IsSuccessful = true;
                        workflow.WorkflowSourceId = added.LeaveRequestId;
                        db.Commit();
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


                //response.Status.IsSuccessful = true;
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

        public SettingRegRespObj UpdateLeaveRequest(EditLeaveRequestObj regObj)
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

                //var searchObj = new LeaveRequestSearchObj
                //{
                //    AdminUserId = 0,
                //    LeaveType = 0,
                //    LeaveRequestId = 0,
                //    StaffId = regObj.AdminUserId,
                //    LeaveTitle = "",
                //    SysPathCode = "",
                //};
                var staffId = staffInfo.StaffId;
                //retrieve previous contacts and validate with the new one
                var staffLeaveRequest = getStaffLeaveRequest(staffId);
                if (staffLeaveRequest == null || !staffLeaveRequest.Any())
                {
                    response.Status.Message.FriendlyMessage = "No Staff Leave Request Information found";
                    response.Status.Message.TechnicalMessage = "No Staff Leave Request Information found";
                    return response;
                }
                if (DateTime.Parse(regObj.ProposedStartDate) >= DateTime.Parse(regObj.ProposedEndDate))
                {
                    response.Status.Message.FriendlyMessage = "Proposed Start Date cannot be greater than End Date";
                    response.Status.Message.TechnicalMessage = "Proposed Start Date cannot be greater than End Date";
                    return response;
                }

                if (IsStaffLeaveRequestDuplicate(staffInfo.StaffId, regObj.LeaveTitle, regObj.LeaveTypeId,
                    regObj.ProposedStartDate, regObj.ProposedEndDate, 2, ref response)) return response;

                var thisLeaveRequest = staffLeaveRequest.Find(m => m.LeaveRequestId == regObj.LeaveRequestId);
                if (thisLeaveRequest.LeaveRequestId < 1)
                {
                    response.Status.Message.FriendlyMessage = "This Staff Leave Request Information not found";
                    response.Status.Message.TechnicalMessage = "This Staff Leave Request Information not found";
                    return response;
                }

                if (thisLeaveRequest.Status == LeaveRequestStatus.Registered)
                {
                    response.Status.Message.FriendlyMessage = "This Staff Leave Request Information Cannot Edited due to the fact that it has been registered";
                    response.Status.Message.TechnicalMessage = "This Staff Leave Request Information Cannot Edited due to the fact that it has been registered";
                    return response;
                }
                var leaveTypeValid = new LeaveTypeRepository().GetLeaveType(regObj.LeaveTypeId);
                var leaveDays = HelperMethods.NumberOfDays(regObj.ProposedStartDate, regObj.ProposedEndDate);

                if (DateTime.Parse(regObj.ProposedStartDate) < DateTime.Now)
                {
                    response.Status.Message.FriendlyMessage = "Proposed Start Date cannot be less than Present Date";
                    response.Status.Message.TechnicalMessage = "Proposed Start Date cannot be less than present Date";
                    return response;
                }

                if (leaveTypeValid.MinDays > leaveDays && leaveTypeValid.MaxDays < leaveDays)
                {
                    response.Status.Message.FriendlyMessage =
                        $"Staff Leave Request not in the Range of Minimum Days {leaveTypeValid.MinDays} and Maximum Days {leaveTypeValid.MaxDays} Prescibed";
                    response.Status.Message.TechnicalMessage =
                        $"Staff Leave Request not in the Range of Minimum Days {leaveTypeValid.MinDays} and Maximum Days {leaveTypeValid.MaxDays} Prescibed";
                    return response;
                }

                thisLeaveRequest.StaffId = staffInfo.StaffId;
                thisLeaveRequest.CompanyId = regObj.CompanyId;
                thisLeaveRequest.DepartmentId = regObj.DepartmentId;
                thisLeaveRequest.LeaveTitle = regObj.LeaveTitle;
                thisLeaveRequest.LeaveType = regObj.LeaveTypeId;
                thisLeaveRequest.ProposedStartDate = regObj.ProposedStartDate;
                thisLeaveRequest.ProposedEndDate = regObj.ProposedEndDate;
                thisLeaveRequest.Purpose = regObj.Purpose;
                thisLeaveRequest.OtherRemarks = regObj.OtherRemarks;

                var retVal = _staffLeaveRequestRepository.Update(thisLeaveRequest);
                _uoWork.SaveChanges();
                if (retVal.LeaveRequestId < 1)
                {
                    response.Status.Message.FriendlyMessage =
                        "Error Occurred! Unable to complete your request. Please try again later";
                    response.Status.Message.TechnicalMessage = "Unable to save to database";
                    return response;
                }
                response.Status.IsSuccessful = true;
                response.SettingId = retVal.LeaveRequestId;
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
        public SettingRegRespObj DeleteLeaveRequest(DeleteLeaveRequestObj regObj)
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

                if (!HelperMethods.IsUserValid(regObj.AdminUserId, regObj.SysPathCode, HelperMethods.getStaffRoles(),
                    ref response.Status.Message))
                    return response;
                var thisLeaveRequest = GetStaffLeaveRequestInfo(regObj.LeaveRequestId);

                if (thisLeaveRequest == null)
                {
                    response.Status.Message.FriendlyMessage = "No Staff Leave Request Information found for the specified Bank Id";
                    response.Status.Message.TechnicalMessage = "No Staff Leave Request Information found!";
                    return response;
                }

                if (thisLeaveRequest.Status == LeaveRequestStatus.Registered)
                {
                    response.Status.Message.FriendlyMessage = "This Staff Leave Request Information Cannot Deleted due to the fact that it has been registered";
                    response.Status.Message.TechnicalMessage = "This Staff Leave Request Information Cannot Deleted due to the fact that it has been registered";
                    return response;
                }

                thisLeaveRequest.LeaveTitle = thisLeaveRequest.LeaveTitle + "_Deleted_" + DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss");
                thisLeaveRequest.Status = LeaveRequestStatus.Deleted;
                var added = _staffLeaveRequestRepository.Update(thisLeaveRequest);
                _uoWork.SaveChanges();

                if (added.LeaveRequestId < 1)
                {
                    response.Status.Message.FriendlyMessage =
                        "Error Occurred! Unable to complete your request. Please try again later";
                    response.Status.Message.TechnicalMessage = "Unable to save to database";
                    return response;
                }
               
                response.Status.IsSuccessful = true;
                response.SettingId = added.LeaveRequestId;
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
        public LeaveRequestRespObj LoadLeaveRequestByAdmin(LeaveRequestSearchObj searchObj)
        {
            var response = new LeaveRequestRespObj
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
                    HelperMethods.getAdminRoles(), ref response.Status.Message))
                    return response;
                var thisLeaveRequests = GetLeaveRequestInfos(searchObj);

                if (thisLeaveRequests == null || !thisLeaveRequests.Any())
                {
                    response.Status.Message.FriendlyMessage = "No Staff Leave Request Information found!";
                    response.Status.Message.TechnicalMessage = "No Staff Leave Request  Information found!";
                    return response;
                }


                var leaveRequestsList = new List<LeaveRequestObj>();

                thisLeaveRequests.ForEachx(m =>
                {
                    leaveRequestsList.Add(new LeaveRequestObj
                    {
                        LeaveRequestId = m.LeaveRequestId,
                        StaffId = m.StaffId,
                        FirstName = new StaffRepository().getStaffInfo(m.StaffId).FirstName,
                        LastName = new StaffRepository().getStaffInfo(m.StaffId).LastName,
                        MiddleName = new StaffRepository().getStaffInfo(m.StaffId).MiddleName,
                        CompanyId = m.CompanyId,
                        Company= new CompanyRepository().GetCompany(m.CompanyId).Name,
                        DepartmentId = m.DepartmentId,
                        Department = new DepartmentRepository().GetDepartment(m.CompanyId).Name,
                        LeaveTypeId = m.LeaveType,
                        LeaveTitle = m.LeaveTitle,
                        LeaveType = new LeaveTypeRepository().GetLeaveType(m.LeaveType).Name,
                        ProposedStartDate = m.ProposedStartDate,
                        ProposedEndDate = m.ProposedEndDate,
                        Purpose = m.Purpose,
                        OtherRemarks = m.OtherRemarks,
                        Status = (int)m.Status,
                        StatusLabel = m.Status.ToString().Replace("_"," "),
                        TimeStampRegistered = m.TimeStampRegistered
                    });
                });

                response.Status.IsSuccessful = true;
                response.LeaveRequests = leaveRequestsList;
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

        #region StaffMemoResponse

        public SettingRegRespObj AddStaffMemoResponse(RegStaffMemoResponseObj regObj)
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

                //retrieve previous contacts and validate with the new one
                var staffMemos = getStaffMemos(staffInfo.StaffId);
                if (staffMemos == null || !staffMemos.Any())
                {
                    response.Status.Message.FriendlyMessage = "No Staff Memo Information found";
                    response.Status.Message.TechnicalMessage = "No Staff Memo Information found";
                    return response;
                }
                var thisMemoFind = staffMemos.Find(m => m.StaffId == staffInfo.StaffId);
                var staffWorkflowlog = GetWorkflowLog(thisMemoFind.StaffId);
                var thisStaffWorkFlowLog = staffWorkflowlog.Find(m => m.StaffId == thisMemoFind.StaffId);
                if (thisMemoFind.StaffId < 1)
                {
                    response.Status.Message.FriendlyMessage = "This Staff Memo Information not found";
                    response.Status.Message.TechnicalMessage = "This Staff Memo Request Information not found";
                    return response;
                }
                if (thisStaffWorkFlowLog.StaffId < 1)
                {
                    response.Status.Message.FriendlyMessage = "This Staff Work flow Log  Information not found";
                    response.Status.Message.TechnicalMessage = "This Staff Work flow Log Information not found";
                    return response;
                }

                var staffMemoResponse = new StaffMemoResponse
                {
                    StaffId = thisMemoFind.StaffId,
                    StaffMemoId = thisMemoFind.StaffMemoId,
                    MemoResponse = regObj.MemoResponse,
                    IssuerId = thisMemoFind.RegisterBy,
                    IssuerRemarks = thisMemoFind.MemoDetail,
                    IssuerRemarkTimeStamp = thisMemoFind.TimeStampRegister,
                    ManagementRemarks = "",
                    ManagementRemarksBy = 0,
                    ManagementRemarkTimeStamp = "",
                    TimeStampRegister = DateMap.CurrentTimeStamp()
                };

                thisStaffWorkFlowLog.WorkflowOrderItemId = 0;
                thisStaffWorkFlowLog.ApprovalType = 0;
                thisStaffWorkFlowLog.ProcessorId = staffInfo.StaffId;
                thisStaffWorkFlowLog.Comment = regObj.MemoResponse;
                thisStaffWorkFlowLog.LogTimeStamp = DateMap.CurrentTimeStamp();
                thisStaffWorkFlowLog.Status = 0;

                thisMemoFind.IsReplied = true;

                using (var db = _uoWork.BeginTransaction())
                {
                    try
                    {
                        var added = _staffMemoResponseRepository.Add(staffMemoResponse);
                        _uoWork.SaveChanges();
                        if (added.StaffMemoResponseId < 1)
                        {
                            response.Status.Message.FriendlyMessage =
                                "Error Occurred! Unable to complete your request. Please try again later";
                            response.Status.Message.TechnicalMessage = "Unable to save to database";
                            return response;
                        }


                        var retVal = _workflowLogRepository.Update(thisStaffWorkFlowLog);
                        _uoWork.SaveChanges();
                        if (retVal.WorkflowLogId < 1)
                        {
                            db.Rollback();
                            response.Status.Message.FriendlyMessage =
                                "Error Occurred! Unable to complete your request. Please try again later";
                            response.Status.Message.TechnicalMessage = "Unable to save to database";
                            return response;
                        }

                        var memoUpdate = _staffMemoRepository.Update(thisMemoFind);
                        _uoWork.SaveChanges();
                        if (memoUpdate.StaffMemoId < 1)
                        {
                            db.Rollback();
                            response.Status.Message.FriendlyMessage =
                                "Error Occurred! Unable to complete your request. Please try again later";
                            response.Status.Message.TechnicalMessage = "Unable to save to database";
                            return response;
                        }
                        response.Status.IsSuccessful = true;
                        response.SettingId = added.StaffMemoResponseId;
                        thisStaffWorkFlowLog.WorkflowLogId = thisStaffWorkFlowLog.WorkflowLogId;
                        db.Commit();
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

        public SettingRegRespObj ApproveStaffMemoResponse(ApprovaStaffMemoResponseObj regObj)
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


                //var searchObj = new StaffMemoResponseSearchObj
                //{
                //    AdminUserId = 0,
                //    StaffMemoId = 0,
                //    StaffMemoResponseId = 0,
                //    StaffId = regObj.AdminUserId,
                //    Status = 0,
                //    SysPathCode = ""
                //};

                //retrieve previous contacts and validate with the new one
                var staffMemoResponses = getStaffMemoResponses(regObj.StaffId);
                if (staffMemoResponses == null || !staffMemoResponses.Any())
                {
                    response.Status.Message.FriendlyMessage = "No Staff Memo Information found";
                    response.Status.Message.TechnicalMessage = "No Staff Memo Information found";
                    return response;
                }
                var thisMemoResponseFind = staffMemoResponses.Find(m => m.StaffId == regObj.StaffId);
                var staffWorkflowlog = GetWorkflowLog(thisMemoResponseFind.StaffId);
                var thisStaffWorkFlowLog = staffWorkflowlog.Find(m => m.StaffId == thisMemoResponseFind.StaffId);

                if (thisMemoResponseFind.StaffId < 1)
                {
                    response.Status.Message.FriendlyMessage = "This Staff Memo Response Information not found";
                    response.Status.Message.TechnicalMessage = "This Staff Memo Response Information not found";
                    return response;
                }
                if (thisStaffWorkFlowLog.StaffId < 1)
                {
                    response.Status.Message.FriendlyMessage = "This Staff Work flow Log  Information not found";
                    response.Status.Message.TechnicalMessage = "This Staff Work flow Log Information not found";
                    return response;
                }


                thisMemoResponseFind.ManagementRemarks = regObj.ManagementRemarks;
                thisMemoResponseFind.ManagementRemarksBy = regObj.AdminUserId;
                thisMemoResponseFind.ManagementRemarkTimeStamp = DateMap.CurrentTimeStamp();

                thisStaffWorkFlowLog.WorkflowOrderItemId = regObj.WorkflowOrderItemId;
                thisStaffWorkFlowLog.ApprovalType = WorkflowApprovalType.HR_Head;
                thisStaffWorkFlowLog.ProcessorId = regObj.AdminUserId;
                thisStaffWorkFlowLog.Comment = regObj.ManagementRemarks;
                thisStaffWorkFlowLog.LogTimeStamp = DateMap.CurrentTimeStamp();
                thisStaffWorkFlowLog.Status = (ApprovalStatus) regObj.Status;

                using (var db = _uoWork.BeginTransaction())
                {
                    try
                    {
                        var updateResponse = _staffMemoResponseRepository.Update(thisMemoResponseFind);
                        _uoWork.SaveChanges();
                        if (updateResponse.StaffMemoResponseId < 1)
                        {
                            response.Status.Message.FriendlyMessage =
                                "Error Occurred! Unable to complete your request. Please try again later";
                            response.Status.Message.TechnicalMessage = "Unable to save to database";
                            return response;
                        }


                        var retVal = _workflowLogRepository.Update(thisStaffWorkFlowLog);
                        _uoWork.SaveChanges();
                        if (retVal.WorkflowLogId < 1)
                        {
                            db.Rollback();
                            response.Status.Message.FriendlyMessage =
                                "Error Occurred! Unable to complete your request. Please try again later";
                            response.Status.Message.TechnicalMessage = "Unable to save to database";
                            return response;
                        }
                        response.Status.IsSuccessful = true;
                        response.SettingId = updateResponse.StaffMemoResponseId;
                        thisStaffWorkFlowLog.WorkflowLogId = thisStaffWorkFlowLog.WorkflowLogId;
                        db.Commit();
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

        public StaffMemoResponseRespObj LoadStaffMemoResponseByAdmin(StaffMemoResponseSearchObj searchObj)
        {
            var response = new StaffMemoResponseRespObj
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
                    HelperMethods.getAllRoles(), ref response.Status.Message))
                    return response;
                var thisStaffs = getStaffMemoResponse(searchObj);
                if (thisStaffs == null || !thisStaffs.Any())
                {
                    response.Status.Message.FriendlyMessage = "No Memo Response Information found!";
                    response.Status.Message.TechnicalMessage = "No Memo Response  Information found!";
                    return response;
                }


                var staffItems = new List<StaffMemoResponseObj>();
                thisStaffs.ForEachx(m =>
                {
                    staffItems.Add(new StaffMemoResponseObj
                    {
                        StaffId = m.StaffId,
                        StaffMemoId = m.StaffMemoId,
                        FirstName = new StaffRepository().getStaffInfo(m.StaffId).FirstName,
                        LastName = new StaffRepository().getStaffInfo(m.StaffId).LastName,
                        MiddleName = new StaffRepository().getStaffInfo(m.StaffId).MiddleName,
                        StaffMemoResponseId = m.StaffMemoResponseId,
                        MemoTitle = new StaffRepository().getStaffMemos(m.StaffId)[0].Title,
                        MemoResponse = m.MemoResponse,
                        IssuerId = m.IssuerId,
                        IssuerRemarks = m.IssuerRemarks,
                        IssuerRemarkTimeStamp = m.IssuerRemarkTimeStamp,
                        ManagementRemarksBy = m.ManagementRemarksBy,
                        ManagementRemarks = m.ManagementRemarks,
                        ManagementRemarkTimeStamp = m.ManagementRemarkTimeStamp,
                        TimeStampRegister = m.TimeStampRegister
                    });
                });

                response.Status.IsSuccessful = true;
                response.StaffMemoResponses = staffItems;
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

        public StaffMemoResponseRespObj LoadStaffMemoResponse(StaffMemoResponseSearchObj searchObj)
        {
            var response = new StaffMemoResponseRespObj
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
                var thisStaffs = getStaffMemoResponse(searchObj);
                if (thisStaffs == null || !thisStaffs.Any())
                {
                    response.Status.Message.FriendlyMessage = "No Memo Response Information found!";
                    response.Status.Message.TechnicalMessage = "No Memo Response  Information found!";
                    return response;
                }


                var staffItems = new List<StaffMemoResponseObj>();
                thisStaffs.ForEachx(m =>
                {
                    staffItems.Add(new StaffMemoResponseObj
                    {
                        StaffId = m.StaffId,
                        StaffMemoId = m.StaffMemoId,
                        FirstName = new StaffRepository().getStaffInfo(m.StaffId).FirstName,
                        LastName = new StaffRepository().getStaffInfo(m.StaffId).LastName,
                        MiddleName = new StaffRepository().getStaffInfo(m.StaffId).MiddleName,
                        StaffMemoResponseId = m.StaffMemoResponseId,
                        MemoTitle = new StaffRepository().getStaffMemos(m.StaffId)[0].Title,
                        MemoResponse = m.MemoResponse,
                        IssuerId = m.IssuerId,
                        IssuerRemarks = m.IssuerRemarks,
                        IssuerRemarkTimeStamp = m.IssuerRemarkTimeStamp,
                        ManagementRemarksBy = m.ManagementRemarksBy,
                        ManagementRemarks = m.ManagementRemarks,
                        ManagementRemarkTimeStamp = m.ManagementRemarkTimeStamp,
                        TimeStampRegister = m.TimeStampRegister
                    });
                });

                response.Status.IsSuccessful = true;
                response.StaffMemoResponses = staffItems;
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

        #region StaffLeave

        public SettingRegRespObj AddStaffLeave(RegStaffLeaveObj regObj)
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
                var leaveRequestId = regObj.LeaveRequestId;
                //retrieve previous contacts and validate with the new one
                var staffLeaveRequest = getStaffLeaveRequestInfo(staffId, leaveRequestId);
                if (staffLeaveRequest == null || !staffLeaveRequest.Any())
                {
                    response.Status.Message.FriendlyMessage = "No Staff Leave Request Information found";
                    response.Status.Message.TechnicalMessage = "No Staff Leave Request Information found";
                    return response;
                }


                var thisLeaveRequest = staffLeaveRequest.Find(m => m.LeaveRequestId == regObj.LeaveRequestId && m.StaffId == regObj.StaffId);
                var workFlowSetup = GetWorkflowSetup(thisLeaveRequest.StaffId);
                var thisWorkFlowSetup = workFlowSetup.Find(m => m.StaffId == thisLeaveRequest.StaffId);
                if (thisWorkFlowSetup.WorkflowSetupId < 1)
                {
                    response.Status.Message.FriendlyMessage = "This WorkFlow Information not found";
                    response.Status.Message.TechnicalMessage = "This WorkFlow Information not found";
                    return response;
                }

                if (thisLeaveRequest.Status == LeaveRequestStatus.Registered)
                {
                    response.Status.Message.FriendlyMessage = "This Staff Leave Request Information Cannot Registered due to the fact that it has been registered";
                    response.Status.Message.TechnicalMessage = "This Staff Leave Request Information Cannot Registered due to the fact that it has been registered";
                    return response;
                }
                if (thisLeaveRequest.LeaveRequestId < 1)
                {
                    response.Status.Message.FriendlyMessage = "This Staff Leave Request Information not found";
                    response.Status.Message.TechnicalMessage = "This Staff Leave Request Information not found";
                    return response;
                }
                if (IsStaffLeaveDuplicate(thisLeaveRequest.StaffId, thisLeaveRequest.LeaveRequestId,
                    thisLeaveRequest.LeaveType, thisLeaveRequest.ProposedStartDate, thisLeaveRequest.ProposedEndDate, 1,
                    ref response)) return response;


                var staffLeave = new StaffLeave
                {
                    StaffId = thisLeaveRequest.StaffId,
                    LeaveRequestId = thisLeaveRequest.LeaveRequestId,
                    ProposedStartDate = thisLeaveRequest.ProposedStartDate,
                    ProposedEndDate = thisLeaveRequest.ProposedEndDate,
                    LeaveTypeId = thisLeaveRequest.LeaveType,
                    TimeStampRequested = thisLeaveRequest.TimeStampRegistered,
                    HODApprovedBy = 0,
                    LMApprovedBy = regObj.AdminUserId,
                    HRApprovedBy = 0,
                    HODApprovedStartDate = " ",
                    HODApprovedEndDate = " ",
                    LMApprovedStartDate = " ",
                    LMApprovedEndDate = " ",
                    HRApprovedStartDate = " ",
                    HRApprovedEndDate = " ",
                    HODApprovedTimeStamp = " ",
                    LMApprovedTimeStamp = " ",
                    HRApprovedTimeStamp = " ",
                    HODComment = " ",
                    HRComment = " ",
                    LMComment = " ",
                    Status = LeaveStatus.Registered
                };

                var workFlowLog = new WorkflowLog
                {
                    WorkflowSetupId =
                        new StaffRepository().GetWorkflowSetupInfo(thisLeaveRequest.StaffId).WorkflowSetupId,
                    ApprovalType = WorkflowApprovalType.Line_Manager,
                    StaffId = thisLeaveRequest.StaffId,
                    ProcessorId = regObj.AdminUserId,
                    WorkflowOrderItemId = new WorkflowOrderItemRepository()
                        .GetWorkflowOrderItem((int) WorkflowApprovalType.Line_Manager).WorkflowOrderItemId,
                    Status = ApprovalStatus.Registered,
                    Comment = "",
                    LogTimeStamp = DateMap.CurrentTimeStamp()
                };

                thisWorkFlowSetup.Status = WorkflowStatus.Approval_In_Progress;

                thisLeaveRequest.Status = LeaveRequestStatus.Registered;

                using (var db = _uoWork.BeginTransaction())
                {
                    try
                    {
                        var added = _staffLeaveRepository.Add(staffLeave);
                        _uoWork.SaveChanges();
                        if (added.StaffLeaveId < 1)
                        {
                            db.Rollback();
                            response.Status.Message.FriendlyMessage =
                                "Error Occurred! Unable to complete your request. Please try again later";
                            response.Status.Message.TechnicalMessage = "Unable to save to database";
                            return response;
                        }


                        var retVal = _workflowLogRepository.Add(workFlowLog);
                        _uoWork.SaveChanges();
                        if (retVal.WorkflowLogId < 1)
                        {
                            db.Rollback();
                            response.Status.Message.FriendlyMessage =
                                "Error Occurred! Unable to complete your request. Please try again later";
                            response.Status.Message.TechnicalMessage = "Unable to save to database";
                            return response;
                        }

                        var updateSetup = _workflowSetupRepository.Update(thisWorkFlowSetup);
                        if (updateSetup.WorkflowSetupId < 1)
                        {
                            db.Rollback();
                            response.Status.Message.FriendlyMessage =
                                "Error Occurred! Unable to complete your request. Please try again later";
                            response.Status.Message.TechnicalMessage = "Unable to save to database";
                            return response;
                        }

                        var updateLeaverequest = _staffLeaveRequestRepository.Update(thisLeaveRequest);
                        if (updateLeaverequest.LeaveRequestId < 1)
                        {
                            db.Rollback();
                            response.Status.Message.FriendlyMessage =
                                "Error Occurred! Unable to complete your request. Please try again later";
                            response.Status.Message.TechnicalMessage = "Unable to save to database";
                            return response;
                        }
                        response.Status.IsSuccessful = true;
                        response.SettingId = added.StaffLeaveId;
                        workFlowLog.WorkflowLogId = workFlowLog.WorkflowLogId;
                        db.Commit();
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

        public SettingRegRespObj ApproveStaffLeave(ApproveStaffLeaveObj regObj)
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
                //var searchObj = new StaffLeaveSearchObj
                //{
                //    AdminUserId = 0,
                //    LeaveTypeId = 0,
                //    LeaveRequestId = 0,
                //    StaffId = 0,
                //    StaffLeaveId = regObj.,
                //    SysPathCode = "",
                //};

                //retrieve previous contacts and validate with the new one
                var staffLeaveList = GetStaffLeave(regObj.StaffLeaveId);

                if (staffLeaveList == null || !staffLeaveList.Any())
                {
                    response.Status.Message.FriendlyMessage = "No Staff Leave Request Information found";
                    response.Status.Message.TechnicalMessage = "No Staff Leave Request Information found";
                    return response;
                }
                var thisStaffLeave = staffLeaveList.Find(m => m.StaffLeaveId == regObj.StaffLeaveId);

                var staffWorkflowlog = GetWorkflowLog(thisStaffLeave.StaffId);

                var thisStaffWorkFlowLog = staffWorkflowlog.Find(m => m.StaffId == thisStaffLeave.StaffId);

                var workFlowSetup = GetWorkflowSetup(thisStaffLeave.StaffId);

                var thisWorkFlowSetup = workFlowSetup.Find(m => m.StaffId == thisStaffLeave.StaffId);

                if (thisWorkFlowSetup.WorkflowSetupId < 1)
                {
                    response.Status.Message.FriendlyMessage = "This WorkFlow Setup Information not found";
                    response.Status.Message.TechnicalMessage = "This WorkFlow Setup Information not found";
                    return response;
                }
                if (thisStaffLeave.StaffLeaveId < 1)
                {
                    response.Status.Message.FriendlyMessage = "This Staff Leave  Information not found";
                    response.Status.Message.TechnicalMessage = "This Staff Leave  Information not found";
                    return response;
                }

                if (thisStaffWorkFlowLog.StaffId < 1)
                {
                    response.Status.Message.FriendlyMessage = "This Staff Work flow Log  Information not found";
                    response.Status.Message.TechnicalMessage = "This Staff Work flow Log Information not found";
                    return response;
                }

                if (regObj.IsLMApproval)
                {
                    if (thisStaffLeave.Status != LeaveStatus.Registered)
                    {
                        response.Status.Message.FriendlyMessage = "Staff Leave Request not Registered! ";
                        response.Status.Message.TechnicalMessage = "Staff Leave Request not Registered! ";
                        return response;
                    }
                    if (DateTime.Parse(regObj.LMApprovedStartDate) > DateTime.Parse(regObj.LMApprovedEndDate))
                    {
                        response.Status.Message.FriendlyMessage =
                            "LMApproved Start Date cannot be greater than End Date";
                        response.Status.Message.TechnicalMessage =
                            "LMApproved Start Date cannot be greater than End Date";
                        return response;
                    }

                    var leaveTypeValid = new LeaveTypeRepository().GetLeaveType(thisStaffLeave.LeaveTypeId);
                    var leaveDays = HelperMethods.NumberOfDays(regObj.LMApprovedStartDate, regObj.LMApprovedStartDate);

                    if (leaveDays < leaveTypeValid.MinDays || leaveDays > leaveTypeValid.MaxDays)
                    {
                        response.Status.Message.FriendlyMessage =
                            "Staff Leave days cannot be greater or less than the Recommended Days";
                        response.Status.Message.TechnicalMessage =
                            "Staff Leave days cannot be greater or less than the Recommended Days";
                        return response;
                    }

                    thisStaffLeave.LMApprovedBy = regObj.AdminUserId;
                    thisStaffLeave.LMApprovedStartDate = regObj.LMApprovedStartDate;
                    thisStaffLeave.LMApprovedEndDate = regObj.LMApprovedEndDate;
                    thisStaffLeave.LMApprovedTimeStamp = DateMap.CurrentTimeStamp();
                    thisStaffLeave.LMComment = regObj.LMComment;
                    thisStaffLeave.Status = (LeaveStatus) regObj.Status;

                    thisStaffWorkFlowLog.WorkflowOrderItemId = regObj.WorkflowOrderItemId;
                    thisStaffWorkFlowLog.ApprovalType = WorkflowApprovalType.Line_Manager;
                    thisStaffWorkFlowLog.ProcessorId = regObj.AdminUserId;
                    thisStaffWorkFlowLog.Comment = regObj.LMComment;
                    thisStaffWorkFlowLog.LogTimeStamp = DateMap.CurrentTimeStamp();
                    if (thisStaffLeave.Status == LeaveStatus.HOD_Approved)
                    {
                        thisStaffWorkFlowLog.Status = ApprovalStatus.Pending;
                        thisWorkFlowSetup.Status = WorkflowStatus.Approval_In_Progress;
                    }
                    else
                    {
                        thisStaffWorkFlowLog.Status = ApprovalStatus.Denied;
                        thisWorkFlowSetup.Status = WorkflowStatus.On_Hold;
                    }


                    using (var db = _uoWork.BeginTransaction())
                    {
                        try
                        {
                            var added = _staffLeaveRepository.Update(thisStaffLeave);
                            _uoWork.SaveChanges();
                            if (added.StaffLeaveId < 1)
                            {
                                db.Rollback();
                                response.Status.Message.FriendlyMessage =
                                    "Error Occurred! Unable to complete your request. Please try again later";
                                response.Status.Message.TechnicalMessage = "Unable to save to database";
                                return response;
                            }


                            var retVal = _workflowLogRepository.Update(thisStaffWorkFlowLog);
                            _uoWork.SaveChanges();
                            if (retVal.WorkflowLogId < 1)
                            {
                                db.Rollback();
                                response.Status.Message.FriendlyMessage =
                                    "Error Occurred! Unable to complete your request. Please try again later";
                                response.Status.Message.TechnicalMessage = "Unable to save to database";
                                return response;
                            }

                            var updateSetup = _workflowSetupRepository.Update(thisWorkFlowSetup);
                            if (updateSetup.WorkflowSetupId < 1)
                            {
                                db.Rollback();
                                response.Status.Message.FriendlyMessage =
                                    "Error Occurred! Unable to complete your request. Please try again later";
                                response.Status.Message.TechnicalMessage = "Unable to save to database";
                                return response;
                            }
                            response.Status.IsSuccessful = true;
                            response.SettingId = added.StaffLeaveId;
                            thisStaffWorkFlowLog.WorkflowLogId = thisStaffWorkFlowLog.WorkflowLogId;
                            db.Commit();
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

                    //var added = _staffLeaveRepository.Update(thisStaffLeave);
                    //_uoWork.SaveChanges();
                    //if (added.StaffLeaveId < 1)
                    //{
                    //    response.Status.Message.FriendlyMessage =
                    //        "Error Occurred! Unable to complete your request. Please try again later";
                    //    response.Status.Message.TechnicalMessage = "Unable to save to database";
                    //    return response;
                    //}
                    //response.Status.IsSuccessful = true;
                    //response.SettingId = added.StaffLeaveId;
                }

                if (regObj.IsHODApproval)
                {
                    if (thisStaffLeave.Status != LeaveStatus.LM_Approved ||
                        thisStaffLeave.Status != LeaveStatus.LM_Denied)
                    {
                        response.Status.Message.FriendlyMessage = "Staff Leave Request not Authorise by Line Manager!";
                        response.Status.Message.TechnicalMessage = "Staff Leave Request not Authorise by Line Manager!";
                        return response;
                    }

                    if (thisStaffWorkFlowLog.Status != ApprovalStatus.Approved ||
                        thisStaffWorkFlowLog.Status != ApprovalStatus.Denied)
                    {
                        response.Status.Message.FriendlyMessage = "Staff Leave Request not Authorise by Line Manager!";
                        response.Status.Message.TechnicalMessage = "Staff Leave Request not Authorise by Line Manager!";
                        return response;
                    }
                    if (DateTime.Parse(regObj.HODApprovedStartDate) > DateTime.Parse(regObj.HODApprovedEndDate))
                    {
                        response.Status.Message.FriendlyMessage =
                            "HODApproved Start Date cannot be greater than End Date";
                        response.Status.Message.TechnicalMessage =
                            "HODApproved Start Date cannot be greater than End Date";
                        return response;
                    }

                    var leaveTypeValid = new LeaveTypeRepository().GetLeaveType(thisStaffLeave.LeaveTypeId);
                    var leaveDays =
                        HelperMethods.NumberOfDays(regObj.HODApprovedStartDate, regObj.HODApprovedStartDate);

                    if (leaveDays < leaveTypeValid.MinDays || leaveDays > leaveTypeValid.MaxDays)
                    {
                        response.Status.Message.FriendlyMessage =
                            "Staff Leave days cannot be greater or less than the Recommended Days";
                        response.Status.Message.TechnicalMessage =
                            "Staff Leave days cannot be greater or less than the Recommended Days";
                        return response;
                    }

                    thisStaffLeave.HODApprovedBy = regObj.AdminUserId;
                    thisStaffLeave.HODApprovedStartDate = regObj.HODApprovedStartDate;
                    thisStaffLeave.HODApprovedEndDate = regObj.HODApprovedEndDate;
                    thisStaffLeave.HODApprovedTimeStamp = DateMap.CurrentTimeStamp();
                    thisStaffLeave.HODComment = regObj.HODComment;
                    thisStaffLeave.Status = (LeaveStatus) regObj.Status;


                    thisStaffWorkFlowLog.WorkflowOrderItemId = thisStaffWorkFlowLog.WorkflowOrderItemId + 1;
                    thisStaffWorkFlowLog.ApprovalType = WorkflowApprovalType.Unit_Head;
                    thisStaffWorkFlowLog.ProcessorId = regObj.AdminUserId;
                    thisStaffWorkFlowLog.Comment = regObj.HODComment;
                    thisStaffWorkFlowLog.LogTimeStamp = DateMap.CurrentTimeStamp();


                    if (thisStaffLeave.Status == LeaveStatus.HOD_Approved)
                    {
                        thisStaffWorkFlowLog.Status = ApprovalStatus.Pending;
                        thisWorkFlowSetup.Status = WorkflowStatus.Approval_In_Progress;
                    }
                    else
                    {
                        thisStaffWorkFlowLog.Status = ApprovalStatus.Denied;
                        thisWorkFlowSetup.Status = WorkflowStatus.On_Hold;
                    }


                    using (var db = _uoWork.BeginTransaction())
                    {
                        try
                        {
                            var added = _staffLeaveRepository.Update(thisStaffLeave);
                            _uoWork.SaveChanges();
                            if (added.StaffLeaveId < 1)
                            {
                                db.Rollback();
                                response.Status.Message.FriendlyMessage =
                                    "Error Occurred! Unable to complete your request. Please try again later";
                                response.Status.Message.TechnicalMessage = "Unable to save to database";
                                return response;
                            }


                            var retVal = _workflowLogRepository.Update(thisStaffWorkFlowLog);
                            _uoWork.SaveChanges();
                            if (retVal.WorkflowLogId < 1)
                            {
                                db.Rollback();
                                response.Status.Message.FriendlyMessage =
                                    "Error Occurred! Unable to complete your request. Please try again later";
                                response.Status.Message.TechnicalMessage = "Unable to save to database";
                                return response;
                            }

                            var updateSetup = _workflowSetupRepository.Update(thisWorkFlowSetup);
                            if (updateSetup.WorkflowSetupId < 1)
                            {
                                db.Rollback();
                                response.Status.Message.FriendlyMessage =
                                    "Error Occurred! Unable to complete your request. Please try again later";
                                response.Status.Message.TechnicalMessage = "Unable to save to database";
                                return response;
                            }
                            response.Status.IsSuccessful = true;
                            response.SettingId = added.StaffLeaveId;
                            thisStaffWorkFlowLog.WorkflowLogId = thisStaffWorkFlowLog.WorkflowLogId;
                            db.Commit();
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

                if (regObj.IsHRApproval)
                {
                    if (thisStaffLeave.Status != LeaveStatus.HOD_Approved ||
                        thisStaffLeave.Status != LeaveStatus.HOD_Denied)
                    {
                        response.Status.Message.FriendlyMessage = "Staff Leave Request not Authorise by Unit Head Yet!";
                        response.Status.Message.TechnicalMessage =
                            "Staff Leave Request not Authorise by Unit Head Yet!";
                        return response;
                    }

                    if (thisStaffWorkFlowLog.Status != ApprovalStatus.Approved ||
                        thisStaffWorkFlowLog.Status != ApprovalStatus.Denied)
                    {
                        response.Status.Message.FriendlyMessage = "Staff Leave Request not Authorise by HR!";
                        response.Status.Message.TechnicalMessage = "Staff Leave Request not Authorise by HR!";
                        return response;
                    }

                    if (DateTime.Parse(regObj.HODApprovedStartDate) > DateTime.Parse(regObj.HODApprovedEndDate))
                    {
                        response.Status.Message.FriendlyMessage =
                            "HRApproved Start Date cannot be greater than End Date";
                        response.Status.Message.TechnicalMessage =
                            "HRApproved Start Date cannot be greater than End Date";
                        return response;
                    }
                    var leaveTypeValid = new LeaveTypeRepository().GetLeaveType(thisStaffLeave.LeaveTypeId);
                    var leaveDays = HelperMethods.NumberOfDays(regObj.HODApprovedStartDate, regObj.HODApprovedEndDate);

                    if (leaveDays < leaveTypeValid.MinDays || leaveDays > leaveTypeValid.MaxDays)
                    {
                        response.Status.Message.FriendlyMessage =
                            "Staff Leave days cannot be greater or less than the Recommended Days";
                        response.Status.Message.TechnicalMessage =
                            "Staff Leave days cannot be greater or less than the Recommended Days";
                        return response;
                    }

                    thisStaffLeave.HODApprovedBy = regObj.AdminUserId;
                    thisStaffLeave.HODApprovedStartDate = regObj.HRApprovedStartDate;
                    thisStaffLeave.HODApprovedEndDate = regObj.HRApprovedEndDate;
                    thisStaffLeave.HODApprovedTimeStamp = DateMap.CurrentTimeStamp();
                    thisStaffLeave.HODComment = regObj.HRComment;
                    thisStaffLeave.Status = (LeaveStatus) regObj.Status;


                    thisStaffWorkFlowLog.WorkflowOrderItemId = thisStaffWorkFlowLog.WorkflowOrderItemId + 1;
                    thisStaffWorkFlowLog.ApprovalType = WorkflowApprovalType.HR_Head;
                    thisStaffWorkFlowLog.ProcessorId = regObj.AdminUserId;
                    thisStaffWorkFlowLog.Comment = regObj.HRComment;
                    thisStaffWorkFlowLog.LogTimeStamp = DateMap.CurrentTimeStamp();
                    if (thisStaffLeave.Status == LeaveStatus.Approved)
                    {
                        thisStaffWorkFlowLog.Status = ApprovalStatus.Approved;
                        thisWorkFlowSetup.Status = WorkflowStatus.Completed;
                    }
                    else
                    {
                        thisStaffWorkFlowLog.Status = ApprovalStatus.Denied;
                        thisWorkFlowSetup.Status = WorkflowStatus.Stopped;
                    }
                    using (var db = _uoWork.BeginTransaction())
                    {
                        try
                        {
                            var added = _staffLeaveRepository.Update(thisStaffLeave);
                            _uoWork.SaveChanges();
                            if (added.StaffLeaveId < 1)
                            {
                                db.Rollback();
                                response.Status.Message.FriendlyMessage =
                                    "Error Occurred! Unable to complete your request. Please try again later";
                                response.Status.Message.TechnicalMessage = "Unable to save to database";
                                return response;
                            }


                            var retVal = _workflowLogRepository.Update(thisStaffWorkFlowLog);
                            _uoWork.SaveChanges();
                            if (retVal.WorkflowLogId < 1)
                            {
                                db.Rollback();
                                response.Status.Message.FriendlyMessage =
                                    "Error Occurred! Unable to complete your request. Please try again later";
                                response.Status.Message.TechnicalMessage = "Unable to save to database";
                                return response;
                            }
                            response.Status.IsSuccessful = true;
                            response.SettingId = added.StaffLeaveId;
                            thisStaffWorkFlowLog.WorkflowLogId = thisStaffWorkFlowLog.WorkflowLogId;
                            db.Commit();
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

        public StaffLeaveRespObj LoadStaffLeaveByAdmin(StaffLeaveSearchObj searchObj)
        {
            var response = new StaffLeaveRespObj
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
                var thisStaffLeaves = GetStaffLeaveInfos(searchObj);

                if (thisStaffLeaves == null || !thisStaffLeaves.Any())
                {
                    response.Status.Message.FriendlyMessage = "No Staff Leave Information found!";
                    response.Status.Message.TechnicalMessage = "No Staff Leave  Information found!";
                    return response;
                }


                var staffLeaveList = new List<StaffLeaveObj>();

                thisStaffLeaves.ForEachx(m =>
                {
                    var thisStaffLeaveInfo = getStaffLeaveRequestInfo(m.StaffId, (int) m.LeaveRequestId);
                    staffLeaveList.Add(new StaffLeaveObj
                    {
                        StaffLeaveId = m.StaffLeaveId,
                        LeaveRequestId = m.LeaveRequestId,
                        StaffId = m.StaffId,
                        FirstName = new StaffRepository().getStaffInfo(m.StaffId).FirstName,
                        LastName = new StaffRepository().getStaffInfo(m.StaffId).LastName,
                        MiddleName = new StaffRepository().getStaffInfo(m.StaffId).MiddleName,
                        DepartmentId = thisStaffLeaveInfo[0].DepartmentId,
                        Department = new DepartmentRepository().GetDepartment(thisStaffLeaveInfo[0].DepartmentId).Name,
                        CompanyId = thisStaffLeaveInfo[0].CompanyId,
                        Company = new CompanyRepository().GetCompany(thisStaffLeaveInfo[0].CompanyId).Name,
                        LeaveTitle = thisStaffLeaveInfo[0].LeaveTitle,
                        LeaveTypeId = thisStaffLeaveInfo[0].LeaveType,
                        LeaveType = new LeaveTypeRepository().GetLeaveType(thisStaffLeaveInfo[0].LeaveType).Name,
                        Purpose = thisStaffLeaveInfo[0].Purpose,
                        OtherRemarks = thisStaffLeaveInfo[0].OtherRemarks,
                        ProposedStartDate = m.ProposedStartDate,
                        ProposedEndDate = m.ProposedEndDate,
                        TimeStampRegistered = m.TimeStampRequested,
                        HRApprovedBy = m.HRApprovedBy,
                        LMApprovedBy = m.LMApprovedBy,
                        HODApprovedBy = m.HODApprovedBy,
                        HRApprovedStartDate = m.HRApprovedStartDate,
                        HRApprovedEndDate = m.HRApprovedEndDate,
                        LMApprovedStartDate = m.LMApprovedStartDate,
                        LMApprovedEndDate = m.LMApprovedEndDate,
                        HODApprovedStartDate = m.HODApprovedStartDate,
                        HODApprovedEndDate = m.HODApprovedEndDate,
                        LMApprovedTimeStamp = m.LMApprovedTimeStamp,
                        HODApprovedTimeStamp = m.HODApprovedTimeStamp,
                        HRApprovedTimeStamp = m.HRApprovedTimeStamp,
                        HRComment = m.HRComment,
                        LMComment = m.LMComment,
                        HODComment = m.HODComment,
                        Status = (int) m.Status,
                        StatusLabel = m.Status.ToString().Replace("_", " ")
                    });
                });

                response.Status.IsSuccessful = true;
                response.StaffLeaves = staffLeaveList;
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

        public LeaveRequestRespObj LoadStaffLeaveRequest(LeaveRequestSearchObj searchObj)
        {
            var response = new LeaveRequestRespObj
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
                var thisStaffLeaverequest = GetLeaveRequestInfos(searchObj);

                if (thisStaffLeaverequest == null || !thisStaffLeaverequest.Any())
                {
                    response.Status.Message.FriendlyMessage = "No Staff Leave Information found!";
                    response.Status.Message.TechnicalMessage = "No Staff Leave  Information found!";
                    return response;
                }


                var staffLeaveRequestList = new List<LeaveRequestObj>();

                thisStaffLeaverequest.ForEachx(m =>
                {
                    staffLeaveRequestList.Add(new LeaveRequestObj
                    {
                        LeaveRequestId = m.LeaveRequestId,
                        StaffId = m.StaffId,
                        FirstName = new StaffRepository().getStaffInfo(m.StaffId).FirstName,
                        LastName = new StaffRepository().getStaffInfo(m.StaffId).LastName,
                        MiddleName = new StaffRepository().getStaffInfo(m.StaffId).MiddleName,
                        DepartmentId = m.DepartmentId,
                        Department = new DepartmentRepository().GetDepartment(m.DepartmentId).Name,
                        CompanyId = m.CompanyId,
                        Company = new CompanyRepository().GetCompany(m.CompanyId).Name,
                        LeaveTitle = m.LeaveTitle,
                        LeaveTypeId = m.LeaveType,
                        LeaveType = new LeaveTypeRepository().GetLeaveType(m.LeaveType).Name,
                        Purpose = m.Purpose,
                        OtherRemarks = m.OtherRemarks,
                        ProposedStartDate = m.ProposedStartDate,
                        ProposedEndDate = m.ProposedEndDate,
                        Status = (int)m.Status,
                        StatusLabel = m.Status.ToString().Replace("_", " "),
                        TimeStampRegistered = m.TimeStampRegistered
                    });
                });

                response.Status.IsSuccessful = true;
                response.LeaveRequests = staffLeaveRequestList;
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

        public WorkFlowLogRespObj LoadWorkFlowByAdmin(WorkFlowLogSearchObj searchObj)
        {
            var response = new WorkFlowLogRespObj
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
                var thisWorkFlowLogs = GetStaffLeaveInfos(searchObj);

                if (thisWorkFlowLogs == null || !thisWorkFlowLogs.Any())
                {
                    response.Status.Message.FriendlyMessage = "No Work Flow Log Information found!";
                    response.Status.Message.TechnicalMessage = "No Work Flow Log  Information found!";
                    return response;
                }


                var workFlowLogList = new List<WorkFlowLogObj>();

                thisWorkFlowLogs.ForEachx(m =>
                {
                    workFlowLogList.Add(new WorkFlowLogObj
                    {
                        WorkflowLogId = m.WorkflowLogId,
                        StaffId = m.StaffId,
                        StaffName = new StaffRepository().getStaffInfo(m.StaffId).LastName,
                        WorkflowOrderItemId = m.WorkflowOrderItemId,
                        WorkflowSetupId = m.WorkflowSetupId,
                        ApprovalType = m.ApprovalType,
                        ProcessorId = m.ProcessorId,
                        Comment = m.Comment,
                        LogTimeStamp = m.LogTimeStamp,
                        Status = (int) m.Status,
                        StatusLabel = m.Status.ToString().Replace("_", " ")
                    });
                });

                response.Status.IsSuccessful = true;
                response.WorkFlowLogs = workFlowLogList;
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

        public WorkFlowLogRespObj LoadWorkFlow(WorkFlowLogSearchObj searchObj)
        {
            var response = new WorkFlowLogRespObj
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
                var thisWorkFlowLogs = GetStaffLeaveInfos(searchObj);

                if (thisWorkFlowLogs == null || !thisWorkFlowLogs.Any())
                {
                    response.Status.Message.FriendlyMessage = "No Work Flow Log Information found!";
                    response.Status.Message.TechnicalMessage = "No Work Flow Log  Information found!";
                    return response;
                }


                var workFlowLogList = new List<WorkFlowLogObj>();

                thisWorkFlowLogs.ForEachx(m =>
                {
                    workFlowLogList.Add(new WorkFlowLogObj
                    {
                        WorkflowLogId = m.WorkflowLogId,
                        StaffId = m.StaffId,
                        StaffName = new StaffRepository().getStaffInfo(m.StaffId).LastName,
                        WorkflowOrderItemId = m.WorkflowOrderItemId,
                        WorkflowSetupId = m.WorkflowSetupId,
                        ApprovalType = m.ApprovalType,
                        ProcessorId = m.ProcessorId,
                        Comment = m.Comment,
                        LogTimeStamp = m.LogTimeStamp,
                        Status = (int) m.Status,
                        StatusLabel = m.Status.ToString().Replace("_", " ")
                    });
                });

                response.Status.IsSuccessful = true;
                response.WorkFlowLogs = workFlowLogList;
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

        public StaffLeaveRespObj LoadStaffLeave(StaffLeaveSearchObj searchObj)
        {
            var response = new StaffLeaveRespObj
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
                var thisStaffLeaves = GetStaffLeaveInfos(searchObj);


                if (thisStaffLeaves == null || !thisStaffLeaves.Any())
                {
                    response.Status.Message.FriendlyMessage = "No Staff Leave Information found!";
                    response.Status.Message.TechnicalMessage = "No Staff Leave  Information found!";
                    return response;
                }


                var staffLeaveList = new List<StaffLeaveObj>();

                thisStaffLeaves.ForEachx(m =>
                {
                    var thisStaffLeaveInfo = getStaffLeaveRequestInfo(m.StaffId, (int) m.LeaveRequestId);
                    staffLeaveList.Add(new StaffLeaveObj
                    {
                        LeaveRequestId = m.LeaveRequestId,
                        StaffId = m.StaffId,
                        FirstName = new StaffRepository().getStaffInfo(m.StaffId).FirstName,
                        LastName = new StaffRepository().getStaffInfo(m.StaffId).LastName,
                        MiddleName = new StaffRepository().getStaffInfo(m.StaffId).MiddleName,
                        DepartmentId = thisStaffLeaveInfo[0].DepartmentId,
                        Department = new DepartmentRepository().GetDepartment(thisStaffLeaveInfo[0].DepartmentId).Name,
                        CompanyId = thisStaffLeaveInfo[0].CompanyId,
                        Company = new CompanyRepository().GetCompany(thisStaffLeaveInfo[0].CompanyId).Name,
                        LeaveTitle = thisStaffLeaveInfo[0].LeaveTitle,
                        LeaveTypeId = thisStaffLeaveInfo[0].LeaveType,
                        LeaveType = new LeaveTypeRepository().GetLeaveType(thisStaffLeaveInfo[0].LeaveType).Name,
                        Purpose = thisStaffLeaveInfo[0].Purpose,
                        OtherRemarks = thisStaffLeaveInfo[0].OtherRemarks,
                        ProposedStartDate = m.ProposedStartDate,
                        ProposedEndDate = m.ProposedEndDate,
                        TimeStampRegistered = m.TimeStampRequested,
                        HRApprovedBy = m.HRApprovedBy,
                        LMApprovedBy = m.LMApprovedBy,
                        HODApprovedBy = m.HODApprovedBy,
                        HRApprovedStartDate = m.HRApprovedStartDate,
                        HRApprovedEndDate = m.HRApprovedEndDate,
                        LMApprovedStartDate = m.LMApprovedStartDate,
                        LMApprovedEndDate = m.LMApprovedEndDate,
                        HODApprovedStartDate = m.HODApprovedStartDate,
                        HODApprovedEndDate = m.HODApprovedEndDate,
                        LMApprovedTimeStamp = m.LMApprovedTimeStamp,
                        HODApprovedTimeStamp = m.HODApprovedTimeStamp,
                        HRApprovedTimeStamp = m.HRApprovedTimeStamp,
                        HRComment = m.HRComment,
                        LMComment = m.LMComment,
                        HODComment = m.HODComment,
                        Status = (int) m.Status,
                        StatusLabel = m.Status.ToString().Replace("_", " ")
                    });
                });

                response.Status.IsSuccessful = true;
                response.StaffLeaves = staffLeaveList;
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