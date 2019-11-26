using System;
using System.Threading;
using System.Threading.Tasks;

namespace Notifications.Wpf.Core
{
    public interface INotificationManager
    {
        /// <summary>
        /// Shows a toast message
        /// </summary>
        /// <param name="text">The text that should be displayed</param>
        /// <param name="areaName">The name of the area in which the toast should appear</param>
        /// <param name="expirationTime">A <see cref="TimeSpan"/> after which the toast disappears</param>
        /// <param name="onClick">An action that is triggered when the toast is clicked</param>
        /// <param name="onClose">An action that is triggered when the toast closes</param>
        Task ShowAsync(string text, string areaName = "", TimeSpan? expirationTime = null, Action? onClick = null, Action? onClose = null,
                CancellationToken token = default);

        /// <summary>
        /// Shows a toast message
        /// </summary>
        /// <param name="content">The <see cref="NotificationContent"/> that should be displayed</param>
        /// <param name="areaName">The name of the area in which the toast should appear</param>
        /// <param name="expirationTime">A <see cref="TimeSpan"/> after which the toast disappears</param>
        /// <param name="onClick">An action that is triggered when the toast is clicked</param>
        /// <param name="onClose">An action that is triggered when the toast closes</param>
        Task ShowAsync(NotificationContent content, string areaName = "", TimeSpan? expirationTime = null, Action? onClick = null, Action? onClose = null,
                CancellationToken token = default);

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TViewModel">Must implement <see cref="INotificationViewModel"/></typeparam>
        /// <param name="viewModel">A Caliburn Micro view model that should be used. Must implement <see cref="INotificationViewModel"/></param>
        /// <param name="areaName">The name of the area in which the toast should appear</param>
        /// <param name="expirationTime">A <see cref="TimeSpan"/> after which the toast disappears</param>
        /// <param name="onClick">An action that is triggered when the toast is clicked</param>
        /// <param name="onClose">An action that is triggered when the toast closes</param>
        Task ShowAsync<TViewModel>(TViewModel viewModel, string areaName = "", TimeSpan? expirationTime = null, Action? onClick = null, Action? onClose = null,
                CancellationToken token = default) where TViewModel : INotificationViewModel;
    }
}