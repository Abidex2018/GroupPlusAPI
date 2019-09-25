namespace GroupPlus.Business.Infrastructure.Contract
{
    internal interface IPlugUoWork
    {
        PlugContext Context { get; }
        void SaveChanges();
    }
}