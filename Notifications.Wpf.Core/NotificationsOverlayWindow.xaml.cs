using Notifications.Wpf.Core.Controls;
using System.Windows;

namespace Notifications.Wpf.Core
{
    /// <summary>
    /// Interaction logic for NotificationsOverlayWindow.xaml
    /// </summary>
    public partial class NotificationsOverlayWindow : Window
    {
        public NotificationsOverlayWindow()
        {
            InitializeComponent();
        }

        public void SetNotificationAreaPosition(NotificationPosition notificationPosition)
        {
            NotificationsOverlayWindow_NotifyArea.Position = notificationPosition;
        }
    }
}