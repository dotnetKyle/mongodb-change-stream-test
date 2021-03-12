using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ChangeStreamTest
{
    public class NotificationsCollection : DAO<Notification>
    {
        protected override string CollectionName => "notifications";

        public NotificationsCollection(AppSettings settings) : base(settings) { }

        public async Task InsertOneAsync(Notification notif)
        {
            await Collection.InsertOneAsync(notif);
        }
        public async Task DismissNotificationAsync(ObjectId id, string userId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var filter = Builders<Notification>.Filter.And(
                // filter by user's login
                Builders<Notification>.Filter.Eq(n => n.UserLogin, userId),
                // only dismiss the notification that the user clicked dismiss on
                Builders<Notification>.Filter.Eq(n => n.Id, id)
            );
            var update = Builders<Notification>.Update
                .Set(n => n.Dismissed, true);

            await Collection.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = false }, cancellationToken);
        }
        public async Task DismissAllNotificationsAsync(string userId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var filter = Builders<Notification>.Filter.And(
                // filter by user's login
                Builders<Notification>.Filter.Eq(n => n.UserLogin, userId),
                // only dismiss the notifications that are not dismissed already
                Builders<Notification>.Filter.Eq(n => n.Dismissed, false)
            );

            var update = Builders<Notification>.Update
                .Set(n => n.Dismissed, true);

            await Collection.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = false }, cancellationToken);
        }
        public async Task<IEnumerable<Notification>> GetNotificationsForUserAsync(string userId)
        {
            var filter = Builders<Notification>.Filter.And(
                // filter by user's login
                Builders<Notification>.Filter.Eq(n => n.UserLogin, userId),
                // only show the non-dismissed notfications
                Builders<Notification>.Filter.Eq(n => n.Dismissed, false)
            );

            return await Collection
                .Find(filter)
                .ToListAsync();
        }
        public IChangeStreamCursor<ChangeStreamDocument<Notification>> Watch(string userId, CancellationToken cancellationToken)
        {
            var filter = Builders<ChangeStreamDocument<Notification>>.Filter.And(
                Builders<ChangeStreamDocument<Notification>>.Filter.Eq(n => n.OperationType, ChangeStreamOperationType.Insert),
                Builders<ChangeStreamDocument<Notification>>.Filter.Eq(n => n.FullDocument.UserLogin, userId)
            );

            var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<Notification>>()
                    .Match(filter);

            return Collection.Watch(pipeline, new ChangeStreamOptions { }, cancellationToken);
        }
    }
}
