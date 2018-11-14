using FileFormat.Sqlite.Demo.Controls;
using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;

namespace FileFormat.Sqlite.Demo.Adorners
{
    public sealed class MaskAdorner : Adorner
    {
        //private static Brush MaskBrush { get; }

        //static MaskAdorner()
        //{
        //    MaskBrush = new SolidColorBrush(Colors.White);
        //    MaskBrush.Opacity = 0.5;
        //    MaskBrush.Freeze();
        //}

        public MaskAdorner(UIElement adornedElement) : base(adornedElement)
        {
            //CurrentWindow = Window.GetWindow(AdornedElement);
            //var element = adornedElement as FrameworkElement;
            //if (element == null)
            //    throw new Exception("只能应用到FrameworkElement");
            //Loaded += (s, e) =>
            //{
            //    element.SizeChanged += SizeChangedCallback;
            //    CurrentWindow.SizeChanged += SizeChangedCallback;
            //};
            //Unloaded += (s, e) =>
            //{
            //    element.SizeChanged -= SizeChangedCallback;
            //    CurrentWindow.SizeChanged -= SizeChangedCallback;
            //};
            Popup_Mask = new MaskPopup() { PlacementTarget = adornedElement };
            AddVisualChild(Popup_Mask);
        }

        //private Window CurrentWindow { get; }

        private MaskPopup Popup_Mask { get; }

        protected override int VisualChildrenCount => 1;

        protected override Visual GetVisualChild(int index)
        {
            return Popup_Mask;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            //var top = AdornedElement.TranslatePoint(new Point(0, 0), CurrentWindow);
            //drawingContext.DrawGeometry(MaskBrush, null, new GeometryGroup()
            //{
            //    Children = {
            //        new RectangleGeometry(new Rect(new Point(-top.X, -top.Y), CurrentWindow.RenderSize)),
            //        new RectangleGeometry(new Rect(new Point(0, 0), AdornedElement.RenderSize))
            //    }
            //});
        }

        //private void SizeChangedCallback(object sender, SizeChangedEventArgs e)
        //{
        //    InvalidateVisual();
        //}
    }
}
