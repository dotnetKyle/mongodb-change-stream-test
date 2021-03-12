using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChangeStreamTest
{
    public enum ServiceStatus
    {
        NotStarted,
        Running,
        Stopped,
        Stopping,
        InError
    }
    public class NotificationService
    {
        NotificationsCollection _collection;
        string _userLogin;
        CancellationTokenSource _cancellationTokenSource;
        public NotificationService(string userLogin, NotificationsCollection collection)
        {
            _collection = collection;
            _userLogin = userLogin;
        }

        public ServiceStatus ServiceStatus
        {
            get
            {
                if (RunningNotificationTask == null)
                    return ServiceStatus.NotStarted;

                var status = RunningNotificationTask.Status;
                if (status == TaskStatus.Running)
                {
                    if (_cancellationTokenSource.IsCancellationRequested)
                        return ServiceStatus.Stopping;

                    return ServiceStatus.Running;
                }
                else if (status == TaskStatus.Created || status == TaskStatus.WaitingForActivation || status == TaskStatus.WaitingToRun)
                {
                    return ServiceStatus.NotStarted;
                }
                else if (status == TaskStatus.Canceled)
                {
                    return ServiceStatus.Stopped;
                }
                else 
                {
                    // status == TaskStatus.WaitingForChildrenToComplete 
                    // status == TaskStatus.RanToCompletion 
                    // status == TaskStatus.Faulted)
                    // (or any other)
                    return ServiceStatus.InError;
                }
            }
        }

        /// <summary>
        /// Starts the service listening for new notifications
        /// </summary>
        public void StartListeningForNewNotification()
        {
            _cancellationTokenSource = new CancellationTokenSource();

            // Task.Run starts the task in the background
            // await the task starts the task but freezes the current thread (because this task really never finishes)
            RunningNotificationTask = Task.Run(() => startListeningToNotification(_cancellationTokenSource.Token), _cancellationTokenSource.Token);
        }
        /// <summary>
        /// Stops the service
        /// </summary>
        public void StopListeningForNotifications()
        {
            if(_cancellationTokenSource.IsCancellationRequested == false)
                _cancellationTokenSource.Cancel();
        }

        int notificationServiceErrorCount;
        Task RunningNotificationTask;
        void startListeningToNotification(CancellationToken cancellationToken)
        {
            try
            {
                using (IChangeStreamCursor<ChangeStreamDocument<Notification>> cursor = _collection.Watch(_userLogin, cancellationToken))
                {
                    try
                    {
                        // might want to actually create an event inside the DAO 
                        //   that way we are not depending on IChangeStreamCursor in the service
                        //    this might make unit tests much harder
                        //  Could also return cursor.ToEnumerable(cancellationToken) inside the DAO that way it simply returns IEnumerable<ChangeStreamDocument<Notification>>
                        //   ChangeStreamDocument<Notification> can easily be faked by creating a bson document
                        foreach (var notif in cursor.ToEnumerable(cancellationToken))
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                            OnNewNotification?.Invoke(new NewNotificationEventArgs(
                                new NewNotificationDTO 
                                {
                                    Id = notif.FullDocument.Id, 
                                    Message = notif.FullDocument.Message, 
                                    DtgUtc = notif.FullDocument.EventDtgUtc,
                                    Type = notif.FullDocument.Type
                                }
                            ));
                        }
                    }
                    catch(OperationCanceledException ex)
                    {
                        Console.WriteLine("Operation was canceled");
                        throw ex;
                    }
                    catch(Exception ex)
                    {
                        notificationServiceErrorCount++;

                        if (notificationServiceErrorCount > 2)
                            throw ex;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Notification service cancellation requested");
            }
            catch(Exception ex)
            {
                Console.WriteLine("Notification service error:");
                Console.WriteLine(ex);
            }
        }

        public event OnNewNotificationEvent OnNewNotification;
    }

    public delegate void OnNewNotificationEvent(NewNotificationEventArgs args);
}
