using System;
using System.Collections;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Notifications.Wpf.Core.Controls
{
    public class NotificationArea : Control, IDisposable
    {
        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        public NotificationPosition Position
        {
            get
            {
                return (NotificationPosition)GetValue(PositionProperty);
            }
            set
            {
                SetValue(PositionProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for Position.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register("Position", typeof(NotificationPosition), typeof(NotificationArea), new PropertyMetadata(NotificationPosition.BottomRight));

        public int MaxItems
        {
            get
            {
                return (int)GetValue(MaxItemsProperty);
            }
            set
            {
                SetValue(MaxItemsProperty, value);
            }
        }

        public static readonly DependencyProperty MaxItemsProperty =
            DependencyProperty.Register("MaxItems", typeof(int), typeof(NotificationArea), new PropertyMetadata(int.MaxValue));

        private IList? _items;

        public NotificationArea()
        {
            NotificationManager.AddArea(this);
        }

        static NotificationArea()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NotificationArea),
                new FrameworkPropertyMetadata(typeof(NotificationArea)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            var itemsControl = GetTemplateChild("PART_Items") as Panel;
            _items = itemsControl?.Children;
        }

        public async Task ShowAsync(object content, TimeSpan? expirationTime, Action? onClick, Action? onClose, CancellationToken token)
        {
            if (token.IsCancellationRequested)
            {
                return;
            }

            var notification = new Notification
            {
                Content = content
            };

            notification.MouseLeftButtonDown += async (sender, args) =>
            {
                if (onClick != null)
                {
                    onClick.Invoke();

                    if (sender is Notification senderNotification)
                    {
                        await senderNotification.CloseAsync();
                    }
                }
            };

            notification.NotificationClosed += (sender, args) => onClose?.Invoke();
            notification.NotificationClosed += OnNotificationClosed;

            if (!IsLoaded || _items == null)
            {
                return;
            }

            var w = Window.GetWindow(this);
            var x = PresentationSource.FromVisual(w);

            if (x == null)
            {
                return;
            }

            await semaphore.WaitAsync(token);

            if (token.IsCancellationRequested)
                return;

            try
            {
                _items.Add(notification);

                if (_items.OfType<Notification>().Count(i => !i.IsClosing) > MaxItems)
                {
                    await _items.OfType<Notification>().First(i => !i.IsClosing).CloseAsync();
                }
            }
            finally
            {
                semaphore.Release();
            }

            if (expirationTime == TimeSpan.MaxValue)
            {
                return;
            }

            await Task.Delay(expirationTime ?? TimeSpan.FromSeconds(5));
            await notification.CloseAsync();
        }

        private void OnNotificationClosed(object sender, RoutedEventArgs routedEventArgs)
        {
            var notification = sender as Notification;
            _items?.Remove(notification);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            semaphore?.Dispose();
        }
    }

    public enum NotificationPosition
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight
    }
}