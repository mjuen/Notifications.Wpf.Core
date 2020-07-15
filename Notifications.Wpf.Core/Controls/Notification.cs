using Notifications.Wpf.Core.Utils;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Notifications.Wpf.Core.Controls
{
    [TemplatePart(Name = "PART_CloseButton", Type = typeof(Button))]
    public class Notification : ContentControl
    {
        private TimeSpan _closingAnimationTime = TimeSpan.Zero;

        public Guid Identifier { get; private set; }

        public bool IsClosing { get; set; }

        public static readonly RoutedEvent NotificationCloseInvokedEvent = EventManager.RegisterRoutedEvent(
            "NotificationCloseInvoked", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Notification));

        public static readonly RoutedEvent NotificationClosedEvent = EventManager.RegisterRoutedEvent(
            "NotificationClosed", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Notification));

        public Notification(Guid identifier)
        {
            Identifier = identifier;
        }

        static Notification()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Notification),
                new FrameworkPropertyMetadata(typeof(Notification)));
        }

        public event RoutedEventHandler NotificationCloseInvoked
        {
            add
            {
                AddHandler(NotificationCloseInvokedEvent, value);
            }

            remove
            {
                RemoveHandler(NotificationCloseInvokedEvent, value);
            }
        }

        public event RoutedEventHandler NotificationClosed
        {
            add
            {
                AddHandler(NotificationClosedEvent, value);
            }

            remove
            {
                RemoveHandler(NotificationClosedEvent, value);
            }
        }

        public static bool GetCloseOnClick(DependencyObject obj)
        {
            return (bool)obj.GetValue(CloseOnClickProperty);
        }

        public static void SetCloseOnClick(DependencyObject obj, bool value)
        {
            obj.SetValue(CloseOnClickProperty, value);
        }

        public static readonly DependencyProperty CloseOnClickProperty =
            DependencyProperty.RegisterAttached("CloseOnClick", typeof(bool), typeof(Notification), new FrameworkPropertyMetadata(false, CloseOnClickChanged));

        private static void CloseOnClickChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            if (dependencyObject is Button button)
            {
                var value = (bool)dependencyPropertyChangedEventArgs.NewValue;

                if (value)
                {
                    button.Click += async (sender, args) =>
                    {
                        var notification = VisualTreeHelperExtensions.GetParent<Notification>(button);

                        if (notification != null)
                        {
                            await notification.CloseAsync();
                        }
                    };
                }
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (GetTemplateChild("PART_CloseButton") is Button closeButton)
            {
                closeButton.Click += OnCloseButtonOnClickAsync;
            }

            var storyboards = Template.Triggers.OfType<EventTrigger>()
                .FirstOrDefault(t => t.RoutedEvent == NotificationCloseInvokedEvent)?.Actions.OfType<BeginStoryboard>()
                .Select(a => a.Storyboard);

            _closingAnimationTime = new TimeSpan(storyboards?.Max(s => Math.Min((s.Duration.HasTimeSpan ? s.Duration.TimeSpan + (s.BeginTime ?? TimeSpan.Zero) : TimeSpan.MaxValue).Ticks,
                s.Children.Select(ch => ch.Duration.TimeSpan + (s.BeginTime ?? TimeSpan.Zero)).Max().Ticks)) ?? 0);
        }

        private async void OnCloseButtonOnClickAsync(object sender, RoutedEventArgs args)
        {
            if (sender is Button button)
            {
                button.Click -= OnCloseButtonOnClickAsync;
                await CloseAsync();
            }
        }

        public async Task CloseAsync()
        {
            if (IsClosing)
            {
                return;
            }

            IsClosing = true;

            RaiseEvent(new RoutedEventArgs(NotificationCloseInvokedEvent));
            await Task.Delay(_closingAnimationTime);
            RaiseEvent(new RoutedEventArgs(NotificationClosedEvent));
        }
    }
}