using System.Windows;
using System.Windows.Media;

namespace Focused.Adorner
{
    public class RectangleAdorner : System.Windows.Documents.Adorner
    {
        private readonly RectangleAdornerSettings settings;

        public RectangleAdorner(UIElement adornedElement, RectangleAdornerSettings settings) : base(adornedElement)
        {
            this.settings = settings;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            var rectangle = this.GetRectangle();
            var pen = this.GetPen();
            drawingContext.DrawRectangle(null, pen, rectangle);
        }

        private Rect GetRectangle()
        {
            var adornedRect = new Rect(this.AdornedElement.RenderSize);
            var margin = this.settings.Margin;
            var startPoint = new Point(adornedRect.TopLeft.X + margin, adornedRect.TopLeft.Y + margin);
            var endPoint = new Point(adornedRect.BottomRight.X - margin, adornedRect.BottomRight.Y - margin);
            return new Rect(startPoint, endPoint);
        }

        private Pen GetPen()
        {
            var pen = new Pen(this.settings.Stroke, this.settings.StrokeThickness);
            pen.DashStyle = this.settings.DashStyle;
            return pen;
        }
    }
}
