using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using GroupPlus.Business.Core;
using GroupPlus.Business.DataManager;
using GroupPlus.Business.Infrastructure;
using GroupPlus.Business.Infrastructure.Contract;
using GroupPlus.BusinessContract.CommonAPIs;
using GroupPlus.BusinessObject.Settings;
using GroupPlus.Common;
using XPLUG.WEBTOOLS;

namespace GroupPlus.Business.Repository.Common
{
    internal class LeaveTypeRepository
    {
        private readonly IPlugRepository<LeaveType> _repository;
        private readonly PlugUoWork _uoWork;

        public LeaveTypeRepository()
        {
            _uoWork = new PlugUoWork();
            _repository = new PlugRepository<LeaveType>(_uoWork);
        }

        public LeaveType GetLeaveType(int leaveTypeId)
        {
            try
            {
                return GetLeaveTypes().Find(k => k.LeaveTypeId == leaveTypeId) ?? new LeaveType();
            }
            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                return new LeaveType();
            }
        }

        public List<LeaveType> GetLeaveTypes()
        {
            try
            {
                if (!(CacheManager.GetCache("ccLeaveTypeList") is List<LeaveType> settings) || settings.IsNullOrEmpty())
                {
                    var myItemList = _repository.GetAll().OrderBy(m => m.LeaveTypeId);
                    if (!myItemList.Any()) return new List<LeaveType>();
                    settings = myItemList.ToList();
                    if (settings.IsNullOrEmpty())
                        return new List<LeaveType>();
                    CacheManager.SetCache("ccLeaveTypeList", settings, DateTime.Now.AddYears(1));
                }
                return settings;
            }
            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                return new List<LeaveType>();
            }
        }

        internal void resetCache()
        {
            try
            {
                HelperMethods.clearCache("ccLeaveTypeList");
                GetLeaveTypes();
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public SettingRegRespObj AddLeaveType(RegLeaveTypeObj regObj)
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
                    HelperMethods.getSeniorAccountant(), ref response.Status.Message))
                    return response;

                if (IsLeaveTypeDuplicate(regObj.Name, 1, ref response)) return response;

                if (regObj.MinDays > regObj.MaxDays)
                {
                    response.Status.Message.FriendlyMessage = "Maximum Days must be higher than the Minimum Days";
                    response.Status.Message.TechnicalMessage = "Unable to save to database";
                    return response;
                }
                if (regObj.MinDays == regObj.MaxDays)
                {
                    response.Status.Message.FriendlyMessage = "Minimum/Maximum Days must not be equal to each other";
                    response.Status.Message.TechnicalMessage = "Unable to save to database";
                    return response;
                }
                var leaveType = new LeaveType
                {
                    Name = regObj.Name,
                    MinDays = regObj.MinDays,
                    MaxDays = regObj.MaxDays,
                    Status = (ItemStatus) regObj.Status
                };

                var added = _repository.Add(leaveType);

                _uoWork.SaveChanges();

                if (added.LeaveTypeId < 1)
                {
                    response.Status.Message.FriendlyMessage =
                        "Error Occurred! Unable to complete your request. Please try again later";
                    response.Status.Message.TechnicalMessage = "Unable to save to database";
                    return response;
                }
                resetCache();
                response.Status.IsSuccessful = true;
                response.SettingId = added.LeaveTypeId;
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

        public SettingRegRespObj UpdateLeaveType(EditLeaveTypeObj regObj)
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
                    HelperMethods.getSeniorAccountant(), ref response.Status.Message))
                    return response;

                var thisLeaveType = getLeaveTypeInfo(regObj.LeaveTypeId);

                if (thisLeaveType == null)
                {
                    response.Status.Message.FriendlyMessage =
                        "No LeaveType Information found for the specified LeaveType Id";
                    response.Status.Message.TechnicalMessage = "No LeaveType Information found!";
                    return response;
                }

                if (regObj.MinDays > regObj.MaxDays)
                {
                    response.Status.Message.FriendlyMessage = "Maximum Days must be higher than the Minimum Days";
                    response.Status.Message.TechnicalMessage =
                        "Maximum Days Point must be higher than the Minimum Days";
                    return response;
                }
                if (regObj.MinDays == regObj.MaxDays)
                {
                    response.Status.Message.FriendlyMessage = "Minimum/Maximum Days must not be equal to each other";
                    response.Status.Message.TechnicalMessage = "Minimum/Maximum Days must not be equal to each other";
                    return response;
                }
                if (IsLeaveTypeDuplicate(regObj.Name, 2, ref response)) return response;
                thisLeaveType.Name = regObj.Name;
                thisLeaveType.MinDays = regObj.MinDays;
                thisLeaveType.MaxDays = regObj.MaxDays;
                thisLeaveType.Status = (ItemStatus) regObj.Status;
                var added = _repository.Update(thisLeaveType);
                _uoWork.SaveChanges();

                if (added.LeaveTypeId < 1)
                {
                    response.Status.Message.FriendlyMessage =
                        "Error Occurred! Unable to complete your request. Please try again later";
                    response.Status.Message.TechnicalMessage = "Unable to save to database";
                    return response;
                }
                resetCache();
                response.Status.IsSuccessful = true;
                response.SettingId = added.LeaveTypeId;
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

        public SettingRegRespObj DeleteLeaveType(DeleteLeaveTypeObj regObj)
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
                    HelperMethods.getSeniorAccountant(), ref response.Status.Message))
                    return response;
                var thisLeaveType = getLeaveTypeInfo(regObj.LeaveTypeId);

                if (thisLeaveType == null)
                {
                    response.Status.Message.FriendlyMessage =
                        "No Leave Type Information found for the specified LeaveType Id";
                    response.Status.Message.TechnicalMessage = "No Leave Type Information found!";
                    return response;
                }
                thisLeaveType.Name = thisLeaveType.Name + "_Deleted_" + DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss");
                thisLeaveType.Status = ItemStatus.Deleted;
                var added = _repository.Update(thisLeaveType);
                _uoWork.SaveChanges();

                if (added.LeaveTypeId < 1)
                {
                    response.Status.Message.FriendlyMessage =
                        "Error Occurred! Unable to complete your request. Please try again later";
                    response.Status.Message.TechnicalMessage = "Unable to save to database";
                    return response;
                }
                resetCache();
                response.Status.IsSuccessful = true;
                response.SettingId = added.LeaveTypeId;
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

        public LeaveTypeRespObj LoadLeaveTypes(CommonSettingSearchObj searchObj)
        {
            var response = new LeaveTypeRespObj
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

                //if (!HelperMethods.IsUserValid(searchObj.AdminUserId, searchObj.SysPathCode, HelperMethods.getAllRoles(), ref response.Status.Message))
                //{
                //    return response;
                //}
                var thisLeaveTypes = GetLeaveTypes();
                if (!thisLeaveTypes.Any())
                {
                    response.Status.Message.FriendlyMessage = "No Leave Type Information found!";
                    response.Status.Message.TechnicalMessage = "No Leave Type  Information found!";
                    return response;
                }

                if (searchObj.Status > -1)
                    thisLeaveTypes = thisLeaveTypes.FindAll(p => p.Status == (ItemStatus) searchObj.Status);

                var leaveTypeItems = new List<LeaveTypeObj>();
                thisLeaveTypes.ForEachx(m =>
                {
                    leaveTypeItems.Add(new LeaveTypeObj
                    {
                        LeaveTypeId = m.LeaveTypeId,
                        Name = m.Name,
                        MinDays = m.MinDays,
                        MaxDays = m.MaxDays,
                        Status = (int) m.Status,
                        StatusLabel = m.Status.ToString().Replace("_", " ")
                    });
                });

                response.Status.IsSuccessful = true;
                response.LeaveTypes = leaveTypeItems;
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

        private bool IsLeaveTypeDuplicate(string name, int callType, ref SettingRegRespObj response)
        {
            try
            {
                var sql1 =
                    $"Select coalesce(Count(\"LeaveTypeId\"), 0) FROM  \"GPlus\".\"LeaveType\"  WHERE lower(\"Name\") = lower('{name.Replace("'", "''")}')";
                var check = _repository.RepositoryContext().Database.SqlQuery<long>(sql1).ToList();

                if (callType == 1)
                {
                    if (check.IsNullOrEmpty())
                    {
                        response.Status.Message.FriendlyMessage =
                            "Unable to complete your request due to validation error. Please try again later";
                        response.Status.Message.TechnicalMessage = "Unable to check for duplicate";
                        return true;
                    }

                    if (check.Count != 1)
                    {
                        response.Status.Message.FriendlyMessage =
                            "Unable to complete your request due to validation error. Please try again later";
                        response.Status.Message.TechnicalMessage = "Unable to check for duplicate";
                        return true;
                    }

                    if (check[0] < 1)
                        return false;

                    if (check[0] > 0)
                        if (callType != 2 || check[0] > 1)
                        {
                            response.Status.Message.FriendlyMessage = "Duplicate Error! LeaveType Name already exist";
                            response.Status.Message.TechnicalMessage = "Duplicate Error! LeaveType Name already exist";
                            return true;
                        }
                }
                return false;
            }
            catch (Exception ex)
            {
                response.Status.Message.FriendlyMessage =
                    "Unable to complete your request due to validation error. Please try again later";
                response.Status.Message.TechnicalMessage = "Duplicate Check Error: " + ex.Message;
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                return true;
            }
        }

        private LeaveType getLeaveTypeInfo(int leaveTypeId)
        {
            try
            {
                var sql1 = $"SELECT *  FROM  \"GPlus\".\"LeaveType\" WHERE  \"LeaveTypeId\" = {leaveTypeId};";
                var agentInfos = _repository.RepositoryContext().Database.SqlQuery<LeaveType>(sql1).ToList();
                if (!agentInfos.Any() || agentInfos.Count != 1)
                    return null;
                return agentInfos[0];
            }

            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }
    }
}