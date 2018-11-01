using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace FileFormat.Sqlite.Demo.Behaviors
{
    public sealed class LostKeyboardFocusBehavior : Behavior<TextBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.GotKeyboardFocus += GotKeyboardFocus;
            AssociatedObject.LostKeyboardFocus += LostKeyboardFocus;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.GotKeyboardFocus -= GotKeyboardFocus;
            AssociatedObject.LostKeyboardFocus -= LostKeyboardFocus;
        }

        private int Index { get; set; } = -1;

        private void GotKeyboardFocus(object sender, RoutedEventArgs e)
        {
            if (Index != -1)
                throw new Exception("已绑定命令");
            Index = (sender as TextBox).InputBindings.Add(new KeyBinding(new DelegateCommand(() => Keyboard.ClearFocus()),new KeyGesture(Key.Enter)));
        }

        private void LostKeyboardFocus(object sender, RoutedEventArgs e)
        {
            if (Index == -1)
                throw new Exception("还未绑定命令");
            (sender as TextBox).InputBindings.RemoveAt(Index);
            Index = -1;
        }
    }
}
