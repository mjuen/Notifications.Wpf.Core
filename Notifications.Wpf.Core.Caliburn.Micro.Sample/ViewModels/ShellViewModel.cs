using Caliburn.Micro;
using System;
using System.Threading.Tasks;

namespace Notifications.Wpf.Core.Caliburn.Micro.Sample.ViewModels
{
    public class ShellViewModel : PropertyChangedBase
    {
        private readonly INotificationManager _manager;

        public ShellViewModel(INotificationManager manager)
        {
            _manager = manager;
        }

        public async Task Show()
        {
            var content = new NotificationViewModel(_manager)
            {
                Title = "Custom notification.",
                Message = "Click on buttons!"
            };

            await _manager.ShowAsync(content, expirationTime: TimeSpan.FromSeconds(30));
        }

        public async Task ShowInWindow()
        {
            await _manager.ShowAsync(new NotificationContent { Title = "Message", Message = "Message in window" }, areaName: "WindowArea");
        }
    }
}