using System;
using System.Data.Entity;
using GroupPlus.Business.Infrastructure.Contract;

namespace GroupPlus.Business.Infrastructure
{
    internal class PlugUoWork : IPlugUoWork, IDisposable
    {
        public PlugUoWork(PlugContext context)
        {
            Context = context;
        }

        public PlugUoWork()
        {
            Context = new PlugContext();
        }

        public void SaveChanges()
        {
            Context.PlugDbContext.SaveChanges();
        }

        public PlugContext Context { get; }


        //Class File Generated from codeZAPP 3.0.135 | www.xplugng.com | All Rights Reserved.
        public DbContextTransaction BeginTransaction()
        {
            return Context.PlugDbContext.Database.BeginTransaction();
        }

        #region Implementation of IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!disposing) return;
            if (_disposed) return;
            Context.Dispose();
            _disposed = true;
        }

        private bool _disposed;

        ~PlugUoWork()
        {
            Dispose(false);
        }

        #endregion
    }
}