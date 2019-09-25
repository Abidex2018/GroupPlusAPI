using System;
using System.Data.Entity;

namespace GroupPlus.Business.Infrastructure.Contract
{
    internal interface IPlugContext : IDisposable
    {
        DbContext PlugDbContext { get; }
    }
}