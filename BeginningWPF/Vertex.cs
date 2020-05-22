using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SimulationWPF
{
    /// <summary>
    /// Класс, представляющий нарисованную в 
    /// <see cref="Canvas"/> вершину ориентированного 
    /// метрического графа.
    /// </summary>
    public class Vertex
    {
        /// <summary>
        /// Поле для свойства <see cref="X"/>.
        /// </summary>
        int x;
        /// <summary>
        /// X-координата центра вершины.
        /// </summary>
        /// <remarks>
        /// Поле для данного свойства - 
        /// <see cref="x"/>.
        /// </remarks>
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
        /// <summary>
        /// Поле для свойства <see cref="Y"/>.
        /// </summary>
        int y;
        /// <summary>
        /// Y-координата центра вершины.
        /// </summary>
        /// <remarks>
        /// Поле для данного свойства - 
        /// <see cref="y"/>.
        /// </remarks>
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

        /// <summary>
        /// Кольцо, обозначающее границы вершинки (элемент 
        /// для <see cref="Canvas"/>).
        /// </summary>
        public Ellipse Ring { get; private set; }
        /// <summary>
        /// Номер вершины, расположенный внутри кольца 
        /// <see cref="Ring"/> (элемент для <see cref="Canvas"/>).
        /// </summary>
        public TextBox RingContent { get; set; }

        /// <summary>
        /// Инициализируют вершину для графа с координатами центра 
        /// (<paramref name="_x"/>, <paramref name="_y"/>) и с 
        /// номером <paramref name="_ringContent"/>.
        /// </summary>
        /// <param name="_x">
        /// X-координата центра вершины. 
        /// </param>
        /// <param name="_y">
        /// Y-координата центра вершины.
        /// </param>
        /// <param name="_ringContent">
        /// Номер вершины.
        /// </param>
        public Vertex(int _x, int _y, int _ringContent)
        {
            Ring = new Ellipse();
            Ring.Height = 40;
            Ring.Width = 40;
            Ring.Fill = Brushes.LightGray;
            Ring.Stroke = Brushes.Goldenrod;
            Ring.StrokeThickness = 5;

            RingContent = new TextBox();
            RingContent.Text = _ringContent.ToString();
            RingContent.Background = Brushes.LightGray;
            RingContent.BorderBrush = Brushes.LightGray;
            RingContent.FontSize = 14;
            RingContent.IsEnabled = false;

            X = _x;
            Y = _y;
        }
    }
}
