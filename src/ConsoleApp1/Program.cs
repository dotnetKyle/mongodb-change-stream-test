using System;

namespace ChangeStreamTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var settings = AppSettings.GetSettings();
            var service = new NotificationService(
                "kyle", 
                new NotificationsCollection(
                    AppSettings.GetSettings()
                )
            );

            service.OnNewNotification += Service_OnNewNotification;

            Console.WriteLine("Connecting...");
            service.StartListeningForNewNotification();

            string currentServiceStatus = null;
            while (true)
            {
                if(currentServiceStatus != service.ServiceStatus.ToString())
                {
                    currentServiceStatus = service.ServiceStatus.ToString();
                    Console.WriteLine("Service Status:" + currentServiceStatus);
                }

                if (service.ServiceStatus == ServiceStatus.InError)
                    break;
                if (service.ServiceStatus == ServiceStatus.Stopped)
                    break;
                // stay open
            }
        }

        private static void Service_OnNewNotification(NewNotificationEventArgs args)
        {
            Console.WriteLine("{0} {1} - \"{2}\"",
                args.Notification.Type,
                args.Notification.DtgUtc.ToShortTimeString(),
                args.Notification.Message
            );
        }
    }
}
