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

        public NotificationManager() : this(NotificationPosition.BottomRight, null)
        {
        }

        public NotificationManager(NotificationPosition mainNotificationPosition) : this(mainNotificationPosition, null)
        {
        }

        public NotificationManager(Dispatcher? dispatcher) : this(NotificationPosition.BottomRight, dispatcher)
        {
        }

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

        public async Task ShowAsync(object content, string areaName = "", TimeSpan? expirationTime = null, Action? onClick = null,
            Action? onClose = null, CancellationToken token = default)
        {
            if (token.IsCancellationRequested)
            {
                return;
            }

            if (!_dispatcher.CheckAccess())
            {
                await _dispatcher.BeginInvoke(
                    new Action(async () => await ShowAsync(content, areaName, expirationTime, onClick, onClose)));
                return;
            }

            if (expirationTime == null)
            {
                expirationTime = TimeSpan.FromSeconds(5);
            }

            if (areaName == string.Empty)
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
                        Height = workArea.Height
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

        internal static void AddArea(NotificationArea area)
        {
            Areas.Add(area);
        }

        internal static void RemoveArea(NotificationArea area)
        {
            Areas.Remove(area);
        }
    }
}