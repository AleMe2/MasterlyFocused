using Focused.BulletinBoard;
using Focused.Utils;
using System;
using System.Windows;
using System.Windows.Input;

namespace Focused.Behaviors
{
    /// <summary>
    /// This Extension uses the bound property name to set the focus the UI element which is bount the source.
    /// If the bound value has been changed the visual tree is search for an TextBox which has its TextProperty
    /// bound to the set PropertyName.
    /// </summary>
    public class FocusBySourcePropertyNameExtension
    {
        public static string GetSourcePropertyName(DependencyObject obj)
        {
            return (string)obj.GetValue(SourcePropertyNameProperty);
        }

        public static void SetSourcePropertyName(DependencyObject obj, string value)
        {
            obj.SetValue(SourcePropertyNameProperty, value);
        }

        // Using a DependencyProperty as the backing store for TargetPropertyName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SourcePropertyNameProperty =
            DependencyProperty.RegisterAttached(
                "SourcePropertyName", 
                typeof(string), 
                typeof(FocusBySourcePropertyNameExtension), 
                new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSourcePropertyNameChanged));

        private static void OnSourcePropertyNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var uie = (FrameworkElement)d;
            if (!string.IsNullOrEmpty((string)e.NewValue) && uie.Dispatcher != null)
            {
                var elementToFocus = uie.FindChildWithTargetPropertyNameBinding((string)e.NewValue);
                if (elementToFocus != null)
                {
                    Action setFocusAction = () => SetNewFocus(elementToFocus, uie);
                    var waitIndicatorKey = new BusyIndicatorNotice();
                    waitIndicatorKey.ExecuteActionIfBulletinAllowsIt(setFocusAction);
                }
            }
        }

        private static void SetNewFocus(FrameworkElement elementToFocus, FrameworkElement uie)
        {
            Action setFocus = () =>
            {
                Keyboard.Focus(elementToFocus);

                // set bound value to string.Empty directly
                // otherwise some ugly side effects can happen
                // i.e. some other focused element forces a rendering but should keep focus
                // without the following line it would loose it
                uie.SetValue(SourcePropertyNameProperty, string.Empty);
            };

            if (elementToFocus.IsLoaded)
            {
                setFocus();
            }
            else
            {
                RoutedEventHandler onLoadedEventHandler = null;
                onLoadedEventHandler = (sender, args) =>
                {
                    elementToFocus.Loaded -= onLoadedEventHandler;
                    setFocus();
                };

                elementToFocus.Loaded += onLoadedEventHandler;
            }
        }
    }
}
