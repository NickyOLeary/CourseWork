using System;
using GraphLib;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace SimulationWPF
{
    /// <summary>
    /// Логика взаимодействия для DrawnGraph.xaml
    /// </summary>
    public partial class DrawnGraph : Window
    {
        /// <summary>
        /// Количество вершин в графе.
        /// </summary>
        int amOfVertices = 0;
        /// <summary>
        /// Количество рёбер в графе.
        /// </summary>
        int amOfEdges = 0;
        /// <summary>
        /// Метрический ориентированный граф.
        /// </summary>
        List<Pair<int, double>>[] graph;
        /// <summary>
        /// Массив графических вершин.
        /// </summary>
        Vertex[] drawnVertices;
        /// <summary>
        /// Массив списков графических рёбер.
        /// </summary>
        List<Edge>[] drawnEdges;
        /// <summary>
        /// Индекс вершины для запоминания 
        /// (перемещение вершин или рисование рёбер).
        /// </summary>
        int index = -1;
        /// <summary>
        /// X-координата центра захваченной вершины.
        /// </summary>
        int startX = -1;
        /// <summary>
        /// Y-координата центра захваченной вершины.
        /// </summary>
        int startY = -1;

        /// <summary>
        /// Инициализирует массивы пустыми 
        /// элементами.
        /// </summary>
        public DrawnGraph()
        {
            InitializeComponent();
            PlaceToDraw.IsEnabled = true;
            DrawVertices.IsEnabled = false;
            graph = new List<Pair<int, double>>[0];
            drawnVertices = new Vertex[0];
            drawnEdges = new List<Edge>[0];
        }
        /// <summary>
        /// Инициализирует <see cref="graph"/> подготовленным 
        /// заранее графом <paramref name="_graph"/>.
        /// </summary>
        /// <param name="_graph">
        /// Начальный вид графа.
        /// </param>
        public DrawnGraph(List<Pair<int, double>>[] _graph)
        {
            InitializeComponent();
            Random rnd = new Random();
            PlaceToDraw.IsEnabled = true;
            DrawVertices.IsEnabled = false;

            graph = _graph;
            amOfVertices = graph.Length;

            //Инициализируем массив нарисованных вершин.
            drawnVertices = new Vertex[amOfVertices];
            for (int i = 0; i < amOfVertices; i++)
            {
                //Располагаем их случайным образом на PlaceToDraw.
                drawnVertices[i] = new Vertex(
                    rnd.Next(25, 500),
                    rnd.Next(25, 500),
                    i + 1);
                PlaceToDraw.Children.Add(drawnVertices[i].Ring);
                PlaceToDraw.Children.Add(drawnVertices[i].RingContent);
            }

            //Инициализируем массив списков рисуемых рёбер.
            drawnEdges = new List<Edge>[amOfVertices];
            //Массив количества рёбер между парой вершин 
            //(симметричен относительно диагонали).
            int[,] amounts = new int[amOfVertices, amOfVertices];
            for (int i = 0; i < amOfVertices; i++)
            {
                drawnEdges[i] = new List<Edge>();
                for (int j = 0; j < graph[i].Count; j++, amOfEdges++)
                {
                    //Между i-й и (graph[i][j].First-1)-й вершинами 
                    //появляется +1 ребро.
                    amounts[i, graph[i][j].First - 1]++;
                    amounts[graph[i][j].First - 1, i]++;
                    drawnEdges[i].Add(new Edge(
                        new Point(drawnVertices[i].X, drawnVertices[i].Y),
                        new Point(drawnVertices[graph[i][j].First - 1].X,
                        drawnVertices[graph[i][j].First - 1].Y),
                        amounts[i, graph[i][j].First - 1]));
                    drawnEdges[i][j].Weight.Text = graph[i][j].Second.ToString();
                    PlaceToDraw.Children.Add(drawnEdges[i][j].DrawnEdge);
                    PlaceToDraw.Children.Add(drawnEdges[i][j].Arrow);
                    PlaceToDraw.Children.Add(drawnEdges[i][j].Weight);
                }
            }
        }

        #region Нажатия на кнопки
        /// <summary>
        /// Кнопка выхода из программы.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitProgram_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Кнопка, при нажатии которой взаимодействие с <see cref="PlaceToDraw"/> 
        /// переходит на режим отмечания (и перемещения) вершин.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DrawVertices_Click(object sender, RoutedEventArgs e)
        {
            DrawEdges.IsEnabled = true;
            DrawVertices.IsEnabled = false;
        }

        /// <summary>
        /// Кнопка, при нажатии которой взаимодействие с <see cref="PlaceToDraw"/>
        /// переходит на режим отмечания рёбер.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DrawEdges_Click(object sender, RoutedEventArgs e)
        {
            DrawVertices.IsEnabled = true;
            DrawEdges.IsEnabled = false;
        }

        /// <summary>
        /// Кнопка, при нажатии которой производится попытка получения графа, 
        /// сохранения его в файл и запуска формы моделирования.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FindSaturationTime_Click(object sender, RoutedEventArgs e)
        {
            if (amOfEdges == 0)
            {
                MessageBox.Show("Граф не содержит ни одного ребра. ");
                return;
            }
            //Преобразуем значения TextBox-ов у рисуемых рёбер 
            //к дествительным числам в диапазоне [1; 100].
            for (int i = 0; i < drawnEdges.Length; i++)
                for (int j = 0; j < drawnEdges[i].Count; j++)
                {
                    double edgeWeight;
                    if (!double.TryParse(drawnEdges[i][j].Weight.Text,
                        out edgeWeight))
                    {
                        MessageBox.Show($"Одно из рёбер из вершины {i + 1} " +
                            $"в вершину {graph[i][j].First} невозможно " +
                            $"преобразовать к действительному числу. ");
                        return;
                    }
                    if (edgeWeight < 1 || edgeWeight > 100)
                    {
                        MessageBox.Show($"Одно из рёбер из вершины {i + 1} " +
                            $"в вершину {graph[i][j].First} имеет " +
                            $"некорректный вес (вес должен находиться " +
                            $"в диапазоне [1; 100]). ");
                        return;
                    }
                    graph[i][j].Second = edgeWeight;
                }
            //Получаем значение для epsilon.
            double epsilon;
            if (!double.TryParse(EpsilonTextBox.Text, out epsilon))
            {
                MessageBox.Show("Невозможно преобразовать " +
                    "текстовое поле для эпсилон-окрестности в " +
                    "действительное число. ");
                return;
            }
            if (epsilon <= 0 || epsilon >= 0.5)
            {
                MessageBox.Show("Эпсилон-окрестность должна " +
                    "находиться в диапазоне (0; 0.5). ");
                return;
            }
            //Получаем номер стартовой вершины.
            int startVertex;
            if (!int.TryParse(StartVertexTextBox.Text, out startVertex))
            {
                MessageBox.Show("Невозможно преобразовать " +
                    "текстовое поле для стартовой вершины в " +
                    "целое число. ");
                return;
            }
            if (startVertex < 1 || startVertex > graph.Length)
            {
                MessageBox.Show("Номер начальной вершины должен " +
                    "быть от 1 до количества вершиин включительно. ");
                return;
            }
            //Проверяем, является ли граф сильно-связным.
            if (MetricOrientedGraph.FindAmountOfStronglyConnectedComponents(graph) > 1)
            {
                MessageBox.Show("Граф не является сильно-связным. ");
                return;
            }
            //Пытаемся сохранить граф в файл.
            try
            {
                MetricOrientedGraph.SaveGraphToFile(FilePathTextBox.Text, graph);
            }
            catch (Exception ex)
            {
                MessageBox.Show("При попытке записать граф в файл \"" +
                    FilePathTextBox.Text + "\" возникло искючение типа " +
                    ex.GetType() + ": " + Environment.NewLine + "\"" +
                    ex.Message + "\". ");
                return;
            }
            //Удаляем из PlaceToDraw все рисуемые рёбра и вершины 
            //(высвобождаем их для другого Canvas).
            for (int i = 0; i < drawnVertices.Length; i++)
            {
                PlaceToDraw.Children.Remove(drawnVertices[i].Ring);
                PlaceToDraw.Children.Remove(drawnVertices[i].RingContent);
            }
            for (int i = 0; i < drawnEdges.Length; i++)
                for (int j = 0; j < drawnEdges[i].Count; j++)
                {
                    PlaceToDraw.Children.Remove(drawnEdges[i][j].Arrow);
                    PlaceToDraw.Children.Remove(drawnEdges[i][j].Weight);
                    PlaceToDraw.Children.Remove(drawnEdges[i][j].DrawnEdge);
                }
            //Запускаем симуляцию.
            (new Simulation(graph, drawnVertices, drawnEdges, epsilon,
                startVertex)).Show();
            //Закрываем форму.
            Close();
        }
        #endregion

        #region События мыши. 
        /// <summary>
        /// События опускания левой кнопки мыши.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlaceToDraw_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //Если активирована кнопка DrawEdges, то пользователь 
            //отмечает (перемещает) вершины.
            if (DrawEdges.IsEnabled)
            {
                //В цикле проверяем, не попал ли пользователь при нажатии на
                //одну из вершин. Если попал, запоминаем индекс и координаты 
                //центра вершины для перемещения.
                for (int i = 0; i < amOfVertices; i++)
                    if ((e.GetPosition(this).X - drawnVertices[i].X) *
                        (e.GetPosition(this).X - drawnVertices[i].X) +
                        (e.GetPosition(this).Y - drawnVertices[i].Y) *
                        (e.GetPosition(this).Y - drawnVertices[i].Y) <=
                        25 * 25)
                    {
                        index = i;
                        startX = drawnVertices[i].X;
                        startY = drawnVertices[i].Y;
                        break;
                    }
                //Проверяем, запомнили ли вершинку.
                if (index != -1)
                {
                    //Отмечаем выбранную вершинку особым цветом.
                    drawnVertices[index].Ring.Stroke = Brushes.Cyan;
                    //Захватываем курсор мыши для отслеживания её координат.
                    Mouse.Capture(PlaceToDraw);
                }
                else
                {
                    /*Так как не попали ни в какую вершину, отмечаем новую 
                     * на некоторых условиях: должна быть не ближе определённого 
                     * расстояния от других вершин и не должна заходить за рамки 
                     * PlaceToDraw.
                     */
                    for (int i = 0; i < amOfVertices; i++)
                        if ((e.GetPosition(this).X - drawnVertices[i].X) *
                        (e.GetPosition(this).X - drawnVertices[i].X) +
                        (e.GetPosition(this).Y - drawnVertices[i].Y) *
                        (e.GetPosition(this).Y - drawnVertices[i].Y) <=
                        60 * 60)
                            return;
                    if (e.GetPosition(this).X < 25 ||
                        e.GetPosition(this).X > PlaceToDraw.ActualWidth - 25 ||
                        e.GetPosition(this).Y < 25 ||
                        e.GetPosition(this).Y > PlaceToDraw.ActualHeight)
                        return;
                    //Добавляем новую вершину в graph.
                    Array.Resize(ref graph, amOfVertices + 1);
                    //Инициализируем список рёбер из новой вершины.
                    graph[amOfVertices] = new List<Pair<int, double>>();
                    //Добавляем новую рисуемую вершину в drawnVertices.
                    Array.Resize(ref drawnVertices, amOfVertices + 1);
                    //Координаты берём по курсору, номер по количеству вершин.
                    drawnVertices[amOfVertices] =
                        new Vertex((int)e.GetPosition(this).X,
                        (int)e.GetPosition(this).Y,
                        drawnVertices.Length);
                    //Добавляем новую вершину в drawnEdges.
                    Array.Resize(ref drawnEdges, amOfVertices + 1);
                    //Инициализируем список рисуемых рёбер из новой вершины.
                    drawnEdges[amOfVertices] = new List<Edge>();
                    //Добавляем вершину в PlaceToDraw.
                    PlaceToDraw.Children.Add(drawnVertices[amOfVertices].Ring);
                    PlaceToDraw.Children.Add(drawnVertices[amOfVertices].RingContent);
                    //Увеличиваем количество вершин.
                    amOfVertices++;
                }
            }
            //Кнопка DrawEdges неактивна, значит, добавляем рёбра.
            else
            {
                //Проверяем, была ли запомнена какая-либо вершина.
                if (index == -1)
                {
                    //Если нет, то проходимся по вершинам и запоминаем 
                    //какую-то, если в неё попали. Вершину помечаем 
                    //отличающимся цветом.
                    for (int i = 0; i < amOfVertices; i++)
                        if ((e.GetPosition(this).X - drawnVertices[i].X) *
                            (e.GetPosition(this).X - drawnVertices[i].X) +
                            (e.GetPosition(this).Y - drawnVertices[i].Y) *
                            (e.GetPosition(this).Y - drawnVertices[i].Y) <=
                            25 * 25)
                        {
                            index = i;
                            drawnVertices[index].Ring.Stroke = Brushes.Blue;
                            break;
                        }
                }
                else
                {
                    //Если да, то опять проходимся по вершинамю
                    for (int i = 0; i < amOfVertices; i++)
                        if ((e.GetPosition(this).X - drawnVertices[i].X) *
                            (e.GetPosition(this).X - drawnVertices[i].X) +
                            (e.GetPosition(this).Y - drawnVertices[i].Y) *
                            (e.GetPosition(this).Y - drawnVertices[i].Y) <=
                            25 * 25)
                        {
                            //Если отметили ещё одну, то нужно нарисовать между ними 
                            //ребро (петли не предусмотрены).
                            if (index == i) break;
                            //Добавляем ребро в graph и получаем индекс на котором 
                            //ребро стоит.
                            int curIndex = MetricOrientedGraph.AddEdgeToGraph(graph,
                                index + 1, new Pair<int, double>(i + 1, 0));
                            //Считаем количество рёбер между двумя рассматриваемыми вершинами.
                            int amOfEdgesBetweenVertices = 0;
                            foreach (Pair<int, double> edge in graph[index])
                                if (edge.First == i + 1) amOfEdgesBetweenVertices++;
                            foreach (Pair<int, double> edge in graph[i])
                                if (edge.First == index + 1) amOfEdgesBetweenVertices++;
                            //Вставляем по найдённому индексу новое рисуемое ребро 
                            //(в качестве специального числа используем количество 
                            //уже существущих между данными вершинами рёбер.
                            drawnEdges[index].Insert(curIndex, new Edge(
                                new Point(drawnVertices[index].X, drawnVertices[index].Y),
                                new Point(drawnVertices[i].X, drawnVertices[i].Y),
                                amOfEdgesBetweenVertices));
                            //Добавляем ребро в PlaceToDraw.
                            PlaceToDraw.Children.Add(
                                drawnEdges[index][curIndex].DrawnEdge);
                            PlaceToDraw.Children.Add(
                                drawnEdges[index][curIndex].Arrow);
                            PlaceToDraw.Children.Add(
                                drawnEdges[index][curIndex].Weight);
                            //Увеличиваем количество рёбер на 1 и выходим.
                            amOfEdges++;
                            break;
                        }
                    //Освобождаем индекс и возвращаем вершине прежний цвет.
                    drawnVertices[index].Ring.Stroke = Brushes.Goldenrod;
                    index = -1;
                }
            }
        }

        /// <summary>
        /// Событие перемещения мыши.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlaceToDraw_MouseMove(object sender, MouseEventArgs e)
        {
            //Если курсор не захвачен (то есть не нужно отслеживать его кооринаты), 
            //то выходим.
            if (!PlaceToDraw.IsMouseCaptured) return;
            //В противном случае, мы перемещаем вершину, 
            //то есть меняем координаты её центра в соответствии 
            //с координатами курсора.
            drawnVertices[index].X = (int)e.GetPosition(this).X;
            drawnVertices[index].Y = (int)e.GetPosition(this).Y;
        }

        /// <summary>
        /// Событи поднятия левой кнопки мыши.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlaceToDraw_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //Если мышь не захвачена (то есть не нужно отслеживать её кооринаты), 
            //то выходим.
            if (!PlaceToDraw.IsMouseCaptured) return;
            //В противном случае, мы знаем, что переместили какую-то вершину.
            //Проверяем, не слишком ли близко она находится к другим вершинам 
            //(если слишком, то возвращаем её назад).
            for (int i = 0; i < amOfVertices; i++)
                if (index != i &&
                    (e.GetPosition(this).X - drawnVertices[i].X) *
                    (e.GetPosition(this).X - drawnVertices[i].X) +
                    (e.GetPosition(this).Y - drawnVertices[i].Y) *
                    (e.GetPosition(this).Y - drawnVertices[i].Y) <=
                    60 * 60)
                {
                    drawnVertices[index].X = startX;
                    drawnVertices[index].Y = startY;
                    break;
                }
            //Проверяем, не слишком ли близко она находится к границам PlaceToDraw
            //(если слишком, то возвращаем её назад).
            if (e.GetPosition(this).X < 25 ||
                e.GetPosition(this).Y < 25 ||
                e.GetPosition(this).X > PlaceToDraw.ActualWidth - 25 ||
                e.GetPosition(this).Y > PlaceToDraw.ActualHeight - 25)
            {
                drawnVertices[index].X = startX;
                drawnVertices[index].Y = startY;
            }
            //Присоединяем к ней все рёбра, выходящие из неё.
            for (int i = 0; i < graph[index].Count; i++)
                drawnEdges[index][i].First = new Point(
                    drawnVertices[index].X,
                    drawnVertices[index].Y);
            //Присоединяем к ней все рёбра, входящие в неё.
            for (int i = 0; i < amOfVertices; i++)
            {
                if (i == index) continue;
                for (int j = 0; j < graph[i].Count; j++)
                    if (graph[i][j].First == index + 1)
                        drawnEdges[i][j].Final = new Point(
                            drawnVertices[index].X,
                            drawnVertices[index].Y);
            }
            //Возвращаем вершине прежний цвет.
            drawnVertices[index].Ring.Stroke = Brushes.Goldenrod;
            //Высвобождаем индекс и координаты.
            index = -1;
            startX = -1;
            startY = -1;
            //Высвобождаем курсор.
            Mouse.Capture(null);
        }
        #endregion
    }
}
