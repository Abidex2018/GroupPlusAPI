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
    internal class SalaryGradeRepository
    {
        private readonly IPlugRepository<SalaryGrade> _repository;
        private readonly PlugUoWork _uoWork;

        public SalaryGradeRepository()
        {
            _uoWork = new PlugUoWork();
            _repository = new PlugRepository<SalaryGrade>(_uoWork);
        }

        public SalaryGrade GetSalaryGrade(int salaryGradeId)
        {
            try
            {
                return GetSalaryGrades().Find(k => k.SalaryGradeId == salaryGradeId) ?? new SalaryGrade();
            }
            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                return new SalaryGrade();
            }
        }

        public List<SalaryGrade> GetSalaryGrades()
        {
            try
            {
                if (!(CacheManager.GetCache("ccSalaryGradeList") is List<SalaryGrade> settings) ||
                    settings.IsNullOrEmpty())
                {
                    var myItemList = _repository.GetAll().OrderBy(m => m.SalaryGradeId);
                    if (!myItemList.Any()) return new List<SalaryGrade>();
                    settings = myItemList.ToList();
                    if (settings.IsNullOrEmpty())
                        return new List<SalaryGrade>();
                    CacheManager.SetCache("ccSalaryGradeList", settings, DateTime.Now.AddYears(1));
                }
                return settings;
            }
            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                return new List<SalaryGrade>();
            }
        }

        internal void resetCache()
        {
            try
            {
                HelperMethods.clearCache("ccSalaryGradeList");
                GetSalaryGrades();
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public SettingRegRespObj AddSalaryGrade(RegSalaryGradeObj regObj)
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

                //if (!HelperMethods.IsUserValid(regObj.AdminUserId, regObj.SysPathCode, HelperMethods.getSeniorAccountant(), ref response.Status.Message))
                //{
                //    return response;
                //}

                if (IsSalaryGradeDuplicate(regObj.Name, 1, ref response)) return response;

                var salaryGrade = new SalaryGrade
                {
                    Name = regObj.Name,
                    Status = (ItemStatus) regObj.Status
                };

                var added = _repository.Add(salaryGrade);

                _uoWork.SaveChanges();

                if (added.SalaryGradeId < 1)
                {
                    response.Status.Message.FriendlyMessage =
                        "Error Occurred! Unable to complete your request. Please try again later";
                    response.Status.Message.TechnicalMessage = "Unable to save to database";
                    return response;
                }
                resetCache();
                response.Status.IsSuccessful = true;
                response.SettingId = added.SalaryGradeId;
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

        public SettingRegRespObj UpdateSalaryGrade(EditSalaryGradeObj regObj)
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

                var thisSalaryGrade = getSalaryGradeInfo(regObj.SalaryGradeId);

                if (thisSalaryGrade == null)
                {
                    response.Status.Message.FriendlyMessage =
                        "No Salary Grade Information found for the specified SalaryGrade Id";
                    response.Status.Message.TechnicalMessage = "No Salary Grade Information found!";
                    return response;
                }

                if (IsSalaryGradeDuplicate(regObj.Name, 2, ref response)) return response;
                thisSalaryGrade.Name = regObj.Name;
                thisSalaryGrade.Status = (ItemStatus) regObj.Status;
                var added = _repository.Update(thisSalaryGrade);
                _uoWork.SaveChanges();

                if (added.SalaryGradeId < 1)
                {
                    response.Status.Message.FriendlyMessage =
                        "Error Occurred! Unable to complete your request. Please try again later";
                    response.Status.Message.TechnicalMessage = "Unable to save to database";
                    return response;
                }
                resetCache();
                response.Status.IsSuccessful = true;
                response.SettingId = added.SalaryGradeId;
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

        public SettingRegRespObj DeleteSalaryGrade(DeleteSalaryGradeObj regObj)
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
                var thisSalaryGrade = getSalaryGradeInfo(regObj.SalaryGradeId);

                if (thisSalaryGrade == null)
                {
                    response.Status.Message.FriendlyMessage =
                        "No Salary Grade Information found for the specified SalaryGrade Id";
                    response.Status.Message.TechnicalMessage = "No Salary Grade Information found!";
                    return response;
                }
                thisSalaryGrade.Name =
                    thisSalaryGrade.Name + "_Deleted_" + DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss");
                thisSalaryGrade.Status = ItemStatus.Deleted;
                var added = _repository.Update(thisSalaryGrade);
                _uoWork.SaveChanges();

                if (added.SalaryGradeId < 1)
                {
                    response.Status.Message.FriendlyMessage =
                        "Error Occurred! Unable to complete your request. Please try again later";
                    response.Status.Message.TechnicalMessage = "Unable to save to database";
                    return response;
                }
                resetCache();
                response.Status.IsSuccessful = true;
                response.SettingId = added.SalaryGradeId;
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

        public SalaryGradeRespObj LoadSalaryGrades(CommonSettingSearchObj searchObj)
        {
            var response = new SalaryGradeRespObj
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
                var thisSalaryGrades = GetSalaryGrades();
                if (!thisSalaryGrades.Any())
                {
                    response.Status.Message.FriendlyMessage = "No Salary Grade Information found!";
                    response.Status.Message.TechnicalMessage = "No Salary Grade  Information found!";
                    return response;
                }

                if (searchObj.Status > -1)
                    thisSalaryGrades = thisSalaryGrades.FindAll(p => p.Status == (ItemStatus) searchObj.Status);

                var salaryGradeItems = new List<SalaryGradeObj>();
                thisSalaryGrades.ForEachx(m =>
                {
                    salaryGradeItems.Add(new SalaryGradeObj
                    {
                        SalaryGradeId = m.SalaryGradeId,
                        Name = m.Name,
                        Status = (int) m.Status,
                        StatusLabel = m.Status.ToString().Replace("_", " ")
                    });
                });

                response.Status.IsSuccessful = true;
                response.SalaryGrades = salaryGradeItems;
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

        private bool IsSalaryGradeDuplicate(string name, int callType, ref SettingRegRespObj response)
        {
            try
            {
                var sql1 =
                    $"Select coalesce(Count(\"SalaryGradeId\"), 0) FROM  \"GPlus\".\"SalaryGrade\"  WHERE lower(\"Name\") = lower('{name.Replace("'", "''")}')";
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
                        response.Status.Message.FriendlyMessage = "Duplicate Error! Salary Grade Name already exist";
                        response.Status.Message.TechnicalMessage = "Duplicate Error! Salary Grade Name already exist";
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

        private SalaryGrade getSalaryGradeInfo(int salaryGradeId)
        {
            try
            {
                var sql1 = $"SELECT *  FROM  \"GPlus\".\"SalaryGrade\" WHERE  \"SalaryGradeId\" = {salaryGradeId};";
                var agentInfos = _repository.RepositoryContext().Database.SqlQuery<SalaryGrade>(sql1).ToList();
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