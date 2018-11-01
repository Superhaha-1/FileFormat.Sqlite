using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace FileFormat.Sqlite.Demo.Adorners
{
    public sealed class ErrorsAdorner : Adorner
    {
        static ErrorsAdorner()
        {
            ErrorBrush = Brushes.Red;
            TextBrush = Brushes.White;
            MarkLength = 5d;
            BorderPen = new Pen(ErrorBrush, 1d);
            BorderPen.Freeze();
        }

        public ErrorsAdorner(UIElement adornedElement) : base(adornedElement)
        {
            TextBlock_Error = new TextBlock();
            TextBlock_Error.Padding = new Thickness(3);
            TextBlock_Error.Background = ErrorBrush;
            TextBlock_Error.Foreground = TextBrush;
            TextBlock_Error.MouseEnter += (s, e) =>
            {
                TextBlock_Error.Opacity = 0.2;
            };
            TextBlock_Error.MouseLeave += (s, e) =>
            {
                TextBlock_Error.Opacity = 1;
            };
            AddVisualChild(TextBlock_Error);
        }

        private TextBlock TextBlock_Error { get; }

        private static Pen BorderPen { get; }

        private static Brush ErrorBrush { get; }

        private static Brush TextBrush { get; }

        private static double MarkLength { get; }

        public void SetErrors(IEnumerable errors)
        {
            foreach (var error in errors)
            {
                TextBlock_Error.Text = error.ToString();
                break;
            }
        }

        protected override Visual GetVisualChild(int index)
        {
            return TextBlock_Error;
        }

        protected override int VisualChildrenCount => 1;

        protected override Size MeasureOverride(Size constraint)
        {
            var availableSize = new Size(AdornedElement.RenderSize.Width, AdornedElement.RenderSize.Height);
            for (int i = 0; i < VisualChildrenCount; i++)
            {
                if (GetVisualChild(i) is UIElement childElement)
                {
                    var renderSize = childElement.RenderSize;
                    childElement.Measure(new Size(Math.Max(renderSize.Width, availableSize.Width), Math.Max(renderSize.Height, availableSize.Height)));
                }
            }
            return availableSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var size = AdornedElement.RenderSize;
            TextBlock_Error.Arrange(new Rect(new Point(size.Width + 3, 3), TextBlock_Error.RenderSize));
            return base.ArrangeOverride(finalSize);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            var rect = new Rect(AdornedElement.RenderSize);
            drawingContext.DrawRectangle(null, BorderPen, rect);
            var errorMark = new StreamGeometry();
            using (var context = errorMark.Open())
            {
                context.BeginFigure(new Point(rect.Right - MarkLength, 0), true, true);
                context.LineTo(rect.TopRight, false, false);
                context.LineTo(new Point(rect.Right, MarkLength), false, false);
            }
            errorMark.Freeze();
            drawingContext.DrawGeometry(ErrorBrush, null, errorMark);
        }
    }
}
