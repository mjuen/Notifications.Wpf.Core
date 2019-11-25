using System;
using System.Timers;
using System.Windows;

namespace Notifications.Wpf.Core.Sample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly NotificationManager _notificationManager = new NotificationManager();
        private readonly Random _random = new Random();

        public MainWindow()
        {
            InitializeComponent();

            Timer timer = new Timer { Interval = 3000 };
            timer.Elapsed += async (sender, args) => await _notificationManager.ShowAsync("Pink string from another thread!");
            timer.Start();

            Closing += (s, e) =>
            {
                _notificationManager?.Dispose();
            };
        }

        private async void Show_Click(object sender, RoutedEventArgs e)
        {
            var content = new NotificationContent
            {
                Title = "Sample notification",
                Message = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.",
                Type = (NotificationType)_random.Next(0, 4)
            };

            await _notificationManager.ShowAsync(content);
        }

        private async void ShowWindow_Click(object sender, RoutedEventArgs e)
        {
            var content = new NotificationContent { Title = "Notification in window", Message = "Click me!" };

            var clickContent = new NotificationContent
            {
                Title = "Clicked!",
                Message = "Window notification was clicked!",
                Type = NotificationType.Success
            };

            await _notificationManager.ShowAsync(content, "WindowArea", onClick: async () => await _notificationManager.ShowAsync(clickContent));
        }
    }
}