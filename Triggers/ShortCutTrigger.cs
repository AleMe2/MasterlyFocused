using Focused.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Focused.Triggers
{
    public class ShortCutTrigger : ManagedTriggerBase<FrameworkElement>
    {
        private KeyGesture shortCut;
        private readonly bool designTime;

        public static readonly DependencyProperty KeyGestureProperty = DependencyProperty.Register(
            "KeyGesture", typeof(KeyGesture), typeof(ShortCutTrigger), new PropertyMetadata(default(KeyGesture)));

        public KeyGesture KeyGesture
        {
            get { return (KeyGesture)this.GetValue(KeyGestureProperty); }
            set { this.SetValue(KeyGestureProperty, value); }
        }

        public static readonly DependencyProperty ParameterProperty = DependencyProperty.Register(
            "Parameter", typeof(object), typeof(ShortCutTrigger), new PropertyMetadata(default(object)));

        public static readonly DependencyProperty RegistrationModeProperty = DependencyProperty.Register("RegistrationMode", typeof(RegistrationMode), typeof(ShortCutTrigger), new PropertyMetadata(default(RegistrationMode)));

        public static readonly DependencyProperty RegisterOnProperty = DependencyProperty.Register("RegisterOn", typeof(FrameworkElement), typeof(ShortCutTrigger), new PropertyMetadata(default(FrameworkElement)));

        public FrameworkElement RegisterOn
        {
            get
            {
                return (FrameworkElement)this.GetValue(RegisterOnProperty);
            }
            set
            {
                this.SetValue(RegisterOnProperty, value);
            }
        }

        private Window window;

        private RegistrationMode currentRegistrationMode;

        private UserControl userControl;

        private object parameter;

        public RegistrationMode RegistrationMode
        {
            get
            {
                return (RegistrationMode)this.GetValue(RegistrationModeProperty);
            }
            set
            {
                this.SetValue(RegistrationModeProperty, value);
            }
        }

        public object Parameter
        {
            get { return (object)this.GetValue(ParameterProperty); }
            set { this.SetValue(ParameterProperty, value); }
        }

        public ShortCutTrigger()
        {
            this.designTime = System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject());
        }

        protected override void OnSetup()
        {
            if (this.designTime)
                return;
            this.shortCut = this.KeyGesture;
            this.parameter = this.Parameter;
            this.window = this.AssociatedObject.FindParent<Window>();
            if (this.shortCut != null)
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

        protected override void OnCleanup()
        {
            if (this.designTime)
                return;

            if (this.shortCut != null)
            {
                this.PreviewKeyDownScope.PreviewKeyDown -= this.OnPreviewKeyDown;
            }
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            if (keyEventArgs.Key == Key.Enter)
            {
                var focusedElement = FocusManager.GetFocusedElement(this.PreviewKeyDownScope);
                if (focusedElement is ButtonBase && focusedElement != this.AssociatedObject)
                {
                    return;
                }
            }

            if (this.shortCut.Matches(null, keyEventArgs) && this.Execute())
            {
                keyEventArgs.Handled = true;
            }
        }

        private bool Execute()
        {
            if (this.AssociatedObject == null || this.AssociatedObject.Visibility != Visibility.Visible || !this.AssociatedObject.IsEnabled)
            {
                return false;
            }

            this.InvokeActions(this.parameter);
            return true;
        }
    }
    public enum RegistrationMode
    {
        Window,

        UserControl,

        Explicit
    }
}
