using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Notifications.Wpf.Core.Controls
{
    public class NotificationArea : Control
    {
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

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
            DependencyProperty.Register(nameof(Position), typeof(NotificationPosition), typeof(NotificationArea),
                new PropertyMetadata(NotificationPosition.BottomRight));

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
            DependencyProperty.Register(nameof(MaxItems), typeof(int), typeof(NotificationArea),
                new PropertyMetadata(int.MaxValue));

        public string? BindableName
        {
            get
            {
                return (string?)GetValue(BindableNameProperty);
            }
            set
            {
                SetValue(BindableNameProperty, value);
            }
        }

        public static readonly DependencyProperty BindableNameProperty =
            DependencyProperty.Register(nameof(BindableName), typeof(string), typeof(NotificationArea), new PropertyMetadata(null));

        public string Identifier
        {
            get
            {
                return BindableName ?? Name;
            }
        }

        private IList? _items;

        public NotificationArea()
        {
            NotificationManager.AddArea(this);
            Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
        }

        private void Dispatcher_ShutdownStarted(object? sender, EventArgs e)
        {
            // Clean up resources
            NotificationManager.RemoveArea(this);
            _semaphore?.Dispose();
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

        [Obsolete("This method is deprecated. Please use the new ShowAsync that includes an identifier instead", false)]
        public async Task ShowAsync(object content, TimeSpan expirationTime, Action? onClick, Action? onClose, CancellationToken token = default)
        {
            await ShowAsync(Guid.NewGuid(), content, expirationTime, (i) => onClick?.Invoke(), (i) => onClose?.Invoke(), token);
        }

        public async Task ShowAsync(Guid identifier, object content, TimeSpan expirationTime, Action<Guid>? onClick, Action<Guid>? onClose, CancellationToken token = default)
        {
            if (token.IsCancellationRequested)
            {
                return;
            }

            var notification = new Notification(identifier)
            {
                Content = content
            };

            notification.MouseLeftButtonDown += async (sender, args) =>
            {
                if (onClick != null)
                {
                    onClick.Invoke(identifier);

                    if (sender is Notification senderNotification)
                    {
                        await senderNotification.CloseAsync();
                    }
                }
            };

            notification.NotificationClosed += (sender, args) => onClose?.Invoke(identifier);
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

            try
            {
                await _semaphore.WaitAsync(token);

                try
                {
                    if (token.IsCancellationRequested)
                    {
                        return;
                    }

                    _items.Add(notification);

                    if (_items.OfType<Notification>().Count(i => !i.IsClosing) > MaxItems)
                    {
                        await _items.OfType<Notification>().First(i => !i.IsClosing).CloseAsync();
                    }
                }
                finally
                {
                    _semaphore.Release();
                }
            }
            catch (ObjectDisposedException)
            {
                return;
            }

            if (expirationTime != TimeSpan.MaxValue)
            {
                await Task.Delay(expirationTime);
                await notification.CloseAsync();
            }
        }

        public async Task CloseAsync(Guid identifier)
        {
            var notifications = _items.OfType<Notification>()?.Where(x => x.Identifier == identifier)?.ToList();

            await CloseNotificationsAsync(notifications);
        }

        public async Task CloseAllAsync()
        {
            var notifications = _items.OfType<Notification>()?.ToList();

            await CloseNotificationsAsync(notifications);
        }

        private async Task CloseNotificationsAsync(IList<Notification>? notifications)
        {
            if (notifications != null && notifications.Count > 0)
            {
                foreach (var item in notifications)
                {
                    await item.CloseAsync();
                }
            }
        }

        private void OnNotificationClosed(object sender, RoutedEventArgs routedEventArgs)
        {
            var notification = sender as Notification;
            _items?.Remove(notification);
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