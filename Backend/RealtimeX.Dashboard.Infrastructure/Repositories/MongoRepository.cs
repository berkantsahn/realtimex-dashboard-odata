using System.Linq.Expressions;
using MongoDB.Driver;
using MongoDB.Bson;
using RealtimeX.Dashboard.Core.Interfaces;

namespace RealtimeX.Dashboard.Infrastructure.Repositories
{
    public class MongoRepository<T> : IRepository<T> where T : class
    {
        private readonly IMongoCollection<T> _collection;

        public MongoRepository(IMongoDatabase database)
        {
            _collection = database.GetCollection<T>(typeof(T).Name);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task<T> GetByIdAsync(string id)
        {
            var objectId = ObjectId.Parse(id);
            var filter = Builders<T>.Filter.Eq("_id", objectId);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _collection.Find(predicate).ToListAsync();
        }

        public async Task AddAsync(T entity)
        {
            await _collection.InsertOneAsync(entity);
        }

        public async Task UpdateAsync(T entity)
        {
            var idProperty = typeof(T).GetProperty("Id");
            if (idProperty == null)
                throw new InvalidOperationException($"Entity {typeof(T).Name} does not have an Id property");

            var id = idProperty.GetValue(entity);
            if (id == null)
                throw new InvalidOperationException($"Entity {typeof(T).Name} has a null Id value");

            var objectId = ObjectId.Parse(id.ToString());
            var filter = Builders<T>.Filter.Eq("_id", objectId);
            await _collection.ReplaceOneAsync(filter, entity);
        }

        public async Task DeleteAsync(T entity)
        {
            var idProperty = typeof(T).GetProperty("Id");
            if (idProperty == null)
                throw new InvalidOperationException($"Entity {typeof(T).Name} does not have an Id property");

            var id = idProperty.GetValue(entity);
            if (id == null)
                throw new InvalidOperationException($"Entity {typeof(T).Name} has a null Id value");

            var objectId = ObjectId.Parse(id.ToString());
            var filter = Builders<T>.Filter.Eq("_id", objectId);
            await _collection.DeleteOneAsync(filter);
        }
    }
} 