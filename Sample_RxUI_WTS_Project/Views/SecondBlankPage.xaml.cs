using System;

using Sample_RxUI_WTS_Project.ViewModels;

using Windows.UI.Xaml.Controls;

namespace Sample_RxUI_WTS_Project.Views
{
    public sealed partial class SecondBlankPage : Page
    {
        private SecondBlankViewModel ViewModel
        {
            get { return DataContext as SecondBlankViewModel; }
        }

        public SecondBlankPage()
        {
            InitializeComponent();
        }
    }
}
