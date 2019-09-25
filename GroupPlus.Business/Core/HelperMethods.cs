using System;
using System.Configuration;
using System.Globalization;
using System.Linq;
using GroupPlus.Business.Repository.StaffManagement;
using GroupPlus.BusinessContract.CommonAPIs;
using GroupPlus.BusinessObject.StaffManagement;
using GroupPlus.Common;
using PlugPortalManager;
using PlugPortalManager.APIContract;
using XPLUG.WEBTOOLS;

namespace GroupPlus.Business.Core
{
    internal class HelperMethods
    {
        internal static UserDetailObj GetUserDetail(int userId)
        {
            try
            {
                var obj = new UserSearchObj {UserId = userId};
                var user = AdminPortalService.GetUserDetail(obj);
                if (user == null || user.Status.IsSuccessful == false || user.UserDetail == null ||
                    user.UserDetail.UserId < 1)
                    return new UserDetailObj();

                return user.UserDetail;
            }
            catch (Exception ex)
            {
                return new UserDetailObj();
            }
        }

        internal static bool IsUserValid(int adminUserId, string token, string[] roleNames,
            ref APIResponseMessage response)
        {
            try
            {
                var obj = new UserAuthObj {AdminUserId = adminUserId, SysPathCode = token};
                var user = AdminPortalService.GetPortalUser(obj);
                if (user == null || user.Status.IsSuccessful == false || user.Users == null || !user.Users.Any())
                {
                    response.FriendlyMessage = "Invalid / Unauthorized User";
                    response.TechnicalMessage = "Invalid / Unauthorized User";
                    return false;
                }
                if (user.Users.Count != 1)
                {
                    response.FriendlyMessage = "Invalid / Unauthorized User";
                    response.TechnicalMessage = "Invalid / Unauthorized User";
                    return false;
                }

                if (AdminPortalService.IsUserInRole(adminUserId, roleNames)) return true;
                response.FriendlyMessage = "Unauthorized Access";
                response.TechnicalMessage = "User does not belong to any role! ";

                return false;
            }
            catch (Exception ex)
            {
                response.FriendlyMessage = "Unable to authenticate Admin User";
                response.TechnicalMessage = "Error: " + ex.Message;
                return false;
            }
        }

        internal static bool IsUserValid(int adminUserId, string token, ref APIResponseMessage response)
        {
            try
            {
                var obj = new UserAuthObj {AdminUserId = adminUserId, SysPathCode = token};
                var user = AdminPortalService.GetPortalUser(obj);
                if (user == null || user.Status.IsSuccessful == false || user.Users == null || !user.Users.Any())
                {
                    response.FriendlyMessage = "Invalid / Unauthorized User";
                    response.TechnicalMessage = "Invalid / Unauthorized User";
                    return false;
                }
                if (user.Users.Count != 1)
                {
                    response.FriendlyMessage = "Invalid / Unauthorized User";
                    response.TechnicalMessage = "Invalid / Unauthorized User";
                    return false;
                }

                var roleSearch = new RoleSearchObj
                {
                    UserId = adminUserId,
                    AdminUserId = adminUserId,
                    SysPathCode = token
                };
                var roles = AdminPortalService.GetAllRoles(roleSearch);
                if (roles == null || roles.Status.IsSuccessful == false || !roles.Roles.Any())
                {
                    response.FriendlyMessage = "Unauthorized User";
                    response.TechnicalMessage = "User does not belong to any role!";
                    return false;
                }


                return true;
            }
            catch (Exception ex)
            {
                response.FriendlyMessage = "Unable to authenticate Admin User";
                response.TechnicalMessage = "Error: " + ex.Message;
                return false;
            }
        }

        internal static bool IsStaffUserValid(int staffId, string token, ref APIResponseMessage response)
        {
            try
            {
                var staffAccess = new StaffRepository().getStaffAccessInfo(staffId);
                if (staffAccess == null || staffAccess.StaffAccessId < 1)
                {
                    response.FriendlyMessage = "Invalid / Unauthorized Staff";
                    response.TechnicalMessage = "Invalid / Unauthorized Staff";
                    return false;
                }

                if (string.IsNullOrEmpty(staffAccess?.Username))
                {
                    response.FriendlyMessage = "Invalid / Unauthorized Staff";
                    response.TechnicalMessage = "Invalid / Unauthorized Staff";
                    return false;
                }

                var user = new StaffRepository().getStaffInfo(staffId);
                if (user.Status != StaffStatus.Active)
                {
                    response.FriendlyMessage = "Unauthorized / Deleted Staff Record";
                    response.TechnicalMessage = "Unauthorized / Deleted Staff Record";
                    return false;
                }

                if (!new StaffRepository().IsTokenValid(staffId, token, "", false, ref response))
                    return false;
                return true;
            }
            catch (Exception ex)
            {
                response.FriendlyMessage = "Unable to authenticate Staff";
                response.TechnicalMessage = "Error: " + ex.Message;
                return false;
            }
        }

