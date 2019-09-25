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
    internal class JobTypeRepository
    {
        private readonly IPlugRepository<JobType> _repository;
        private readonly PlugUoWork _uoWork;

        public JobTypeRepository()
        {
            _uoWork = new PlugUoWork();
            _repository = new PlugRepository<JobType>(_uoWork);
        }

        public JobType GetJobType(int jobTypeId)
        {
            try
            {
                return GetJobTypes().Find(k => k.JobTypeId == jobTypeId) ?? new JobType();
            }
            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                return new JobType();
            }
        }

        public List<JobType> GetJobTypes()
        {
            try
            {
                if (!(CacheManager.GetCache("ccJobTypeList") is List<JobType> settings) || settings.IsNullOrEmpty())
                {
                    var myItemList = _repository.GetAll().OrderBy(m => m.JobTypeId);
                    if (!myItemList.Any()) return new List<JobType>();
                    settings = myItemList.ToList();
                    if (settings.IsNullOrEmpty())
                        return new List<JobType>();
                    CacheManager.SetCache("ccJobTypeList", settings, DateTime.Now.AddYears(1));
                }
                return settings;
            }
            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                return new List<JobType>();
            }
        }

        internal void resetCache()
        {
            try
            {
                HelperMethods.clearCache("ccJobTypeList");
                GetJobTypes();
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public SettingRegRespObj AddJobType(RegJobTypeObj regObj)
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

                if (IsJobTypeDuplicate(regObj.Name, 1, ref response)) return response;

                var jobType = new JobType
                {
                    Name = regObj.Name,
                    Status = (ItemStatus) regObj.Status
                };

                var added = _repository.Add(jobType);

                _uoWork.SaveChanges();

                if (added.JobTypeId < 1)
                {
                    response.Status.Message.FriendlyMessage =
                        "Error Occurred! Unable to complete your request. Please try again later";
                    response.Status.Message.TechnicalMessage = "Unable to save to database";
                    return response;
                }
                resetCache();
                response.Status.IsSuccessful = true;
                response.SettingId = added.JobTypeId;
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

        public SettingRegRespObj UpdateJobType(EditJobTypeObj regObj)
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

                var thisJobType = getJobTypeInfo(regObj.JobTypeId);

                if (thisJobType == null)
                {
                    response.Status.Message.FriendlyMessage =
                        "No Job Type Information found for the specified JobType Id";
                    response.Status.Message.TechnicalMessage = "No Job Type Information found!";
                    return response;
                }

                if (IsJobTypeDuplicate(regObj.Name, 2, ref response)) return response;
                thisJobType.Name = regObj.Name;
                thisJobType.Status = (ItemStatus) regObj.Status;
                var added = _repository.Update(thisJobType);
                _uoWork.SaveChanges();

                if (added.JobTypeId < 1)
                {
                    response.Status.Message.FriendlyMessage =
                        "Error Occurred! Unable to complete your request. Please try again later";
                    response.Status.Message.TechnicalMessage = "Unable to save to database";
                    return response;
                }
                resetCache();
                response.Status.IsSuccessful = true;
                response.SettingId = added.JobTypeId;
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

        public SettingRegRespObj DeleteJobType(DeleteJobTypeObj regObj)
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
                var thisJobType = getJobTypeInfo(regObj.JobTypeId);

                if (thisJobType == null)
                {
                    response.Status.Message.FriendlyMessage =
                        "No Job Type Information found for the specified JobType Id";
                    response.Status.Message.TechnicalMessage = "No Job Type Information found!";
                    return response;
                }
                thisJobType.Name = thisJobType.Name + "_Deleted_" + DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss");
                thisJobType.Status = ItemStatus.Deleted;
                var added = _repository.Update(thisJobType);
                _uoWork.SaveChanges();

                if (added.JobTypeId < 1)
                {
                    response.Status.Message.FriendlyMessage =
                        "Error Occurred! Unable to complete your request. Please try again later";
                    response.Status.Message.TechnicalMessage = "Unable to save to database";
                    return response;
                }
                resetCache();
                response.Status.IsSuccessful = true;
                response.SettingId = added.JobTypeId;
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

        public JobTypeRespObj LoadJobTypes(CommonSettingSearchObj searchObj)
        {
            var response = new JobTypeRespObj
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
                var thisJobTypes = GetJobTypes();
                if (!thisJobTypes.Any())
                {
                    response.Status.Message.FriendlyMessage = "No Job Type Information found!";
                    response.Status.Message.TechnicalMessage = "No Job Type  Information found!";
                    return response;
                }

                if (searchObj.Status > -1)
                    thisJobTypes = thisJobTypes.FindAll(p => p.Status == (ItemStatus) searchObj.Status);

                var jobTypeItems = new List<JobTypeObj>();
                thisJobTypes.ForEachx(m =>
                {
                    jobTypeItems.Add(new JobTypeObj
                    {
                        JobTypeId = m.JobTypeId,
                        Name = m.Name,
                        Status = (int) m.Status,
                        StatusLabel = m.Status.ToString().Replace("_", " ")
                    });
                });

                response.Status.IsSuccessful = true;
                response.JobTypes = jobTypeItems;
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

        private bool IsJobTypeDuplicate(string name, int callType, ref SettingRegRespObj response)
        {
            try
            {
                var sql1 =
                    $"Select coalesce(Count(\"JobTypeId\"), 0) FROM  \"GPlus\".\"JobType\"  WHERE lower(\"Name\") = lower('{name.Replace("'", "''")}')";
                var check = _repository.RepositoryContext().Database.SqlQuery<long>(sql1).ToList();

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
                        response.Status.Message.FriendlyMessage = "Duplicate Error! JobType Name already exist";
                        response.Status.Message.TechnicalMessage = "Duplicate Error! JobType Name already exist";
                        return true;
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

        private JobType getJobTypeInfo(int jobTypeId)
        {
            try
            {
                var sql1 = $"SELECT *  FROM  \"GPlus\".\"JobType\" WHERE  \"JobTypeId\" = {jobTypeId};";
                var agentInfos = _repository.RepositoryContext().Database.SqlQuery<JobType>(sql1).ToList();
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