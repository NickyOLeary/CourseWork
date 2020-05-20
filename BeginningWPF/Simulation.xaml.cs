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
using System.Threading;
using System.Windows.Threading;
using System.Diagnostics;

namespace BeginningWPF
{
    /// <summary>
    /// Логика взаимодействия для Simulation.xaml
    /// </summary>
    public partial class Simulation : Window
    {
        List<Pair<int, double>>[] graph;
        Vertex[] drawnVertices;
        List<Edge>[] drawnEdges;
        List<EpsilonNeighbourhood> neighs;
        double epsilon;
        DispatcherTimer tmr;

        public Simulation(List<Pair<int, double>>[] _graph,
            Vertex[] _drawnVertices, List<Edge>[] _drawnEdges,
            double _epsilon, int _startVertex)
        {
            InitializeComponent();
            graph = _graph;
            drawnVertices = _drawnVertices;
            drawnEdges = _drawnEdges;
            epsilon = _epsilon;
            neighs = new List<EpsilonNeighbourhood>();

            for (int i = 0; i < drawnVertices.Length; i++)
            {
                SimulationCanvas.Children.Add(drawnVertices[i].Ring);
                SimulationCanvas.Children.Add(drawnVertices[i].RingContent);
            }
            for (int i = 0; i < drawnEdges.Length; i++)
                for (int j = 0; j < drawnEdges[i].Count; j++)
                {
                    drawnEdges[i][j].Weight.IsEnabled = false;
                    SimulationCanvas.Children.Add(drawnEdges[i][j].DrawnEdge);
                    SimulationCanvas.Children.Add(drawnEdges[i][j].Weight);
                    SimulationCanvas.Children.Add(drawnEdges[i][j].Arrow);
                }
            for (int i = 0; i < graph[_startVertex - 1].Count; i++)
            {
                neighs.Add(new EpsilonNeighbourhood(
                    drawnEdges[_startVertex - 1][i].First,
                    drawnEdges[_startVertex - 1][i].Middle,
                    drawnEdges[_startVertex - 1][i].Last,
                    _startVertex - 1, i, graph[_startVertex - 1][i].Second,
                    epsilon / 2, epsilon, epsilon / 3));
                SimulationCanvas.Children.Add(neighs[i].Neighbourhood);
            }


        }



        private List<EpsilonNeighbourhood> MoveAllNeighs()
        {
            List<EpsilonNeighbourhood> newNeighs =
                new List<EpsilonNeighbourhood>();
            for (int i = 0; i < neighs.Count; i++)
            {
                double newEps = neighs[i].MoveNeighbourhood();
                if (newEps >= 0)
                {
                    newNeighs.Add(neighs[i]);
                    Trace.WriteLine("Moved: " + neighs[i].EdgeIndexI + " " + neighs[i].EdgeIndexJ +
                    " " + neighs[i].Neighbourhood.Points[2]);
                    int vertexIndex = graph[neighs[i].EdgeIndexI]
                        [neighs[i].EdgeIndexJ].First - 1;
                    for (int j = 0; j < drawnEdges[vertexIndex].Count; j++)
                    {
                        newNeighs.Add(new EpsilonNeighbourhood(
                            drawnEdges[vertexIndex][j].First,
                            drawnEdges[vertexIndex][j].Middle,
                            drawnEdges[vertexIndex][j].Last,
                            vertexIndex, j, graph[vertexIndex][j].Second,
                            newEps, epsilon, epsilon / 3));
                        Trace.WriteLine("Moved: " + newNeighs[newNeighs.Count - 1].EdgeIndexI + " " +
                            newNeighs[newNeighs.Count - 1].EdgeIndexJ +
                        " " + newNeighs[newNeighs.Count - 1].Neighbourhood.Points[2]);
                    }
                }
                else if (newEps == -2)
                {
                    newNeighs.Add(neighs[i]);
                    Trace.WriteLine("Moved: " + neighs[i].EdgeIndexI + " " + neighs[i].EdgeIndexJ +
                    " " + neighs[i].Neighbourhood.Points[2]);
                }
            }
            return newNeighs;
        }

        private async void TmrTick(object sender, EventArgs e)
        {
            tmr.Stop();
            /*await Task.Run(() => Application.Current.Dispatcher.Invoke(
                   () =>
                   {
                       foreach (EpsilonNeighbourhood neigh in neighs)
                           SimulationCanvas.Children.Remove(neigh.Neighbourhood);
                   }));*/
            for (int i = 0; i < neighs.Count; i++)
            {
                Trace.WriteLine("Remove: " + neighs[i].EdgeIndexI + " " + neighs[i].EdgeIndexJ +
                    " " + neighs[i].Neighbourhood.Points[2]);
                SimulationCanvas.Children.Remove(neighs[i].Neighbourhood);
            }
            neighs = await Task.Run(() => Application.Current.Dispatcher.Invoke(MoveAllNeighs));
            /*await Task.Run(() => Application.Current.Dispatcher.Invoke(
                   () =>
                   {
                       foreach (EpsilonNeighbourhood neigh in neighs)
                           SimulationCanvas.Children.Add(neigh.Neighbourhood);
                   }));*/
            for (int i = 0; i < neighs.Count; i++)
            {
                Trace.WriteLine("Add: " + neighs[i].EdgeIndexI + " " + neighs[i].EdgeIndexJ +
                    " " + neighs[i].Neighbourhood.Points[2]);

                try
                {
                    SimulationCanvas.Children.Add(neighs[i].Neighbourhood);
                }
                catch { }
            }
            tmr.Start();
            /*List<EpsilonNeighbourhood> newNeighs =
                new List<EpsilonNeighbourhood>();
            for (int i = 0; i < neighs.Count; i++)
            {
                //Trace.WriteLine("=====================" + neighs.Count);
                double newEps = await Task.Run(() => Application.Current.Dispatcher.Invoke(
                   new Func<double>(neighs[i].MoveNeighbourhood)));
                //double newEps = neighs[i].MoveNeighbourhood();
                if (newEps >= 0)
                {
                    int vertexIndex = graph[neighs[i].EdgeIndexI]
                        [neighs[i].EdgeIndexJ].First - 1;
                    for (int j = 0; j < drawnEdges[vertexIndex].Count; j++)
                    {
                        newNeighs.Add(new EpsilonNeighbourhood(
                            drawnEdges[vertexIndex][j].First,
                            drawnEdges[vertexIndex][j].Middle,
                            drawnEdges[vertexIndex][j].Last,
                            vertexIndex, j, graph[vertexIndex][j].Second,
                            newEps, epsilon, epsilon / 3));
                    }
                }
                else if (newEps == -1)
                {
                    SimulationCanvas.Children.Remove(neighs[i].Neighbourhood);
                    neighs.RemoveAt(i);
                    i--;
                }
            }
            foreach (EpsilonNeighbourhood neigh in newNeighs)
            {
                SimulationCanvas.Children.Add(neigh.Neighbourhood);
                neighs.Add(neigh);
            }*/
        }

        private void ExitProgram_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SetSimulationBtn_Click(object sender, RoutedEventArgs e)
        {
            tmr = new DispatcherTimer();
            tmr.Interval = new TimeSpan(0, 0, 0, 0, 1);
            tmr.Tick += TmrTick;
            tmr.Start();
        }
    }
}
