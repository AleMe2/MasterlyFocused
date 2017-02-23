using System.Windows.Media;

namespace Focused.Adorner
{
    public struct RectangleAdornerSettings
    {
        public double Margin { get; set; }

        public double StrokeThickness { get; set; }

        public Brush Stroke { get; set; }

        public DashStyle DashStyle { get; set; }
    }
}
