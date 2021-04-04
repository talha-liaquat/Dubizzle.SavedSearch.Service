using Dubizzle.SavedSearch.Contracts;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

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

        public T Create<T>(T entity)
        {
            GetCollection<T>().InsertOne(entity);

            return entity;
        }

        public void Delete<T>(Expression<Func<T, bool>> filter) where T : IEntity
        {
            var update = Builders<T>.Update.Set(s => s.IsDeleted, true);

            var result = GetCollection<T>().UpdateOne(filter, update);
        }

        public IEnumerable<T> GetAll<T>(Expression<Func<T, bool>> filter) where T : IEntity
        {
            return GetCollection<T>().Find(filter).ToList();
        }

        public T GetById<T>(Expression<Func<T, bool>> filter) where T : IEntity
        {
            return GetAll(filter).FirstOrDefault();
        }

        public T Update<T>(T entity, Expression<Func<T, bool>> filter) where T : IEntity
        {
            GetCollection<T>().ReplaceOne(filter, entity);

            return entity;
        }

        private IMongoCollection<T> GetCollection<T>()
        {
            return _database.GetCollection<T>(_collection);
        }
    }
}