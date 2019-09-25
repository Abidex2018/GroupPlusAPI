using System;
using System.Data.Entity.Migrations;

namespace GroupPlus.Business.DataManager
{
    internal class MigrationService
    {
        public static bool Migrate(out string msg)
        {
            try
            {
                var configuration = new Migrations.Configuration();
                var migrator = new DbMigrator(configuration);
                migrator.Update();
                msg = "";
                return true;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return false;
            }
        }
    }
}