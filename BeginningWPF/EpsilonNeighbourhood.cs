using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SimulationWPF
{
    public class EpsilonNeighbourhood
    {
        public Point FirstPoint { get; }
        public Point MiddlePoint { get; }
        public Point FinalPoint { get; }
        public int EdgeIndexI { get; }
        public int EdgeIndexJ { get; }

        public Polyline Neighbourhood { get; private set; }
        int backPosition = 0, frontPosition = 1;
        double drawnEdgeLength, epsilon, coef;
        double delta1X, delta1Y, delta2X, delta2Y;

        public EpsilonNeighbourhood(Point _firstPoint,
            Point _middlePoint, Point _finalPoint,
            int _edgeIndexI, int _edgeIndexJ,
            double _edgeWeight, double _startPosition,
            double _epsilon, double _step)
        {
            FirstPoint = _firstPoint;
            MiddlePoint = _middlePoint;
            FinalPoint = _finalPoint;
            EdgeIndexI = _edgeIndexI;
            EdgeIndexJ = _edgeIndexJ;

            drawnEdgeLength = 2 * Math.Sqrt(
                (MiddlePoint.X - FirstPoint.X) *
                (MiddlePoint.X - FirstPoint.X) +
                (MiddlePoint.Y - FirstPoint.Y) *
                (MiddlePoint.Y - FirstPoint.Y));
            coef = drawnEdgeLength / _edgeWeight;
            epsilon = _epsilon * coef;

            delta1X = 2 * (MiddlePoint.X - FirstPoint.X) * _step / _edgeWeight;
            delta1Y = 2 * (MiddlePoint.Y - FirstPoint.Y) * _step / _edgeWeight;
            delta2X = 2 * (FinalPoint.X - MiddlePoint.X) * _step / _edgeWeight;
            delta2Y = 2 * (FinalPoint.Y - MiddlePoint.Y) * _step / _edgeWeight;

            double startPosition = _startPosition * coef;
            Point startPoint = new Point(
                2 * ((drawnEdgeLength / 2 - startPosition) * FirstPoint.X +
                startPosition * MiddlePoint.X) / drawnEdgeLength,
                2 * ((drawnEdgeLength / 2 - startPosition) * FirstPoint.Y +
                startPosition * MiddlePoint.Y) / drawnEdgeLength);
            Neighbourhood = new Polyline();
            Neighbourhood.Stroke = Brushes.Black;
            Neighbourhood.StrokeThickness = 5;
            Neighbourhood.Points = new PointCollection(
                new Point[] { FirstPoint, startPoint, startPoint });
            if (startPosition == 0)
                frontPosition = 0;
        }

        public double MoveNeighbourhood()
        {
            Point nextFrontP = new Point(), nextBackP = new Point();
            if (frontPosition == 0 || frontPosition == 1)
            {
                nextFrontP.X = Neighbourhood.Points[2].X + delta1X;
                nextFrontP.Y = Neighbourhood.Points[2].Y + delta1Y;
                if ((MiddlePoint.X - Neighbourhood.Points[2].X) *
                    (MiddlePoint.X - nextFrontP.X) < 0)
                {
                    frontPosition = 3;
                    double part = Math.Sqrt(
                        (MiddlePoint.X - nextFrontP.X) *
                        (MiddlePoint.X - nextFrontP.X) +
                        (MiddlePoint.Y - nextFrontP.Y) *
                        (MiddlePoint.Y - nextFrontP.Y));
                    nextFrontP.X = 2 *
                        ((drawnEdgeLength / 2 - part) * MiddlePoint.X +
                        part * FinalPoint.X) / drawnEdgeLength;
                    nextFrontP.Y = 2 *
                        ((drawnEdgeLength / 2 - part) * MiddlePoint.Y +
                        part * FinalPoint.Y) / drawnEdgeLength;
                    if (drawnEdgeLength / 2 + part > epsilon)
                    {
                        backPosition = 1;
                        nextBackP.X = 2 *
                            (((drawnEdgeLength / 2 + part - epsilon) * MiddlePoint.X +
                            (epsilon - part) * FirstPoint.X) / drawnEdgeLength);
                        nextBackP.Y = 2 *
                            (((drawnEdgeLength / 2 + part - epsilon) * MiddlePoint.Y +
                            (epsilon - part) * FirstPoint.Y) / drawnEdgeLength);
                    }
                    else
                    {
                        backPosition = 0;
                        nextBackP = FirstPoint;
                    }
                    Neighbourhood.Points[1] = MiddlePoint;
                }
                else if ((MiddlePoint.X - Neighbourhood.Points[2].X) *
                  (MiddlePoint.X - nextFrontP.X) == 0)
                {
                    frontPosition = 2;
                    nextFrontP = MiddlePoint;
                    Neighbourhood.Points[1] = MiddlePoint;
                    //КСЮША меняет
                    if (drawnEdgeLength / 2 <= epsilon)
                        nextBackP = FirstPoint;
                    else
                    {
                        nextBackP.X = 2 *
                            ((drawnEdgeLength / 2 - epsilon) * MiddlePoint.X +
                            (epsilon * FirstPoint.X) / drawnEdgeLength);
                        nextBackP.Y = 2 *
                            ((drawnEdgeLength / 2 - epsilon) * MiddlePoint.Y +
                            (epsilon * FirstPoint.Y) / drawnEdgeLength);
                    }
                    //Поменяла
                }
                else
                {
                    frontPosition = 1;
                    double helper = Math.Sqrt(
                        (nextFrontP.X - FirstPoint.X) *
                        (nextFrontP.X - FirstPoint.X) +
                        (nextFrontP.Y - FirstPoint.Y) *
                        (nextFrontP.Y - FirstPoint.Y));
                    if (helper >
                        epsilon)
                    {
                        backPosition = 1;
                        nextBackP.X = (epsilon * FirstPoint.X +
                            (helper - epsilon) * nextFrontP.X) / helper;
                        nextBackP.Y = (epsilon * FirstPoint.Y +
                            (helper - epsilon) * nextFrontP.Y) / helper;
                    }
                    else
                    {
                        backPosition = 0;
                        nextBackP = FirstPoint;
                    }
                    Neighbourhood.Points[1] = nextFrontP;
                }
                Neighbourhood.Points[0] = nextBackP;
                Neighbourhood.Points[2] = nextFrontP;
                return -2;

            }
            else if (frontPosition == 2 || frontPosition == 3)
            {
                nextFrontP.X = Neighbourhood.Points[2].X + delta2X;
                nextFrontP.Y = Neighbourhood.Points[2].Y + delta2Y;
                if ((FinalPoint.X - Neighbourhood.Points[2].X) *
                    (FinalPoint.X - nextFrontP.X) <= 0)
                {
                    frontPosition = 4;
                    Neighbourhood.Points[1] = FinalPoint;
                }
                else
                {
                    frontPosition = 3;
                    Neighbourhood.Points[1] = nextFrontP;
                }
                double helper = Math.Sqrt(
                    (nextFrontP.X - MiddlePoint.X) *
                    (nextFrontP.X - MiddlePoint.X) +
                    (nextFrontP.Y - MiddlePoint.Y) *
                    (nextFrontP.Y - MiddlePoint.Y));
                if (helper + drawnEdgeLength / 2 <= epsilon)
                {
                    backPosition = 0;
                    nextBackP = FirstPoint;
                    Neighbourhood.Points[1] = MiddlePoint;
                }
                else if (helper > epsilon)
                {
                    backPosition = 3;
                    nextBackP.X = (epsilon * MiddlePoint.X +
                        (helper - epsilon) * nextFrontP.X) / helper;
                    nextBackP.Y = (epsilon * MiddlePoint.Y +
                        (helper - epsilon) * nextFrontP.Y) / helper;
                }
                else if (helper == epsilon)
                {
                    backPosition = 2;
                    nextBackP = MiddlePoint;
                }
                else
                {
                    backPosition = 1;
                    nextBackP.X = 2 *
                        ((drawnEdgeLength / 2 + helper - epsilon) * MiddlePoint.X +
                        (epsilon - helper) * FirstPoint.X) / drawnEdgeLength;
                    nextBackP.Y = 2 *
                        ((drawnEdgeLength / 2 + helper - epsilon) * MiddlePoint.Y +
                        (epsilon - helper) * FirstPoint.Y) / drawnEdgeLength;
                    Neighbourhood.Points[1] = MiddlePoint;
                }
                Neighbourhood.Points[0] = nextBackP;
                if ((FinalPoint.X - Neighbourhood.Points[2].X) *
                    (FinalPoint.X - nextFrontP.X) <= 0)
                {
                    Neighbourhood.Points[2] = FinalPoint;
                    return Math.Sqrt(
                        (nextFrontP.X - FinalPoint.X) *
                        (nextFrontP.X - FinalPoint.X) +
                        (nextFrontP.Y - FinalPoint.Y) *
                        (nextFrontP.Y - FinalPoint.Y)) / coef;
                }
                else
                {
                    Neighbourhood.Points[2] = nextFrontP;
                    return -2;
                }
            }
            else
            {
                if (backPosition == 1)
                {
                    nextBackP.X = Neighbourhood.Points[0].X + delta1X;
                    nextBackP.Y = Neighbourhood.Points[0].Y + delta1Y;
                    if ((MiddlePoint.X - Neighbourhood.Points[0].X) *
                        (MiddlePoint.X - nextBackP.X) < 0)
                    {
                        backPosition = 3;
                        double part = Math.Sqrt(
                            (MiddlePoint.X - nextBackP.X) *
                            (MiddlePoint.X - nextBackP.X) +
                            (MiddlePoint.Y - nextBackP.Y) *
                            (MiddlePoint.Y - nextBackP.Y));
                        nextBackP.X = 2 *
                            ((drawnEdgeLength / 2 - part) * MiddlePoint.X +
                            part * FinalPoint.X) / drawnEdgeLength;
                        nextBackP.Y = 2 *
                            ((drawnEdgeLength / 2 - part) * MiddlePoint.Y +
                            part * FinalPoint.Y) / drawnEdgeLength;
                        Neighbourhood.Points[1] = FinalPoint;
                    }
                    else if ((MiddlePoint.X - Neighbourhood.Points[0].X) *
                        (MiddlePoint.X - nextBackP.X) == 0)
                    {
                        backPosition = 2;
                        nextBackP = MiddlePoint;
                        Neighbourhood.Points[1] = FinalPoint;
                    }
                    else
                    {
                        backPosition = 1;
                        Neighbourhood.Points[1] = MiddlePoint;
                    }
                }
                else
                {
                    nextBackP.X = Neighbourhood.Points[0].X + delta2X;
                    nextBackP.Y = Neighbourhood.Points[0].Y + delta2Y;
                    Neighbourhood.Points[1] = FinalPoint;
                    if ((FinalPoint.X - Neighbourhood.Points[0].X) *
                        (FinalPoint.X - nextBackP.X) <= 0)
                    {
                        backPosition = 4;
                        Neighbourhood.Points[1] = FinalPoint;
                        return -1;
                    }
                    else
                        backPosition = 3;
                }
                Neighbourhood.Points[0] = nextBackP;
                Neighbourhood.Points[2] = FinalPoint;
                return -2;
            }
        }//public double MoveNeighbourhood()
    }
}
