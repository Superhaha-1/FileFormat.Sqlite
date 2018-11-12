using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace FileFormat.Sqlite.Demo.Behaviors
{
    public sealed class LostKeyboardFocusBehavior : Behavior<TextBox>
    {
        static LostKeyboardFocusBehavior()
        {
            EnterKeyBinding = new KeyBinding(new DelegateCommand(() => Keyboard.ClearFocus()), new KeyGesture(Key.Enter));
            EnterKeyBinding.Freeze();
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.InputBindings.Add(EnterKeyBinding);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.InputBindings.Remove(EnterKeyBinding);
        }

        private static KeyBinding EnterKeyBinding { get; }
    }
}
