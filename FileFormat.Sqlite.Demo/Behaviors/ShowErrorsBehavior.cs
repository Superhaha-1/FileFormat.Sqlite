using System.Collections;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Collections.Generic;
using System;
using System.Text;

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
                    var adornerLayer = AdornerLayer.GetAdornerLayer(AssociatedObject);
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

    public sealed class ErrorsAdorner : Adorner
    {
        public ErrorsAdorner(UIElement adornedElement) : base(adornedElement)
        {
            TextBlock_Errors = new TextBlock();
            TextBlock_Errors.Background = Brushes.Red;
        }

        private TextBlock TextBlock_Errors { get; }

        public void SetErrors(IEnumerable errors)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var error in errors)
                stringBuilder.Append(error.ToString());
            TextBlock_Errors.Text = stringBuilder.ToString();
        }

        protected override Visual GetVisualChild(int index)
        {
            return TextBlock_Errors;
        }

        protected override int VisualChildrenCount
        {
            get
            {
                return 1;
            }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            var availableSize = new Size(AdornedElement.RenderSize.Width, AdornedElement.RenderSize.Height);
            for (int i = 0; i < VisualChildrenCount; i++)
            {
                var childElement = GetVisualChild(i) as UIElement;
                if (childElement != null)
                {
                    var renderSize = childElement.RenderSize;
                    childElement.Measure(new Size(Math.Max(renderSize.Width, availableSize.Width), Math.Max(renderSize.Height, availableSize.Height)));
                }
            }
            return availableSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            TextBlock_Errors.Arrange(new Rect(new Point(-10, 20), TextBlock_Errors.DesiredSize));
            return base.ArrangeOverride(finalSize);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            var rect = new Rect(AdornedElement.RenderSize);
            drawingContext.DrawRectangle(new SolidColorBrush(Colors.Transparent), new Pen(new SolidColorBrush(Colors.Red), 1d), rect);
        }
    }
}
