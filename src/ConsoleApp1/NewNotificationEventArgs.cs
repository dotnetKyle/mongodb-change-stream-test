using System;

namespace ChangeStreamTest
{
    public class NewNotificationEventArgs : EventArgs
    {
        public NewNotificationEventArgs(NewNotificationDTO notification)
        {
            Notification = notification;
        }
        public NewNotificationDTO Notification { get; set; }
    }
}
