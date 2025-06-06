using System;
using System.Threading.Tasks;

namespace RealtimeX.Dashboard.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<T> Repository<T>() where T : class;
        Task<int> CompleteAsync();
    }
} 