# Notifications.Wpf.Core
Toast notifications for WPF apps based on .NET Core 3.0  

This is a fork of ![Notifications.Wpf](https://github.com/Federerer/Notifications.Wpf)

![Demo](https://i.imgur.com/UvYIVFV.gif)

### Installation:
```
Install-Package Notifications.Wpf.Core
```
### Usage:

#### Notification over the taskbar:
```C#
var notificationManager = new NotificationManager();

await notificationManager.ShowAsync(new NotificationContent
           {
               Title = "Sample notification",
               Message = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.",
               Type = NotificationType.Information
           });
```

You can also alter this position by passing the desired position as an argument

```C#
var notificationManager = new NotificationManager(NotificationPosition.TopRight);
```

#### Notification inside application window:
- Adding namespace:
```XAML
xmlns:notifications="clr-namespace:Notifications.Wpf.Core.Controls;assembly=Notifications.Wpf.Core"
```
- Adding new NotificationArea:
```XAML
<notifications:NotificationArea x:Name="WindowArea" Position="TopLeft" MaxItems="3"/>
```
- Displaying notification:
```C#
await notificationManager.ShowAsync(
                new NotificationContent {Title = "Notification", Message = "Notification in window!"},
                areaName: "WindowArea");
```

#### Simple text with OnClick & OnClose actions:
```C#
await notificationManager.ShowAsync("String notification", onClick: () => Console.WriteLine("Click"),
               onClose: () => Console.WriteLine("Closed!"));
```
### Caliburn.Micro MVVM support:
- App.xaml:
```XAML
xmlns:controls="clr-namespace:Notifications.Wpf.Core.Controls;assembly=Notifications.Wpf.Core"

<Application.Resources>
    [...]
    <Style TargetType="controls:Notification">
        <Style.Resources>
            <DataTemplate DataType="{x:Type micro:PropertyChangedBase}">
                <ContentControl cal:View.Model="{Binding}"/>
            </DataTemplate>
        </Style.Resources>
    </Style>
</Application.Resources>
```
- NotificationViewModel:
```C#
// ViewModel must implement INotificationViewModel
public class NotificationViewModel : PropertyChangedBase, INotificationViewModel
    {
        private readonly INotificationManager _manager;

        public string? Title { get; set; }
        public string? Message { get; set; }

        public NotificationViewModel(INotificationManager manager)
        {
            _manager = manager;
        }

        public async Task Ok()
        {
            await Task.Delay(500);
            await _manager.ShowAsync(new NotificationContent { Title = "Success!", Message = "Ok button was clicked.", Type = NotificationType.Success });
        }

        public async Task Cancel()
        {
            await Task.Delay(500);
            await _manager.ShowAsync(new NotificationContent { Title = "Error!", Message = "Cancel button was clicked!", Type = NotificationType.Error });
        }
    }
```

- ShellViewModel:
```C#
var content = new NotificationViewModel(_manager)
{
    Title = "Custom notification.",
    Message = "Click on buttons!"
};

await _manager.ShowAsync(content, expirationTime: TimeSpan.FromSeconds(30));
```
- NotificationView:
```XAML
<DockPanel LastChildFill="False">
    <!--Using CloseOnClick attached property to close notification when button is pressed-->
    <Button x:Name="Ok" Content="Ok" DockPanel.Dock="Right" controls:Notification.CloseOnClick="True"/>
    <Button x:Name="Cancel" Content="Cancel" DockPanel.Dock="Right" Margin="0,0,8,0" controls:Notification.CloseOnClick="True"/>
</DockPanel>
```
- Result:

![Demo](https://i.imgur.com/G1ZU2ID.gif)
