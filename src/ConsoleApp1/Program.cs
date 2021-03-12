using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChangeStreamTest
{
    class Program
    {
        static CancellationTokenSource cancellationTokenSource;
        static async Task Main(string[] args)
        {
            cancellationTokenSource = new CancellationTokenSource();

            var settings = AppSettings.GetSettings();
            var dao = new NotificationsCollection(settings, "kyle");

            await dao.InsertOneAsync(new Notification
            {
                Dismissed = false,
                EventDtgUtc = DateTime.UtcNow,
                Message = "Notification 1",
                Type = "askldjf",
                UserLogin = "kyle"
            });
            await dao.InsertOneAsync(new Notification
            {
                Dismissed = false,
                EventDtgUtc = DateTime.UtcNow,
                Message = "Notification 2",
                Type = "askldjf",
                UserLogin = "kyle"
            });
            await dao.InsertOneAsync(new Notification
            {
                Dismissed = false,
                EventDtgUtc = DateTime.UtcNow,
                Message = "Notification 3",
                Type = "askldjf",
                UserLogin = "kyle"
            });

            dao.OnNewChange += Dao_OnNewChange;
            dao.OnNewNotification += Dao_OnNewNotification;

            Console.WriteLine("Connecting...");

            await dao.watchAsync(cancellationTokenSource.Token);

            Console.WriteLine("Finished, exiting...");
        }

        private static void Dao_OnNewChange(object sender, string e)
        {
            Console.WriteLine(e);
        }

        private static void Dao_OnNewNotification(object sender, Notification e)
        {
            Console.WriteLine(e.Message);
        }
    }
}
