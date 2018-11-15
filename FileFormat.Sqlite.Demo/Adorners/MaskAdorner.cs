using System;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace FileFormat.Sqlite.Demo.Adorners
{
    public sealed class MaskAdorner : Adorner
    {
        private static Brush MaskBrush { get; }

        static MaskAdorner()
        {
            MaskBrush = new SolidColorBrush(Colors.White);
            MaskBrush.Opacity = 0.5;
            MaskBrush.Freeze();
        }

        public MaskAdorner(UIElement adornedElement) : base(adornedElement)
        {
            Loaded += Local_Loaded;
        }

        private Window CurrentWindow { get; set; }

        private void Local_Loaded(object sender, EventArgs e)
        {
            CurrentWindow = Window.GetWindow(AdornedElement);
            if (CurrentWindow == null)
                return;
            AdornedElement.KeyDown -= AdornedElement_KeyDownOrUp;
            AdornedElement.KeyDown += AdornedElement_KeyDownOrUp;
            AdornedElement.IsKeyboardFocusWithinChanged -= AdornedElement_IsKeyboardFocusWithinChanged;
            AdornedElement.IsKeyboardFocusWithinChanged += AdornedElement_IsKeyboardFocusWithinChanged;
            CurrentWindow.SizeChanged -= SizeChangedCallback;
            CurrentWindow.SizeChanged += SizeChangedCallback;
            Unloaded -= Local_Unloaded;
            Unloaded += Local_Unloaded;
        }

        private void Local_Unloaded(object sender, EventArgs e)
        {
            if (CurrentWindow == null)
                return;
            AdornedElement.KeyDown -= AdornedElement_KeyDownOrUp;
            AdornedElement.KeyUp -= AdornedElement_KeyDownOrUp;
            AdornedElement.IsKeyboardFocusWithinChanged -= AdornedElement_IsKeyboardFocusWithinChanged;
            CurrentWindow.SizeChanged -= SizeChangedCallback;
            Unloaded -= Local_Unloaded;
        }

        private static Key[] ExcludedKey { get; } = new Key[] {  Key.Tab, Key.Right, Key.Left, Key.Up, Key.Down, Key.PageUp, Key.PageDown, Key.System, Key.F1, Key.F2, Key.F3, Key.F4, Key.F5, Key.F6, Key.F6, Key.F7, Key.F8, Key.F9, Key.F10, Key.F11, Key.F12, Key.F13, Key.F14, Key.F15, Key.F16, Key.F17, Key.F18, Key.F19, Key.F20, Key.F21, Key.F22, Key.F23, Key.F24 };

        private void AdornedElement_KeyDownOrUp(object sender, KeyEventArgs e)
        {
            if (ExcludedKey.Contains(e.Key))
            {
                e.Handled = true;
                return;
            }
            if (e.KeyboardDevice.Modifiers != ModifierKeys.None)
            {
                e.Handled = true;
            }
        }

        private void SizeChangedCallback(object sender, SizeChangedEventArgs e)
        {
            InvalidateVisual();
        }

        private void AdornedElement_IsKeyboardFocusWithinChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            if (!AdornedElement.IsKeyboardFocusWithin)
            {
                return;
            }
            var top = AdornedElement.TranslatePoint(new Point(0, 0), CurrentWindow);
            drawingContext.DrawGeometry(MaskBrush, null, new GeometryGroup()
            {
                Children = {
                    new RectangleGeometry(new Rect(new Point(-top.X, -top.Y), CurrentWindow.RenderSize)),
                    new RectangleGeometry(new Rect(new Point(0, 0), AdornedElement.RenderSize))
                }
            });
        }
    }
}
