using Notifications.Wpf.Core.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Notifications.Wpf.Core
{
    public class NotificationManager : INotificationManager
    {
        private static readonly List<NotificationArea> Areas = new List<NotificationArea>();
        private static NotificationsOverlayWindow? _window;

        private readonly Dispatcher _dispatcher;
        private readonly NotificationPosition _mainNotificationPosition;

        /// <summary>
        /// Creates an instance of the <see cref="NotificationManager"/>
        /// </summary>
        public NotificationManager() : this(NotificationPosition.BottomRight, null)
        {
        }

        /// <summary>
        /// Creates an instance of the <see cref="NotificationManager"/>
        /// </summary>
        /// <param name="mainNotificationPosition">The position where notifications with no custom area should
        /// be displayed</param>
        public NotificationManager(NotificationPosition mainNotificationPosition) : this(mainNotificationPosition, null)
        {
        }

        /// <summary>
        /// Creates an instance of the <see cref="NotificationManager"/>
        /// </summary>
        /// <param name="dispatcher">The <see cref="Dispatcher"/> that should be used</param>
        public NotificationManager(Dispatcher? dispatcher) : this(NotificationPosition.BottomRight, dispatcher)
        {
        }

        /// <summary>
        /// Creates an instance of the <see cref="NotificationManager"/>
        /// </summary>
        /// <param name="mainNotificationPosition">The position where notifications with no custom area should
        /// be displayed</param>
        /// <param name="dispatcher">The <see cref="Dispatcher"/> that should be used</param>
        public NotificationManager(NotificationPosition mainNotificationPosition,
            Dispatcher? dispatcher)
        {
            _mainNotificationPosition = mainNotificationPosition;

            if (dispatcher == null)
            {
                dispatcher = Application.Current?.Dispatcher ?? Dispatcher.CurrentDispatcher;
            }

            _dispatcher = dispatcher;
        }

        public async Task ShowAsync(string text, string? areaName = null, TimeSpan? expirationTime = null, Action? onClick = null, Action? onClose = null, CancellationToken token = default)
        {
            await InternalShowAsync(text, areaName, expirationTime, onClick, onClose, token);
        }

        public async Task ShowAsync(NotificationContent content, string? areaName = null, TimeSpan? expirationTime = null,
            Action? onClick = null, Action? onClose = null, CancellationToken token = default)
        {
            await InternalShowAsync(content, areaName, expirationTime, onClick, onClose, token);
        }

        public async Task ShowAsync<TViewModel>(TViewModel viewModel, string? areaName = null, TimeSpan? expirationTime = null,
            Action? onClick = null, Action? onClose = null, CancellationToken token = default)
            where TViewModel : INotificationViewModel
        {
            await InternalShowAsync(viewModel, areaName, expirationTime, onClick, onClose, token);
        }

        internal static void AddArea(NotificationArea area)
        {
            Areas.Add(area);
        }

        internal static void RemoveArea(NotificationArea area)
        {
            Areas.Remove(area);
        }

        private async Task InternalShowAsync(object content, string? areaName, TimeSpan? expirationTime, Action? onClick,
           Action? onClose, CancellationToken token)
        {
            if (token.IsCancellationRequested)
            {
                return;
            }

            if (!_dispatcher.CheckAccess())
            {
                await _dispatcher.BeginInvoke(
                    new Action(async () => await InternalShowAsync(content, areaName, expirationTime, onClick, onClose, token)));
                return;
            }

            if (expirationTime == null)
            {
                expirationTime = TimeSpan.FromSeconds(5);
            }

            if (string.IsNullOrEmpty(areaName))
            {
                areaName = "NotificationsOverlayWindow_NotifyArea";

                if (_window == null)
                {
                    var workArea = SystemParameters.WorkArea;

                    _window = new NotificationsOverlayWindow
                    {
                        Left = workArea.Left,
                        Top = workArea.Top,
                        Width = workArea.Width,
                        Height = workArea.Height,
                        Owner = Application.Current.MainWindow
                    };

                    _window.SetNotificationAreaPosition(_mainNotificationPosition);
                    _window.Show();
                }
            }

            if (token.IsCancellationRequested)
            {
                return;
            }

            foreach (var area in Areas.Where(a => a.Name == areaName).ToList())
            {
                await area.ShowAsync(content, (TimeSpan)expirationTime, onClick, onClose, token);
            }
        }
    }
}