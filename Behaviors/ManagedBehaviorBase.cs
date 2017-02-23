using System;
using System.Windows;
using System.Windows.Interactivity;

namespace Focused.Behaviors
{

    public abstract class ManagedBehaviorBase<T> : Behavior<T> where T : FrameworkElement
    {
        private bool isSetup;
        private bool isHookedUp;
        private WeakReference weakTarget;

        protected virtual void OnSetup() { }
        protected virtual void OnCleanup() { }
        protected override void OnChanged()
        {
            var target = this.AssociatedObject;
            if (target != null)
            {
                this.HookupBehavior(target);
            }
            else
            {
                this.UnHookupBehavior();
            }
        }

        private void OnTargetLoaded(object sender, RoutedEventArgs e) { this.SetupBehavior(); }

        private void OnTargetUnloaded(object sender, RoutedEventArgs e) { this.CleanupBehavior(); }

        private void HookupBehavior(T target)
        {
            if (this.isHookedUp) return;
            this.weakTarget = new WeakReference(target);
            this.isHookedUp = true;
            target.Unloaded += this.OnTargetUnloaded;
            target.Loaded += this.OnTargetLoaded;
            if (target.IsLoaded)
            {
                this.SetupBehavior();
            }
        }

        private void UnHookupBehavior()
        {
            if (!this.isHookedUp) return;
            this.isHookedUp = false;
            var target = this.AssociatedObject ?? (T)this.weakTarget.Target;
            if (target != null)
            {
                target.Unloaded -= this.OnTargetUnloaded;
                target.Loaded -= this.OnTargetLoaded;
            }
            this.CleanupBehavior();
        }

        private void SetupBehavior()
        {
            if (this.isSetup) return;
            this.isSetup = true;
            this.OnSetup();
        }

        private void CleanupBehavior()
        {
            if (!this.isSetup) return;
            this.isSetup = false;
            this.OnCleanup();
        }
    }
}
