using MongoDB.Bson;
using System;

namespace ChangeStreamTest
{
    public class NewNotificationDTO
    {
        public ObjectId Id { get; set; }
        public string Message { get; set; }
        public string Type { get; set; }
        public DateTime DtgUtc { get; set; }
    }
}
