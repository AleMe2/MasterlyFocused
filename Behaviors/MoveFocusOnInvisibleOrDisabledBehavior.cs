using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Focused.Behaviors
{
    /// <summary>
    /// This behavior moves the focus from the associated object, if
    /// - this associated object is focused AND
    /// - its visibility changes or it gets disabled
    /// </summary>
    public class MoveFocusOnInvisibleOrDisabledBehavior : Behavior<FrameworkElement>
    {
        protected override void OnAttached()
        {
            this.AssociatedObject.IsVisibleChanged += this.OnVisibilityChanged;
            this.AssociatedObject.IsEnabledChanged += this.OnEnabledChanged;
        }

        private void OnEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.MoveFocus(e.NewValue);
        }

        private void OnVisibilityChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.MoveFocus(e.NewValue);
        }

        private void MoveFocus(object newValue)
        {
            if (!this.AssociatedObject.IsKeyboardFocused && !this.AssociatedObject.IsKeyboardFocusWithin)
            {
                return;
            }

            if (newValue is bool)
            {
                if (!(bool)newValue)
                {
                    this.AssociatedObject.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                }
            }
        }

        protected override void OnDetaching()
        {
            this.AssociatedObject.IsVisibleChanged -= this.OnVisibilityChanged;
            this.AssociatedObject.IsEnabledChanged -= this.OnEnabledChanged;
        }
    }
}
