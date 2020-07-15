using System;

namespace Notifications.Wpf.Core
{
    public interface INotificationViewModel
    {
        /// <summary>
        /// This method is called when the notification with the implementing view model is shown.
        /// It can be used to receive the identifier of the notification
        /// </summary>
        /// <param name="identifier">The identifier of the notification</param>
        public void SetNotificationIdentifier(Guid identifier)
        {
            // Default implementation, to provide backward compatibility
        }
    }
}