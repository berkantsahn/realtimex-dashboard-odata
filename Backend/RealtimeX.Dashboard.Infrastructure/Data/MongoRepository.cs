using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using RealtimeX.Dashboard.Core.Interfaces;

namespace RealtimeX.Dashboard.Infrastructure.Data
{
    public class MongoRepository<T> : IRepository<T> where T : class
    {
        private readonly IMongoCollection<T> _collection;

        public MongoRepository(IMongoDatabase database, string collectionName)
        {
            _collection = database.GetCollection<T>(collectionName);
        }

        public async Task<T> GetByIdAsync(int id)
        {
            var filter = Builders<T>.Filter.Eq("_id", id);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate)
        {
            return await _collection.Find(predicate).ToListAsync();
        }

        public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeString = null,
            bool disableTracking = true)
        {
            var query = _collection.AsQueryable();

            if (predicate != null)
                query = query.Where(predicate);

            if (orderBy != null)
                query = orderBy(query);

            return await query.ToListAsync();
        }

        public async Task<T> AddAsync(T entity)
        {
            await _collection.InsertOneAsync(entity);
            return entity;
        }

        public async Task UpdateAsync(T entity)
        {
            var filter = Builders<T>.Filter.Eq("_id", GetEntityId(entity));
            await _collection.ReplaceOneAsync(filter, entity);
        }

        public async Task DeleteAsync(T entity)
        {
            var filter = Builders<T>.Filter.Eq("_id", GetEntityId(entity));
            await _collection.DeleteOneAsync(filter);
        }

        public IQueryable<T> AsQueryable()
        {
            return _collection.AsQueryable();
        }

        private object GetEntityId(T entity)
        {
            var idProperty = typeof(T).GetProperty("Id");
            return idProperty?.GetValue(entity);
        }
    }
} 