using System.Windows;
using System;
using System.Reactive.Linq;
using ReactiveUI;
using System.Linq;
using System.Windows.Controls;

namespace FileFormat.Sqlite.Demo
{
    public sealed class WpfDataTemplateBindingHook : IPropertyBindingHook
    {
        public static Lazy<DataTemplate> DefaultItemTemplate = new Lazy<DataTemplate>(() =>
        {
            return Application.Current.Resources["ItemTemplate"] as DataTemplate;
        });

        public bool ExecuteHook(object source, object target, Func<IObservedChange<object, object>[]> getCurrentViewModelProperties, Func<IObservedChange<object, object>[]> getCurrentViewProperties, BindingDirection direction)
        {
            var viewProperties = getCurrentViewProperties();
            var lastViewProperty = viewProperties.LastOrDefault();
            if (lastViewProperty == null)
            {
                return true;
            }

            var itemsControl = lastViewProperty.Sender as ItemsControl;
            if (itemsControl == null)
            {
                return true;
            }

            if (!string.IsNullOrEmpty(itemsControl.DisplayMemberPath))
            {
                return true;
            }

            if (viewProperties.Last().GetPropertyName() != "ItemsSource")
            {
                return true;
            }

            if (itemsControl.ItemTemplate != null)
            {
                return true;
            }

            if (itemsControl.ItemTemplateSelector != null)
            {
                return true;
            }

            itemsControl.ItemTemplate = DefaultItemTemplate.Value;
            return true;
        }
    }
}
