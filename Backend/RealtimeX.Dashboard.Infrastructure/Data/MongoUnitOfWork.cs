using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using RealtimeX.Dashboard.Core.Interfaces;

namespace RealtimeX.Dashboard.Infrastructure.Data
{
    public class MongoUnitOfWork : IUnitOfWork
    {
        private readonly IMongoDatabase _database;
        private readonly MongoDbSettings _settings;
        private readonly Dictionary<Type, object> _repositories;
        private bool _disposed;

        public MongoUnitOfWork(IOptions<MongoDbSettings> settings)
        {
            _settings = settings.Value;
            var client = new MongoClient(_settings.ConnectionString);
            _database = client.GetDatabase(_settings.DatabaseName);
            _repositories = new Dictionary<Type, object>();
        }

        public IRepository<T> Repository<T>() where T : class
        {
            var type = typeof(T);

            if (!_repositories.ContainsKey(type))
            {
                var collectionName = GetCollectionName<T>();
                var repositoryType = typeof(MongoRepository<>).MakeGenericType(type);
                var repository = Activator.CreateInstance(repositoryType, _database, collectionName);
                _repositories.Add(type, repository);
            }

            return (IRepository<T>)_repositories[type];
        }

        public async Task<int> CompleteAsync()
        {
            // MongoDB'de transaction yönetimi için gerekirse burada implementasyon yapılabilir
            return await Task.FromResult(1);
        }

        private string GetCollectionName<T>()
        {
            var type = typeof(T).Name;
            return type switch
            {
                "RealTimeData" => _settings.RealTimeDataCollectionName,
                "Announcement" => _settings.AnnouncementsCollectionName,
                "ChatMessage" => _settings.ChatMessagesCollectionName,
                "User" => _settings.UsersCollectionName,
                _ => throw new ArgumentException($"Unknown entity type: {type}")
            };
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _repositories.Clear();
            }
            _disposed = true;
        }
    }
} 