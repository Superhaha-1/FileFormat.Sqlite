using System;
using System.Windows;
using System.Windows.Documents;
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
            var window = AdornedElement.FindTree<Window>();
            var element = adornedElement as FrameworkElement;
            if (element == null)
                throw new Exception("只能应用到FrameworkElement");
            Loaded += (s, e) =>
            {
                element.SizeChanged += SizeChangedCallback;
                window.SizeChanged += SizeChangedCallback;
            };
            Unloaded += (s, e) =>
            {
                element.SizeChanged -= SizeChangedCallback;
                window.SizeChanged -= SizeChangedCallback;
            };
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            var window = AdornedElement.FindTree<Window>();
            var top = AdornedElement.TranslatePoint(new Point(0, 0), window);
            drawingContext.DrawGeometry(MaskBrush, null, new GeometryGroup()
            {
                Children ={
                    new RectangleGeometry(new Rect(new Point(-top.X, -top.Y),window.RenderSize)),
                    new RectangleGeometry(new Rect(new Point(0, 0), AdornedElement.RenderSize))
                }
            });
        }

        private void SizeChangedCallback(object sender, SizeChangedEventArgs e)
        {
            InvalidateVisual();
        }
    }
}
