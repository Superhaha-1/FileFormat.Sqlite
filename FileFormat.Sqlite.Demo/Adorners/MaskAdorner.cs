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
          
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            var window = AdornedElement.FindTree<Window>();
            var top = AdornedElement.TranslatePoint(new Point(0, 0), window);
            drawingContext.DrawGeometry(MaskBrush, null, new RectangleGeometry(new Rect(top, AdornedElement.RenderSize)));
        }
    }
}
