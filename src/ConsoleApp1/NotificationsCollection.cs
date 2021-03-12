using MongoDB;
using MongoDB.Driver;
using MongoDB.Driver.Core;
using MongoDB.Bson;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ChangeStreamTest
{
    public class NotificationsCollection : DAO<Notification>
    {
        protected override string CollectionName => "notifications";
        
        string _userLogin;
        public NotificationsCollection(AppSettings settings, string userLogin) 
            : base(settings) 
        {
            _userLogin = userLogin;
        }

        public async Task InsertOneAsync(Notification notif)
        {
            await Collection.InsertOneAsync(notif);
        }

        public async Task watchAsync(CancellationToken cancellationToken)
        {
            //var emptyPipeline = new EmptyPipelineDefinition<ChangeStreamDocument<BsonDocument>>()
            //    .Match(n => n.OperationType == ChangeStreamOperationType.Insert);
            //var projectionPipe = matchPipeline.Project(n => n.FullDocument);
            var coll = database.GetCollection<BsonDocument>("notifications");

            var filter =
                Builders<ChangeStreamDocument<BsonDocument>>.Filter.And(
                    Builders<ChangeStreamDocument<BsonDocument>>.Filter.Eq(n => n.OperationType, ChangeStreamOperationType.Insert)
                );

            using (var cursor = await coll.WatchAsync(
                new EmptyPipelineDefinition<ChangeStreamDocument<BsonDocument>>()
                    .Match(filter)
                    .Project(d => d.FullDocument)

                , new ChangeStreamOptions { }
                , cancellationToken))
            {
                Console.WriteLine("Change Stream Started...");

                try
                {
                    foreach(var document in cursor.ToEnumerable(cancellationToken))
                    {
                        //var backingDoc = document.BackingDocument;
                        //var key = document.DocumentKey;

                        var elems = "";
                        foreach (var elem in document.Elements)
                            elems += "     " + elem.Name + " = " + elem.Value.ToString() + "\r\n";

                        OnNewChange?.Invoke(this, $"New BSON Doc\r\n" +
                            $"  Doc Type:{document.GetType().Name}\r\n" +
                            $"  Elements:\r\n" + elems);
                    }
                }
                catch(Exception ex)
                {

                }
            }
        }

        public event EventHandler<Notification> OnNewNotification;
        public event EventHandler<string> OnNewChange;
    }
}
