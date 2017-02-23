using Focused.Utils;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Focused.Behaviors
{
    public class ButtonShortCutBehavior : ManagedBehaviorBase<ButtonBase>
    {
        private Window window;

        private UserControl userControl;

        private RegistrationMode currentRegistrationMode;

        public static readonly DependencyProperty KeyGestureProperty = DependencyProperty.Register(
            "KeyGesture", typeof(KeyGesture), typeof(ButtonShortCutBehavior), new PropertyMetadata(default(KeyGesture)));

        public static readonly DependencyProperty IsHiddenProperty = DependencyProperty.Register(
            "IsHidden", typeof(bool), typeof(ButtonShortCutBehavior), new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty RegistrationModeProperty = DependencyProperty.Register(
            "RegistrationMode", typeof(RegistrationMode), typeof(ButtonShortCutBehavior), new PropertyMetadata(default(RegistrationMode)));

        public static readonly DependencyProperty RegisterOnProperty = DependencyProperty.Register(
            "RegisterOn", typeof(FrameworkElement), typeof(ButtonShortCutBehavior), new PropertyMetadata(default(FrameworkElement)));

        public FrameworkElement RegisterOn
        {
            get
            {
                return (FrameworkElement)GetValue(RegisterOnProperty);
            }
            set
            {
                SetValue(RegisterOnProperty, value);
            }
        }

        public RegistrationMode RegistrationMode
        {
            get
            {
                return (RegistrationMode)GetValue(RegistrationModeProperty);
            }
            set
            {
                SetValue(RegistrationModeProperty, value);
            }
        }

        public KeyGesture KeyGesture
        {
            get { return (KeyGesture)this.GetValue(KeyGestureProperty); }
            set { this.SetValue(KeyGestureProperty, value); }
        }

        public bool IsHidden
        {
            get { return (bool)this.GetValue(IsHiddenProperty); }
            set { this.SetValue(IsHiddenProperty, value); }
        }

        protected override void OnSetup()               
        {
            this.window = this.AssociatedObject.FindParent<Window>();
            if (this.KeyGesture != null)
            {
                this.currentRegistrationMode = this.RegistrationMode;
                this.PreviewKeyDownScope.PreviewKeyDown += this.OnPreviewKeyDown;
            }
        }

        private UIElement PreviewKeyDownScope
        {
            get
            {
                switch (this.currentRegistrationMode)
                {
                    case RegistrationMode.Window:
                        return this.window;
                    case RegistrationMode.UserControl:
                        return this.userControl ?? (this.userControl = this.AssociatedObject.FindParent<UserControl>());
                    case RegistrationMode.Explicit:
                        if (this.RegisterOn == null)
                        {
                            throw new NullReferenceException("In explicit mode you have to bind RegisterOn property");
                        }
                        return this.RegisterOn;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            if (this.KeyGesture != null)
            {
                if (keyEventArgs.Key == Key.Enter)
                {
                    var focusedElement = FocusManager.GetFocusedElement(this.PreviewKeyDownScope);
                    if (focusedElement is ButtonBase && focusedElement != this.AssociatedObject)
                    {
                        return;
                    }
                }

                if (this.KeyGesture.Matches(null, keyEventArgs) && this.Execute())
                {
                    keyEventArgs.Handled = true;
                }
            }
        }

        protected override void OnCleanup()
        {
            if (this.KeyGesture != null)
            {
                this.PreviewKeyDownScope.PreviewKeyDown -= this.OnPreviewKeyDown;
            }
        }

        private bool Execute()
        {
            var canHit = this.AssociatedObject.CanHit(this.window);
            if (this.AssociatedObject == null || !canHit || (this.AssociatedObject.Visibility != Visibility.Visible && !this.IsHidden) || !this.AssociatedObject.IsEnabled)
            {
                return false;
            }

            if (this.AssociatedObject.Command != null && this.AssociatedObject.Command.CanExecute(this.AssociatedObject.CommandParameter))
            {
                this.AssociatedObject.Command.Execute(this.AssociatedObject.CommandParameter);
                return true;
            }

            if (this.AssociatedObject.Command == null)
            {
                this.AssociatedObject.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent, this.AssociatedObject));
                return true;
            }

            return false;
        }
    }

    public enum RegistrationMode
    {
        Window,

        UserControl,

        Explicit
    }
}
