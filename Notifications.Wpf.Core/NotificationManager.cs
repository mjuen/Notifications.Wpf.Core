using Notifications.Wpf.Core.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Notifications.Wpf.Core
{
    public class NotificationManager : INotificationManager
    {
        private static readonly List<NotificationArea> Areas = new List<NotificationArea>();

        private readonly Dispatcher _dispatcher;
        private static NotificationsOverlayWindow? _window;

        public NotificationManager(Dispatcher? dispatcher = null)
        {
            if (dispatcher == null)
            {
                dispatcher = Application.Current?.Dispatcher ?? Dispatcher.CurrentDispatcher;
            }

            _dispatcher = dispatcher;
        }

        public async Task ShowAsync(object content, string areaName = "", TimeSpan? expirationTime = null, Action? onClick = null,
            Action? onClose = null)
        {
            if (!_dispatcher.CheckAccess())
            {
                await _dispatcher.BeginInvoke(
                    new Action(async () => await ShowAsync(content, areaName, expirationTime, onClick, onClose)));
                return;
            }

            if (expirationTime == null) expirationTime = TimeSpan.FromSeconds(5);

            if (areaName == string.Empty && _window == null)
            {
                var workArea = SystemParameters.WorkArea;

                _window = new NotificationsOverlayWindow
                {
                    Left = workArea.Left,
                    Top = workArea.Top,
                    Width = workArea.Width,
                    Height = workArea.Height
                };

                _window.Show();
            }

            foreach (var area in Areas.Where(a => a.Name == areaName).ToList())
            {
                await area.ShowAsync(content, (TimeSpan)expirationTime, onClick, onClose);
            }
        }

        internal static void AddArea(NotificationArea area)
        {
            Areas.Add(area);
        }
    }
}