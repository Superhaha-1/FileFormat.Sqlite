using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FileFormat.Sqlite.Demo.Controls
{
    public sealed class MaskPopup : Popup
    {
        private static Brush Brush_Mask { get; }

        static MaskPopup()
        {
            Brush_Mask = new SolidColorBrush(Colors.White);
            Brush_Mask.Opacity = 0.5;
            Brush_Mask.Freeze();
        }

        public MaskPopup()
        {
            AllowsTransparency = true;
            PopupAnimation = PopupAnimation.None;
            IsOpen = true;
            Placement = PlacementMode.Custom;
            CustomPopupPlacementCallback = PlacePopup;
            Path_Mask = new Path() { Fill = Brush_Mask, Stroke = null, StrokeThickness = 0 };
            Child = Path_Mask;
            Loaded += Local_Loaded;
        }

        private Path Path_Mask { get; }

        private Window Window_Host { get; set; }

        private void Local_Loaded(object sender, RoutedEventArgs e)
        {
            var target = PlacementTarget as FrameworkElement;
            if (target == null)
            {
                return;
            }

            Window_Host = Window.GetWindow(target);
            if (Window_Host == null)
            {
                return;
            }

            Window_Host.LocationChanged -= Window_Host_LocationChanged;
            Window_Host.LocationChanged += Window_Host_LocationChanged;
            Window_Host.SizeChanged -= MaskSizeChanged;
            Window_Host.SizeChanged += MaskSizeChanged;
            target.SizeChanged -= MaskSizeChanged;
            target.SizeChanged += MaskSizeChanged;
            //HostWindow.StateChanged -= this.hostWindow_StateChanged;
            //HostWindow.StateChanged += this.hostWindow_StateChanged;
            Window_Host.Activated -= Window_Host_Activated;
            Window_Host.Activated += Window_Host_Activated;
            Window_Host.Deactivated -= Window_Host_Deactivated;
            Window_Host.Deactivated += Window_Host_Deactivated;

            Unloaded -= Local_Unloaded;
            Unloaded += Local_Unloaded;

            DrawMask();
        }

        private void Window_Host_LocationChanged(object sender, EventArgs e)
        {
            var offset = HorizontalOffset;
            HorizontalOffset = offset + 1;
            HorizontalOffset = offset;
        }

        private void Window_Host_Deactivated(object sender, EventArgs e)
        {
            IsOpen = false;
        }

        private void Window_Host_Activated(object sender, EventArgs e)
        {
            IsOpen = true;
        }

        private void Local_Unloaded(object sender, RoutedEventArgs e)
        {
            var target = PlacementTarget as FrameworkElement;
            if (target == null)
            {
                return;
            }
            if (Window_Host == null)
            {
                return;
            }

            Window_Host.LocationChanged -= Window_Host_LocationChanged;
            Window_Host.SizeChanged -= MaskSizeChanged;
            target.SizeChanged -= MaskSizeChanged;
            Window_Host.Activated -= Window_Host_Activated;
            Window_Host.Deactivated -= Window_Host_Deactivated;
            Unloaded -= Local_Unloaded;
        }

        private void MaskSizeChanged(object sender, EventArgs e)
        {
            DrawMask();
        }

        private void DrawMask()
        {
            var top = PlacementTarget.TranslatePoint(new Point(0, 0), Window_Host);
            var windowSize = Window_Host.RenderSize;
            Path_Mask.Data = new GeometryGroup()
            {
                Children = {
                    new RectangleGeometry(new Rect(1, 1, windowSize.Width - 2, windowSize.Height - 2)),
                    new RectangleGeometry(new Rect(top, PlacementTarget.RenderSize))
                }
            };
        }

        public CustomPopupPlacement[] PlacePopup(Size popupSize, Size targetSize, Point offset)
        {
            Point location;
            if (Window_Host == null)
            {
                location = new Point();
            }
            else
            {
                var top = PlacementTarget.TranslatePoint(new Point(0, 0), Window_Host);
                location = new Point(-top.X, -top.Y);
            }
            return new CustomPopupPlacement[] { new CustomPopupPlacement(location, PopupPrimaryAxis.None) };
        }
    }
}
