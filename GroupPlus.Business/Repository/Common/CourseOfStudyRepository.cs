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
    internal class CourseOfStudyRepository
    {
        private readonly IPlugRepository<CourseOfStudy> _repository;
        private readonly PlugUoWork _uoWork;

        public CourseOfStudyRepository()
        {
            _uoWork = new PlugUoWork();
            _repository = new PlugRepository<CourseOfStudy>(_uoWork);
        }

        public CourseOfStudy GetCourseOfStudy(int courseOfStudyId)
        {
            try
            {
                return GetCourseOfStudys().Find(k => k.CourseOfStudyId == courseOfStudyId) ?? new CourseOfStudy();
            }
            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                return new CourseOfStudy();
            }
        }

        public List<CourseOfStudy> GetCourseOfStudys()
        {
            try
            {
                if (!(CacheManager.GetCache("ccCourseOfStudyList") is List<CourseOfStudy> settings) ||
                    settings.IsNullOrEmpty())
                {
                    var myItemList = _repository.GetAll().OrderBy(m => m.CourseOfStudyId);
                    if (!myItemList.Any()) return new List<CourseOfStudy>();
                    settings = myItemList.ToList();
                    if (settings.IsNullOrEmpty())
                        return new List<CourseOfStudy>();
                    CacheManager.SetCache("ccCourseOfStudyList", settings, DateTime.Now.AddYears(1));
                }
                return settings;
            }
            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                return new List<CourseOfStudy>();
            }
        }

        internal void resetCache()
        {
            try
            {
                HelperMethods.clearCache("ccCourseOfStudyList");
                GetCourseOfStudys();
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public SettingRegRespObj AddCourseOfStudy(RegCourseOfStudyObj regObj)
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

                if (IsCourseOfStudyDuplicate(regObj.Name, regObj.DisciplineId, 1, ref response)) return response;

                var courseOfStudy = new CourseOfStudy
                {
                    Name = regObj.Name,
                    DisciplineId = regObj.DisciplineId,
                    Status = (ItemStatus) regObj.Status
                };

                var added = _repository.Add(courseOfStudy);

                _uoWork.SaveChanges();

                if (added.CourseOfStudyId < 1)
                {
                    response.Status.Message.FriendlyMessage =
                        "Error Occurred! Unable to complete your request. Please try again later";
                    response.Status.Message.TechnicalMessage = "Unable to save to database";
                    return response;
                }
                resetCache();
                response.Status.IsSuccessful = true;
                response.SettingId = added.CourseOfStudyId;
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

        public SettingRegRespObj UpdateCourseOfStudy(EditCourseOfStudyObj regObj)
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

                var thisCourseOfStudy = getCourseOfStudyInfo(regObj.CourseOfStudyId);

                if (thisCourseOfStudy == null)
                {
                    response.Status.Message.FriendlyMessage =
                        "No CourseOfStudy Information found for the specified CourseOfStudy Id";
                    response.Status.Message.TechnicalMessage = "No CourseOfStudy Information found!";
                    return response;
                }

                if (IsCourseOfStudyDuplicate(regObj.Name, regObj.DisciplineId, 2, ref response)) return response;

                thisCourseOfStudy.Name = regObj.Name;
                thisCourseOfStudy.DisciplineId = regObj.DisciplineId;
                thisCourseOfStudy.Status = (ItemStatus) regObj.Status;

                var added = _repository.Update(thisCourseOfStudy);
                _uoWork.SaveChanges();

                if (added.CourseOfStudyId < 1)
                {
                    response.Status.Message.FriendlyMessage =
                        "Error Occurred! Unable to complete your request. Please try again later";
                    response.Status.Message.TechnicalMessage = "Unable to save to database";
                    return response;
                }
                resetCache();
                response.Status.IsSuccessful = true;
                response.SettingId = added.CourseOfStudyId;
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

        public SettingRegRespObj DeleteCourseOfStudy(DeleteCourseOfStudyObj regObj)
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
                var thisCourseOfStudy = getCourseOfStudyInfo(regObj.CourseOfStudyId);

                if (thisCourseOfStudy == null)
                {
                    response.Status.Message.FriendlyMessage =
                        "No CourseOfStudy Information found for the specified CourseOfStudy Id";
                    response.Status.Message.TechnicalMessage = "No CourseOfStudy Information found!";
                    return response;
                }
                thisCourseOfStudy.Name =
                    thisCourseOfStudy.Name + "_Deleted_" + DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss");
                thisCourseOfStudy.Status = ItemStatus.Deleted;
                var added = _repository.Update(thisCourseOfStudy);
                _uoWork.SaveChanges();

                if (added.CourseOfStudyId < 1)
                {
                    response.Status.Message.FriendlyMessage =
                        "Error Occurred! Unable to complete your request. Please try again later";
                    response.Status.Message.TechnicalMessage = "Unable to save to database";
                    return response;
                }
                resetCache();
                response.Status.IsSuccessful = true;
                response.SettingId = added.CourseOfStudyId;
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

        public CourseOfStudyRespObj LoadCourseOfStudys(CommonSettingSearchObj searchObj)
        {
            var response = new CourseOfStudyRespObj
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
                var thisCourseOfStudys = GetCourseOfStudys();
                if (!thisCourseOfStudys.Any())
                {
                    response.Status.Message.FriendlyMessage = "No CourseOfStudy Information found!";
                    response.Status.Message.TechnicalMessage = "No CourseOfStudy  Information found!";
                    return response;
                }

                if (searchObj.Status > -1)
                    thisCourseOfStudys = thisCourseOfStudys.FindAll(p => p.Status == (ItemStatus) searchObj.Status);

                var courseOfStudyItems = new List<CourseOfStudyObj>();
                thisCourseOfStudys.ForEachx(m =>
                {
                    courseOfStudyItems.Add(new CourseOfStudyObj
                    {
                        CourseOfStudyId = m.CourseOfStudyId,
                        Name = m.Name,
                        DisciplineId = m.DisciplineId,
                        DisciplineLabel = new DisciplineRepository().GetDiscipline(m.DisciplineId).Name,
                        Status = (int) m.Status,
                        StatusLabel = m.Status.ToString().Replace("_", " ")
                    });
                });

                response.Status.IsSuccessful = true;
                response.CourseOfStudys = courseOfStudyItems;
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

        private bool IsCourseOfStudyDuplicate(string name, int disciplineId, int callType,
            ref SettingRegRespObj response)
        {
            try
            {
                var sql1 =
                    $"Select coalesce(Count(\"CourseOfStudyId\"), 0) FROM  \"GPlus\".\"CourseOfStudy\"  WHERE lower(\"Name\") = lower('{name.Replace("'", "''")}') AND \"DisciplineId\" = {disciplineId} ";
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
                {
                    if (callType != 2 || check[0] > 1)
                    {
                        response.Status.Message.FriendlyMessage = "Duplicate Error!CourseOfStudy Name already exist";
                        response.Status.Message.TechnicalMessage = "Duplicate Error! CourseOfStudy Name already exist";
                        return true;
                    }

                    return false;
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

        private CourseOfStudy getCourseOfStudyInfo(int courseOfStudyId)
        {
            try
            {
                var sql1 =
                    $"SELECT *  FROM  \"GPlus\".\"CourseOfStudy\" WHERE  \"CourseOfStudyId\" = {courseOfStudyId};";
                var agentInfos = _repository.RepositoryContext().Database.SqlQuery<CourseOfStudy>(sql1).ToList();
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