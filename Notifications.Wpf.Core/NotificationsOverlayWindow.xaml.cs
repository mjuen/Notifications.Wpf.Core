using Notifications.Wpf.Core.Controls;
using System.Windows;

namespace Notifications.Wpf.Core
{
    /// <summary>
    /// Interaction logic for NotificationsOverlayWindow.xaml
    /// </summary>
    public partial class NotificationsOverlayWindow : Window
    {
        /// <summary>
        /// Constructor of the NotificationsOverlayWindow class
        /// </summary>
        public NotificationsOverlayWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Sets the position where notifications are displayed
        /// </summary>
        /// <param name="notificationPosition">The position where toast should be displayed</param>
        public void SetNotificationAreaPosition(NotificationPosition notificationPosition)
        {
            NotificationsOverlayWindow_NotifyArea.Position = notificationPosition;
        }
    }
}