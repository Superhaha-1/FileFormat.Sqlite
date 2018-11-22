using System.Windows;
using System;
using System.Reflection;
using System.Reactive.Linq;
using ReactiveUI;
using System.Linq;

namespace FileFormat.Sqlite.Demo
{
    public sealed class WpfActivationForViewFetcher : IActivationForViewFetcher
    {
        public WpfActivationForViewFetcher()
        {

        }

        public int GetAffinityForView(Type view)
        {
            return typeof(FrameworkElement).GetTypeInfo().IsAssignableFrom(view.GetTypeInfo()) ? int.MaxValue : 0;
        }

        public IObservable<bool> GetActivationForView(IActivatable view)
        {
            var fe = view as FrameworkElement;

            if (fe == null)
            {
                return Observable.Empty<bool>();
            }

            var viewLoaded = Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(
                x => fe.Loaded += x,
                x => fe.Loaded -= x).Select(_ => true);

            var viewUnloaded = Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(
                x => fe.Unloaded += x,
                x => fe.Unloaded -= x).Select(_ => false);

            return viewLoaded
                .Merge(viewUnloaded)
                //.Select(b => b ? fe.WhenAnyValue(x => x.IsHitTestVisible).SkipWhile(x => !x) : Observable.Return(false))
                //.Switch()
                .DistinctUntilChanged();
        }
    }
}
