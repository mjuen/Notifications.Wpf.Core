using System;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;

namespace Notifications.Wpf.Core.Sample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly NotificationManager _notificationManager = new NotificationManager(Controls.NotificationPosition.TopRight);
        private readonly Random _random = new Random();

        public MainWindow()
        {
            InitializeComponent();

            Timer timer = new Timer { Interval = 3000 };
            timer.Elapsed += async (sender, args) => await _notificationManager.ShowAsync("Pink string from another thread!");
            timer.Start();

            Closing += (s, e) =>
            {
                timer.Stop();
                timer.Dispose();
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

            var identifier = Guid.NewGuid();

            var clickContent = new NotificationContent
            {
                Title = "Clicked!",
                Message = $"Window notification with identifier\n{identifier}\nwas clicked!",
                Type = NotificationType.Success
            };

            await _notificationManager.ShowAsync(identifier, content, "WindowArea", expirationTime: TimeSpan.MaxValue,
                onClick: async (identifier) => await _notificationManager.ShowAsync(clickContent));

            await Task.Delay(3000);
            await _notificationManager.CloseAsync(identifier);
        }

        private async void CloseAll_Click(object sender, RoutedEventArgs e)
        {
            await _notificationManager.CloseAllAsync();
        }
    }
}