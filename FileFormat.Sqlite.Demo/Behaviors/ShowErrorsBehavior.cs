using System.Collections;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Interactivity;
using FileFormat.Sqlite.Demo.Adorners;

namespace FileFormat.Sqlite.Demo.Behaviors
{
    public sealed class ShowErrorsBehavior : Behavior<UIElement>
    {
        #region Errors

        public IEnumerable Errors
        {
            get
            {
                return GetValue(ErrorsProperty) as IEnumerable;
            }

            set
            {
                SetValue(ErrorsProperty, value);
            }
        }

        public static readonly DependencyProperty ErrorsProperty =
            DependencyProperty.Register(nameof(Errors), typeof(IEnumerable), typeof(ShowErrorsBehavior), new PropertyMetadata(ErrorsPropertyChangedCallback));

        private static void ErrorsPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            (dependencyObject as ShowErrorsBehavior).ErrorsChanged(e.NewValue as IEnumerable);
        }

        private void ErrorsChanged(IEnumerable errors)
        {
            if(errors == null)
            {
                if (ErrorsAdorner != null)
                {
                    (ErrorsAdorner.Parent as AdornerLayer).Remove(ErrorsAdorner);
                    ErrorsAdorner = null;
                }
            }
            else
            {
                if(ErrorsAdorner == null)
                {
                    var adornerLayer = AssociatedObject.GetTopAdornerLayer();
                    ErrorsAdorner = new ErrorsAdorner(AssociatedObject);
                    ErrorsAdorner.SetErrors(Errors);
                    adornerLayer.Add(ErrorsAdorner);
                }
                else
                {
                    ErrorsAdorner.SetErrors(Errors);
                }
            }
        }

        #endregion

        private ErrorsAdorner ErrorsAdorner { get; set; }

        protected override void OnAttached()
        {
            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
        }
    }
}
