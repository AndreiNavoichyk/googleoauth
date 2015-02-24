using System;
using System.Collections.Generic;
using System.Windows.Controls;
using Caliburn.Micro;
using Caliburn.Micro.BindableAppBar;
using GoogleOAuthExample.Services;
using GoogleOAuthExample.ViewModels;
using GoogleOAuthExample.Views;
using NavigationCoercion;
using Telerik.Windows.Controls;
using DebugLog = GoogleOAuthExample.Services.DebugLog;
using ILog = GoogleOAuthExample.Services.ILog;

namespace GoogleOAuthExample
{
    public class Bootstrapper : PhoneBootstrapperBase
    {
        private static PhoneContainer _container;

        public Bootstrapper()
        {
            Initialize();
        }

        protected override void Configure()
        {
            _container = new PhoneContainer();

            _container.RegisterPhoneServices(RootFrame);

            RegisterNavigation();
            RegisterServices();

            _container.PerRequest<MainPageViewModel>();
            _container.PerRequest<UserProfilePageViewModel>();
            _container.PerRequest<PhotosPageViewModel>();

            AddCustomConventions();

            InteractionEffectManager.AllowedTypes.Add(typeof(Border));
        }

        private void RegisterNavigation()
        {
            var fluentNavigation = new FluentNavigation(RootFrame);

            fluentNavigation.WhenNavigatedTo<MainPage>()
                .ThenTo<UserProfilePage>()
                .RemoveEntriesFromBackStack(1);

            fluentNavigation.WhenNavigatedTo<UserProfilePage>()
                .ThenTo<MainPage>()
                .RemoveEntriesFromBackStack(1);
        }

        private void RegisterServices()
        {
            _container.PerRequest<ILog, DebugLog>();
            _container.PerRequest<IStorageService, StorageService>();
            _container.Singleton<ISocialNetworkService, GooglePlusService>();
            _container.Singleton<IImagePicker, ImagePicker>();
            _container.Singleton<INotificationsService, NotificationsService>();
            _container.Singleton<IInputService, InputService>();
        }

        private static void AddCustomConventions()
        {
            ConventionManager.AddElementConvention<BindableAppBarButton>(
                Control.IsEnabledProperty, "DataContext", "Click");
            ConventionManager.AddElementConvention<BindableAppBarMenuItem>(
                Control.IsEnabledProperty, "DataContext", "Click");
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
    }
}
