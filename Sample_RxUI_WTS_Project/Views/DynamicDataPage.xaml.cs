using System;

using Microsoft.Toolkit.Uwp.UI.Controls;

using Sample_RxUI_WTS_Project.ViewModels;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Sample_RxUI_WTS_Project.Views
{
    public sealed partial class DynamicDataPage : Page
    {
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            ViewModel.Dispose();
        }

        private DynamicDataViewModel ViewModel
        {
            get { return DataContext as DynamicDataViewModel; }
        }

        private DataGridColumn lastSortedColumn;


        // TODO WTS: Change the grid as appropriate to your app.
        // For more details see the documentation at https://github.com/Microsoft/WindowsCommunityToolkit/blob/harinikmsft/datagrid/docs/controls/DataGrid.md
        public DynamicDataPage()
        {
            InitializeComponent();
            lastSortedColumn = initialSortColumn;
            initialSortColumn.SortDirection = DataGridSortDirection.Descending;
        }

        private void DataGrid_Sorting(object sender, Microsoft.Toolkit.Uwp.UI.Controls.DataGridColumnEventArgs e)
        {
            if (e.Column.SortDirection == null)
            {
                e.Column.SortDirection = DataGridSortDirection.Ascending;
            }
            else if (e.Column.SortDirection == DataGridSortDirection.Ascending)
            {
                e.Column.SortDirection = DataGridSortDirection.Descending;
            }
            else
            {
                e.Column.SortDirection = null;
            }
            if (lastSortedColumn != e.Column)
            {
                lastSortedColumn.SortDirection = null;
                lastSortedColumn = e.Column;
            }

            ViewModel.SortOption = (e.Column.Header.ToString(), e.Column.SortDirection);
        }
    }
}
