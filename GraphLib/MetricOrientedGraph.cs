using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphLib
{
    public static class MetricOrientedGraph
    {
        public static bool CheckGraphValidity(
            List<Pair<int, double>>[] graph)
        {
            if (graph is null) return false;
            foreach (List<Pair<int, double>> list in graph)
            {
                if (list is null) return false;
                foreach (Pair<int, double> pair in list)
                    if (pair.First < 1 ||
                        pair.First > graph.Length ||
                        pair.Second < 1 ||
                        pair.Second > 100) return false;
                for (int i = 0; i < list.Count - 1; i++)
                    if (list[i].First > list[i + 1].First ||
                        (list[i].First == list[i + 1].First &&
                        list[i].Second > list[i + 1].Second))
                        return false;
            }
            return true;
        }

        public static List<Pair<int, double>>[] ReadGraphFromFile(
            string path)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                string readedString;

                int amountOfVertices;
                if ((readedString = sr.ReadLine()) is null)
                    throw new ArgumentException(
                        "Количество строк в файле меньше, " +
                        "чем требуемое. ");
                if (!int.TryParse(readedString, out amountOfVertices))
                    throw new ArgumentException(
                        "Строка 1: количество вершин в графе должно " +
                        "преобразовываться к целому числу. ");
                if (amountOfVertices <= 0)
                    throw new ArgumentException(
                        "Строка 1: количество вершин в графе должно " +
                        "быть положительным. ");
                List<Pair<int, double>>[] graph =
                    new List<Pair<int, double>>[amountOfVertices];
                for (int i = 0; i < amountOfVertices; i++)
                    graph[i] = new List<Pair<int, double>>();

                int amountOfEdges;
                if ((readedString = sr.ReadLine()) is null)
                    throw new ArgumentException(
                        "Количество строк в файле меньше, " +
                        "чем требуемое. ");
                if (!int.TryParse(readedString, out amountOfEdges))
                    throw new ArgumentException(
                        "Строка 2: количество рёбер в графе должно " +
                        "преобразовываться к целому числу. ");
                if (amountOfEdges <= 0)
                    throw new ArgumentException(
                        "Строка 2: количество рёбер в графе должно " +
                        "быть положительным. ");

                for (int i = 0; i < amountOfEdges; i++)
                {
                    if ((readedString = sr.ReadLine()) is null)
                        throw new ArgumentException(
                            "Количество строк в файле меньше, " +
                            "чем требуемое. ");
                    string[] newEdge = readedString.Split();
                    int arrowHead, arrowTail;
                    double edgeWeight;

                    if (newEdge.Length != 3)
                        throw new ArgumentException(
                            $"Строка {i + 3}: после разделения строки " +
                            "по пробелам должно получаться 3 " +
                            "элемента. ");
                    if (!int.TryParse(newEdge[0], out arrowHead))
                        throw new ArgumentException(
                            $"Строка {i + 3}: номер начальной вершины " +
                            $"должен преобразовываться к целому числу. ");
                    if (arrowHead < 1 || arrowHead > amountOfVertices)
                        throw new ArgumentException(
                            $"Строка {i + 3}: номер начальной вершины " +
                            $"должены быть больше либо равен 1 и " +
                            $"меньше либо равен количеству вершин в графе. ");
                    if (!int.TryParse(newEdge[1], out arrowTail))
                        throw new ArgumentException(
                            $"Строка {i + 3}: номер конечной вершины " +
                            $"должен преобразовываться к целому числу. ");
                    if (arrowTail < 1 || arrowTail > amountOfVertices)
                        throw new ArgumentException(
                            $"Строка {i + 3}: номер конечной вершины " +
                            $"должен быть больше либо равен 1 и " +
                            $"меньше либо равен количеству вершин в графе. ");
                    if (!double.TryParse(newEdge[2], out edgeWeight))
                        throw new ArgumentException(
                            $"Строка {i + 3}: вес ребра должен " +
                            $"преобразовываться к действительному числу. ");
                    if (edgeWeight < 1 || edgeWeight > 100)
                        throw new ArgumentException(
                            $"Строка {i + 3}: вес ребра должен быть " +
                            $"больше либо равен 1 и меньше либо равен 100. ");
                    AddEdgeToGraph(graph, arrowHead,
                        new Pair<int, double>(arrowTail, edgeWeight));
                }
                return graph;
            }
        }

        public static int AddEdgeToGraph(
            List<Pair<int, double>>[] graph,
            int vertex,
            Pair<int, double> arrow)
        {
            if (graph[vertex - 1].Count == 0)
            {
                graph[vertex - 1].Add(arrow);
                return 0;
            }
            if (arrow <= graph[vertex - 1][0])
            {
                graph[vertex - 1].Insert(0, arrow);
                return 0;
            }
            if (arrow >= graph[vertex - 1][graph[vertex - 1].Count - 1])
            {
                graph[vertex - 1].Add(arrow);
                return graph[vertex - 1].Count - 1;
            }
            for (int i = 0; i < graph[vertex - 1].Count - 1; i++)
                if (arrow >= graph[vertex - 1][i] &&
                    arrow <= graph[vertex - 1][i + 1])
                {
                    graph[vertex - 1].Insert(i + 1, arrow);
                    return i + 1;
                }
            //Недостижимо.
            return -1;
        }

        public static List<Pair<int, double>>[] SimplifyGraph(
            List<Pair<int, double>>[] graph)
        {
            List<Pair<int, double>>[] newGraph =
                new List<Pair<int, double>>
                [graph.Length];

            for (int i = 0; i < newGraph.Length; i++)
            {
                if (graph[i].Count > 0 &&
                    graph[i][0].First != i)
                    newGraph[i].Add(graph[i][0]);
                for (int j = 1; j < graph[i].Count; j++)
                {
                    if (graph[i][j].First != i &&
                        graph[i][j].First ==
                        graph[i][j - 1].First)
                        newGraph[i].Add(graph[i][j]);
                }
            }
            return newGraph;
        }

        public static List<Pair<int, double>>[] GetTransposeGraph(
            List<Pair<int, double>>[] graph)
        {
            /*if (!CheckGraphValidity(graph))
                throw new ArgumentException("Graph is invalid. ");*/
            List<Pair<int, double>>[] transposeGraph =
                new List<Pair<int, double>>[graph.Length];
            for (int i = 0; i < graph.Length; i++)
                transposeGraph[i] = new List<Pair<int, double>>();
            for (int i = 0; i < graph.Length; i++)
                for (int j = 0; j < graph[i].Count; j++)
                    AddEdgeToGraph(transposeGraph,
                        graph[i][j].First,
                        new Pair<int, double>(
                            i + 1,
                            graph[i][j].Second));
            return transposeGraph;
        }

        public static int FindAmountOfStronglyConnectedComponents(
            List<Pair<int, double>>[] graph)
        {
            bool[] visited = new bool[graph.Length];
            for (int i = 0; i < graph.Length; i++)
                visited[i] = false;
            List<int> order = new List<int>();
            for (int i = 0; i < graph.Length; i++)
                if (!visited[i])
                    DFS(graph, i + 1,
                        visited, order);

            for (int i = 0; i < graph.Length; i++)
                visited[i] = false;
            List<Pair<int, double>>[] transposeGraph =
                GetTransposeGraph(graph);
            int amountOfComponents = 0;
            for (int i = 0; i < transposeGraph.Length; i++)
                if (!visited[
                    order[transposeGraph.Length - i - 1] - 1])
                {
                    DFS(transposeGraph,
                        order[graph.Length - i - 1], visited);
                    amountOfComponents++;
                }
            return amountOfComponents;
        }

        public static void DFS(
            List<Pair<int, double>>[] graph,
            int vertex,
            bool[] visited)
        {
            visited[vertex - 1] = true;
            for (int i = 0; i < graph[vertex - 1].Count; i++)
                if (!visited[graph[vertex - 1][i].First - 1])
                    DFS(graph, graph[vertex - 1][i].First,
                        visited);
        }
        public static void DFS(
            List<Pair<int, double>>[] graph,
            int vertex,
            bool[] visited,
            List<int> order)
        {
            visited[vertex - 1] = true;
            for (int i = 0; i < graph[vertex - 1].Count; i++)
                if (!visited[graph[vertex - 1][i].First - 1])
                    DFS(graph, graph[vertex - 1][i].First,
                        visited, order);
            order.Add(vertex);
        }

        public static double[] Dijkstra(
            List<Pair<int, double>>[] graph,
            int vertex)
        {
            bool[] visited = new bool[graph.Length];
            for (int i = 0; i < graph.Length; i++)
                visited[i] = false;
            double[] shortestWays = new double[graph.Length];
            for (int i = 0; i < graph.Length; i++)
                shortestWays[i] = double.MaxValue;
            shortestWays[vertex - 1] = 0;
            for (int i = 0; i < graph.Length; i++)
            {
                int v = -1;
                for (int j = 0; j < graph.Length; j++)
                    if (!visited[j] && (v == -1 ||
                        shortestWays[j] < shortestWays[v]))
                        v = j;
                if (shortestWays[v] == double.MaxValue)
                    break;
                visited[v] = true;
                for (int j = 0; j < graph[v].Count; j++)
                    if (shortestWays[j] + graph[v][j].Second <
                        shortestWays[graph[v][j].First])
                        shortestWays[graph[v][j].First] =
                            shortestWays[j] + graph[v][j].Second;
            }
            return shortestWays;
        }

        public static double FindSaturationTime(
            List<Pair<int, double>>[] graph,
            int startVertex,
            double epsilon = 0)
        {
            double[] shortestWays = Dijkstra(
                SimplifyGraph(graph),
                startVertex);
            double saturationTime = 0;
            for (int i = 0; i < graph.Length; i++)
                for (int j = 0; j < graph[i].Count; j++)
                    if (shortestWays[i] + graph[i][j].Second -
                        epsilon > saturationTime)
                        saturationTime = shortestWays[i] +
                            graph[i][j].Second - epsilon;
            return (saturationTime < 0) ? 0 : saturationTime;
        }
    }//public static class MetricOrientedGraph
}//namespace GraphLib
