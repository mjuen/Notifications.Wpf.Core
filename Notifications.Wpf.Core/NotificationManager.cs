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
        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);
        private readonly CancellationTokenSource token = new CancellationTokenSource();
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
            if (token.Token.IsCancellationRequested)
                return;

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

            if (token.Token.IsCancellationRequested)
                return;

            await semaphore.WaitAsync();
            var selAreas = Areas.Where(a => a.Name == areaName).ToList();
            semaphore.Release();

            foreach (var area in selAreas)
            {
                await area.ShowAsync(content, (TimeSpan)expirationTime, onClick, onClose);
            }
        }

        internal static void AddArea(NotificationArea area)
        {
            Areas.Add(area);
        }

        public void Dispose()
        {
            if (token.Token.IsCancellationRequested)
                return;

            token.Cancel();

            _window?.Close();

            semaphore.Wait();
            while (Areas.Count > 0)
            {
                Areas[0].Dispose();
                Areas.RemoveAt(0);
            }
            semaphore.Release();
            semaphore.Dispose();
        }
    }
}