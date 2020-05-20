using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SimulationWPF
{
    public class Vertex
    {
        int x;
        public int X
        {
            get { return x; }
            set
            {
                x = value;
                Canvas.SetLeft(Ring, x - 20);
                Canvas.SetLeft(RingContent, x - 10);
            }
        }
        int y;
        public int Y
        {
            get { return y; }
            set
            {
                y = value;
                Canvas.SetTop(Ring, y - 20);
                Canvas.SetTop(RingContent, y - 11);
            }
        }

        public Ellipse Ring { get; private set; }
        public TextBox RingContent { get; set; }

        public Vertex(int _x, int _y, int _ringContent)
        {
            RingContent = new TextBox();
            RingContent.Text = _ringContent.ToString();
            RingContent.Background = Brushes.LightGray;
            RingContent.BorderBrush = Brushes.LightGray;
            RingContent.FontSize = 14;
            RingContent.IsEnabled = false;

            Ring = new Ellipse();
            Ring.Height = 40;
            Ring.Width = 40;
            Ring.Fill = Brushes.LightGray;
            Ring.Stroke = Brushes.Goldenrod;
            Ring.StrokeThickness = 5;

            X = _x;
            Y = _y;
        }
    }
}
