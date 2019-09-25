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
    internal class ClassOfAwardRepository
    {
        private readonly IPlugRepository<ClassOfAward> _repository;
        private readonly PlugUoWork _uoWork;

        public ClassOfAwardRepository()
        {
            _uoWork = new PlugUoWork();
            _repository = new PlugRepository<ClassOfAward>(_uoWork);
        }

        public ClassOfAward GetClassOfAward(int classOfAwardId)
        {
            try
            {
                return GetClassOfAwards().Find(k => k.ClassOfAwardId == classOfAwardId) ?? new ClassOfAward();
            }
            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                return new ClassOfAward();
            }
        }

        public List<ClassOfAward> GetClassOfAwards()
        {
            try
            {
                if (!(CacheManager.GetCache("ccClassOfAwardList") is List<ClassOfAward> settings) ||
                    settings.IsNullOrEmpty())
                {
                    var myItemList = _repository.GetAll().OrderBy(m => m.ClassOfAwardId);
                    if (!myItemList.Any()) return new List<ClassOfAward>();
                    settings = myItemList.ToList();
                    if (settings.IsNullOrEmpty())
                        return new List<ClassOfAward>();
                    CacheManager.SetCache("ccClassOfAwardList", settings, DateTime.Now.AddYears(1));
                }
                return settings;
            }
            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                return new List<ClassOfAward>();
            }
        }

        internal void resetCache()
        {
            try
            {
                HelperMethods.clearCache("ccClassOfAwardList");
                GetClassOfAwards();
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public SettingRegRespObj AddClassOfAward(RegClassOfAwardObj regObj)
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

                if (IsClassOfAwardDuplicate(regObj.Name, regObj.LowerGradePoint, regObj.UpperGradePoint, 1,
                    ref response)) return response;

                if (regObj.UpperGradePoint < regObj.LowerGradePoint)
                {
                    response.Status.Message.FriendlyMessage =
                        "Upper Grade Point must be higher than the Lower Grade Point";
                    response.Status.Message.TechnicalMessage =
                        "Upper Grade Point must be higher than the Lower Grade Point";
                    return response;
                }

                var classOfAward = new ClassOfAward
                {
                    Name = regObj.Name,
                    LowerGradePoint = regObj.LowerGradePoint,
                    UpperGradePoint = regObj.UpperGradePoint,
                    Status = (ItemStatus) regObj.Status
                };

                var added = _repository.Add(classOfAward);

                _uoWork.SaveChanges();

                if (added.ClassOfAwardId < 1)
                {
                    response.Status.Message.FriendlyMessage =
                        "Error Occurred! Unable to complete your request. Please try again later";
                    response.Status.Message.TechnicalMessage = "Unable to save to database";
                    return response;
                }
                resetCache();
                response.Status.IsSuccessful = true;
                response.SettingId = added.ClassOfAwardId;
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

        public SettingRegRespObj UpdateClassOfAward(EditClassOfAwardObj regObj)
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

                var thisClassOfAward = getClassOfAwardInfo(regObj.ClassOfAwardId);

                if (thisClassOfAward == null)
                {
                    response.Status.Message.FriendlyMessage =
                        "No ClassOfAward Information found for the specified ClassOfAward Id";
                    response.Status.Message.TechnicalMessage = "No ClassOfAward Information found!";
                    return response;
                }

                if (IsClassOfAwardDuplicate(regObj.Name, regObj.LowerGradePoint, regObj.UpperGradePoint, 2,
                    ref response)) return response;

                if (regObj.UpperGradePoint < regObj.LowerGradePoint)
                {
                    response.Status.Message.FriendlyMessage =
                        "Upper Grade Point must be higher than the Lower Grade Point";
                    response.Status.Message.TechnicalMessage =
                        "Upper Grade Point must be higher than the Lower Grade Point";
                    return response;
                }

                thisClassOfAward.Name = regObj.Name;
                thisClassOfAward.LowerGradePoint = regObj.LowerGradePoint;
                thisClassOfAward.UpperGradePoint = regObj.UpperGradePoint;
                thisClassOfAward.Status = (ItemStatus) regObj.Status;

                var added = _repository.Update(thisClassOfAward);
                _uoWork.SaveChanges();

                if (added.ClassOfAwardId < 1)
                {
                    response.Status.Message.FriendlyMessage =
                        "Error Occurred! Unable to complete your request. Please try again later";
                    response.Status.Message.TechnicalMessage = "Unable to save to database";
                    return response;
                }
                resetCache();
                response.Status.IsSuccessful = true;
                response.SettingId = added.ClassOfAwardId;
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

        public SettingRegRespObj DeleteClassOfAward(DeleteClassOfAwardObj regObj)
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
                var thisClassOfAward = getClassOfAwardInfo(regObj.ClassOfAwardId);

                if (thisClassOfAward == null)
                {
                    response.Status.Message.FriendlyMessage =
                        "No ClassOfAward Information found for the specified ClassOfAward Id";
                    response.Status.Message.TechnicalMessage = "No ClassOfAward Information found!";
                    return response;
                }
                thisClassOfAward.Name =
                    thisClassOfAward.Name + "_Deleted_" + DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss");
                thisClassOfAward.Status = ItemStatus.Deleted;
                var added = _repository.Update(thisClassOfAward);
                _uoWork.SaveChanges();

                if (added.ClassOfAwardId < 1)
                {
                    response.Status.Message.FriendlyMessage =
                        "Error Occurred! Unable to complete your request. Please try again later";
                    response.Status.Message.TechnicalMessage = "Unable to save to database";
                    return response;
                }
                resetCache();
                response.Status.IsSuccessful = true;
                response.SettingId = added.ClassOfAwardId;
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

        public ClassOfAwardRespObj LoadClassOfAwards(CommonSettingSearchObj searchObj)
        {
            var response = new ClassOfAwardRespObj
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
                var thisClassOfAwards = GetClassOfAwards();
                if (!thisClassOfAwards.Any())
                {
                    response.Status.Message.FriendlyMessage = "No Class Of Award Information found!";
                    response.Status.Message.TechnicalMessage = "No Class Of Award  Information found!";
                    return response;
                }

                if (searchObj.Status > -1)
                    thisClassOfAwards = thisClassOfAwards.FindAll(p => p.Status == (ItemStatus) searchObj.Status);

                var classOfAwardItems = new List<ClassOfAwardObj>();
                thisClassOfAwards.ForEachx(m =>
                {
                    classOfAwardItems.Add(new ClassOfAwardObj
                    {
                        ClassOfAwardId = m.ClassOfAwardId,
                        Name = m.Name,
                        LowerGradePoint = m.LowerGradePoint,
                        UpperGradePoint = m.UpperGradePoint,
                        Status = (int) m.Status,
                        StatusLabel = m.Status.ToString().Replace("_", " ")
                    });
                });

                response.Status.IsSuccessful = true;
                response.ClassOfAwards = classOfAwardItems;
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

        private bool IsClassOfAwardDuplicate(string name, decimal upperGradePoint, decimal lowerGradePoint,
            int callType, ref SettingRegRespObj response)
        {
            try
            {
                var sql1 =
                    $"Select coalesce(Count(\"ClassOfAwardId\"), 0) FROM  \"GPlus\".\"ClassOfAward\"  WHERE lower(\"Name\") = lower('{name.Replace("'", "''")}') AND \"LowerGradePoint\" = {lowerGradePoint} AND  \"UpperGradePoint\" = {upperGradePoint} ";
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
                    {
                        if (callType != 2 || check[0] > 1)
                        {
                            response.Status.Message.FriendlyMessage =
                                "Duplicate Error! ClassOfAward Name already exist";
                            response.Status.Message.TechnicalMessage =
                                "Duplicate Error! ClassOfAward Name already exist";
                            return true;
                        }

                        return false;
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

        private ClassOfAward getClassOfAwardInfo(int classOfAwardId)
        {
            try
            {
                var sql1 = $"SELECT *  FROM  \"GPlus\".\"ClassOfAward\" WHERE  \"ClassOfAwardId\" = {classOfAwardId};";
                var agentInfos = _repository.RepositoryContext().Database.SqlQuery<ClassOfAward>(sql1).ToList();
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