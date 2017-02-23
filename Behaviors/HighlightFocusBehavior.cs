using Focused.Adorner;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace Focused.Behaviors
{
    public class HighlightFocusBehavior : Behavior<Window> {
        private AdornerLayer adornerLayer;
        private System.Windows.Documents.Adorner adorner;
        private RectangleAdornerSettings adornerSettings;

        protected override void OnAttached()
        {
            this.adornerLayer = AdornerLayer.GetAdornerLayer(this.AssociatedObject);
            this.SetAdornerSettings();

            this.AssociatedObject.PreviewGotKeyboardFocus += this.OnFocusChanging;
        }

        private void SetAdornerSettings()
        {
            this.adornerSettings = new RectangleAdornerSettings();
            this.adornerSettings.DashStyle = DashStyles.Dash;
            this.adornerSettings.Margin = 0D;
            this.adornerSettings.Stroke = Brushes.CornflowerBlue;
            this.adornerSettings.StrokeThickness = 2D;
        }

        private void OnFocusChanging(object sender, KeyboardFocusChangedEventArgs e)
        {
            this.RemoveAdorner();
            this.AddAdorner(e.NewFocus as UIElement);
        }

        private void RemoveAdorner()
        {
            if (this.adorner != null)
            {
                this.adornerLayer.Remove(this.adorner);
                this.adorner = null;
                this.adornerLayer = null;
            }
        }

        private void AddAdorner(UIElement newFocus)
        {
            if (this.adornerLayer == null)
            {
                if (newFocus is Window)
                {
                    return;
                }
                this.CreateNewAdornerLayer(newFocus);
            }

            this.adorner = new RectangleAdorner(newFocus, this.adornerSettings);
            this.adornerLayer.Add(this.adorner);
        }

        private void CreateNewAdornerLayer(UIElement newFocus)
        {
            this.adornerLayer = AdornerLayer.GetAdornerLayer(newFocus);
        }

        protected override void OnDetaching()
        {
            this.RemoveAdorner();
            this.AssociatedObject.PreviewGotKeyboardFocus -= this.OnFocusChanging;
        }
    }
}
