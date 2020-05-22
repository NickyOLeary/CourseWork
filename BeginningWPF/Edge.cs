using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SimulationWPF
{
    /// <summary>
    /// Класс, представляющий нарисованное в 
    /// <see cref="Canvas"/> ребро ориентированного 
    /// метрического графа.
    /// </summary>
    public class Edge
    {
        #region Точки ребра.
        /// <summary>
        /// Поле для свойства <see cref="First"/>.
        /// </summary>
        Point first;
        /// <summary>
        /// Точка центра начальной вершины графа.
        /// </summary>
        /// <remarks>
        /// Поле для данного свойства - <see cref="first"/>.
        /// </remarks>
        public Point First
        {
            get { return first; }
            set
            {
                //Запоминаем новые координаты центра начальной вершины.
                first = value;
                //Пересчитываем смещение для промеежуточной точки.
                CalculateDelta();
                //Смещаем центральную точку по новым переменным смещения 
                //и по специальному числу Number.
                if (Number % 2 == 0)
                    Middle = new Point(
                        (first.X + final.X) / 2 + deltaX * (Number / 2),
                        (first.Y + final.Y) / 2 + deltaY * (Number / 2));
                else
                    Middle = new Point(
                        (first.X + final.X) / 2 - deltaX * (Number / 2),
                        (first.Y + final.Y) / 2 - deltaY * (Number / 2));
                //Переопределяем точки у DrawnEdge.
                DrawnEdge.Points[0] = first;
                DrawnEdge.Points[1] = Middle;
                //Переопределяем стрелку направления.
                CreateArrow();
                //Переопределяем координаты для TextBox веса 
                //ребра Weight.
                Canvas.SetLeft(Weight, Middle.X);
                Canvas.SetTop(Weight, Middle.Y);
            }
        }
        /// <summary>
        /// Промежуточная точка ребра (определяется автоматически 
        /// по заданным начальной и конечной точке).
        /// </summary>
        public Point Middle { get; private set; }
        /// <summary>
        /// Поле для свойства <see cref="Final"/>.
        /// </summary>
        Point final;
        /// <summary>
        /// Точка центра начальной вершины графа.
        /// </summary>
        /// <remarks>
        /// Поле для данного свойства - <see cref="first"/>.
        /// </remarks>
        public Point Final
        {
            get { return final; }
            set
            {
                //Запоминаем новые координаты центра конечной вершины.
                final = value;
                //Пересчитываем смещение для промежуточной точки.
                CalculateDelta();
                //Смещаем центральную точку по новым переменным смещения 
                //и по специальному числу Number.
                if (Number % 2 == 0)
                    Middle = new Point(
                        (first.X + final.X) / 2 + deltaX * (Number / 2),
                        (first.Y + final.Y) / 2 + deltaY * (Number / 2));
                else
                    Middle = new Point(
                        (first.X + final.X) / 2 - deltaX * (Number / 2),
                        (first.Y + final.Y) / 2 - deltaY * (Number / 2));
                //Переопределяем точки у DrawnEdge.
                DrawnEdge.Points[1] = Middle;
                DrawnEdge.Points[2] = final;
                //Переопределяем стрелку направления.
                CreateArrow();
                //Переопределяем координаты для TextBox веса 
                //ребра Weight.
                Canvas.SetLeft(Weight, Middle.X);
                Canvas.SetTop(Weight, Middle.Y);
            }
        }
        #endregion

        /// <summary>
        /// Переменная смещения промежуточной точки по оси OX 
        /// относительно середины отрезка между 
        /// <see cref="First"/> и <see cref="Final"/>.
        /// </summary>
        double deltaX;
        /// <summary>
        /// Переменная смещения промежуточной точки по оси OY 
        /// относительно середины отрезка между 
        /// <see cref="First"/> и <see cref="Final"/>.
        /// </summary>
        double deltaY;
        /// <summary>
        /// Специальное число, задающее расположение 
        /// промежуточной точки.
        /// </summary>
        public int Number { get; }

        /// <summary>
        /// Ребро графа (элемент для <see cref="Canvas"/>).
        /// </summary>
        public Polyline DrawnEdge { get; }
        /// <summary>
        /// Вес ребра (элемент для <see cref="Canvas"/>).
        /// </summary>
        public TextBox Weight { get; set; }
        /// <summary>
        /// Стрелка направления ребра (элемент для <see cref="Canvas"/>).
        /// </summary>
        public Polygon Arrow { get; }

        /// <summary>
        /// Создаёт ребро графа по начальной точке <paramref name="_first"/>, 
        /// конечной точке <paramref name="_last"/> специальному числу 
        /// <paramref name="_number"/>.
        /// </summary>
        /// <param name="_first">
        /// Точка центра начальной вершины графа.
        /// </param>
        /// <param name="_last">
        /// Точка центра конечной вершины графа. 
        /// </param>
        /// <param name="_number">
        /// Специальное число, задающее расположение промежуточной точки.
        /// </param>
        public Edge(Point _first, Point _last, int _number)
        {
            //Запоминаем переданные данные.
            first = _first;
            final = _last;
            Number = _number;

            //Вычисляем переменные смещения.
            CalculateDelta();
            //Вычислем координаты промежуточной точки.
            if (Number % 2 == 0)
                Middle = new Point(
                    (First.X + Final.X) / 2 + deltaX * (Number / 2),
                    (First.Y + Final.Y) / 2 + deltaY * (Number / 2));
            else
                Middle = new Point(
                    (first.X + final.X) / 2 - deltaX * (Number / 2),
                    (first.Y + final.Y) / 2 - deltaY * (Number / 2));

            //Создаём ребро графа.
            DrawnEdge = new Polyline();
            DrawnEdge.Points = new PointCollection(new Point[] { First, Middle, Final });
            DrawnEdge.Stroke = Brushes.DarkGoldenrod;
            DrawnEdge.StrokeThickness = 1;

            //Создаём стрелку направления ребра.
            Arrow = new Polygon();
            Arrow.Fill = Brushes.DarkGoldenrod;
            CreateArrow();

            //Создаём TextBox для веса ребра.
            Weight = new TextBox();
            Weight.Background = Brushes.White;
            Weight.BorderBrush = Brushes.LightGray;
            Weight.FontSize = 14;
            Canvas.SetLeft(Weight, Middle.X);
            Canvas.SetTop(Weight, Middle.Y);
            Weight.Text = "0";
        }

        /// <summary>
        /// Вычисляет переменные смещения для промежуточной точки.
        /// </summary>
        void CalculateDelta()
        {
            //Делим отрезок First-Final в отношении 9:10.
            Point p1 = new Point(
                (10 * First.X + 9 * Final.X) / 19,
                (10 * First.Y + 9 * Final.Y) / 19);
            //Делим отрезок First-Final в отношении 10:9.
            Point p2 = new Point(
                (9 * First.X + 10 * Final.X) / 19,
                (9 * First.Y + 10 * Final.Y) / 19);
            //Поворачиваем вектор p1-p2 на (PI/3) 
            //против часовой стрелки.
            Point p3 = new Point(
                p1.X + (p2.X - p1.X) * Math.Cos(Math.PI / 3) -
                (p2.Y - p1.Y) * Math.Sin(Math.PI / 3),
                p1.Y + (p2.X - p1.X) * Math.Sin(Math.PI / 3) +
                (p2.Y - p1.Y) * Math.Cos(Math.PI / 3));
            //Вычисляем переменные смещения как разность 
            //между координатой полученной p3 и серединой
            //отрезка First-Final.
            deltaX = p3.X - (First.X + Final.X) / 2;
            deltaY = p3.Y - (First.Y + Final.Y) / 2;
            //deltaX всегда должна быть положительной для 
            //корректного вычисления промежуточной точки.
            if (deltaX < 0)
            {
                deltaX = -deltaX;
                deltaY = -deltaY;
            }
        }

        /// <summary>
        /// Создаёт стрелку направления для ребра.
        /// </summary>
        void CreateArrow()
        {
            //Делим отрезок Middle-Final в отношении 4:5.
            Point p1 = new Point(
                (5 * Middle.X + 4 * Final.X) / 9,
                (5 * Middle.Y + 4 * Final.Y) / 9);
            //Делим отрезок Middle-Final в отношении 5:4.
            Point p2 = new Point(
                (4 * Middle.X + 5 * Final.X) / 9,
                (4 * Middle.Y + 5 * Final.Y) / 9);
            //Поворачиваем вектор p2-p1 на (PI/12) 
            //по часовой стрелке.
            Point p3 = new Point(
                p2.X + (p1.X - p2.X) * Math.Cos(Math.PI / 12) -
                (p1.Y - p2.Y) * Math.Sin(Math.PI / 12),
                p2.Y + (p1.X - p2.X) * Math.Sin(Math.PI / 12) +
                (p1.Y - p2.Y) * Math.Cos(Math.PI / 12));
            //Поворачиваем вектор p2-p1 на (PI/12) 
            //против часовой стрелки.
            Point p4 = new Point(
                p2.X + (p1.X - p2.X) * Math.Cos(-Math.PI / 12) -
                (p1.Y - p2.Y) * Math.Sin(-Math.PI / 12),
                p2.Y + (p1.X - p2.X) * Math.Sin(-Math.PI / 12) +
                (p1.Y - p2.Y) * Math.Cos(-Math.PI / 12));
            //Рисуем треугольник.
            Arrow.Points = new PointCollection(new Point[] { p3, p4, p2 });
        }
    }
}
