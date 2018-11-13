using FileFormat.Sqlite.Demo.Adorners;
using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Interactivity;

namespace FileFormat.Sqlite.Demo.Behaviors
{
    public sealed class SetMaskBehavior : Behavior<FrameworkElement>
    {
        #region IsCovered

        public bool IsCovered
        {
            get
            {
                return (bool)GetValue(IsCoveredProperty);
            }

            set
            {
                SetValue(IsCoveredProperty, value);
            }
        }

        public static readonly DependencyProperty IsCoveredProperty =
            DependencyProperty.Register(nameof(IsCovered), typeof(bool), typeof(SetMaskBehavior), new PropertyMetadata(false, IsCoveredPropertyChangedCallback, CoerceIsCovered));

        private static void IsCoveredPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            (dependencyObject as SetMaskBehavior).IsCoveredChanged((bool)e.NewValue);
        }

        private void IsCoveredChanged(bool isCovered)
        {
            if (isCovered)
            {
                if (MaskAdorner != null)
                    throw new Exception("掩膜已存在");
                MaskAdorner = new MaskAdorner(AssociatedObject);
                //var adornerLayer = AssociatedObject.GetTopAdornerLayer();
                var adornerLayer = AdornerLayer.GetAdornerLayer(AssociatedObject);
                adornerLayer.Add(MaskAdorner);
            }
            else
            {
                if (MaskAdorner == null)
                    throw new Exception("掩膜不存在");
                (MaskAdorner.Parent as AdornerLayer).Remove(MaskAdorner);
            }
        }
        private static object CoerceIsCovered(DependencyObject dependencyObject, object value)
        {
            return (dependencyObject as SetMaskBehavior).CoerceIsCoveredCallback(value);
        }

        private object CoerceIsCoveredCallback(object value)
        {
            if (AssociatedObject == null)
                return IsCovered;
            return value;
        }

        #endregion

        private MaskAdorner MaskAdorner { get; set; }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += AssociatedObjectLoadedCallback;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.Loaded -= AssociatedObjectLoadedCallback;
        }

        private void AssociatedObjectLoadedCallback(object sender, RoutedEventArgs e)
        {
            CoerceValue(IsCoveredProperty);
        }
    }
}
