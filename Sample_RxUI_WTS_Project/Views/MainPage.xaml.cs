using System;

using Sample_RxUI_WTS_Project.ViewModels;

using Windows.UI.Xaml.Controls;

namespace Sample_RxUI_WTS_Project.Views
{
    public sealed partial class MainPage : Page
    {
        private MainViewModel ViewModel
        {
            get { return DataContext as MainViewModel; }
        }

        public MainPage()
        {
            InitializeComponent();
        }
    }
}
