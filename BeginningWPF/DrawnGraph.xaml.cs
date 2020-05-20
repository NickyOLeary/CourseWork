using System;
using GraphLib;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SimulationWPF
{
    /// <summary>
    /// Логика взаимодействия для DrawnGraph.xaml
    /// </summary>
    public partial class DrawnGraph : Window
    {
        int amOfVertices = 0, amOfEdges = 0;
        List<Pair<int, double>>[] graph;
        Vertex[] drawnVertices;
        List<Edge>[] drawnEdges;
        int index = -1;
        int startX = -1, startY = -1;

        public DrawnGraph()
        {
            InitializeComponent();
            PlaceToDraw.IsEnabled = true;
            DrawVertices.IsEnabled = false;
            graph = new List<Pair<int, double>>[0];
            drawnVertices = new Vertex[0];
            drawnEdges = new List<Edge>[0];
        }
        public DrawnGraph(List<Pair<int, double>>[] _graph)
        {
            InitializeComponent();
            Random rnd = new Random();
            PlaceToDraw.IsEnabled = true;
            DrawVertices.IsEnabled = false;
            graph = _graph;
            amOfVertices = graph.Length;
            drawnVertices = new Vertex[amOfVertices];
            for (int i = 0; i < amOfVertices; i++)
            {
                drawnVertices[i] = new Vertex(
                    rnd.Next(25, 500),
                    rnd.Next(25, 500),
                    i + 1);
                PlaceToDraw.Children.Add(drawnVertices[i].Ring);
                PlaceToDraw.Children.Add(drawnVertices[i].RingContent);
            }
            drawnEdges = new List<Edge>[amOfVertices];
            int[,] amounts = new int[amOfVertices, amOfVertices];
            for (int i = 0; i < amOfVertices; i++)
            {
                drawnEdges[i] = new List<Edge>();
                for (int j = 0; j < graph[i].Count; j++, amOfEdges++)
                {
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

        private void ExitProgram_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void DrawVertices_Click(object sender, RoutedEventArgs e)
        {
            DrawEdges.IsEnabled = true;
            DrawVertices.IsEnabled = false;
        }

        private void DrawEdges_Click(object sender, RoutedEventArgs e)
        {
            DrawVertices.IsEnabled = true;
            DrawEdges.IsEnabled = false;
        }


        private void FindSaturationTime_Click(object sender, RoutedEventArgs e)
        {
            if (amOfEdges == 0)
            {
                MessageBox.Show("Граф не содержит ни одного ребра. ");
                return;
            }
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
            double epsilon;
            if (!double.TryParse(EpsilonTextBox.Text, out epsilon))
            {
                MessageBox.Show("Невозможно преобразовать " +
                    "текстовое поле для эпсилон-окрестности в " +
                    "действительное число. ");
                return;
            }
            if (epsilon <= 0 || epsilon >= 1)
            {
                MessageBox.Show("Эпсилон-окрестность должна " +
                    "находиться в диапазоне (0; 1). ");
                return;
            }
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
            if (MetricOrientedGraph.FindAmountOfStronglyConnectedComponents(graph) > 1)
            {
                MessageBox.Show("Граф не является сильно-связным. ");
                return;
            }
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
            (new Simulation(graph, drawnVertices, drawnEdges, epsilon,
                startVertex)).Show();
            Close();
        }


        private void PlaceToDraw_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (DrawEdges.IsEnabled)
            {
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
                if (index != -1)
                {
                    drawnVertices[index].Ring.Stroke = Brushes.Cyan;
                    Mouse.Capture(PlaceToDraw);
                }
                else
                {
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
                    Array.Resize(ref graph, amOfVertices + 1);
                    graph[amOfVertices] = new List<Pair<int, double>>();
                    Array.Resize(ref drawnVertices, amOfVertices + 1);
                    drawnVertices[amOfVertices] =
                        new Vertex((int)e.GetPosition(this).X,
                        (int)e.GetPosition(this).Y,
                        drawnVertices.Length);
                    Array.Resize(ref drawnEdges, amOfVertices + 1);
                    drawnEdges[amOfVertices] = new List<Edge>();
                    PlaceToDraw.Children.Add(drawnVertices[amOfVertices].Ring);
                    PlaceToDraw.Children.Add(drawnVertices[amOfVertices].RingContent);
                    amOfVertices++;
                }
            }
            else
            {
                if (index != -1)
                {
                    for (int i = 0; i < amOfVertices; i++)
                        if ((e.GetPosition(this).X - drawnVertices[i].X) *
                            (e.GetPosition(this).X - drawnVertices[i].X) +
                            (e.GetPosition(this).Y - drawnVertices[i].Y) *
                            (e.GetPosition(this).Y - drawnVertices[i].Y) <=
                            25 * 25)
                        {
                            if (index == i) break;
                            int curIndex = MetricOrientedGraph.AddEdgeToGraph(graph,
                                index + 1, new Pair<int, double>(i + 1, 0));
                            int amOfEdgesBetweenVertices = 0;
                            foreach (Pair<int, double> edge in graph[index])
                                if (edge.First == i + 1) amOfEdgesBetweenVertices++;
                            foreach (Pair<int, double> edge in graph[i])
                                if (edge.First == index + 1) amOfEdgesBetweenVertices++;
                            drawnEdges[index].Insert(curIndex, new Edge(
                                new Point(drawnVertices[index].X, drawnVertices[index].Y),
                                new Point(drawnVertices[i].X, drawnVertices[i].Y),
                                amOfEdgesBetweenVertices));
                            PlaceToDraw.Children.Add(
                                drawnEdges[index][curIndex].DrawnEdge);
                            PlaceToDraw.Children.Add(
                                drawnEdges[index][curIndex].Arrow);
                            PlaceToDraw.Children.Add(
                                drawnEdges[index][curIndex].Weight);
                            amOfEdges++;
                            break;
                        }
                    drawnVertices[index].Ring.Stroke = Brushes.Goldenrod;
                    index = -1;
                }
                else
                {
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
            }
        }

        private void PlaceToDraw_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!PlaceToDraw.IsMouseCaptured) return;
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
            if (e.GetPosition(this).X < 25 ||
                e.GetPosition(this).Y < 25 ||
                e.GetPosition(this).X > PlaceToDraw.ActualWidth - 25 ||
                e.GetPosition(this).Y > PlaceToDraw.ActualHeight - 25)
            {
                drawnVertices[index].X = startX;
                drawnVertices[index].Y = startY;
            }
            drawnVertices[index].Ring.Stroke = Brushes.Goldenrod;
            for (int i = 0; i < graph[index].Count; i++)
                drawnEdges[index][i].First = new Point(
                    drawnVertices[index].X,
                    drawnVertices[index].Y);
            for (int i = 0; i < amOfVertices; i++)
            {
                if (i == index) continue;
                for (int j = 0; j < graph[i].Count; j++)
                    if (graph[i][j].First == index + 1)
                        drawnEdges[i][j].Last = new Point(
                            drawnVertices[index].X,
                            drawnVertices[index].Y);
            }
            index = -1;
            startX = -1;
            startY = -1;
            Mouse.Capture(null);
        }

        private void PlaceToDraw_MouseMove(object sender, MouseEventArgs e)
        {
            if (!PlaceToDraw.IsMouseCaptured) return;
            drawnVertices[index].X = (int)e.GetPosition(this).X;
            drawnVertices[index].Y = (int)e.GetPosition(this).Y;
        }
    }
}
