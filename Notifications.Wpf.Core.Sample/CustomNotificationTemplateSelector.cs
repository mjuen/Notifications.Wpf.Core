using System.Windows;

namespace Notifications.Wpf.Core.Sample
{
    // Derive your class from NotificationTemplateSelector
    public class CustomNotificationTemplateSelector : NotificationTemplateSelector
    {
        public override DataTemplate? SelectTemplate(object item, DependencyObject container)
        {
            if (item is string)
            {
                return (container as FrameworkElement)?.FindResource("PinkStringTemplate") as DataTemplate;
            }
            // You can also override the standard themes by using NotificationContent here
            else if (item is CustomThemedNotificationContent)
            {
                return (container as FrameworkElement)?.FindResource("CustomNotificationTemplate") as DataTemplate;
            }

            return base.SelectTemplate(item, container);
        }
    }
}