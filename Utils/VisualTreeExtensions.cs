using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Focused.Utils
{
    public static class VisualTreeExtensions
    {
        public static bool CanHit(this FrameworkElement element, Visual parentScopeElement)
        {
            var point = element.TransformToAncestor(parentScopeElement).Transform(new Point(element.ActualWidth / 2, element.ActualHeight / 2));
            var hitTestOk = false;
            HitTestFilterCallback filterCallback = target =>
            {
                var uiElement = target as UIElement;
                if (uiElement != null)
                {
                    if (!uiElement.IsHitTestVisible || !uiElement.IsVisible)
                    {
                        return HitTestFilterBehavior.ContinueSkipSelfAndChildren;
                    }
                }

                if (Equals(target, element))
                {
                    hitTestOk = true;
                    return HitTestFilterBehavior.Stop;
                }

                return HitTestFilterBehavior.Continue;
            };

            HitTestResultCallback resultCallback = result => HitTestResultBehavior.Stop;
            VisualTreeHelper.HitTest(parentScopeElement, filterCallback, resultCallback, new PointHitTestParameters(point));
            return hitTestOk;
        }

        public static T FindChild<T>(this DependencyObject o, Predicate<T> predicate, bool skipCollapsedOrHidden = true) where T : class
        {
            if (o is T && predicate(o as T))
            {
                return o as T;
            }
            if (skipCollapsedOrHidden && o is FrameworkElement && (o as FrameworkElement).Visibility != Visibility.Visible)
            {
                return null;
            }

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(o); i++)
            {
                var child = VisualTreeHelper.GetChild(o, i);
                var result = child.FindChild(predicate, skipCollapsedOrHidden);

                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        public static object FindChild(this DependencyObject o, Predicate<object> predicate, Type childType, bool skipCollapsedOrHidden = true)
        {
            if (o.GetType() == childType && predicate(o))
            {
                return o;
            }
            if (skipCollapsedOrHidden && o is FrameworkElement && (o as FrameworkElement).Visibility != Visibility.Visible)
            {
                return null;
            }

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(o); i++)
            {
                var child = VisualTreeHelper.GetChild(o, i);
                var result = child.FindChild(predicate, childType, skipCollapsedOrHidden);

                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        public static FrameworkElement FindChildWithTargetPropertyNameBinding(this FrameworkElement element, string targetPropertyName, bool skipCollapsedOrHidden = true)
        {
            if (element.ContainsSourcePropertyName(targetPropertyName))
            {
                return element;
            }

            if (skipCollapsedOrHidden && element.Visibility != Visibility.Visible)
            {
                return null;
            }

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                var child = VisualTreeHelper.GetChild(element, i) as FrameworkElement;
                if (child == null)
                {
                    return null;
                }
                var result = child.FindChildWithTargetPropertyNameBinding(targetPropertyName, skipCollapsedOrHidden);

                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        public static bool ContainsSourcePropertyName(this FrameworkElement element, string targetPropertyName)
        {
            var textBox = element as TextBox;

            var textBindingExpression = textBox?.GetBindingExpression(TextBox.TextProperty);
            if (textBindingExpression?.ResolvedSourcePropertyName == null)
            {
                return false;
            }

            return textBindingExpression.ResolvedSourcePropertyName.Contains(targetPropertyName);
        }

        public static T FindParent<T>(this DependencyObject o) where T : class
        {
            if (o is T)
            {
                return o as T;
            }
            var parent = VisualTreeHelper.GetParent(o) ?? (o as FrameworkElement).Parent;
            if (parent != null)
            {
                return parent.FindParent<T>();
            }
            return null;
        }
    }
}
