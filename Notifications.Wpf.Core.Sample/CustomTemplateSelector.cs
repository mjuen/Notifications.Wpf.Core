using System.Windows;

namespace Notifications.Wpf.Core.Sample
{
    public class CustomTemplateSelector : NotificationTemplateSelector
    {
        public override DataTemplate? SelectTemplate(object item, DependencyObject container)
        {
            if (item is string)
            {
                return (container as FrameworkElement)?.FindResource("PinkStringTemplate") as DataTemplate;
            }

            return base.SelectTemplate(item, container);
        }
    }
}