using MongoDB.Driver;

namespace ChangeStreamTest
{
    public abstract class DAO<T> where T : class, new()
    {
        protected IMongoDatabase database;
        protected IMongoCollection<T> Collection { get; set; }
        protected abstract string CollectionName { get; }

        public DAO(AppSettings settings)
        {
            var client = new MongoClient(settings.DbConnectionString);
            database = client.GetDatabase(settings.DatabaseName);
            Collection = database.GetCollection<T>(CollectionName);
        }
    }
}
