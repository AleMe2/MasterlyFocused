using System;
using System.Windows;
using System.Windows.Interactivity;

namespace Focused.Triggers
{
    public abstract class ManagedTriggerBase<T> : TriggerBase<T> where T : FrameworkElement
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

        private void OnTarget_Loaded(object sender, RoutedEventArgs e) { this.SetupBehavior(); }

        private void OnTarget_Unloaded(object sender, RoutedEventArgs e) { this.CleanupBehavior(); }

        private void HookupBehavior(T target)
        {
            if (this.isHookedUp) return;
            this.weakTarget = new WeakReference(target);
            this.isHookedUp = true;
            target.Unloaded += this.OnTarget_Unloaded;
            target.Loaded += this.OnTarget_Loaded;
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
                target.Unloaded -= this.OnTarget_Unloaded;
                target.Loaded -= this.OnTarget_Loaded;
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
