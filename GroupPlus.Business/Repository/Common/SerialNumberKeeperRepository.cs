using System;
using System.Linq;
using GroupPlus.Business.Infrastructure;
using GroupPlus.Business.Infrastructure.Contract;
using GroupPlus.BusinessObject.CompanyManagement;
using XPLUG.WEBTOOLS;

namespace GroupPlus.Business.Repository.Common
{
    internal class SerialNumberKeeperRepository
    {
        private static IPlugRepository<SerialNumberKeeper> _repository;
        private static IPlugUoWork _uoWork;

        public SerialNumberKeeperRepository()
        {
            _uoWork = new PlugUoWork( /*You can specify you custom context here*/);
            _repository = new PlugRepository<SerialNumberKeeper>(_uoWork);
        }

        public static long Generate()
        {
            try
            {
                _uoWork = new PlugUoWork( /*You can specify you custom context here*/);
                _repository = new PlugRepository<SerialNumberKeeper>(_uoWork);

                Purge();

                var processedScratchPin = _repository.Add(new SerialNumberKeeper());
                _uoWork.SaveChanges();
                return processedScratchPin.SerialNumberKeeperId;
            }
            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.GetBaseException().Message);
                return 0;
            }
        }

        private static void Purge()
        {
            try
            {
                var sql1 =
                    "Select coalesce(Count(\"SerialNumberKeeperId\"), 0) FROM  \"VSalesKiosk\".\"SerialNumberKeeper\";";

                var check = _repository.RepositoryContext().Database.SqlQuery<long>(sql1).ToList();
                if (check.IsNullOrEmpty()) return;
                if (check[0] > 10000)
                {
                    sql1 = "Delete FROM  \"VSalesKiosk\".\"SerialNumberKeeper\";";
                    _repository.RepositoryContext().Database.ExecuteSqlCommand(sql1);
                }
            }
            catch (Exception)
            {
                //ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.GetBaseException().Message);
            }
        }
    }
}