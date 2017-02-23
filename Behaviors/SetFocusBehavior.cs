using Focused.BulletinBoard;
using Focused.Utils;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Focused.Behaviors
{
    public class SetFocusBehavior : PrioritisedFocusBehavior<FrameworkElement>
    {
        public override FocusPriority FocusPriority
        {
            get
            {
                return FocusPriority.Default;
            }
        }

        public Type TargetType
        {
            get { return (Type)GetValue(TargetTypeProperty); }
            set { SetValue(TargetTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TargeType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TargetTypeProperty =
            DependencyProperty.Register("TargetType", typeof(Type), typeof(SetFocusBehavior), new PropertyMetadata(null));



        public static readonly DependencyProperty IgnoreContentControlsProperty = DependencyProperty.Register("IgnoreContentControls", typeof(bool), typeof(SetFocusBehavior), new PropertyMetadata(default(bool)));

        public bool IgnoreContentControls
        {
            get
            {
                return (bool)GetValue(IgnoreContentControlsProperty);
            }
            set
            {
                SetValue(IgnoreContentControlsProperty, value);
            }
        }
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.Loaded += this.AssociatedObjectOnLoaded;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.Loaded -= this.AssociatedObjectOnLoaded;
        }

        private void AssociatedObjectOnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            if (this.ExistsFocusedControlWithHigherFocusPrio())
            {
                return;
            }

            IInputElement inputElement;
            if (this.TargetType == null)
            {
                inputElement = this.AssociatedObject.FindChild<IInputElement>(this.IsValidInputElement);
            }
            else
            {
                inputElement = this.AssociatedObject.FindChild(this.IsValidElement, this.TargetType) as IInputElement;
            }

            var waitIndicatorKey = new BusyIndicatorNotice();
            Action setFocus = () => Keyboard.Focus(inputElement);
            waitIndicatorKey.ExecuteActionIfBulletinAllowsIt(setFocus);

            this.AssociatedObject.Loaded -= this.AssociatedObjectOnLoaded;
        }

        private bool IsValidElement(object element)
        {
            var inputElement = element as IInputElement;
            if (inputElement != null)
            {
                return this.IsValidInputElement(inputElement);
            }

            return false;
        }

        private bool IsValidInputElement(IInputElement element)
        {
            if (element.IsEnabled && element.Focusable && (!this.IgnoreContentControls || !(element is ContentControl)))
            {
                return true;
            }

            return false;
        }
    }
}
