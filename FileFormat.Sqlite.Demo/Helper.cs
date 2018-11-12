using MahApps.Metro.Controls.Dialogs;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace FileFormat.Sqlite.Demo
{
    public static class Helper
    {
        public static AdornerLayer GetTopAdornerLayer(this DependencyObject o)
        {
            AdornerLayer topAdornerLayer = null;
            if (o is Visual visual)
            {
                topAdornerLayer = AdornerLayer.GetAdornerLayer(visual);
            }
            if (topAdornerLayer == null)
                return null;
            return VisualTreeHelper.GetParent(o).GetTopAdornerLayer() ?? topAdornerLayer;
        }

        public static T FindTree<T>(this DependencyObject o) where T : Visual
        {
            if (o == null)
                return null;
            return o as T ?? FindTree<T>(VisualTreeHelper.GetParent(o));
        }

        public static async Task ShowProgressAsync(this object viewModel, string title, string message, int waitMillisecondsDelay = 1000)
        {
            var controller = await DialogCoordinator.Instance.ShowProgressAsync(viewModel, title, message);
            controller.SetIndeterminate();
            await Task.Delay(waitMillisecondsDelay);
            await controller.CloseAsync();
        }

        public static async Task ShowProgressAsync(this object viewModel, string title, string message, Func<Task> work)
        {
            var controller = await DialogCoordinator.Instance.ShowProgressAsync(viewModel, title, message);
            controller.SetIndeterminate();
            await work.Invoke();
            await controller.CloseAsync();
        }

        public static async Task<MessageDialogResult> ShowMessageAsync(this object viewModel, string title, string message, MessageDialogStyle style = MessageDialogStyle.Affirmative, MetroDialogSettings settings = null)
        {
            return await DialogCoordinator.Instance.ShowMessageAsync(viewModel, title, message, style, settings);
        }
    }

    public sealed class Disposable : IDisposable
    {
        public Disposable(string name)
        {
            Name = name;
            Debug.WriteLine($"{Name}:Create");
        }

        public string Name { get; }

        public void Dispose()
        {
            Debug.WriteLine($"{Name}:Dispose");
        }
    }
}