        internal static bool IsStaffUserValid(int staffId, string token, string currentIP, bool check,out Staff staffInfo, ref APIResponseMessage response)
        {
            staffInfo = new Staff();
            try
            {
                var staffAccess = new StaffRepository().getStaffAccessInfo(staffId);
                if (staffAccess == null || staffAccess.StaffAccessId < 1)
                {
                    response.FriendlyMessage = "Invalid / Unauthorized Staff";
                    response.TechnicalMessage = "Invalid / Unauthorized Staff";
                    return false;
                }

                if (string.IsNullOrEmpty(staffAccess?.Username))
                {
                    response.FriendlyMessage = "Invalid / Unauthorized Staff";
                    response.TechnicalMessage = "Invalid / Unauthorized Staff";
                    return false;
                }

                var user = new StaffRepository().getStaffInfo(staffId);
                if (user.Status != StaffStatus.Active)
                {
                    response.FriendlyMessage = "Unauthorized / Deleted Staff Record";
                    response.TechnicalMessage = "Unauthorized / Deleted Staff Record";
                    return false;
                }

                if (!new StaffRepository().IsTokenValid(staffId, token, currentIP, check, ref response))
                    return false;

                staffInfo = user;
                return true;
            }
            catch (Exception ex)
            {
                response.FriendlyMessage = "Unable to authenticate Staff";
                response.TechnicalMessage = "Error: " + ex.Message;
                return false;
            }
        }

        internal static bool IsStaffUserValid(int staffId, string token, out Staff staffInfo,ref APIResponseMessage response)
        {
            staffInfo = new Staff();
            try
            {
                var staffAccess = new StaffRepository().getStaffAccessInfo(staffId);
                if (staffAccess == null || staffAccess.StaffAccessId < 1)
                {
                    response.FriendlyMessage = "Invalid / Unauthorized Staff";
                    response.TechnicalMessage = "Invalid / Unauthorized Staff";
                    return false;
                }

                if (string.IsNullOrEmpty(staffAccess?.Username))
                {
                    response.FriendlyMessage = "Invalid / Unauthorized Staff";
                    response.TechnicalMessage = "Invalid / Unauthorized Staff";
                    return false;
                }

                var user = new StaffRepository().getStaffInfo(staffId);
                if (user.Status != StaffStatus.Active)
                {
                    response.FriendlyMessage = "Unauthorized / Deleted Staff Record";
                    response.TechnicalMessage = "Unauthorized / Deleted Staff Record";
                    return false;
                }

                if (!new StaffRepository().IsTokenValid(staffId, token, "", false, ref response))
                    return false;

                staffInfo = user;
                return true;
            }
            catch (Exception ex)
            {
                response.FriendlyMessage = "Unable to authenticate Staff";
                response.TechnicalMessage = "Error: " + ex.Message;
                return false;
            }
        }


        internal static bool shouldClearCache(string itemName)
        {
            try
            {
                var status = ConfigurationManager.AppSettings.Get(itemName);
                return !string.IsNullOrEmpty(status) && DataCheck.IsNumeric(status) && int.Parse(status) == 1;
            }
            catch (Exception)
            {
                return true;
            }
        }

        internal static void clearCache(string cacheName)
        {
            try
            {
                if (CacheManager.GetCache(cacheName) != null)
                    CacheManager.RemoveCache(cacheName);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        internal static int NumberOfDays(string x, string y)
        {
            var startDate = DateTime.ParseExact(x, "yyyy/MM/dd", CultureInfo.InvariantCulture);
            var endDate = DateTime.ParseExact(y, "yyyy/MM/dd", CultureInfo.InvariantCulture);

            var days = (endDate - startDate).Days;

            return days;
        }

        #region Roles

        internal static string[] getSalesOfficers()
        {
            return new[] {"PortalAdmin", "SalesOfficer", "GeneralManager"};
        }

        internal static string[] getAccountOfficers()
        {
            return new[] {"PortalAdmin", "AccountOfficer", "ChiefAccountant", "ExecutiveAdmin"};
        }

        internal static string[] getSeniorAccountant()
        {
            return new[] {"PortalAdmin", "ChiefAccountant", "ExecutiveAdmin"};
        }

        internal static string[] getSeniorOfficers()
        {
            return new[] {"PortalAdmin", "ChiefAccountant", "GeneralManager", "ExecutiveAdmin"};
        }

        internal static string[] getGeneralManager()
        {
            return new[] {"PortalAdmin", "GeneralManager"};
        }

        internal static string[] getMgtExecutiveAdmin()
        {
            return new[] {"PortalAdmin", "ExecutiveAdmin"};
        }

        internal static string[] getStaffRoles()
        {
            return new[] {"PortalAdmin", "GroupAdmin", "HRAdmin" };
        }
        internal static string[] getAdminRoles()
        {
            return new[] { "PortalAdmin","GroupAdmin", "LMAdmin","HRAdmin","HODAdmin" };
        }
        internal static string[] getAllRoles()
        {
            return new[]
            {
                "PortalAdmin", "ExecutiveAdmin", "ChiefAccountant", "GeneralManager", "AccountOfficer", "SalesOfficer"
            };
        }

        #endregion
    }
}