using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Windows.Input;

//using CommonServiceLocator;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using ReactiveUI;
using Sample_RxUI_WTS_Project.Services;
using Sample_RxUI_WTS_Project.Views;
using Sample_RxUI_WTS_Project.Helpers;

namespace Sample_RxUI_WTS_Project.ViewModels
{
    public class ShellViewModel : ReactiveObject
    {
        private NavigationView _navigationView;
        private NavigationViewItem _selected;

        public NavigationServiceEx NavigationService => ViewModelLocator.Current.NavigationService;


        public NavigationViewItem Selected
        {
            get { return _selected; }
            set { this.RaiseAndSetIfChanged(ref _selected, value); }
        }

        public ReactiveCommand<NavigationViewItemInvokedEventArgs, Unit> ItemInvokedCommand { get; }

        public ShellViewModel()
        {
            ItemInvokedCommand = ReactiveCommand.Create<NavigationViewItemInvokedEventArgs>(OnItemInvoked);
        }

        public void Initialize(Frame frame, NavigationView navigationView)
        {
            _navigationView = navigationView;
            NavigationService.Frame = frame;
            NavigationService.Navigated += Frame_Navigated;
        }

        private void OnItemInvoked(NavigationViewItemInvokedEventArgs args)
        {
            var item = _navigationView.MenuItems
                            .OfType<NavigationViewItem>()
                            .First(menuItem => (string)menuItem.Content == (string)args.InvokedItem);
            var pageKey = item.GetValue(NavHelper.NavigateToProperty) as string;
            NavigationService.Navigate(pageKey);
        }

        private void Frame_Navigated(object sender, NavigationEventArgs e)
        {
            Selected = _navigationView.MenuItems
                            .OfType<NavigationViewItem>()
                            .FirstOrDefault(menuItem => IsMenuItemForPageType(menuItem, e.SourcePageType));
        }

        private bool IsMenuItemForPageType(NavigationViewItem menuItem, Type sourcePageType)
        {
            var navigatedPageKey = NavigationService.GetNameOfRegisteredPage(sourcePageType);
            var pageKey = menuItem.GetValue(NavHelper.NavigateToProperty) as string;
            return pageKey == navigatedPageKey;
        }
    }
}
