using Dubizzle.SavedSearch.Contracts;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Dubizzle.SavedSearch.Api
{   
    public class MongoDbProvider : IDatabaseProvider
    {
        private readonly IMongoDatabase _database;
        private readonly string _collection;

        public MongoDbProvider(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetConnectionString("MongoDb"));

            _database = client.GetDatabase(configuration.GetConnectionString("Database"));

            _collection = configuration.GetConnectionString("SubscriptionsCollectionName");
        }

        public async Task<T> CreateAsync<T>(T entity)
        {
            await GetCollection<T>().InsertOneAsync(entity);

            return entity;
        }

        public async Task DeleteAsync<T>(Expression<Func<T, bool>> filter) where T : IEntity
        {
            var update = Builders<T>.Update.Set(s => s.IsDeleted, true);

            await GetCollection<T>().UpdateOneAsync(filter, update);
        }

        public async Task<IEnumerable<T>> GetAllAsync<T>(Expression<Func<T, bool>> filter) where T : IEntity
        {
            return  (await GetCollection<T>().FindAsync(filter)).ToList();
        }

        public async Task<T> GetByIdAsync<T>(Expression<Func<T, bool>> filter) where T : IEntity
        {
            return (await GetAllAsync(filter)).FirstOrDefault();
        }

        public async Task<T> UpdateAsync<T>(T entity, Expression<Func<T, bool>> filter) where T : IEntity
        {
            await GetCollection<T>().ReplaceOneAsync(filter, entity);

            return entity;
        }

        private IMongoCollection<T> GetCollection<T>()
        {
            return _database.GetCollection<T>(_collection);
        }
    }
}