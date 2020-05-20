using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SimulationWPF
{
    public class Edge
    {
        double deltaX, deltaY;
        public int Number { get; private set; }
        Point first;
        public Point First
        {
            get { return first; }
            set
            {
                first = value;
                CalculateDelta();
                DrawnEdge.Points[0] = first;
                if (Number % 2 == 0)
                    Middle = new Point(
                        (first.X + last.X) / 2 + deltaX * (Number / 2),
                        (first.Y + last.Y) / 2 + deltaY * (Number / 2));
                else
                    Middle = new Point(
                        (first.X + last.X) / 2 - deltaX * (Number / 2),
                        (first.Y + last.Y) / 2 - deltaY * (Number / 2));
                DrawnEdge.Points[1] = Middle;
                CreateArrow();
                Canvas.SetLeft(Weight, Middle.X);
                Canvas.SetTop(Weight, Middle.Y);
            }
        }
        public Point Middle { get; private set; }
        Point last;
        public Point Last
        {
            get { return last; }
            set
            {
                last = value;
                CalculateDelta();
                DrawnEdge.Points[2] = last;
                if (Number % 2 == 0)
                    Middle = new Point(
                        (first.X + last.X) / 2 + deltaX * (Number / 2),
                        (first.Y + last.Y) / 2 + deltaY * (Number / 2));
                else
                    Middle = new Point(
                        (first.X + last.X) / 2 - deltaX * (Number / 2),
                        (first.Y + last.Y) / 2 - deltaY * (Number / 2));
                DrawnEdge.Points[1] = Middle;
                CreateArrow();
                Canvas.SetLeft(Weight, Middle.X);
                Canvas.SetTop(Weight, Middle.Y);
            }
        }

        public Polyline DrawnEdge { get; set; }
        public TextBox Weight { get; set; }
        public Polygon Arrow { get; set; }

        public Edge(Point _first, Point _last, int _number)
        {
            first = _first;
            last = _last;
            Number = _number;
            CalculateDelta();
            if (Number % 2 == 0)
                Middle = new Point(
                    (First.X + Last.X) / 2 + deltaX * (Number / 2),
                    (First.Y + Last.Y) / 2 + deltaY * (Number / 2));
            else
                Middle = new Point(
                    (first.X + last.X) / 2 - deltaX * (Number / 2),
                    (first.Y + last.Y) / 2 - deltaY * (Number / 2));

            DrawnEdge = new Polyline();
            DrawnEdge.Points = new PointCollection(new Point[] { First, Middle, Last });
            DrawnEdge.Stroke = Brushes.DarkGoldenrod;
            DrawnEdge.StrokeThickness = 1;

            Weight = new TextBox();
            Weight.Background = Brushes.White;
            Weight.BorderBrush = Brushes.LightGray;
            Weight.FontSize = 14;
            Canvas.SetLeft(Weight, Middle.X);
            Canvas.SetTop(Weight, Middle.Y);
            Weight.Text = "0";

            Arrow = new Polygon();
            Arrow.Fill = Brushes.DarkGoldenrod;
            CreateArrow();
        }

        void CreateArrow()
        {
            Point p1 = new Point(
                (5 * Middle.X + 4 * Last.X) / 9,
                (5 * Middle.Y + 4 * Last.Y) / 9);
            Point p2 = new Point(
                (4 * Middle.X + 5 * Last.X) / 9,
                (4 * Middle.Y + 5 * Last.Y) / 9);
            Point p3 = new Point(
                p2.X + (p1.X - p2.X) * Math.Cos(Math.PI / 12) -
                (p1.Y - p2.Y) * Math.Sin(Math.PI / 12),
                p2.Y + (p1.X - p2.X) * Math.Sin(Math.PI / 12) +
                (p1.Y - p2.Y) * Math.Cos(Math.PI / 12));
            Point p4 = new Point(
                p2.X + (p1.X - p2.X) * Math.Cos(-Math.PI / 12) -
                (p1.Y - p2.Y) * Math.Sin(-Math.PI / 12),
                p2.Y + (p1.X - p2.X) * Math.Sin(-Math.PI / 12) +
                (p1.Y - p2.Y) * Math.Cos(-Math.PI / 12));
            Arrow.Points = new PointCollection(new Point[] { p3, p4, p2 });
        }

        void CalculateDelta()
        {
            Point p1 = new Point(
                (10 * First.X + 9 * Last.X) / 19,
                (10 * First.Y + 9 * Last.Y) / 19);
            Point p2 = new Point(
                (9 * First.X + 10 * Last.X) / 19,
                (9 * First.Y + 10 * Last.Y) / 19);
            Point p3 = new Point(
                p1.X + (p2.X - p1.X) * Math.Cos(Math.PI / 3) -
                (p2.Y - p1.Y) * Math.Sin(Math.PI / 3),
                p1.Y + (p2.X - p1.X) * Math.Sin(Math.PI / 3) +
                (p2.Y - p1.Y) * Math.Cos(Math.PI / 3));
            deltaX = p3.X - (First.X + Last.X) / 2;
            deltaY = p3.Y - (First.Y + Last.Y) / 2;
            if (deltaX < 0)
            {
                deltaX = -deltaX;
                deltaY = -deltaY;
            }
        }
    }
}
