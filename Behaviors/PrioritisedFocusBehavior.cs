using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace Focused.Behaviors
{
    public abstract class PrioritisedFocusBehavior<T> : Behavior<T> where T : FrameworkElement
    {
        public abstract FocusPriority FocusPriority { get; }

        /// <summary>
        /// Checks wether within the main window a focused control exists 
        /// that has a focus behavior with a higher priority attached. 
        /// </summary>
        /// <returns></returns>
        protected bool ExistsFocusedControlWithHigherFocusPrio()
        {
            var prioritisedFocusBehavior =
                GetAttachedBehaviorOfChildren<PrioritisedFocusBehavior<T>>(Application.Current.MainWindow)
                    .FirstOrDefault(behavior => behavior.FocusPriority > this.FocusPriority && behavior.IsAssociatedObjectFocused);

            return prioritisedFocusBehavior != null;
        }

        private bool IsAssociatedObjectFocused
        {
            get
            {
                if (this.AssociatedObject != null)
                {
                    return this.AssociatedObject.IsKeyboardFocusWithin;
                }

                return false;
            }
        }

        private static IEnumerable<TB> GetAttachedBehaviorOfChildren<TB>(DependencyObject depObj) where TB : Behavior
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    var bc = Interaction.GetBehaviors(child).OfType<TB>();
                    foreach (var behavior in bc)
                    {
                        yield return behavior;
                    }

                    foreach (var grandchild in GetAttachedBehaviorOfChildren<TB>(child))
                    {
                        yield return grandchild;
                    }
                }
            }
        }
    }

    public enum FocusPriority
    {
        Default,
        Low,
        Medium,
        High
    }
}
