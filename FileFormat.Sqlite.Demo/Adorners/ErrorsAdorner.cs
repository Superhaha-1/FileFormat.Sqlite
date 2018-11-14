using MahApps.Metro.Controls;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace FileFormat.Sqlite.Demo.Adorners
{
    public sealed class ErrorsAdorner : Adorner
    {
        private static Pen BorderPen { get; }

        private static Brush ErrorBrush { get; }

        private static Brush TextBrush { get; }

        static ErrorsAdorner()
        {
            ErrorBrush = Brushes.Red;
            TextBrush = Brushes.White;
            BorderPen = new Pen(ErrorBrush, 1d);
            BorderPen.Freeze();
        }

        public ErrorsAdorner(UIElement adornedElement) : base(adornedElement)
        {
            Popup_Error = new CustomValidationPopup() { AllowsTransparency = true, IsOpen = true, CloseOnMouseLeftButtonDown = false, PlacementTarget = adornedElement };
            TextBlock_Error = new TextBlock() { Padding = new Thickness(3), Background = ErrorBrush, Foreground = TextBrush };
            Popup_Error.Child = TextBlock_Error;
            TextBlock_Error.MouseEnter += (s, e) =>
            {
                TextBlock_Error.Opacity = 0.2;
            };
            TextBlock_Error.MouseLeave += (s, e) =>
            {
                TextBlock_Error.Opacity = 1;
            };
            AddVisualChild(Popup_Error);
        }

        public void SetErrors(IEnumerable errors)
        {
            foreach (var error in errors)
            {
                TextBlock_Error.Text = error.ToString();
                break;
            }
        }

        private CustomValidationPopup Popup_Error { get; }

        private TextBlock TextBlock_Error { get; }

        protected override Visual GetVisualChild(int index)
        {
            return Popup_Error;
        }

        protected override int VisualChildrenCount => 1;

        protected override Size ArrangeOverride(Size finalSize)
        {
            Popup_Error.PlacementRectangle = new Rect(new Point(-3, 0), new Size(finalSize.Width + 6, finalSize.Height));
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
                context.BeginFigure(new Point(rect.Right - 5, 0), true, true);
                context.LineTo(rect.TopRight, false, false);
                context.LineTo(new Point(rect.Right, 5), false, false);
            }
            errorMark.Freeze();
            drawingContext.DrawGeometry(ErrorBrush, null, errorMark);
        }
    }
}
