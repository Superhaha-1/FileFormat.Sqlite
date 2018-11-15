using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
            Popup_Error = new Popup() { AllowsTransparency = true, IsOpen = true, Placement = PlacementMode.Right };
            TextBlock_Error = new TextBlock() { Padding = new Thickness(3), Background = ErrorBrush, Foreground = TextBrush };
            Popup_Error.Child = TextBlock_Error;
            AddVisualChild(Popup_Error);
            Loaded += Local_Loaded;
        }

        public void SetErrors(IEnumerable errors)
        {
            foreach (var error in errors)
            {
                TextBlock_Error.Text = error.ToString();
                break;
            }
        }

        private Popup Popup_Error { get; }

        private TextBlock TextBlock_Error { get; }

        private Window Window_Host { get; set; }

        protected override Visual GetVisualChild(int index)
        {
            return Popup_Error;
        }

        protected override int VisualChildrenCount => 1;

        private void Local_Loaded(object sender, EventArgs e)
        {
            Window_Host = Window.GetWindow(AdornedElement);
            if (Window_Host == null)
                return;
            AdornedElement.IsKeyboardFocusWithinChanged -= AdornedElement_IsKeyboardFocusWithinChanged;
            AdornedElement.IsKeyboardFocusWithinChanged += AdornedElement_IsKeyboardFocusWithinChanged;
            Window_Host.LocationChanged -= Window_Host_LocationChanged;
            Window_Host.LocationChanged += Window_Host_LocationChanged;
            TextBlock_Error.MouseEnter -= TextBlock_Error_MouseEnter;
            TextBlock_Error.MouseEnter += TextBlock_Error_MouseEnter;
            TextBlock_Error.MouseLeave -= TextBlock_Error_MouseLeave;
            TextBlock_Error.MouseLeave += TextBlock_Error_MouseLeave;
            Unloaded -= Local_Unloaded;
            Unloaded += Local_Unloaded;
        }

        private void Local_Unloaded(object sender, EventArgs e)
        {
            if (Window_Host == null)
                return;
            AdornedElement.IsKeyboardFocusWithinChanged -= AdornedElement_IsKeyboardFocusWithinChanged;
            Window_Host.LocationChanged -= Window_Host_LocationChanged;
            TextBlock_Error.MouseEnter -= TextBlock_Error_MouseEnter;
            TextBlock_Error.MouseLeave -= TextBlock_Error_MouseLeave;
            Unloaded -= Local_Unloaded;
            Window_Host = null;
        }

        private void TextBlock_Error_MouseEnter(object sender, EventArgs e)
        {
            TextBlock_Error.Opacity = 0.2;
        }

        private void TextBlock_Error_MouseLeave(object sender, EventArgs e)
        {
            TextBlock_Error.Opacity = 1;
        }

        private void Window_Host_LocationChanged(object sender, EventArgs e)
        {
            Popup_Error.Placement = PlacementMode.Custom;
            Popup_Error.Placement = PlacementMode.Right;
        }

        private void AdornedElement_IsKeyboardFocusWithinChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
                Popup_Error.IsOpen = true;
            else
                Popup_Error.IsOpen = false;
        }

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
