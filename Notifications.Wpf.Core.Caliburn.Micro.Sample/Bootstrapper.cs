using Caliburn.Micro;
using Notifications.Wpf.Core.Caliburn.Micro.Sample.ViewModels;
using System;
using System.Collections.Generic;
using System.Timers;
using System.Windows;

namespace Notifications.Wpf.Core.Caliburn.Micro.Sample
{
    public class Bootstrapper : BootstrapperBase
    {
        private readonly SimpleContainer _container = new SimpleContainer();

        public Bootstrapper()
        {
            Initialize();
        }

        protected override void Configure()
        {
            base.Configure();

            _container.Singleton<IWindowManager, WindowManager>();
            _container.Singleton<INotificationManager, NotificationManager>();
            _container.Singleton<ShellViewModel>();
        }

        protected override object GetInstance(Type service, string key)
        {
            return _container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }

        protected override async void OnStartup(object sender, StartupEventArgs e)
        {
            base.OnStartup(sender, e);
            await DisplayRootViewForAsync<ShellViewModel>();
            Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;

            var timer = new Timer { Interval = 12000 };
            timer.Elapsed += async (o, args) => await IoC.Get<INotificationManager>().ShowAsync("String from Bootstrapper!");
            timer.Start();
        }
    }
}