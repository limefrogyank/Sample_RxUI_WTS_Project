using System;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;

using DynamicData;
using DynamicData.Binding;

using Microsoft.Toolkit.Uwp.UI.Controls;

using ReactiveUI;

using Sample_RxUI_WTS_Project.Models;
using Sample_RxUI_WTS_Project.Services;

namespace Sample_RxUI_WTS_Project.ViewModels
{
    public class DynamicDataViewModel : ReactiveObject
    {
        private readonly CompositeDisposable compositeDisposable;

        private string filterText = "";
        public string FilterText
        {
            get => filterText;
            set => this.RaiseAndSetIfChanged(ref filterText, value);
        }

        private (string headerName, DataGridSortDirection? sortDirection) sortOption;
        public (string headerName, DataGridSortDirection? sortDirection) SortOption
        {
            get => sortOption;
            set => this.RaiseAndSetIfChanged(ref sortOption, value);
        }

        // Load all data that contain a unique key into the SourceCache.  Data that doesn't have a unique key can use SourceList instead.
        private SourceCache<SampleOrder, long> orderCache = new SourceCache<SampleOrder, long>(x => x.OrderId);

        // You can pass the public IObservableCache to other classes so that they cannot modify the data within, just manipulate it.  (unused for now)
        public IObservableCache<SampleOrder, long> OrderCache => orderCache.AsObservableCache();

        // Required for binding to XAML controls.  This is just a projection of the underlying data.
        private ReadOnlyObservableCollection<SampleOrder> orders;
        public ReadOnlyObservableCollection<SampleOrder> Orders => orders;

        public DynamicDataViewModel()
        {
            compositeDisposable = new CompositeDisposable();

        // Observable to apply dynamic sorting.  When the SortOption changes, sort expression comparer is updated and sent out to observers.
        var sortOptionChanged = this.WhenValueChanged(@this => @this.SortOption)
            .Select(opt =>
            {
                if (opt.headerName == "OrderId")
                {
                    if (opt.sortDirection == DataGridSortDirection.Descending)
                        return SortExpressionComparer<SampleOrder>.Descending(x => x.OrderId);
                    else
                        return SortExpressionComparer<SampleOrder>.Ascending(x => x.OrderId);
                }
                else if (opt.headerName == "OrderDate")
                {
                    if (opt.sortDirection == DataGridSortDirection.Descending)
                        return SortExpressionComparer<SampleOrder>.Descending(x => x.OrderDate);
                    else
                        return SortExpressionComparer<SampleOrder>.Ascending(x => x.OrderDate);
                }
                else if (opt.headerName == "Company")
                {
                    if (opt.sortDirection == DataGridSortDirection.Descending)
                        return SortExpressionComparer<SampleOrder>.Descending(x => x.Company);
                    else
                        return SortExpressionComparer<SampleOrder>.Ascending(x => x.Company);
                }
                else if (opt.headerName == "ShipTo")
                {
                    if (opt.sortDirection == DataGridSortDirection.Descending)
                        return SortExpressionComparer<SampleOrder>.Descending(x => x.ShipTo);
                    else
                        return SortExpressionComparer<SampleOrder>.Ascending(x => x.ShipTo);
                }
                else if (opt.headerName == "OrderTotal")
                {
                    if (opt.sortDirection == DataGridSortDirection.Descending)
                        return SortExpressionComparer<SampleOrder>.Descending(x => x.OrderTotal);
                    else
                        return SortExpressionComparer<SampleOrder>.Ascending(x => x.OrderTotal);
                }
                else if (opt.headerName == "Status")
                {
                    if (opt.sortDirection == DataGridSortDirection.Descending)
                        return SortExpressionComparer<SampleOrder>.Descending(x => x.Status);
                    else
                        return SortExpressionComparer<SampleOrder>.Ascending(x => x.Status);
                }
                else if (opt.headerName == "Symbol")
                {
                    if (opt.sortDirection == DataGridSortDirection.Descending)
                        return SortExpressionComparer<SampleOrder>.Descending(x => x.Symbol);
                    else
                        return SortExpressionComparer<SampleOrder>.Ascending(x => x.Symbol);
                }
                else
                {
                    return SortExpressionComparer<SampleOrder>.Descending(x => x.OrderId);
                }
            });

            // Observable to apply dynamic filtering.  When the FilterText changes, filter predicate is updated and sent out to observers.
            var filterChanged = this.WhenValueChanged(@this => @this.FilterText)
                .Select((txt) =>
                {
                    var trimCaps = txt.Trim().ToUpper();
                    bool Predicate(SampleOrder order)
                    {
                        if (order.ShipTo.ToUpper().Contains(trimCaps) || order.Status.ToUpper().Contains(trimCaps) || order.Company.ToUpper().Contains(trimCaps))
                            return true;
                        else
                            return false;
                    }
                    return (Func<SampleOrder, bool>)Predicate;
                });


            

            // Connect to the source data, apply any filtering and/or sorting, and bind results to a readonly observable.
            orderCache.Connect()
                .Filter(filterChanged)
                .Sort(sortOptionChanged)
                .Bind(out orders)
                .Subscribe()
                .DisposeWith(compositeDisposable);


            // Load initial data and receive new data every 5 seconds.  Must be disposed manually since observable never completes.
            SampleDataService.GetDynamicDataObservable()
                .Subscribe(order =>
                {
                    orderCache.AddOrUpdate(order);
                })
                .DisposeWith(compositeDisposable);
        }

        public void Dispose()
        {
            // All subscriptions are disposed here in one convenient disposable.
            compositeDisposable.Dispose();
        }
    }
}
