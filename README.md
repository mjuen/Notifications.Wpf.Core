# Notifications.Wpf.Core

[![Version](https://img.shields.io/nuget/v/Notifications.Wpf.Core.svg)](https://www.nuget.org/packages/Notifications.Wpf.Core)  [![Downloads](https://img.shields.io/nuget/dt/Notifications.Wpf.Core.svg)](https://www.nuget.org/packages/Notifications.Wpf.Core)

Toast notifications for WPF apps based on .NET 6 

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
<notifications:NotificationArea  
     MaxItems="3"
     x:Name="WindowArea"
     Position="TopLeft" />
```

It is also possible to add the area name with a `Binding`. But as binding to the `Name` property directly does not work, we offer you the `BindableName` property instead.

```XAML
<notifications:NotificationArea
    BindableName="{Binding NotificationAreaIdentifier}"
    MaxItems="3"
    Position="TopRight" />
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

#### Notifications with identifiers

Sometimes it comes in handy if you can close specific notifications via code. To do that you have the possibility to specify an identifier in the form of a `Guid` for a notification.

```C#

var identifier = Guid.NewGuid(); 

await notificationManager.ShowAsync(identifier, "I'm here to stay", 
           expirationTime: TimeSpan.MaxValue, 
           onClose: (id) => {
    NotifySomeoneAboutClose(id);
});

await notificationManager.CloseAsync(identifier);
```

#### Adjust style of notificiations

You do not like the default styles of the notifications? No problem. You can use your own custom styles. Take a look the sample project `Notifications.Wpf.Core.Sample` where this is demonstrated. Basically you have to create a custom `NotificationTemplateSelector` in which you specify which templates should be used.

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

The used view model must implement `INotificationViewModel`

```C#
public class NotificationViewModel : PropertyChangedBase, INotificationViewModel
    {
        private readonly INotificationManager _manager;

        private Guid? _notificationIdentifier;

        public string? Title { get; set; }
        public string? Message { get; set; }

        public NotificationViewModel(INotificationManager manager)
        {
            _manager = manager;
        }
        
        // This method is called when the notification with this view/view model is
        // shown. It can be used to receive the identifier of the notification
        public void SetNotificationIdentifier(Guid identifier)
        {
            _notificationIdentifier = identifier;
        }

        public async Task Ok()
        {
            await Task.Delay(500);
            await _manager.ShowAsync(new NotificationContent { Title = "Success!", 
                      Message = "Ok button was clicked.", Type = NotificationType.Success });
        }

        public async Task Cancel()
        {
            await Task.Delay(500);
            await _manager.ShowAsync(new NotificationContent { Title = "Error!", 
                      Message = "Cancel button was clicked!", Type = NotificationType.Error });
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
    <Button x:Name="Cancel" Content="Cancel" DockPanel.Dock="Right" Margin="0,0,8,0" 
           controls:Notification.CloseOnClick="True"/>
</DockPanel>
```
- Result:

![Demo](https://i.imgur.com/G1ZU2ID.gif)
