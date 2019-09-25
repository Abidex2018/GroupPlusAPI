using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using GroupPlus.Business.Core;
using GroupPlus.Business.DataManager;
using GroupPlus.BusinessContract.CommonAPIs;
using GroupPlus.BusinessObject.StaffManagement;
using GroupPlus.BussinessObject.Workflow;
using GroupPlus.Common;
using Mono.Security.Authenticode;
using XPLUG.WEBTOOLS;

namespace GroupPlus.Business.Repository.StaffManagement
{
    internal partial class StaffRepository
    {
        #region StaffMemo

        private static int workFlowSourceId;
        public SettingRegRespObj AddStaffMemo(RegStaffMemoObj regObj)
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

                var staffMemo = new StaffMemo
                {
                    StaffId = regObj.StaffId,
                    Title = regObj.Title,
                    MemoType = (MemoType) regObj.MemoType,
                    MemoDetail = regObj.MemoDetail,
                    IsReplied = false,
                    RegisterBy = regObj.AdminUserId,
                    ApprovedBy = 0,
                    Status = ApprovalStatus.Registered,
                    TimeStampRegister = DateMap.CurrentTimeStamp()
                };

                

                using (var db = _uoWork.BeginTransaction())
                {
                    try
                    {
                        var added = _staffMemoRepository.Add(staffMemo);
                        _uoWork.SaveChanges();
                        if (added.StaffMemoId < 1)
                        {
                            db.Rollback();
                            response.Status.Message.FriendlyMessage =
                                "Error Occurred! Unable to complete your request. Please try again later";
                            response.Status.Message.TechnicalMessage = "Unable to save to database";
                            return response;
                        }
                        workFlowSourceId = added.StaffMemoId;
                        var workflow = new WorkflowSetup
                        {
                            Description = regObj.Title,
                            InitiatorId = regObj.AdminUserId,
                            InitiatorType = WorkflowInitiatorType.HR,
                            Item = WorkflowItem.Staff_Memo,
                            WorkflowOrderId = regObj.WorkflowOrderId,
                            WorkflowSourceId = workFlowSourceId,
                            LastTimeStampModified = DateMap.CurrentTimeStamp(),
                            StaffId = regObj.StaffId,
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
                        response.SettingId = added.StaffMemoId;
                        workflow.WorkflowSourceId = added.StaffMemoId;
                        db.Commit();
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

        public SettingRegRespObj AddBulkStaffMemo(RegBulkStaffMemoObj regObj)
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
                if (regObj?.StaffMemoItem == null || !regObj.StaffMemoItem.Any())
                {
                    response.Status.Message.FriendlyMessage = "Empty Request Item! Please try again later";
                    response.Status.Message.TechnicalMessage = "Registration Object is empty / invalid";
                    return response;
                }

                foreach (var reqitem in regObj.StaffMemoItem)
                    if (!EntityValidatorHelper.Validate(reqitem, out var valItemResults))
                    {
                        var errorDetail = new StringBuilder();
                        if (!valItemResults.IsNullOrEmpty())
                        {
                            errorDetail.AppendLine("Following error occurred:");
                            valItemResults.ForEachx(m => errorDetail.AppendLine(m.ErrorMessage));
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

                var staffMemoList = new List<StaffMemo>();
                var workflowList = new List<WorkflowSetup>();
                var results = staffMemoList.GroupBy(e => e.StaffMemoId, (key, g) => new
                {
                    StaffMemoId = key,
                    StaffMemoItems = g.ToList()
                }).ToList();

                if (!results.Any())
                {
                    response.Status.Message.FriendlyMessage = "Invalid Expense Item list";
                    response.Status.Message.TechnicalMessage = "Invalid Expense Item list";
                    response.Status.IsSuccessful = false;
                    return response;
                }

                foreach (var reqItem in results)
                {
                    if (reqItem.StaffMemoItems.Count > 1)
                    {
                        response.Status.Message.FriendlyMessage =
                            $"Duplicate Staff Memo Item {getStaffMemo(reqItem.StaffMemoItems[0].StaffId).MemoType.ToString()}";
                        response.Status.Message.TechnicalMessage =
                            $"Duplicate Staff Memo Item {getStaffMemo(reqItem.StaffMemoItems[0].StaffId).MemoType.ToString()}";
                        response.Status.IsSuccessful = false;
                        return response;
                    }
                    staffMemoList.Add(new StaffMemo
                    {
                        StaffId = reqItem.StaffMemoItems[0].StaffId,
                        Title = reqItem.StaffMemoItems[0].Title,
                        MemoType = reqItem.StaffMemoItems[0].MemoType,
                        MemoDetail = reqItem.StaffMemoItems[0].MemoDetail,
                        IsReplied = false,
                        RegisterBy = regObj.AdminUserId,
                        ApprovedBy = 0,
                        Status = ApprovalStatus.Approved,
                        TimeStampRegister = DateMap.CurrentTimeStamp()
                    });
                    workFlowSourceId = staffMemoList[0].StaffMemoId;
                    workflowList.Add(new WorkflowSetup
                    {
                        Description = reqItem.StaffMemoItems[0].Title,
                        InitiatorId = regObj.AdminUserId,
                        InitiatorType = WorkflowInitiatorType.HR,
                        Item = WorkflowItem.Staff_Memo,
                        WorkflowOrderId = regObj.StaffMemoItem[0].WorkflowOrderId,
                        WorkflowSourceId = workFlowSourceId,
                        LastTimeStampModified = DateMap.CurrentTimeStamp(),
                        StaffId = reqItem.StaffMemoItems[0].StaffId,
                        TimeStampInitiated = DateMap.CurrentTimeStamp(),
                        Status = WorkflowStatus.Initiated
                    });
                }

                using (var db = _uoWork.BeginTransaction())
                {
                    try
                    {
                        var added = _staffMemoRepository.AddRange(staffMemoList);
                        _uoWork.SaveChanges();
                        if (added == null || !added.Any())
                        {
                            db.Rollback();
                            response.Status.Message.FriendlyMessage =
                                "Error Occurred! Unable to complete your request. Please try again later";
                            response.Status.Message.TechnicalMessage = "Unable to save to database";
                            return response;
                        }

                      
                        var retVal = _workflowSetupRepository.AddRange(workflowList);
                        _uoWork.SaveChanges();
                        if (retVal == null || !retVal.Any())
                        {
                            db.Rollback();
                            response.Status.Message.FriendlyMessage =
                                "Error Occurred! Unable to complete your request. Please try again later";
                            response.Status.Message.TechnicalMessage = "Unable to save to database";
                            return response;
                        }
                        response.Status.IsSuccessful = true;
                        workflowList[0].WorkflowSourceId = staffMemoList[0].StaffMemoId;
                        db.Commit();
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

        public SettingRegRespObj ApproveStaffMemo(ApprovaStaffMemoObj regObj)
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
                    HelperMethods.getAdminRoles(), ref response.Status.Message))
                    return response;


                //var searchObj = new StaffMemoSearchObj
                //{
                //    AdminUserId = 0,
                //    StaffMemoId = 0,
                //    StaffId = regObj.AdminUserId,
                //    Status = 0,
                //    SysPathCode = ""
                //};

                //retrieve previous contacts and validate with the new one
                var staffMemos = getStaffMemos(regObj.StaffId);

                if (staffMemos == null || !staffMemos.Any())
                {
                    response.Status.Message.FriendlyMessage = "No Staff Memo Information found";
                    response.Status.Message.TechnicalMessage = "No Staff Memo Information found";
                    return response;
                }
                var thisMemoFind = staffMemos.Find(m => m.StaffMemoId == regObj.StaffMemoId);
                var staffWorkflowlog = GetWorkflowLog(thisMemoFind.StaffId);
                var thisStaffWorkFlowLog = staffWorkflowlog.Find(m => m.StaffId == thisMemoFind.StaffId);
                if (thisMemoFind.StaffMemoId < 1)
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


                thisMemoFind.MemoType = (MemoType) regObj.MemoType;
                thisMemoFind.ApprovedBy = regObj.ApprovedBy;
                thisMemoFind.Status = (ApprovalStatus) regObj.Status;

                thisStaffWorkFlowLog.WorkflowOrderItemId = regObj.WorkflowOrderItemId;
                thisStaffWorkFlowLog.ApprovalType = WorkflowApprovalType.HR_Head;
                thisStaffWorkFlowLog.ProcessorId = regObj.AdminUserId;
                thisStaffWorkFlowLog.Comment = regObj.MemoDetail;
                thisStaffWorkFlowLog.LogTimeStamp = DateMap.CurrentTimeStamp();
                thisStaffWorkFlowLog.Status = (ApprovalStatus) regObj.Status;

                using (var db = _uoWork.BeginTransaction())
                {
                    try
                    {
                        var added = _staffMemoRepository.Update(thisMemoFind);
                        _uoWork.SaveChanges();
                        if (added.StaffMemoId < 1)
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
                        response.SettingId = added.StaffMemoId;
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

        public StaffMemoRespObj LoadStaffMemoByAdmin(StaffMemoSearchObj searchObj)
        {
            var response = new StaffMemoRespObj
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
                var thisStaffs = getStaffMemo(searchObj);
                if (thisStaffs == null || !thisStaffs.Any())
                {
                    response.Status.Message.FriendlyMessage = "No Staff Information found!";
                    response.Status.Message.TechnicalMessage = "No Staff  Information found!";
                    return response;
                }


                var staffMemoItems = new List<StaffMemoObj>();
                thisStaffs.ForEachx(m =>
                {
                    staffMemoItems.Add(new StaffMemoObj
                    {
                        StaffId = m.StaffId,
                        FirstName = new StaffRepository().getStaffInfo(m.StaffId).FirstName,
                        LastName = new StaffRepository().getStaffInfo(m.StaffId).LastName,
                        StaffMemoId = m.StaffMemoId,
                        Title = m.Title,
                        MemoTypeId = (int) m.MemoType,
                        MemoType = m.MemoType.ToString().Replace("_", " "),
                        MemoDetail = m.MemoDetail,
                        ApprovedBy = m.ApprovedBy,
                        RegisterBy = m.RegisterBy,
                        IsReplied = m.IsReplied,
                        Status = (int) m.Status,
                        StatusLabel = m.Status.ToString().Replace("_", " "),
                        TimeStampRegister = m.TimeStampRegister
                    });
                });

                response.Status.IsSuccessful = true;
                response.StaffMemos = staffMemoItems;
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

        public StaffMemoRespObj LoadStaffMemo(StaffMemoSearchObj searchObj)
        {
            var response = new StaffMemoRespObj
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
                var thisStaffs = getStaffMemo(searchObj);
                if (thisStaffs == null || !thisStaffs.Any())
                {
                    response.Status.Message.FriendlyMessage = "No Staff Information found!";
                    response.Status.Message.TechnicalMessage = "No Staff  Information found!";
                    return response;
                }


                var staffMemoItems = new List<StaffMemoObj>();
                thisStaffs.ForEachx(m =>
                {
                    staffMemoItems.Add(new StaffMemoObj
                    {
                        StaffId = m.StaffId,
                        FirstName = new StaffRepository().getStaffInfo(m.StaffId).FirstName,
                        LastName = new StaffRepository().getStaffInfo(m.StaffId).LastName,
                        StaffMemoId = m.StaffMemoId,
                        Title = m.Title,
                        MemoTypeId = (int) m.MemoType,
                        MemoType = m.MemoType.ToString().Replace("_", " "),
                        MemoDetail = m.MemoDetail,
                        IsReplied = m.IsReplied,
                        ApprovedBy = m.ApprovedBy,
                        RegisterBy = m.RegisterBy,
                        Status = (int) m.Status,
                        StatusLabel = m.Status.ToString().Replace("_", " "),
                        TimeStampRegister = m.TimeStampRegister
                    });
                });

                response.Status.IsSuccessful = true;
                response.StaffMemos = staffMemoItems;
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

        #endregion
    }
}