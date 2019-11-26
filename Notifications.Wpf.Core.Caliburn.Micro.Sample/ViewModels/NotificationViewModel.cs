using Caliburn.Micro;
using System.Threading.Tasks;

namespace Notifications.Wpf.Core.Caliburn.Micro.Sample.ViewModels
{
    public class NotificationViewModel : PropertyChangedBase, INotificationViewModel
    {
        private readonly INotificationManager _manager;

        public string? Title { get; set; }
        public string? Message { get; set; }

        public NotificationViewModel(INotificationManager manager)
        {
            _manager = manager;
        }

        public async Task Ok()
        {
            await Task.Delay(500);
            await _manager.ShowAsync(new NotificationContent { Title = "Success!", Message = "Ok button was clicked.", Type = NotificationType.Success });
        }

        public async Task Cancel()
        {
            await Task.Delay(500);
            await _manager.ShowAsync(new NotificationContent { Title = "Error!", Message = "Cancel button was clicked!", Type = NotificationType.Error });
        }
    }
}