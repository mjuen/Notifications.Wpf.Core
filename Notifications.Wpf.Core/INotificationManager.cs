using System;
using System.Threading.Tasks;

namespace Notifications.Wpf.Core
{
    public interface INotificationManager : IDisposable
    {
        Task ShowAsync(object content, string areaName = "", TimeSpan? expirationTime = null, Action? onClick = null, Action? onClose = null);
    }
}