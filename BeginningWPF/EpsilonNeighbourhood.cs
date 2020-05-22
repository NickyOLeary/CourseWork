using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SimulationWPF
{
    /// <summary>
    /// Класс, представляющий нарисованную в 
    /// <see cref="Canvas"/> эпсилон-окрестность.
    /// </summary>
    public class EpsilonNeighbourhood
    {
        /// <summary>
        /// Начальная точка ребра, по которому 
        /// передвигается эпсилон-окрестность.
        /// </summary>
        public Point FirstPoint { get; }
        /// <summary>
        /// Промежуточная точка ребра, по которому 
        /// передвигается эпсилон-окрестность.
        /// </summary>
        public Point MiddlePoint { get; }
        /// <summary>
        /// Конечная точка ребра, по которому 
        /// передвигается эпсилон-окрестность.
        /// </summary>
        public Point FinalPoint { get; }
        /// <summary>
        /// i-й индекс ребра, по которому
        /// передвигается окрестность.
        /// </summary>
        public int EdgeIndexI { get; }
        /// <summary>
        /// j-й индекс ребра, по которому
        /// передвигается окрестность.
        /// </summary>
        public int EdgeIndexJ { get; }

        /// <summary>
        /// Передвигающаяся по экрану эпсилон-окрестность.
        /// </summary>
        public Polyline Neighbourhood { get; private set; }
        /// <summary>
        /// Расположение задней точки окрестности.
        /// </summary>
        /// <remarks>
        /// 0, если совпадает с <see cref="FirstPoint"/>;
        /// 1, если  находится между <see cref="FirstPoint"/> 
        /// и <see cref="MiddlePoint"/>;
        /// 2, если совпадает с <see cref="MiddlePoint"/>;
        /// 3, если находится между <see cref="MiddlePoint"/>
        /// и <see cref="FinalPoint"/>;
        /// 4, если совпадает с <see cref="FinalPoint"/>.
        /// </remarks>
        int backPosition = 0;
        /// <summary>
        /// Расположение передней точки окрестности.
        /// </summary>
        /// <remarks>
        /// 0, если совпадает с <see cref="FirstPoint"/>;
        /// 1, если  находится между <see cref="FirstPoint"/> 
        /// и <see cref="MiddlePoint"/>;
        /// 2, если совпадает с <see cref="MiddlePoint"/>;
        /// 3, если находится между <see cref="MiddlePoint"/>
        /// и <see cref="FinalPoint"/>;
        /// 4, если совпадает с <see cref="FinalPoint"/>.
        /// </remarks>
        int frontPosition = 1;

        /// <summary>
        /// Длина ребра на экране.
        /// </summary>
        double drawnEdgeLength;
        /// <summary>
        /// Длина эпсилон-окрестности на экране.
        /// </summary>
        double epsilon;
        /// <summary>
        /// Коэффициент отношения линейных значений 
        /// на экране к номинальным линейным значениям.
        /// </summary>
        double coef;

        /// <summary>
        /// Смещение по оси OX на первой части 
        /// ребра при каждом шаге.
        /// </summary>
        double delta1X;
        /// <summary>
        /// Смещение по оси OY на первой части 
        /// ребра при каждом шаге.
        /// </summary>
        double delta1Y;
        /// <summary>
        /// Смещение по оси OX на второй части 
        /// ребра при каждом шаге.
        /// </summary>
        double delta2X;
        /// <summary>
        /// Смещение по оси OY на второй части 
        /// ребра при каждом шаге.
        /// </summary>
        double delta2Y;

        /// <summary>
        /// Инициализирует экземпляр эпсилон-окрестности 
        /// через параметры.
        /// </summary>
        /// <param name="_firstPoint">
        /// Начальная точка ребра.
        /// </param>
        /// <param name="_middlePoint">
        /// Промежуточная точка ребра.
        /// </param>
        /// <param name="_finalPoint">
        /// Конечная точка ребра.
        /// </param>
        /// <param name="_edgeIndexI">
        /// i-индекс ребра, на котором находися 
        /// эпсилон-окрестность. 
        /// </param>
        /// <param name="_edgeIndexJ">
        /// j-индекс ребра, на котором находится 
        /// эпсилон-окрестность. 
        /// </param>
        /// <param name="_edgeWeight">
        /// Номинальный вес ребра.
        /// </param>
        /// <param name="_startPosition">
        /// Номинальная длина, на которую изначально 
        /// выступает эпсилон-окрестность. 
        /// </param>
        /// <param name="_epsilon">
        /// Номинальная длина эпсилон-окрестности. 
        /// </param>
        /// <param name="_step">
        /// Номинальная длина шага. 
        /// </param>
        public EpsilonNeighbourhood(Point _firstPoint,
            Point _middlePoint, Point _finalPoint,
            int _edgeIndexI, int _edgeIndexJ,
            double _edgeWeight, double _startPosition,
            double _epsilon, double _step)
        {
            //Запоминаем необходимые величины.
            FirstPoint = _firstPoint;
            MiddlePoint = _middlePoint;
            FinalPoint = _finalPoint;
            EdgeIndexI = _edgeIndexI;
            EdgeIndexJ = _edgeIndexJ;

            //Высчитываем длину ребра на экране.
            drawnEdgeLength = 2 * Math.Sqrt(
                (MiddlePoint.X - FirstPoint.X) *
                (MiddlePoint.X - FirstPoint.X) +
                (MiddlePoint.Y - FirstPoint.Y) *
                (MiddlePoint.Y - FirstPoint.Y));
            //Получаем с её помощью коэффициент.
            coef = drawnEdgeLength / _edgeWeight;
            //Получаем длину эпсилон окрестности на экране.
            epsilon = _epsilon * coef;

            //Высчитываем переменные смещения по двум частям ребра.
            delta1X = 2 * (MiddlePoint.X - FirstPoint.X) * _step / _edgeWeight;
            delta1Y = 2 * (MiddlePoint.Y - FirstPoint.Y) * _step / _edgeWeight;
            delta2X = 2 * (FinalPoint.X - MiddlePoint.X) * _step / _edgeWeight;
            delta2Y = 2 * (FinalPoint.Y - MiddlePoint.Y) * _step / _edgeWeight;

            //Высчитываем длину на экране начального положения 
            //эпсилон-окрестности.
            double startPosition = _startPosition * coef;
            //Получаем начальную точку для эпсилон-окрестности 
            //на экране.
            Point startPoint = new Point(
                2 * ((drawnEdgeLength / 2 - startPosition) * FirstPoint.X +
                startPosition * MiddlePoint.X) / drawnEdgeLength,
                2 * ((drawnEdgeLength / 2 - startPosition) * FirstPoint.Y +
                startPosition * MiddlePoint.Y) / drawnEdgeLength);
            //Иницилизируем передвигающуюся по экрану 
            //эпсилон-окрестность.
            Neighbourhood = new Polyline();
            Neighbourhood.Stroke = Brushes.Black;
            Neighbourhood.StrokeThickness = 5;
            Neighbourhood.Points = new PointCollection(
                new Point[] { FirstPoint, startPoint, startPoint });
            if (startPosition == 0)
                frontPosition = 0;
        }

        /// <summary>
        /// Метод, пересчитывающий положение эпсилон-окрестности 
        /// при смещении на шаг.
        /// </summary>
        /// <returns>
        /// Неотрицательное значение, если эпсилон-окрестность 
        /// впервые пересекла <see cref="FinalPoint"/>.
        /// -1, если эпсилон-окрестность полностью пересекла 
        /// <see cref="FinalPoint"/>.
        /// -2 в противном случае.
        /// </returns>
        public double MoveNeighbourhood()
        {
            //Создаём точки для дальнейшего перемещения.
            Point nextFrontP = new Point(), nextBackP = new Point();
            if (frontPosition == 0 || frontPosition == 1)
            {
                //Смещаем.
                nextFrontP.X = Neighbourhood.Points[2].X + delta1X;
                nextFrontP.Y = Neighbourhood.Points[2].Y + delta1Y;
                //Если переступил через MiddlePoint.
                if ((MiddlePoint.X - Neighbourhood.Points[2].X) *
                    (MiddlePoint.X - nextFrontP.X) < 0)
                {
                    frontPosition = 3;
                    //Отрезок, на который выступает.
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
                    //Высчитываем заднюю точку.
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
                    //Средняя точка в данной ветке всегда MiddlePoint.
                    Neighbourhood.Points[1] = MiddlePoint;
                }
                //Попадает в среднюю точку.
                else if ((MiddlePoint.X - Neighbourhood.Points[2].X) *
                  (MiddlePoint.X - nextFrontP.X) == 0)
                {
                    frontPosition = 2;
                    nextFrontP = MiddlePoint;
                    //Средняя совпадает с передней.
                    Neighbourhood.Points[1] = MiddlePoint;
                    //Вычисляем заднюю.
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
                }
                //Если первая точка не переступила.
                else
                {
                    frontPosition = 1;
                    double helper = Math.Sqrt(
                        (nextFrontP.X - FirstPoint.X) *
                        (nextFrontP.X - FirstPoint.X) +
                        (nextFrontP.Y - FirstPoint.Y) *
                        (nextFrontP.Y - FirstPoint.Y));
                    //Вычисляем заднюю точку.
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
                    //Средняя равна передней.
                    Neighbourhood.Points[1] = nextFrontP;
                }
                Neighbourhood.Points[0] = nextBackP;
                Neighbourhood.Points[2] = nextFrontP;
                return -2;

            }
            else if (frontPosition == 2 || frontPosition == 3)
            {
                //Смещаем переднюю.
                nextFrontP.X = Neighbourhood.Points[2].X + delta2X;
                nextFrontP.Y = Neighbourhood.Points[2].Y + delta2Y;
                //Заранее вычисляем среднюю.
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
                //Вычисляем расстояние до средней точки.
                double helper = Math.Sqrt(
                    (nextFrontP.X - MiddlePoint.X) *
                    (nextFrontP.X - MiddlePoint.X) +
                    (nextFrontP.Y - MiddlePoint.Y) *
                    (nextFrontP.Y - MiddlePoint.Y));
                //Если эпсилон больше, чем расстояние до первой точки.
                if (helper + drawnEdgeLength / 2 <= epsilon)
                {
                    backPosition = 0;
                    nextBackP = FirstPoint;
                    Neighbourhood.Points[1] = MiddlePoint;
                }
                //Если эсилон меньше, чем расстояние до средней точки.
                else if (helper > epsilon)
                {
                    backPosition = 3;
                    nextBackP.X = (epsilon * MiddlePoint.X +
                        (helper - epsilon) * nextFrontP.X) / helper;
                    nextBackP.Y = (epsilon * MiddlePoint.Y +
                        (helper - epsilon) * nextFrontP.Y) / helper;
                }
                //Если эпсилон равно расстоянию до средней точки.
                else if (helper == epsilon)
                {
                    backPosition = 2;
                    nextBackP = MiddlePoint;
                }
                //Задняя точка лежит на первой половине ребра.
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
                    //Окрестность в первый раз заступила.
                    Neighbourhood.Points[2] = FinalPoint;
                    return Math.Sqrt(
                        (nextFrontP.X - FinalPoint.X) *
                        (nextFrontP.X - FinalPoint.X) +
                        (nextFrontP.Y - FinalPoint.Y) *
                        (nextFrontP.Y - FinalPoint.Y)) / coef;
                }
                else
                {
                    //Не заступила.
                    Neighbourhood.Points[2] = nextFrontP;
                    return -2;
                }
            }
            //Если передня точка уже за FinalPoint (или на ней).
            else
            {
                if (backPosition == 1)
                {
                    nextBackP.X = Neighbourhood.Points[0].X + delta1X;
                    nextBackP.Y = Neighbourhood.Points[0].Y + delta1Y;
                    //Переносим заднюю точку.
                    if ((MiddlePoint.X - Neighbourhood.Points[0].X) *
                        (MiddlePoint.X - nextBackP.X) < 0)
                    {
                        backPosition = 3;
                        //Расстояние, на которое выступает задняя 
                        //точка окрестности за MiddlePoint.
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
                    }//Задняя точка стала MiddlePoint.
                    else if ((MiddlePoint.X - Neighbourhood.Points[0].X) *
                        (MiddlePoint.X - nextBackP.X) == 0)
                    {
                        backPosition = 2;
                        nextBackP = MiddlePoint;
                        Neighbourhood.Points[1] = FinalPoint;
                    }
                    //Задняя точка не перешли через MiddlePoint.
                    else
                    {
                        backPosition = 1;
                        Neighbourhood.Points[1] = MiddlePoint;
                    }
                }
                //Задняя точка уже на второй половине.
                else
                {
                    nextBackP.X = Neighbourhood.Points[0].X + delta2X;
                    nextBackP.Y = Neighbourhood.Points[0].Y + delta2Y;
                    Neighbourhood.Points[1] = FinalPoint;
                    //Переступила через FinalPoint.
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
