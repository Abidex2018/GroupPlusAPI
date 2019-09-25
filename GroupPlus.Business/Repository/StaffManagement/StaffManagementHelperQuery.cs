using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GroupPlus.BusinessContract.CommonAPIs;
using GroupPlus.BusinessObject.StaffDetail;
using GroupPlus.BusinessObject.StaffManagement;
using GroupPlus.BussinessObject.Workflow;
using XPLUG.WEBTOOLS;

namespace GroupPlus.Business.Repository.StaffManagement
{
    internal partial class StaffRepository
    {
        #region Helpers

        #region Staff

        internal Staff getStaffInfo(int staffId)
        {
            try
            {
                var sql1 = $"SELECT *  FROM  \"GPlus\".\"Staff\" WHERE  \"StaffId\" = {staffId};";
                var agentInfos = _repository.RepositoryContext().Database.SqlQuery<Staff>(sql1).ToList();
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

        internal StaffAccess getStaffAccessInfo(string username, out string msg)
        {
            try
            {
                var sql1 =
                    $"SELECT *  FROM  \"GPlus\".\"StaffAccess\" WHERE  lower(\"Username\") = lower('{username}');";
                var agentInfos = _repository.RepositoryContext().Database.SqlQuery<StaffAccess>(sql1).ToList();
                if (!agentInfos.Any())
                {
                    msg = "No Staff Information Found!";
                    return null;
                }
                if (agentInfos.Count > 1)
                {
                    msg = "Unexpected Staff Records Found!";
                    return null;
                }
                msg = "xxxxx";
                return agentInfos[0];
            }

            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                msg = "Error Occurred! Unable to load Staff Information";
                return null;
            }
        }

        internal StaffAccess getStaffAccessInfo(int staffId)
        {
            try
            {
                var sql1 = $"SELECT *  FROM  \"GPlus\".\"StaffAccess\" WHERE  \"StaffId\" = {staffId};";
                var agentInfos = _repository.RepositoryContext().Database.SqlQuery<StaffAccess>(sql1).ToList();
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

        private bool IsStaffDuplicate(int companyId, string lastName, string firstName, string middleName, string email,
            string mobileNumber, string username, int callType, ref SettingRegRespObj response)
        {
            try
            {
                #region Staff

                var sql1 =
                    $"Select coalesce(Count(\"StaffId\"), 0) FROM \"GPlus\".\"Staff\" WHERE lower(\"FirstName\") = lower('{firstName.Trim()}') AND " +
                    $" lower(\"LastName\") = lower('{lastName.Trim()}') AND lower(\"MiddleName\") = lower('{middleName.Trim()}') AND  \"CompanyId\" = {companyId} ";

                var sqlEmail =
                    $"Select coalesce(Count(\"StaffId\"), 0) FROM \"GPlus\".\"StaffAccess\" WHERE lower(\"Email\") = lower('{email.Trim()}') ";

                var sqlUname =
                    $"Select coalesce(Count(\"StaffId\"), 0) FROM \"GPlus\".\"StaffAccess\" WHERE lower(\"Username\") = lower('{username.Trim()}') ";

                var sqlMobile =
                    $"Select coalesce(Count(\"StaffId\"), 0) FROM \"GPlus\".\"StaffAccess\" WHERE lower(\"MobileNumber\") = lower('{mobileNumber.Trim()}') ";

                var check1 = _repository.RepositoryContext().Database.SqlQuery<long>(sql1).ToList();
                var checkEmail = _repository.RepositoryContext().Database.SqlQuery<long>(sqlEmail).ToList();
                var checkMobile = _repository.RepositoryContext().Database.SqlQuery<long>(sqlMobile).ToList();

                if (!check1.Any())
                {
                    response.Status.Message.FriendlyMessage =
                        "Unable to check duplicate [Staff Name]! Please try again later";
                    response.Status.Message.TechnicalMessage =
                        "Unable to check duplicate [Staff Name]! Please try again later";
                    return true;
                }

                if (check1.Count != 1)
                {
                    response.Status.Message.FriendlyMessage = "Unable to check duplicate! Please try again later";
                    response.Status.Message.TechnicalMessage = "Unable to check duplicate! Please try again later";
                    return true;
                }

                if (check1[0] > 0)
                    if (callType != 2)
                    {
                        response.Status.Message.FriendlyMessage = "Duplicate Error! Staff Information already exist";
                        response.Status.Message.TechnicalMessage = "Duplicate Error! Staff Information already exist";
                        return true;
                    }

                //Email Validation
                if (!checkEmail.Any() || checkEmail.Count != 1)
                {
                    response.Status.Message.FriendlyMessage =
                        "Unable to check duplicate[Email]! Please try again later";
                    response.Status.Message.TechnicalMessage =
                        "Unable to check duplicate[Email]! Please try again later";
                    return true;
                }

                if (checkEmail[0] > 0)
                    if (callType != 2)
                    {
                        response.Status.Message.FriendlyMessage = "Duplicate Error! Staff Email already exist";
                        response.Status.Message.TechnicalMessage = "Duplicate Error! Staff Email already exist";
                        return true;
                    }

                //Username Validation
                if (callType == 1)
                {
                    var checkUname = _repository.RepositoryContext().Database.SqlQuery<long>(sqlUname).ToList();
                    if (!checkUname.Any() || checkUname.Count != 1)
                    {
                        response.Status.Message.FriendlyMessage =
                            "Unable to check duplicate[Username]! Please try again later";
                        response.Status.Message.TechnicalMessage =
                            "Unable to check duplicate[Username]! Please try again later";
                        return true;
                    }

                    if (checkUname[0] > 0)
                        if (callType != 2)
                        {
                            response.Status.Message.FriendlyMessage =
                                "Duplicate Error! Supplied Username already exist";
                            response.Status.Message.TechnicalMessage =
                                "Duplicate Error! Supplied Username already exist";
                            return true;
                        }
                }


                //Mobile Number Validation
                if (!checkMobile.Any() || checkMobile.Count != 1)
                {
                    response.Status.Message.FriendlyMessage =
                        "Unable to check duplicate[Mobile No]! Please try again later";
                    response.Status.Message.TechnicalMessage =
                        "Unable to check duplicate[Mobile No]! Please try again later";
                    return true;
                }

                if (checkMobile[0] > 0)
                    if (callType != 2)
                    {
                        response.Status.Message.FriendlyMessage =
                            "Duplicate Error! Supplied Mobile Number already exist";
                        response.Status.Message.TechnicalMessage =
                            "Duplicate Error! Supplied Mobile Number already exist";
                        return true;
                    }

                #endregion


                return false;
            }
            catch (Exception ex)
            {
                response.Status.Message.FriendlyMessage =
                    "Unable to complete your request due to Validation Error:Please try  again later";
                response.Status.Message.TechnicalMessage = "Duplicate check error:" + ex.Message;
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                return true;
            }
        }

        private List<Staff> getStaffInfos(StaffSearchObj searchObj)
        {
            try
            {
                var thisSql = new StringBuilder();
                thisSql.Append("SELECT *  FROM  \"GPlus\".\"Staff\" WHERE  1 = 1");
                if (searchObj.CompanyId > 0)
                {
                    thisSql.Append($" AND  \"CompanyId\" = {searchObj.CompanyId}");
                }
                if (!string.IsNullOrEmpty(searchObj.FirstName) && searchObj.FirstName.Length > 2)
                {
                    thisSql.Append($" AND  lower(\"FirstName\") = lower('{searchObj.FirstName.Trim()}')");
                }
                if (!string.IsNullOrEmpty(searchObj.LastName) && searchObj.LastName.Length > 2)
                {
                    thisSql.Append($" AND  lower(\"LastName\") = lower('{searchObj.LastName.Trim()}')");
                }
                if (!string.IsNullOrEmpty(searchObj.Email) && searchObj.Email.Length > 5)
                {
                    thisSql.Append($" AND  lower(\"Email\") = lower('{searchObj.Email.Trim()}')");
                }
                if (!string.IsNullOrEmpty(searchObj.MobileNumber) && searchObj.MobileNumber.Length == 11)
                {
                    thisSql.Append($" AND  lower(\"MobileNumber\") = lower('{searchObj.MobileNumber.Trim()}')");
                }
                if (!string.IsNullOrEmpty(searchObj.Username) && searchObj.Username.Length > 2)
                {
                    thisSql.Append(
                        $" AND   \"StaffId\"  = (SELECT coalesce(\"StaffId\", 0)  FROM  \"GPlus\".\"StaffAccess\" WHERE lower(\"Username\") = lower('{searchObj.Username.Trim()}'))");
                }
                if (searchObj.Status > -100)
                {
                    thisSql.Append($" AND  \"Status\" = {searchObj.Status}");
                }
                var staffList = _repository.RepositoryContext().Database.SqlQuery<Staff>(thisSql.ToString()).ToList();
                if (!staffList.Any() || staffList.Count < 1)
                {
                    return null;
                }
                  
                return staffList;
            }

            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }

        private List<Staff> getStaffInfos()
        {
            try
            {
                var thisSql = new StringBuilder();
                thisSql.Append("SELECT *  FROM  \"GPlus\".\"Staff\" ORDER BY \"StaffId\" ");
               
                var staffList = _repository.RepositoryContext().Database.SqlQuery<Staff>(thisSql.ToString()).ToList();
                if (!staffList.Any() || staffList.Count < 1)
                    return null;
                return staffList;
            }

            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }

        #endregion


        #region StaffContact

        public bool IsStaffContactDefault(int staffId)
        {
            try
            {
                var sql1 =
                    $"SELECT coalesce(Count(\"StaffContactId\"), 0)  FROM  \"GPlus\".\"StaffContact\" WHERE  \"StaffId\" = {staffId} AND  \"IsDefault\" = {true};";
                var staffContactInfos = _repository.RepositoryContext().Database.SqlQuery<long>(sql1).ToList();

                if (!staffContactInfos.Any() || staffContactInfos.Count < 1)
                    return false;
                return true;
            }

            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);

                return false;
            }
        }

        //private StaffContact GetStaffContactInfo(int staffId)
        //{
        //    try
        //    {
        //        var sql1 = $"SELECT *  FROM  \"GPlus\".\"StaffContact\" WHERE  \"StaffId\" = {staffId};";
        //        var agentInfos = _repository.RepositoryContext().Database.SqlQuery<StaffContact>(sql1).ToList();
        //        if (!agentInfos.Any() || agentInfos.Count != 1)
        //        {
        //            return null;
        //        }
        //        return agentInfos[0];
        //    }

        //    catch (Exception ex)
        //    {
        //        ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
        //        return null;
        //    }
        //}

        private List<StaffContact> getStaffContactInfos(StaffContactSearchObj searchObj)
        {
            try
            {
                var thisSql = new StringBuilder();
                thisSql.Append("SELECT *  FROM  \"GPlus\".\"StaffContact\" WHERE  1 = 1");

                if (searchObj.Status > -10)
                    thisSql.Append($" AND  \"Status\" = {searchObj.Status}");
                if (searchObj.StaffId > 0)
                    thisSql.Append($" AND  \"StaffId\" = {searchObj.StaffId}");
                if (searchObj.StaffContactId > 0)
                    thisSql.Append($" AND  \"StaffContactId\" = {searchObj.StaffContactId}");
                if (!string.IsNullOrEmpty(searchObj.ResidentialAddress) && searchObj.ResidentialAddress.Length > 3)
                    thisSql.Append(
                        $" AND  lower(\"ResidentialAddress\") = lower('{searchObj.ResidentialAddress.Trim()}')");

                if (!string.IsNullOrEmpty(searchObj.LocationLandmark) && searchObj.LocationLandmark.Length > 1)
                    thisSql.Append($" AND  lower(\"LocationLandmark\") = lower('{searchObj.LocationLandmark.Trim()}')");

                if (!string.IsNullOrEmpty(searchObj.TownOfResidence) && searchObj.TownOfResidence.Length > 1)
                    thisSql.Append($" AND  lower(\"TownOfResidence\") = lower('{searchObj.TownOfResidence.Trim()}')");

                if (searchObj.StateOfResidenceId > 0)
                    thisSql.Append($" AND  \"StateOfResidenceId\" = {searchObj.StateOfResidenceId}");

                if (searchObj.LocalAreaOfResidenceId > 0)
                    thisSql.Append($" AND  \"LocalAreaOfResidenceId\" = {searchObj.LocalAreaOfResidenceId}");

                if (searchObj.Status > -1)
                    thisSql.Append($" AND  \"Status\" = {searchObj.Status}");

                var staffContactList = _staffContactRepository.RepositoryContext().Database
                    .SqlQuery<StaffContact>(thisSql.ToString()).ToList();
                if (!staffContactList.Any() || staffContactList.Count < 1)
                    return null;
                return staffContactList;
            }

            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }

        private List<StaffContact> getStaffContactInfo(int staffId)
        {
            try
            {
                var sql1 = $"SELECT *  FROM  \"GPlus\".\"StaffContact\" WHERE  \"StaffId\" = {staffId};";


                var staffContacttList = _staffContactRepository.RepositoryContext().Database
                    .SqlQuery<StaffContact>(sql1).ToList();
                if (!staffContacttList.Any() || staffContacttList.Count < 1)
                    return null;
                return staffContacttList;
            }

            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }

        #endregion

        #region Staff_Next_Of_Kin

        private bool IsStaffNextOfKinDuplicate(string lastName, string firstName, string middleName, string email,
            string mobileNumber, string landphone, int callType, ref SettingRegRespObj response)
        {
            try
            {
                #region FullName

                var sql1 =
                    $"Select coalesce(Count(\"StaffNextOfKinId\"), 0) FROM \"GPlus\".\"StaffNextOfKin\" WHERE lower(\"FirstName\") = lower('{firstName.Trim()}') AND " +
                    $" lower(\"LastName\") = lower('{lastName.Trim()}') AND lower(\"MiddleName\") = lower('{middleName.Trim()}')";

                var sqlEmail =
                    $"Select coalesce(Count(\"StaffNextOfKinId\"), 0) FROM \"GPlus\".\"StaffNextOfKin\" WHERE lower(\"Email\") = lower('{email}')";

                var sqlUname =
                    $"Select coalesce(Count(\"StaffNextOfKinId\"), 0) FROM \"GPlus\".\"StaffNextOfKin\" WHERE lower(\"Landphone\") = lower('{landphone}') ";

                var sqlMobile =
                    $"Select coalesce(Count(\"StaffNextOfKinId\"), 0) FROM \"GPlus\".\"StaffNextOfKin\" WHERE lower(\"MobileNumber\") = lower('{mobileNumber.Trim()}') ";

                var check1 = _staffNextOfKinRepository.RepositoryContext().Database.SqlQuery<long>(sql1).ToList();
                var checkEmail = _staffNextOfKinRepository.RepositoryContext().Database.SqlQuery<long>(sqlEmail)
                    .ToList();
                var checkMobile = _staffNextOfKinRepository.RepositoryContext().Database.SqlQuery<long>(sqlMobile)
                    .ToList();

                if (!check1.Any())
                {
                    response.Status.Message.FriendlyMessage =
                        "Unable to check duplicate [Next of Kin Name]! Please try again later";
                    response.Status.Message.TechnicalMessage =
                        "Unable to check duplicate [Next of Kin Name]! Please try again later";
                    return true;
                }

                if (check1.Count != 1)
                {
                    response.Status.Message.FriendlyMessage = "Unable to check duplicate! Please try again later";
                    response.Status.Message.TechnicalMessage = "Unable to check duplicate! Please try again later";
                    return true;
                }

                if (check1[0] > 0)
                    if (callType != 2)
                    {
                        response.Status.Message.FriendlyMessage =
                            "Duplicate Error! Next of Kin  Information already exist";
                        response.Status.Message.TechnicalMessage =
                            "Duplicate Error!  Next of Kin  Information already exist";
                        return true;
                    }

                //Email Validation
                if (!checkEmail.Any() || checkEmail.Count != 1)
                {
                    response.Status.Message.FriendlyMessage =
                        "Unable to check duplicate[Email]! Please try again later";
                    response.Status.Message.TechnicalMessage =
                        "Unable to check duplicate[Email]! Please try again later";
                    return true;
                }

                if (checkEmail[0] > 0)
                    if (callType != 2)
                    {
                        response.Status.Message.FriendlyMessage = "Duplicate Error! Next of Kin  Email already exist";
                        response.Status.Message.TechnicalMessage = "Duplicate Error! Next of Kin  Email already exist";
                        return true;
                    }

                //LandPhone Validation
                if (callType == 1)
                {
                    var checkLandphone = _repository.RepositoryContext().Database.SqlQuery<long>(sqlUname).ToList();
                    if (!checkLandphone.Any() || checkLandphone.Count != 1)
                    {
                        response.Status.Message.FriendlyMessage =
                            "Unable to check duplicate[LandPhone]! Please try again later";
                        response.Status.Message.TechnicalMessage =
                            "Unable to check duplicate[LandPhone]! Please try again later";
                        return true;
                    }

                    if (checkLandphone[0] > 0)
                        if (callType != 2)
                        {
                            response.Status.Message.FriendlyMessage =
                                "Duplicate Error! Supplied LandPhone already exist";
                            response.Status.Message.TechnicalMessage =
                                "Duplicate Error! Supplied LandPhone already exist";
                            return true;
                        }
                }


                //Mobile Number Validation
                if (!checkMobile.Any() || checkMobile.Count != 1)
                {
                    response.Status.Message.FriendlyMessage =
                        "Unable to check duplicate[Mobile No]! Please try again later";
                    response.Status.Message.TechnicalMessage =
                        "Unable to check duplicate[Mobile No]! Please try again later";
                    return true;
                }

                if (checkMobile[0] > 0)
                    if (callType != 2)
                    {
                        response.Status.Message.FriendlyMessage =
                            "Duplicate Error! Supplied Mobile Number already exist";
                        response.Status.Message.TechnicalMessage =
                            "Duplicate Error! Supplied Mobile Number already exist";
                        return true;
                    }

                #endregion


                return false;
            }
            catch (Exception ex)
            {
                response.Status.Message.FriendlyMessage =
                    "Unable to complete your request due to Validation Error:Please try  again later";
                response.Status.Message.TechnicalMessage = "Duplicate check error:" + ex.Message;
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                return true;
            }
        }

        private StaffNextOfKin getStaffNextInfoInfo(int staffNextofKinId)
        {
            try
            {
                var sql1 = $"SELECT *  FROM  \"GPlus\".\"StaffNextOfKin\" WHERE  \"StaffId\" = {staffNextofKinId};";
                var agentInfos = _repository.RepositoryContext().Database.SqlQuery<StaffNextOfKin>(sql1).ToList();
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

        private List<StaffNextOfKin> getStaffNextOfKinInfos(StaffNextOfKinSearchObj searchObj)
        {
            try
            {
                var thisSql = new StringBuilder();
                thisSql.Append("SELECT *  FROM  \"GPlus\".\"StaffNextOfKin\" WHERE  1 = 1");


                if (searchObj.StaffId > 0)
                    thisSql.Append($" AND  \"StaffId\" = {searchObj.StaffId}");
                if (searchObj.StaffNextOfKinId > 0)
                    thisSql.Append($" AND  \"StaffNextOfKinId\" = {searchObj.StaffNextOfKinId}");
                if (!string.IsNullOrEmpty(searchObj.ResidentialAddress) && searchObj.ResidentialAddress.Length > 3)
                    thisSql.Append(
                        $" AND  lower(\"ResidentialAddress\") = lower('{searchObj.ResidentialAddress.Trim()}')");

                if (!string.IsNullOrEmpty(searchObj.MobileNumber) && searchObj.MobileNumber.Length > 11)
                    thisSql.Append($" AND  lower(\"MobileNumber\") = lower('{searchObj.MobileNumber.Trim()}')");

                if (searchObj.StateOfLocationId > 0)
                    thisSql.Append($" AND  \"StateOfLocationId\" = {searchObj.StateOfLocationId}");

                if (searchObj.LocalAreaOfLocationId > 0)
                    thisSql.Append($" AND  \"LocalAreaOfLocationId\" = {searchObj.LocalAreaOfLocationId}");


                var staffNextOfkin = _staffContactRepository.RepositoryContext().Database
                    .SqlQuery<StaffNextOfKin>(thisSql.ToString()).ToList();
                if (!staffNextOfkin.Any() || staffNextOfkin.Count < 1)
                    return null;
                return staffNextOfkin;
            }

            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }

        #endregion

        #region EducationalQualification

        private bool IsStaffEducationalQualificationDuplicate(int staffId, int institutionId, int qualificationId,int disciplineId, int courseOfStudtId, int startYear, int endYear, int callType,ref SettingRegRespObj response)
        {
            try
            {
                #region

                var sql1 =
                    $"Select coalesce(Count(\"HigherEducationId\"), 0) FROM \"GPlus\".\"HigherEducation\" WHERE \"StaffId\" = {staffId} AND " +
                    $" \"StartYear\" = {startYear} AND \"EndYear\" = {endYear} AND \"DisciplineId\" = {disciplineId} AND \"CourseOfStudyId\" = {courseOfStudtId} AND \"QualificationId\" = {qualificationId} AND \"InstitutionId\" = {institutionId} ";


                var check1 = _staffHigherEducationRepository.RepositoryContext().Database.SqlQuery<long>(sql1).ToList();


                if (!check1.Any())
                {
                    response.Status.Message.FriendlyMessage =
                        "Unable to check duplicate [Educational Qualification]! Please try again later";
                    response.Status.Message.TechnicalMessage =
                        "Unable to check duplicate [Educational Qualification]! Please try again later";
                    return true;
                }

                if (check1.Count != 1)
                {
                    response.Status.Message.FriendlyMessage = "Unable to check duplicate! Please try again later";
                    response.Status.Message.TechnicalMessage = "Unable to check duplicate! Please try again later";
                    return true;
                }

                if (check1[0] > 0)
                    if (callType != 2)
                    {
                        response.Status.Message.FriendlyMessage =
                            "Duplicate Error! Educational Qualification  Information already exist";
                        response.Status.Message.TechnicalMessage =
                            "Duplicate Error!  Educational Qualification  Information already exist";
                        return true;
                    }


                return false;
            }
            catch (Exception ex)
            {
                response.Status.Message.FriendlyMessage =
                    "Unable to complete your request due to Validation Error:Please try  again later";
                response.Status.Message.TechnicalMessage = "Duplicate check error:" + ex.Message;
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                return true;
            }
        }

        private List<HigherEducation> getHigherEducationInfos(EducationalQualificationSearchObj searchObj)
        {
            try
            {
                var thisSql = new StringBuilder();
                thisSql.Append("SELECT *  FROM  \"GPlus\".\"HigherEducation\" WHERE  1 = 1");


                if (searchObj.StaffId > 0)
                    thisSql.Append($" AND  \"StaffId\" = {searchObj.StaffId}");

                if (searchObj.HigherEducationId > 0)
                    thisSql.Append($" AND  \"HigherEducationId\" = {searchObj.HigherEducationId}");

                if (searchObj.DisciplineId > 0)
                    thisSql.Append($" AND  \"DisciplineId\" = {searchObj.DisciplineId}");

                if (searchObj.CourseOfStudyId > 0)
                    thisSql.Append($" AND  \"CourseOfStudyId\" = {searchObj.CourseOfStudyId}");

                if (searchObj.QualificationId > 0)
                    thisSql.Append($" AND  \"QualificationId\" = {searchObj.QualificationId}");

                if (searchObj.InstitutionId > 0)
                    thisSql.Append($" AND  \"InstitutionId\" = {searchObj.InstitutionId}");
                var higherEducationList = _staffContactRepository.RepositoryContext().Database
                    .SqlQuery<HigherEducation>(thisSql.ToString()).ToList();
                if (!higherEducationList.Any() || higherEducationList.Count < 1)
                    return null;
                return higherEducationList;
            }

            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }

        private HigherEducation getHigherEducationInfo(int educationQualfId)
        {
            try
            {
                var sql1 =
                    $"SELECT *  FROM  \"GPlus\".\"HigherEducation\" WHERE  \"HigherEducationId\" = {educationQualfId};";
                var agentInfos = _repository.RepositoryContext().Database.SqlQuery<HigherEducation>(sql1).ToList();
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

        #endregion

        #endregion

        #region ProfessionalMembership

        private ProfessionalMembership GetStaffProfessionalMembershipInfo(int staffId)
        {
            try
            {
                var sql1 = $"SELECT *  FROM  \"GPlus\".\"ProfessionalMembership\" WHERE  \"StaffId\" = {staffId};";
                var agentInfos = _staffProfessionalMembershipRepository.RepositoryContext().Database
                    .SqlQuery<ProfessionalMembership>(sql1).ToList();
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

        private List<ProfessionalMembership> getProfessionalMembershipInfos(ProfessionalMemberShipSearchObj searchObj)
        {
            try
            {
                var thisSql = new StringBuilder();
                thisSql.Append("SELECT *  FROM  \"GPlus\".\"ProfessionalMembership\" WHERE  1 = 1");


                if (searchObj.StaffId > 0)
                    thisSql.Append($" AND  \"StaffId\" = {searchObj.StaffId}");
                if (searchObj.ProfessionalMembershipId > 0)
                    thisSql.Append($" AND  \"ProfessionalMembershipId\" = {searchObj.ProfessionalMembershipId}");
                if (searchObj.ProfessionalMembershipId > 0)
                    thisSql.Append($" AND  \"ProfessionalBodyId\" = {searchObj.ProfessionalMembershipId}");
                if (searchObj.ProfessionalMembershipTypeId > 0)
                    thisSql.Append(
                        $" AND  \"ProfessionalMembershipTypeId\" = {searchObj.ProfessionalMembershipTypeId}");

                var staffProfessionalMembershipList = _staffProfessionalMembershipRepository.RepositoryContext()
                    .Database.SqlQuery<ProfessionalMembership>(thisSql.ToString()).ToList();
                if (!staffProfessionalMembershipList.Any() || staffProfessionalMembershipList.Count < 1)
                    return null;
                return staffProfessionalMembershipList;
            }

            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }

        private List<ProfessionalMembership> GetStaffProfmeMembershipsInfo(int staffId)
        {
            try
            {
                var sql1 = $"SELECT *  FROM  \"GPlus\".\"ProfessionalMembership\" WHERE  \"StaffId\" = {staffId};";


                var staffProMembershipstList = _staffProfessionalMembershipRepository.RepositoryContext().Database
                    .SqlQuery<ProfessionalMembership>(sql1).ToList();
                if (!staffProMembershipstList.Any() || staffProMembershipstList.Count < 1)
                    return null;
                return staffProMembershipstList;
            }

            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }

        #endregion

        #region Staff Insurance

        //public bool IsStaffContactDefault(int staffId)
        //{
        //    try
        //    {
        //        var sql1 =
        //            $"SELECT coalesce(Count(\"StaffContactId\"), 0)  FROM  \"GPlus\".\"StaffContact\" WHERE  \"StaffId\" = {staffId} AND  \"IsDefault\" = {true};";
        //        var staffContactInfos = _repository.RepositoryContext().Database.SqlQuery<long>(sql1).ToList();

        //        if (!staffContactInfos.Any() || staffContactInfos.Count < 1)
        //        {
        //            return false;
        //        }
        //        return true;
        //    }

        //    catch (Exception ex)
        //    {
        //        ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);

        //        return false;
        //    }
        //}

        private StaffInsurance GetStaffInsuranceInfo(int staffId)
        {
            try
            {
                var sql1 = $"SELECT *  FROM  \"GPlus\".\"StaffInsurance\" WHERE  \"StaffId\" = {staffId};";
                var agentInfos = _staffInsuranceRepository.RepositoryContext().Database.SqlQuery<StaffInsurance>(sql1)
                    .ToList();
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

        private List<StaffInsurance> getStaffInsuranceInfos(StaffInsuranceSearchObj searchObj)
        {
            try
            {
                var thisSql = new StringBuilder();
                thisSql.Append("SELECT *  FROM  \"GPlus\".\"StaffInsurance\" WHERE  1 = 1");

                if (searchObj.Status > -100)
                    thisSql.Append($" AND  \"Status\" = {searchObj.Status}");
                if (searchObj.StaffId > 0)
                    thisSql.Append($" AND  \"StaffId\" = {searchObj.StaffId}");

                if (searchObj.Status > -1)
                    thisSql.Append($" AND  \"Status\" = {searchObj.Status}");

                var staffContactList = _staffInsuranceRepository.RepositoryContext().Database
                    .SqlQuery<StaffInsurance>(thisSql.ToString()).ToList();
                if (!staffContactList.Any() || staffContactList.Count < 1)
                    return null;
                return staffContactList;
            }

            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }

        private List<StaffInsurance> getStafInsuranceInfo(int staffId)
        {
            try
            {
                var sql1 = $"SELECT *  FROM  \"GPlus\".\"StaffInsurance\" WHERE  \"StaffId\" = {staffId};";


                var staffInsurancetList = _staffInsuranceRepository.RepositoryContext().Database
                    .SqlQuery<StaffInsurance>(sql1).ToList();
                if (!staffInsurancetList.Any() || staffInsurancetList.Count < 1)
                    return null;
                return staffInsurancetList;
            }

            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }

        #endregion

        #region Emergency

        //private bool IsStaffEmergencycontactDuplicate(string lastName, string firstName, string middleName, string mobileNumber, int callType, ref SettingRegRespObj response)
        //{
        //    try
        //    {
        //        #region 

        //        var sql1 =
        //            $"Select coalesce(Count(\"EmergencyContactId\"), 0) FROM \"GPlus\".\"EmergencyContact\" WHERE lower('\"LastName\") = lower('{lastName.Trim()}') AND " +
        //            $" lower('\"FirstName\") = lower('{firstName.Trim()}') AND lower('\"MiddleName\") = lower('{middleName.Trim()}')";

        //        var sqlMobile =
        //            $"Select coalesce(Count(\"EmergencyContactId\"), 0) FROM \"GPlus\".\"EmergencyContact\" WHERE lower(\"MobileNumber\") = lower('{mobileNumber.Trim()}') )";


        //        var check1 = _emergencyContactRepository.RepositoryContext().Database.SqlQuery<long>(sql1).ToList();
        //        var checkMobile = _emergencyContactRepository.RepositoryContext().Database.SqlQuery<long>(sqlMobile).ToList();

        //        if (!check1.Any())
        //        {
        //            response.Status.Message.FriendlyMessage =
        //                "Unable to check duplicate [Educational Qualification]! Please try again later";
        //            response.Status.Message.TechnicalMessage =
        //                "Unable to check duplicate [Educational Qualification]! Please try again later";
        //            return true;
        //        }

        //        if (check1.Count != 1)
        //        {
        //            response.Status.Message.FriendlyMessage = "Unable to check duplicate! Please try again later";
        //            response.Status.Message.TechnicalMessage = "Unable to check duplicate! Please try again later";
        //            return true;
        //        }

        //        if (check1[0] > 0)
        //        {
        //            if (callType != 2)
        //            {
        //                response.Status.Message.FriendlyMessage = "Duplicate Error! Educational Qualification  Information already exist";
        //                response.Status.Message.TechnicalMessage = "Duplicate Error!  Educational Qualification  Information already exist";
        //                return true;
        //            }
        //        }
        //        //Mobile Number Validation
        //        if (!checkMobile.Any() || checkMobile.Count != 1)
        //        {
        //            response.Status.Message.FriendlyMessage =
        //                "Unable to check duplicate[Mobile No]! Please try again later";
        //            response.Status.Message.TechnicalMessage =
        //                "Unable to check duplicate[Mobile No]! Please try again later";
        //            return true;
        //        }

        //        if (checkMobile[0] > 0)
        //        {
        //            if (callType != 2)
        //            {
        //                response.Status.Message.FriendlyMessage =
        //                    "Duplicate Error! Supplied Mobile Number already exist";
        //                response.Status.Message.TechnicalMessage =
        //                    "Duplicate Error! Supplied Mobile Number already exist";
        //                return true;
        //            }
        //        }

        //        #endregion


        //        return false;

        //    }
        //    catch (Exception ex)
        //    {
        //        response.Status.Message.FriendlyMessage =
        //            "Unable to complete your request due to Validation Error:Please try  again later";
        //        response.Status.Message.TechnicalMessage = "Duplicate check error:" + ex.Message;
        //        ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
        //        return true;
        //    }
        //}

        private List<EmergencyContact> GetStaffEmergencyContactsInfos(EmergencyContactSearchObj searchObj)
        {
            try
            {
                var thisSql = new StringBuilder();
                thisSql.Append("SELECT *  FROM  \"GPlus\".\"EmergencyContact\" WHERE  1 = 1");

                if (searchObj.Status > -10)
                    thisSql.Append($" AND  \"Status\" = {searchObj.Status}");
                if (searchObj.StaffId > 0)
                    thisSql.Append($" AND  \"StaffId\" = {searchObj.StaffId}");

                if (!string.IsNullOrEmpty(searchObj.MobileNumber) && searchObj.MobileNumber.Length > 1)
                    thisSql.Append($" AND  lower(\"MobileNumber\") = lower('{searchObj.MobileNumber.Trim()}')");

                if (!string.IsNullOrEmpty(searchObj.FirstName) && searchObj.FirstName.Length > 1)
                    thisSql.Append($" AND  lower(\"FirstName\") = lower('{searchObj.FirstName.Trim()}')");


                if (searchObj.EmergencyContactId > 0)
                    thisSql.Append($" AND  \"EmergencyContactId\" = {searchObj.EmergencyContactId}");

                if (searchObj.Status > -100)
                    thisSql.Append($" AND  \"Status\" = {searchObj.Status}");

                var staffEmergencyContactstList = _emergencyContactRepository.RepositoryContext().Database
                    .SqlQuery<EmergencyContact>(thisSql.ToString()).ToList();
                if (!staffEmergencyContactstList.Any() || staffEmergencyContactstList.Count < 1)
                    return null;
                return staffEmergencyContactstList;
            }

            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }

        private List<EmergencyContact> getEmergencyContactInfo(int staffId)
        {
            try
            {
                var sql1 = $"SELECT *  FROM  \"GPlus\".\"EmergencyContact\" WHERE  \"StaffId\" = {staffId};";


                var staffBankAccounttList = _emergencyContactRepository.RepositoryContext().Database
                    .SqlQuery<EmergencyContact>(sql1).ToList();
                if (!staffBankAccounttList.Any() || staffBankAccounttList.Count < 1)
                    return null;
                return staffBankAccounttList;
            }

            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }

        private EmergencyContact GetEmergencyContactInfo(int staffId)
        {
            try
            {
                var sql1 = $"SELECT *  FROM  \"GPlus\".\"EmergencyContact\" WHERE  \"StaffId\" = {staffId}";
                var emergencyContactInfos =
                    _repository.RepositoryContext().Database.SqlQuery<EmergencyContact>(sql1).ToList();
                if (!emergencyContactInfos.Any() || emergencyContactInfos.Count != 1)
                    return null;
                return emergencyContactInfos[0];
            }

            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }

        #endregion

        #region staffBankAccount

        private StaffBankAccount GetStaffBankAccountInfo(int bankAccountId)
        {
            try
            {
                var sql1 =
                    $"SELECT *  FROM  \"GPlus\".\"StaffBankAccount\" WHERE  \"StaffBankAccountId\" = {bankAccountId};";
                var staffBankAcountInfos = _staffBankAccRepository.RepositoryContext().Database
                    .SqlQuery<StaffBankAccount>(sql1).ToList();

                if (!staffBankAcountInfos.Any() || staffBankAcountInfos.Count != 1)
                    return null;
                return staffBankAcountInfos[0];
            }

            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }

        private List<StaffBankAccount> getStaffBankAccInfos(StaffBankAccountSearchObj searchObj)
        {
            try
            {
                var thisSql = new StringBuilder();
                thisSql.Append("SELECT *  FROM  \"GPlus\".\"StaffBankAccount\" WHERE  1 = 1");

                if (searchObj.Status > -10)
                    thisSql.Append($" AND  \"Status\" = {searchObj.Status}");
                if (searchObj.StaffId > 0)
                    thisSql.Append($" AND  \"StaffId\" = {searchObj.StaffId}");

                if (!string.IsNullOrEmpty(searchObj.AccountNumber) && searchObj.AccountNumber.Length > 1)
                    thisSql.Append($" AND  lower(\"AccountNumber\") = lower('{searchObj.AccountNumber.Trim()}')");

                if (!string.IsNullOrEmpty(searchObj.AccountName) && searchObj.AccountName.Length > 1)
                    thisSql.Append($" AND  lower(\"AccountName\") = lower('{searchObj.AccountName.Trim()}')");


                if (searchObj.StaffBankAccountId > 0)
                    thisSql.Append($" AND  \"StaffBankAccountId\" = {searchObj.StaffBankAccountId}");

                if (searchObj.Status > -1)
                    thisSql.Append($" AND  \"Status\" = {searchObj.Status}");

                var staffBankAccounttList = _staffBankAccRepository.RepositoryContext().Database
                    .SqlQuery<StaffBankAccount>(thisSql.ToString()).ToList();
                if (!staffBankAccounttList.Any() || staffBankAccounttList.Count < 1)
                    return null;
                return staffBankAccounttList;
            }

            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }

        private List<StaffBankAccount> getStaffBankAccInfo(int staffId)
        {
            try
            {
                var sql1 = $"SELECT *  FROM  \"GPlus\".\"StaffBankAccount\" WHERE  \"StaffId\" = {staffId};";


                var staffBankAccounttList = _staffBankAccRepository.RepositoryContext().Database
                    .SqlQuery<StaffBankAccount>(sql1).ToList();
                if (!staffBankAccounttList.Any() || staffBankAccounttList.Count < 1)
                    return null;
                return staffBankAccounttList;
            }

            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }

        #endregion

        #region Leave Request

        private LeaveRequest GetStaffLeaveRequestInfo(long staffId)
        {
            try
            {
                var sql1 = $"SELECT *  FROM  \"GPlus\".\"LeaveRequest\" WHERE  \"StaffId\" = {staffId};";
                var staffLeaveRequestInfos = _staffLeaveRequestRepository.RepositoryContext().Database
                    .SqlQuery<LeaveRequest>(sql1).ToList();

                if (!staffLeaveRequestInfos.Any() || staffLeaveRequestInfos.Count != 1)
                    return null;
                return staffLeaveRequestInfos[0];
            }

            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }

        private WorkflowSetup GetWorkflowSetupInfo(int staffId)
        {
            try
            {
                var sql1 = $"SELECT *  FROM  \"GPlus\".\"WorkflowSetup\" WHERE  \"StaffId\" = {staffId};";
                var workflowSetupInfos = _workflowSetupRepository.RepositoryContext().Database
                    .SqlQuery<WorkflowSetup>(sql1).ToList();

                if (!workflowSetupInfos.Any() || workflowSetupInfos.Count != 1)
                    return null;
                return workflowSetupInfos[0];
            }

            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }

        private List<LeaveRequest> GetLeaveRequestInfos(LeaveRequestSearchObj searchObj)
        {
            try
            {
                var thisSql = new StringBuilder();
                thisSql.Append("SELECT *  FROM  \"GPlus\".\"LeaveRequest\" WHERE  1 = 1");

                if (searchObj.Status > -100)
                    thisSql.Append($" AND  \"Status\" = {searchObj.Status}");
                if (!string.IsNullOrEmpty(searchObj.LeaveTitle))
                    thisSql.Append($" AND  lower(\"LeaveTitle\") = lower('{searchObj.LeaveTitle.Trim()}')");
                if (searchObj.StaffId > 0)
                    thisSql.Append($" AND  \"StaffId\" = {searchObj.StaffId}");
                if (searchObj.LeaveType > 0)
                    thisSql.Append($" AND  \"LeaveType\" = {searchObj.LeaveType}");
                if (searchObj.LeaveRequestId > 0)
                    thisSql.Append($" AND  \"LeaveRequestId\" = {searchObj.LeaveRequestId}");


                var leaveRequestList = _staffLeaveRequestRepository.RepositoryContext().Database
                    .SqlQuery<LeaveRequest>(thisSql.ToString()).ToList();
                if (!leaveRequestList.Any() || leaveRequestList.Count < 1)
                    return null;

                return leaveRequestList;
            }

            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }

        private List<LeaveRequest> getStaffLeaveRequestInfo(int staffId, int leaveRequestId)
        {
            try
            {
                var sql1 =
                    $"SELECT *  FROM  \"GPlus\".\"LeaveRequest\" WHERE  \"StaffId\" = {staffId} AND \"LeaveRequestId\" = {leaveRequestId} ;";


                var staffLeaveRequestList = _staffLeaveRequestRepository.RepositoryContext().Database
                    .SqlQuery<LeaveRequest>(sql1).ToList();
                if (!staffLeaveRequestList.Any() || staffLeaveRequestList.Count < 1)
                    return null;
                return staffLeaveRequestList;
            }

            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }

        private List<LeaveRequest> getStaffLeaveRequest(int staffId)
        {
            try
            {
                var sql1 = $"SELECT *  FROM  \"GPlus\".\"LeaveRequest\" WHERE  \"StaffId\" = {staffId};";


                var staffLeaveRequestList = _staffLeaveRequestRepository.RepositoryContext().Database
                    .SqlQuery<LeaveRequest>(sql1).ToList();
                if (!staffLeaveRequestList.Any() || staffLeaveRequestList.Count < 1)
                    return null;
                return staffLeaveRequestList;
            }

            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }


        private bool IsStaffLeaveRequestDuplicate(int staffId, string leaveTitle, int leaveTypeId,
            string proposedStartDate, string proposedEndDate, int callType, ref SettingRegRespObj response)
        {
            try
            {
                var sql1 =
                    $"Select coalesce(Count(\"LeaveRequestId\"), 0) FROM \"GPlus\".\"LeaveRequest\" WHERE \"StaffId\" = {staffId} AND " +
                    $" lower(\"LeaveTitle\") = lower('{leaveTitle.Trim()}') AND lower(\"ProposedStartDate\") = lower('{proposedStartDate.Trim()}')AND lower(\"ProposedEndDate\") = lower('{proposedEndDate.Trim()}') AND  \"LeaveType\" = {leaveTypeId}";


                var check1 = _staffLeaveRequestRepository.RepositoryContext().Database.SqlQuery<long>(sql1).ToList();

                if (!check1.Any())
                {
                    response.Status.Message.FriendlyMessage =
                        "Unable to check duplicate [Leave Request]! Please try again later";
                    response.Status.Message.TechnicalMessage =
                        "Unable to check duplicate [Leave Request]! Please try again later";
                    return true;
                }

                if (check1.Count != 1)
                {
                    response.Status.Message.FriendlyMessage = "Unable to check duplicate! Please try again later";
                    response.Status.Message.TechnicalMessage = "Unable to check duplicate! Please try again later";
                    return true;
                }

                if (check1[0] > 0)
                    if (callType != 2)
                    {
                        response.Status.Message.FriendlyMessage =
                            "Duplicate Error! Leave Request  Information already exist";
                        response.Status.Message.TechnicalMessage =
                            "Duplicate Error!  Leave Request  Information already exist";
                        return true;
                    }


                return false;
            }
            catch (Exception ex)
            {
                response.Status.Message.FriendlyMessage =
                    "Unable to complete your request due to Validation Error:Please try  again later";
                response.Status.Message.TechnicalMessage = "Duplicate check error:" + ex.Message;
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                return true;
            }
        }

        #endregion

        #region StaffLeave

        private StaffLeave GetStaffLeaveInfo(int staffId)
        {
            try
            {
                var sql1 = $"SELECT *  FROM  \"GPlus\".\"StaffLeave\" WHERE  \"StaffId\" = {staffId};";
                var staffLeaveRequestInfos = _staffLeaveRequestRepository.RepositoryContext().Database
                    .SqlQuery<StaffLeave>(sql1).ToList();

                if (!staffLeaveRequestInfos.Any() || staffLeaveRequestInfos.Count != 1)
                    return null;
                return staffLeaveRequestInfos[0];
            }

            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }

        private List<StaffLeave> GetStaffLeaveInfos(StaffLeaveSearchObj searchObj)
        {
            try
            {
                var thisSql = new StringBuilder();
                thisSql.Append("SELECT *  FROM  \"GPlus\".\"StaffLeave\" WHERE  1 = 1");

                if (searchObj.Status > -100)
                    thisSql.Append($" AND  \"Status\" = {searchObj.Status}");

                if (searchObj.StaffLeaveId > 0)
                    thisSql.Append($" AND  \"StaffLeaveId\" = {searchObj.StaffLeaveId}");
                if (searchObj.StaffId > 0)
                    thisSql.Append($" AND  \"StaffId\" = {searchObj.StaffId}");
                if (searchObj.LeaveTypeId > 0)
                    thisSql.Append($" AND  \"LeaveTypeId\" = {searchObj.LeaveTypeId}");
                if (searchObj.LeaveRequestId > 0)
                    thisSql.Append($" AND  \"LeaveRequestId\" = {searchObj.LeaveRequestId}");


                var staffLeaveList = _staffLeaveRepository.RepositoryContext().Database
                    .SqlQuery<StaffLeave>(thisSql.ToString()).ToList();
                if (!staffLeaveList.Any() || staffLeaveList.Count < 1)
                    return null;

                return staffLeaveList;
            }

            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }

        private List<WorkflowLog> GetStaffLeaveInfos(WorkFlowLogSearchObj searchObj)
        {
            try
            {
                var thisSql = new StringBuilder();
                thisSql.Append("SELECT *  FROM  \"GPlus\".\"StaffLeave\" WHERE  1 = 1");

                if (searchObj.Status > -100)
                    thisSql.Append($" AND  \"Status\" = {searchObj.Status}");

                if (searchObj.WorkflowLogId > 0)
                    thisSql.Append($" AND  \"WorkflowLogId\" = {searchObj.WorkflowLogId}");
                if (searchObj.StaffId > 0)
                    thisSql.Append($" AND  \"StaffId\" = {searchObj.StaffId}");
                if (searchObj.ProcessorId > 0)
                    thisSql.Append($" AND  \"ProcessorId\" = {searchObj.ProcessorId}");


                var workflowLogList = _workflowLogRepository.RepositoryContext().Database
                    .SqlQuery<WorkflowLog>(thisSql.ToString()).ToList();
                if (!workflowLogList.Any() || workflowLogList.Count < 1)
                    return null;

                return workflowLogList;
            }

            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }

        private List<StaffLeave> GetStaffLeave(long staffLeaveId)
        {
            try
            {
                var sql1 = $"SELECT *  FROM  \"GPlus\".\"StaffLeave\" WHERE  \"StaffLeaveId\" = {staffLeaveId};";


                var staffLeaveList = _staffLeaveRepository.RepositoryContext().Database.SqlQuery<StaffLeave>(sql1)
                    .ToList();
                if (!staffLeaveList.Any() || staffLeaveList.Count < 1)
                    return null;
                return staffLeaveList;
            }

            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }

        private List<WorkflowLog> GetWorkflowLog(long staffId)
        {
            try
            {
                var sql1 = $"SELECT *  FROM  \"GPlus\".\"WorkflowLog\" WHERE  \"StaffId\" = {staffId};";


                var workflowLogList = _workflowLogRepository.RepositoryContext().Database.SqlQuery<WorkflowLog>(sql1)
                    .ToList();
                if (!workflowLogList.Any() || workflowLogList.Count < 1)
                    return null;
                return workflowLogList;
            }

            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }

        private List<WorkflowSetup> GetWorkflowSetup(long initiatorId)
        {
            try
            {
                var sql1 = $"SELECT *  FROM  \"GPlus\".\"WorkflowSetup\" WHERE  \"InitiatorId\" = {initiatorId};";


                var workflowSetupList = _workflowSetupRepository.RepositoryContext().Database
                    .SqlQuery<WorkflowSetup>(sql1).ToList();
                if (!workflowSetupList.Any() || workflowSetupList.Count < 1)
                    return null;
                return workflowSetupList;
            }

            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }

        private bool IsStaffLeaveDuplicate(int staffId, long leaveRequestId, int leaveTypeId, string proposedStartDate,
            string proposedEndDate, int callType, ref SettingRegRespObj response)
        {
            try
            {
                var sql1 =
                    $"Select coalesce(Count(\"StaffLeaveId\"), 0) FROM \"GPlus\".\"StaffLeave\" WHERE \"StaffId\" = {staffId} AND " +
                    $" \"LeaveRequestId\" = {leaveRequestId} AND \"LeaveTypeId\" = {leaveTypeId} AND lower(\"ProposedStartDate\") = lower('{proposedStartDate.Trim()}') AND lower(\"ProposedEndDate\") = lower('{proposedEndDate.Trim()}')";


                var check1 = _staffLeaveRepository.RepositoryContext().Database.SqlQuery<long>(sql1).ToList();

                if (!check1.Any())
                {
                    response.Status.Message.FriendlyMessage =
                        "Unable to check duplicate [Staff Leave]! Please try again later";
                    response.Status.Message.TechnicalMessage =
                        "Unable to check duplicate [Staff Leave]! Please try again later";
                    return true;
                }

                if (check1.Count != 1)
                {
                    response.Status.Message.FriendlyMessage = "Unable to check duplicate! Please try again later";
                    response.Status.Message.TechnicalMessage = "Unable to check duplicate! Please try again later";
                    return true;
                }

                if (check1[0] > 0)
                    if (callType != 2)
                    {
                        response.Status.Message.FriendlyMessage =
                            "Duplicate Error! Staff Leave  Information already exist";
                        response.Status.Message.TechnicalMessage =
                            "Duplicate Error! Staff Leave  Information already exist";
                        return true;
                    }


                return false;
            }
            catch (Exception ex)
            {
                response.Status.Message.FriendlyMessage =
                    "Unable to complete your request due to Validation Error:Please try  again later";
                response.Status.Message.TechnicalMessage = "Duplicate check error:" + ex.Message;
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                return true;
            }
        }

        #endregion


        #region StaffSalary

        private StaffSalary GetSalaryInfo(int staffId)
        {
            try
            {
                var sql1 = $"SELECT *  FROM  \"GPlus\".\"StaffSalary\" WHERE  \"StaffId\" = {staffId};";
                var staffSalaryInfos = _staffSalaryRepository.RepositoryContext().Database.SqlQuery<StaffSalary>(sql1)
                    .ToList();

                if (!staffSalaryInfos.Any() || staffSalaryInfos.Count != 1)
                    return null;
                return staffSalaryInfos[0];
            }

            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }

        private List<StaffSalary> GetStaffSalaryInfos(StaffSalarySearchObj searchObj)
        {
            try
            {
                var thisSql = new StringBuilder();
                thisSql.Append("SELECT *  FROM  \"GPlus\".\"StaffSalary\" WHERE  1 = 1");

                if (searchObj.Status > -100)
                    thisSql.Append($" AND  \"Status\" = {searchObj.Status}");

                if (searchObj.StaffId > 0)
                    thisSql.Append($" AND  \"StaffId\" = {searchObj.StaffId}");
                if (searchObj.StaffSalaryId > 0)
                    thisSql.Append($" AND  \"StaffSalaryId\" = {searchObj.StaffSalaryId}");


                var salaryList = _staffSalaryRepository.RepositoryContext().Database
                    .SqlQuery<StaffSalary>(thisSql.ToString()).ToList();
                if (!salaryList.Any() || salaryList.Count < 1)
                    return null;

                return salaryList;
            }

            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }

        private List<StaffSalary> getStaffSalaryInfo(int staffId)
        {
            try
            {
                var sql1 = $"SELECT *  FROM  \"GPlus\".\"StaffSalary\" WHERE  \"StaffId\" = {staffId};";


                var staffSalaryList = _staffSalaryRepository.RepositoryContext().Database.SqlQuery<StaffSalary>(sql1)
                    .ToList();
                if (!staffSalaryList.Any() || staffSalaryList.Count < 1)
                    return null;
                return staffSalaryList;
            }

            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }

        private bool IsStaffSalaryDuplicate(int staffId, int callType, ref SettingRegRespObj response)
        {
            try
            {
                var sql1 =
                    $"Select coalesce(Count(\"StaffSalaryId\"), 0) FROM \"GPlus\".\"StaffSalary\" WHERE \"StaffId\" = {staffId}";


                var check1 = _staffSalaryRepository.RepositoryContext().Database.SqlQuery<long>(sql1).ToList();

                if (!check1.Any())
                {
                    response.Status.Message.FriendlyMessage =
                        "Unable to check duplicate [Staff Salary]! Please try again later";
                    response.Status.Message.TechnicalMessage =
                        "Unable to check duplicate [Staff Salary]! Please try again later";
                    return true;
                }

                if (check1.Count != 1)
                {
                    response.Status.Message.FriendlyMessage = "Unable to check duplicate! Please try again later";
                    response.Status.Message.TechnicalMessage = "Unable to check duplicate! Please try again later";
                    return true;
                }

                if (check1[0] > 0)
                    if (callType != 2)
                    {
                        response.Status.Message.FriendlyMessage =
                            "Duplicate Error! Staff Salary  Information already exist";
                        response.Status.Message.TechnicalMessage =
                            "Duplicate Error!  Staff Salary  Information already exist";
                        return true;
                    }


                return false;
            }
            catch (Exception ex)
            {
                response.Status.Message.FriendlyMessage =
                    "Unable to complete your request due to Validation Error:Please try  again later";
                response.Status.Message.TechnicalMessage = "Duplicate check error:" + ex.Message;
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                return true;
            }
        }

        #endregion

        #region StaffJobInfo

        private bool IsStaffJobInfoDuplicate(int staffId, int callType, ref SettingRegRespObj response)
        {
            try
            {
                #region

                var sql1 =
                    $"Select coalesce(Count(\"StaffJobInfoId\"), 0) FROM \"GPlus\".\"StaffJobInfo\" WHERE \"StaffId\" = {staffId};";


                var check1 = _staffJobInfoRepository.RepositoryContext().Database.SqlQuery<long>(sql1).ToList();


                if (!check1.Any())
                {
                    response.Status.Message.FriendlyMessage =
                        "Unable to check duplicate [Staff Job Info]! Please try again later";
                    response.Status.Message.TechnicalMessage =
                        "Unable to check duplicate [Staff Job Info]! Please try again later";
                    return true;
                }

                if (check1.Count != 1)
                {
                    response.Status.Message.FriendlyMessage = "Unable to check duplicate! Please try again later";
                    response.Status.Message.TechnicalMessage = "Unable to check duplicate! Please try again later";
                    return true;
                }

                if (check1[0] > 0)
                    if (callType != 2)
                    {
                        response.Status.Message.FriendlyMessage =
                            "Duplicate Error! Staff Job  Information already exist";
                        response.Status.Message.TechnicalMessage =
                            "Duplicate Error!  Staff Job  Information already exist";
                        return true;
                    }


                return false;
            }
            catch (Exception ex)
            {
                response.Status.Message.FriendlyMessage =
                    "Unable to complete your request due to Validation Error:Please try  again later";
                response.Status.Message.TechnicalMessage = "Duplicate check error:" + ex.Message;
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                return true;
            }
        }

        private StaffJobInfo getStaffJobInfo(int staffId)
        {
            try
            {
                var sql1 = $"SELECT *  FROM  \"GPlus\".\"StaffJobInfo\" WHERE  \"StaffId\" = {staffId};";
                var agentInfos = _staffJobInfoRepository.RepositoryContext().Database.SqlQuery<StaffJobInfo>(sql1)
                    .ToList();
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
        

        private List<StaffJobInfo> getStaffJobInfos(StaffJobInfoSearchObj searchObj)
        {
            try
            {
                var thisSql = new StringBuilder();
                thisSql.Append("SELECT *  FROM  \"GPlus\".\"StaffJobInfo\" WHERE  1 = 1");

                if (searchObj.StaffId > 1)
                    thisSql.Append($" AND  \"StaffId\" = {searchObj.StaffId}");
                if (searchObj.StaffJobInfoId > 1)
                    thisSql.Append($" AND  \"StaffJobInfoId\" = {searchObj.StaffJobInfoId}");

                if (!string.IsNullOrEmpty(searchObj.JobTitle))
                    thisSql.Append($" AND  \"JobTitle\" = {searchObj.JobTitle}");
                var staffJobInfosList = _staffJobInfoRepository.RepositoryContext().Database
                    .SqlQuery<StaffJobInfo>(thisSql.ToString()).ToList();
                if (!staffJobInfosList.Any() || staffJobInfosList.Count < 1)
                    return null;
                return staffJobInfosList;
            }

            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }

        #endregion

        #region StaffMemo

        private StaffMemo getStaffMemo(int staffId)
        {
            try
            {
                var sql1 = $"SELECT *  FROM  \"GPlus\".\"StaffMemo\" WHERE  \"StaffId\" = {staffId};";
                var agentInfos = _staffMemoRepository.RepositoryContext().Database.SqlQuery<StaffMemo>(sql1).ToList();
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

        private List<StaffMemo> getStaffMemos(long staffId)
        {
            try
            {
                var sql1 = $"SELECT *  FROM  \"GPlus\".\"StaffMemo\" WHERE  \"StaffId\" = {staffId};";


                var staffMemoList = _staffMemoRepository.RepositoryContext().Database.SqlQuery<StaffMemo>(sql1)
                    .ToList();
                if (!staffMemoList.Any() || staffMemoList.Count < 1)
                    return null;
                return staffMemoList;
            }

            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }


        private List<StaffMemo> getStaffMemo(StaffMemoSearchObj searchObj)
        {
            try
            {
                var thisSql = new StringBuilder();
                thisSql.Append("SELECT *  FROM  \"GPlus\".\"StaffMemo\" WHERE  1 = 1");

                if (searchObj.StaffId > 0)
                    thisSql.Append($" AND  \"StaffId\" = {searchObj.StaffId}");
                if (searchObj.StaffMemoId > 1)
                    thisSql.Append($" AND  \"StaffMemoId\" = {searchObj.StaffMemoId}");

                if (searchObj.MemoType > 0)
                    thisSql.Append($" AND  \"MemoType\" = {searchObj.MemoType}");
                if (searchObj.Status > -10)
                    thisSql.Append($" AND  \"Status\" = {searchObj.Status}");
                var staffMemoList = _staffMemoRepository.RepositoryContext().Database
                    .SqlQuery<StaffMemo>(thisSql.ToString()).ToList();
                if (!staffMemoList.Any() || staffMemoList.Count < 1)
                    return null;
                return staffMemoList;
            }

            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }

        #endregion

        #endregion

        #region StaffPension

        private StaffPension getStaffPension(int staffId)
        {
            try
            {
                var sql1 = $"SELECT *  FROM  \"GPlus\".\"StaffPension\" WHERE  \"StaffId\" = {staffId};";
                var agentInfos = _staffPensionRepository.RepositoryContext().Database.SqlQuery<StaffPension>(sql1)
                    .ToList();
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

        private List<StaffPension> getStaffPensions(StaffPensionSearchObj searchObj)
        {
            try
            {
                var thisSql = new StringBuilder();
                thisSql.Append("SELECT *  FROM  \"GPlus\".\"StaffPension\" WHERE  1 = 1");

                if (searchObj.StaffId > 0)
                
                    thisSql.Append($" AND  \"StaffId\" = {searchObj.StaffId}");
                

                if (searchObj.StaffPensionId > 0)
                
                    thisSql.Append($" AND  \"StaffPensionId\" = {searchObj.StaffPensionId}"); 
                    
                

                if (!string.IsNullOrEmpty(searchObj.PensionNumber))
                
                  
                thisSql.Append($" AND  lower(\"PensionNumber\") = lower('{searchObj.PensionNumber.Trim()}')");



                var staffPensionList = _staffPensionRepository.RepositoryContext().Database
                    .SqlQuery<StaffPension>(thisSql.ToString()).ToList();
                if (!staffPensionList.Any() || staffPensionList.Count < 1)
                
                    return null;
                
                  
                return staffPensionList;
            }

            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }

        #endregion


        #region StaffMedical

        private StaffMedical getStaffMedical(int staffId)
        {
            try
            {
                var sql1 = $"SELECT *  FROM  \"GPlus\".\"StaffMedical\" WHERE  \"StaffId\" = {staffId};";
                var agentInfos = _staffMedicalRepository.RepositoryContext().Database.SqlQuery<StaffMedical>(sql1)
                    .ToList();
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

        private List<StaffMedical> getStaffMedicals(StaffMedicalSearchObj searchObj)
        {
            try
            {
                var thisSql = new StringBuilder();
                thisSql.Append("SELECT *  FROM  \"GPlus\".\"StaffMedical\" WHERE  1 = 1");

                if (searchObj.StaffId > 1)
                    thisSql.Append($" AND  \"StaffId\" = {searchObj.StaffId}");
                if (searchObj.StaffMedicalId > 1)
                    thisSql.Append($" AND  \"StaffMedicalId\" = {searchObj.StaffMedicalId}");


                var staffMedicalsList = _staffMedicalRepository.RepositoryContext().Database
                    .SqlQuery<StaffMedical>(thisSql.ToString()).ToList();
                if (!staffMedicalsList.Any() || staffMedicalsList.Count < 1)
                    return null;
                return staffMedicalsList;
            }

            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }

        #endregion

        #region Staff key performance indicator

        private StaffKPIndex getStaffKPIndex(int staffId)
        {
            try
            {
                var sql1 = $"SELECT *  FROM  \"GPlus\".\"StaffKPIndex\" WHERE  \"StaffId\" = {staffId};";
                var agentInfos = _staffKPIndexRepository.RepositoryContext().Database.SqlQuery<StaffKPIndex>(sql1)
                    .ToList();
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

        private List<StaffKPIndex> getStaffKPIndexs(StaffKPIIndexSearchObj searchObj)
        {
            try
            {
                var thisSql = new StringBuilder();
                thisSql.Append("SELECT *  FROM  \"GPlus\".\"StaffKPIndex\" WHERE  1 = 1");

                if (searchObj.StaffId > 1)
                    thisSql.Append($" AND  \"StaffId\" = {searchObj.StaffId}");
                if (searchObj.StaffKPIndexId > 1)
                    thisSql.Append($" AND  \"StaffKPIndexId\" = {searchObj.StaffKPIndexId}");

                if (DateTime.Parse(searchObj.StartDate).Date < DateTime.Parse(searchObj.EndDate).Date)
                    thisSql.Append(
                        $" AND  \"TimeStampRegistered\" >= {DateTime.Parse(searchObj.StartDate).Date} AND  \"TimeStampRegistered\" <= {DateTime.Parse(searchObj.EndDate).Date}");
                var staffKPIndexList = _staffKPIndexRepository.RepositoryContext().Database
                    .SqlQuery<StaffKPIndex>(thisSql.ToString()).ToList();
                if (!staffKPIndexList.Any() || staffKPIndexList.Count < 1)
                    return null;
                return staffKPIndexList;
            }

            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }

        #endregion

        #region StaffMemoResponse

        private StaffMemoResponse getStaffMemoResponse(int staffId)
        {
            try
            {
                var sql1 = $"SELECT *  FROM  \"GPlus\".\"StaffMemoResponse\" WHERE  \"StaffId\" = {staffId};";
                var agentInfos = _staffMemoResponseRepository.RepositoryContext().Database
                    .SqlQuery<StaffMemoResponse>(sql1).ToList();
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

        private bool IsStaffMemoResponseDuplicate(int staffId, int institutionId, int qualificationId, int disciplineId,
            int courseOfStudtId, int startYear, int endYear, int callType, ref SettingRegRespObj response)
        {
            try
            {
                #region

                var sql1 =
                    $"Select coalesce(Count(\"StaffMemoResponseId\"), 0) FROM \"GPlus\".\"StaffMemoResponse\" WHERE \"StaffId\" = {staffId} AND " +
                    $" \"StartYear\" = {startYear} AND \"EndYear\" = {endYear} AND \"DisciplineId\" = {disciplineId} AND \"CourseOfStudyId\" = {courseOfStudtId} AND \"QualificationId\" = {qualificationId} AND \"InstitutionId\" = {institutionId} ";


                var check1 = _staffMemoResponseRepository.RepositoryContext().Database.SqlQuery<long>(sql1).ToList();


                if (!check1.Any())
                {
                    response.Status.Message.FriendlyMessage =
                        "Unable to check duplicate [Educational Qualification]! Please try again later";
                    response.Status.Message.TechnicalMessage =
                        "Unable to check duplicate [Educational Qualification]! Please try again later";
                    return true;
                }

                if (check1.Count != 1)
                {
                    response.Status.Message.FriendlyMessage = "Unable to check duplicate! Please try again later";
                    response.Status.Message.TechnicalMessage = "Unable to check duplicate! Please try again later";
                    return true;
                }

                if (check1[0] > 0)
                    if (callType != 2)
                    {
                        response.Status.Message.FriendlyMessage =
                            "Duplicate Error! Educational Qualification  Information already exist";
                        response.Status.Message.TechnicalMessage =
                            "Duplicate Error!  Educational Qualification  Information already exist";
                        return true;
                    }


                return false;
            }
            catch (Exception ex)
            {
                response.Status.Message.FriendlyMessage =
                    "Unable to complete your request due to Validation Error:Please try  again later";
                response.Status.Message.TechnicalMessage = "Duplicate check error:" + ex.Message;
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                return true;
            }
        }


        private List<StaffMemoResponse> getStaffMemoResponse(StaffMemoResponseSearchObj searchObj)
        {
            try
            {
                var thisSql = new StringBuilder();
                thisSql.Append("SELECT *  FROM  \"GPlus\".\"StaffMemoResponse\" WHERE  1 = 1");

                if (searchObj.StaffId > 0)
                    thisSql.Append($" AND  \"StaffId\" = {searchObj.StaffId}");
                if (searchObj.StaffMemoId > 0)
                    thisSql.Append($" AND  \"StaffMemoId\" = {searchObj.StaffMemoId}");

                if (searchObj.StaffMemoResponseId > 0)
                    thisSql.Append($" AND  \"StaffMemoResponseId\" = {searchObj.StaffMemoResponseId}");


                var staffMemoResponseList = _staffMemoResponseRepository.RepositoryContext().Database
                    .SqlQuery<StaffMemoResponse>(thisSql.ToString()).ToList();
                if (!staffMemoResponseList.Any() || staffMemoResponseList.Count < 1)
                    return null;
                return staffMemoResponseList;
            }

            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }

        private List<StaffMemoResponse> getStaffMemoResponses(long staffId)
        {
            try
            {
                var sql1 = $"SELECT *  FROM  \"GPlus\".\"StaffMemoResponse\" WHERE  \"StaffId\" = {staffId};";


                var staffMemoresponseList = _staffMemoResponseRepository.RepositoryContext().Database
                    .SqlQuery<StaffMemoResponse>(sql1).ToList();
                if (!staffMemoresponseList.Any() || staffMemoresponseList.Count < 1)
                    return null;
                return staffMemoresponseList;
            }

            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }

        #endregion

        #endregion

        #region Comment

        private List<Comment> GetCommentInfos(CommentSearchObj searchObj)
        {
            try
            {
                var thisSql = new StringBuilder();
                thisSql.Append("SELECT *  FROM  \"GPlus\".\"Comment\" WHERE  1 = 1");

               

                if (searchObj.StaffId > 0)
                    thisSql.Append($" AND  \"StaffId\" = {searchObj.StaffId}");

                if (searchObj.CommentId > 0)
                    thisSql.Append($" AND  \"CommentId\" = {searchObj.CommentId}");


                var commentList = _adminCommmentRepository.RepositoryContext().Database
                    .SqlQuery<Comment>(thisSql.ToString()).ToList();
                if (!commentList.Any() || commentList.Count < 1)
                    return null;

                return commentList;
            }

            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }

        #endregion
    }

    #endregion
}