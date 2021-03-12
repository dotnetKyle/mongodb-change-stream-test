using System;

namespace ChangeStreamTest
{
    public class Notification
    {
        public string UserLogin { get; set; }
        public DateTime EventDtgUtc { get; set; }
        public string Message { get; set; }
        public string Type { get; set; }
        public bool Dismissed { get; set; }
    }
}
