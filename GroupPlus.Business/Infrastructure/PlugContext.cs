using System;
using System.Data.Entity;
using GroupPlus.Business.DataManager;
using GroupPlus.Business.Infrastructure.Contract;

namespace GroupPlus.Business.Infrastructure
{
    internal class PlugContext : IPlugContext
    {
        public PlugContext(DbContext context)
        {
            PlugDbContext = context ?? throw new ArgumentNullException(nameof(context));
            PlugDbContext.Configuration.LazyLoadingEnabled = false;
        }

        public PlugContext()
        {
            PlugDbContext = new PlugModel();
            PlugDbContext.Configuration.LazyLoadingEnabled = false;
        }

        public void Dispose()
        {
            PlugDbContext.Dispose();
        }

        public DbContext PlugDbContext { get; }
    }
}