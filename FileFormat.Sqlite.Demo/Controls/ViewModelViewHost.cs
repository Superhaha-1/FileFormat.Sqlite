using ReactiveUI;
using Splat;
using System;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;

namespace FileFormat.Sqlite.Demo.Controls
{
    public class ViewModelViewHost : ContentControl, IViewFor, IEnableLogger
    {
        /// <summary>
        /// The view model dependency property.
        /// </summary>
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(object), typeof(ViewModelViewHost), new PropertyMetadata(null));

        /// <summary>
        /// The default content dependency property.
        /// </summary>
        public static readonly DependencyProperty DefaultContentProperty =
            DependencyProperty.Register("DefaultContent", typeof(object), typeof(ViewModelViewHost), new PropertyMetadata(null));

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelViewHost"/> class.
        /// </summary>
        public ViewModelViewHost()
        {
            this.WhenActivated(d =>
            {
                d(this.WhenAnyValue(x => x.ViewModel).CombineLatest(this.WhenAnyValue(x => x.DefaultContent), (viewModel, defaultContent) => viewModel).Subscribe(viewModel =>
                {
                    if (viewModel == null)
                    {
                        Content = DefaultContent;
                        return;
                    }

                    var viewLocator = ViewLocator ?? ReactiveUI.ViewLocator.Current;
                    var view = viewLocator.ResolveView(viewModel);

                    if (view == null)
                    {
                        throw new Exception($"Couldn't find view for '{viewModel}'.");
                    }

                    view.ViewModel = viewModel;
                    Content = view;
                }));
            });
        }

        /// <summary>
        /// If no ViewModel is displayed, this content (i.e. a control) will be displayed.
        /// </summary>
        public object DefaultContent
        {
            get => GetValue(DefaultContentProperty);
            set => SetValue(DefaultContentProperty, value);
        }

        /// <summary>
        /// The ViewModel to display.
        /// </summary>
        public object ViewModel
        {
            get => GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        /// <summary>
        /// Gets or sets the view locator.
        /// </summary>
        public IViewLocator ViewLocator { get; set; }
    }
}
