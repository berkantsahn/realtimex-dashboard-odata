using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using RealtimeX.Dashboard.Core.Interfaces;
using RealtimeX.Dashboard.Infrastructure.Repositories;

namespace RealtimeX.Dashboard.Infrastructure.Data
{
    public class MongoUnitOfWork : IUnitOfWork
    {
        private readonly IMongoDatabase _database;
        private readonly Dictionary<Type, object> _repositories;
        private IClientSessionHandle _session;
        private bool _disposed;

        public MongoUnitOfWork(IMongoDatabase database)
        {
            _database = database;
            _repositories = new Dictionary<Type, object>();
        }

        public IRepository<T> GetRepository<T>() where T : class
        {
            if (_repositories.ContainsKey(typeof(T)))
            {
                return (IRepository<T>)_repositories[typeof(T)];
            }

            var repository = new MongoRepository<T>(_database);
            _repositories.Add(typeof(T), repository);
            return repository;
        }

        public async Task<bool> SaveChangesAsync()
        {
            if (_session != null && _session.IsInTransaction)
            {
                await _session.CommitTransactionAsync();
                return true;
            }
            return true;
        }

        public async Task BeginTransactionAsync()
        {
            if (_session == null)
            {
                _session = await _database.Client.StartSessionAsync();
            }
            _session.StartTransaction();
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                if (_session != null && _session.IsInTransaction)
                {
                    await _session.CommitTransactionAsync();
                }
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_session != null && _session.IsInTransaction)
            {
                await _session.AbortTransactionAsync();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _session?.Dispose();
                }
                _disposed = true;
            }
        }
    }
} 