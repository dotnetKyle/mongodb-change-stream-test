using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ChangeStreamTest
{
    public class Notification
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string UserLogin { get; set; }
        public DateTime EventDtgUtc { get; set; }
        public string Message { get; set; }
        public string Type { get; set; }
        public bool Dismissed { get; set; }
    }
}
