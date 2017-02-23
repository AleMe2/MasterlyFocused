using Focused.BulletinBoard;
using System;
using System.Windows;
using System.Windows.Input;

namespace Focused.Behaviors
{
    /// <summary>
    /// This extension can be used to set keyboard focus to an ui element 
    /// via bound a boolean property of a view model
    /// </summary>
    public static class BooleanFocusExtension
    {
        public static bool GetIsFocused(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsFocusedProperty);
        }


        public static void SetIsFocused(DependencyObject obj, bool value)
        {
            obj.SetValue(IsFocusedProperty, value);
        }


        public static readonly DependencyProperty IsFocusedProperty = DependencyProperty.RegisterAttached(
            "IsFocused",
            typeof(bool),
            typeof(BooleanFocusExtension),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnIsFocusedPropertyChanged));
        
        private static void OnIsFocusedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var uie = (FrameworkElement)d;
            if ((bool)e.NewValue && uie.Dispatcher != null)
            {
                Action setFocusAction = () => SetNewFocus(uie);
                var waitIndicatorKey = new BusyIndicatorNotice();
                waitIndicatorKey.ExecuteActionIfBulletinAllowsIt(setFocusAction);
            }
        }

        private static void SetNewFocus(FrameworkElement uie)
        {
            Action setFocus = () =>
            {
                uie.Focus();
                Keyboard.Focus(uie);

                // set bound value to false directly
                // otherwise some ugly side effects can happen
                // i.e. some other focused element forces a rendering but should keep focus
                // without the following line it would loose it
                uie.SetValue(IsFocusedProperty, false);
            };

            if (uie.IsLoaded)
            {
                setFocus();
            }
            else
            {
                RoutedEventHandler onLoadedEventHandler = null;
                onLoadedEventHandler = (sender, args) =>
                {
                    uie.Loaded -= onLoadedEventHandler;
                    setFocus();
                };

                uie.Loaded += onLoadedEventHandler;
            }
        }
    }
}
