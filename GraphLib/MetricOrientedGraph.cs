using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace GraphLib
{
    /// <summary>
    /// Статический класс, содержащий методы для работы с 
    /// метрическими ориентированными графами, реализованными 
    /// через <see cref="Pair{T, U}"/>. 
    /// </summary>
    public static class MetricOrientedGraph
    {
        /// <summary>
        /// Записываем граф <paramref name="graph"/> в 
        /// файл <paramref name="path"/>.
        /// </summary>
        /// <param name="path">
        /// Путь к файлу для записи.
        /// </param>
        /// <param name="graph">
        /// Граф, который необходимо записать в файл.
        /// </param>
        public static void SaveGraphToFile(string path,
            List<Pair<int, double>>[] graph)
        {
            //Создаём поток для записи в файл path.
            using (StreamWriter sw = new StreamWriter(path))
            {
                //Записываем в файл количестве вершин графа.
                sw.WriteLine(graph.Length);
                //Записываем в файл количество рёбер графа.
                sw.WriteLine(graph.Sum(list =>
                {
                    return list.Count;
                }));
                //Записываем в файл строки по 3 числа, разделённых 
                //пробелом: номер начальной вершины, номер конечной 
                //вершины, вес ребра.
                for (int i = 0; i < graph.Length; i++)
                    graph[i].ForEach(edge =>
                    {
                        sw.WriteLine((i + 1) + " " +
                            edge.First + " " + edge.Second);
                    });
            }
        }

        /// <summary>
        /// Считывает граф из файла <paramref name="path"/>.
        /// </summary>
        /// <param name="path">
        /// Путь к файлу для чтения.
        /// </param>
        /// <returns>
        /// Считанный из файла <paramref name="path"/> граф. 
        /// </returns>
        /// <exception cref="FormatException">
        /// Если данные в файле <paramref name="path"/> 
        /// имеют некорректный формат. 
        /// </exception>
        public static List<Pair<int, double>>[] ReadGraphFromFile(string path)
        {
            //Создаём поток для чтения из файла path. 
            using (StreamReader sr = new StreamReader(path))
            {
                //Переменная, в которую будет считываться 
                //каждая новая строка файла.
                string readedString;
                //Граф, который необходимо вернуть. 
                List<Pair<int, double>>[] graph;

                //1-я строка - количество вершин в графе 
                //(целое число, большее 0).
                int amountOfVertices;
                if ((readedString = sr.ReadLine()) is null)
                    throw new FormatException(
                        "Количество строк в файле меньше, " +
                        "чем требуемое. ");
                if (!int.TryParse(readedString, out amountOfVertices))
                    throw new FormatException(
                        "Строка 1: количество вершин в графе должно " +
                        "преобразовываться к целому числу. ");
                if (amountOfVertices <= 0)
                    throw new FormatException(
                        "Строка 1: количество вершин в графе должно " +
                        "быть положительным. ");
                //Создаём граф из amountOfVertices вершин.
                graph = new List<Pair<int, double>>[amountOfVertices];
                //Инициализируем каждый список рёбер.
                for (int i = 0; i < amountOfVertices; i++)
                    graph[i] = new List<Pair<int, double>>();

                //2-я строка - количество рёбер в графе 
                //(целое число, большее 0).
                int amountOfEdges;
                if ((readedString = sr.ReadLine()) is null)
                    throw new FormatException(
                        "Количество строк в файле меньше, " +
                        "чем требуемое. ");
                if (!int.TryParse(readedString, out amountOfEdges))
                    throw new FormatException(
                        "Строка 2: количество рёбер в графе должно " +
                        "преобразовываться к целому числу. ");
                if (amountOfEdges <= 0)
                    throw new FormatException(
                        "Строка 2: количество рёбер в графе должно " +
                        "быть положительным. ");

                /* Далее, в файле должны присутствовать amOfEdges строк, 
                 * в каждой из которых должно находиться по 3 числа, 
                 * разделённых произвольным количеством пробелов. 
                 * 1-е число - номер начальной вершины (целое число 
                 * в диапазоне [1; amOfVertices]). 
                 * 2-е число - номер конечной вершины (целое число 
                 * в диапазоне [1; amOfVertices]). 
                 * 3-е число - вес ребра (действительное число 
                 * в диапазоне [1; 100]).
                 * Вес может задаваться в виде √[число].
                 */
                for (int i = 0; i < amountOfEdges; i++)
                {
                    if ((readedString = sr.ReadLine()) is null)
                        throw new FormatException(
                            "Количество строк в файле меньше, " +
                            "чем требуемое. ");
                    //Разделяем очередную строку по пробелам.
                    string[] newEdge = readedString.Split(new char[] { ' ' },
                        StringSplitOptions.RemoveEmptyEntries);
                    //Начальная и конечная вершины соответственно.
                    int head, tail;
                    //Вес ребра.
                    double edgeWeight;

                    //Из разделения очередной строки из файла 
                    //должны были получиться 3 элемента.
                    if (newEdge.Length != 3)
                        throw new FormatException(
                            $"Строка {i + 3}: после разделения строки " +
                            "по пробелам должно получаться 3 " +
                            "элемента. ");
                    //Преобразуем номер начальной вершины.
                    if (!int.TryParse(newEdge[0], out head))
                        throw new FormatException(
                            $"Строка {i + 3}: номер начальной вершины " +
                            $"должен преобразовываться к целому числу. ");
                    //Проверяем диапазон номера начальной вершины.
                    if (head < 1 || head > amountOfVertices)
                        throw new FormatException(
                            $"Строка {i + 3}: номер начальной вершины " +
                            $"должены быть больше либо равен 1 и " +
                            $"меньше либо равен количеству вершин в графе. ");
                    //Преобразуем номер конечной вершины.
                    if (!int.TryParse(newEdge[1], out tail))
                        throw new FormatException(
                            $"Строка {i + 3}: номер конечной вершины " +
                            $"должен преобразовываться к целому числу. ");
                    //Проверяем диапазон номера конечной вершины.
                    if (tail < 1 || tail > amountOfVertices)
                        throw new FormatException(
                            $"Строка {i + 3}: номер конечной вершины " +
                            $"должен быть больше либо равен 1 и " +
                            $"меньше либо равен количеству вершин в графе. ");
                    //Преобразуем вес ребра.
                    if (!double.TryParse(newEdge[2], out edgeWeight))
                        throw new FormatException(
                            $"Строка {i + 3}: вес ребра должен " +
                            $"преобразовываться к действительному числу. ");
                    //Проверяем диапазон веса ребра.
                    if (edgeWeight < 1 || edgeWeight > 100)
                        throw new FormatException(
                            $"Строка {i + 3}: вес ребра должен быть " +
                            $"больше либо равен 1 и меньше либо равен 100. ");
                    //Добавляем очередное ребро в граф.
                    AddEdgeToGraph(graph, head,
                        new Pair<int, double>(tail, edgeWeight));
                }
                //Возвращаем полученный из файла path граф.
                return graph;
            }
        }

        /// <summary>
        /// Добавляет ребро <paramref name="edge"/>, 
        /// начинающееся в вершине с номером <paramref name="vertex"/>, 
        /// в граф <paramref name="graph"/>.
        /// </summary>
        /// <param name="graph">
        /// Граф, в который добавляется вершина.
        /// </param>
        /// <param name="vertex">
        /// Номер начальной вершины добавляемого ребра. 
        /// </param>
        /// <param name="edge">
        /// Добавляемое ребро.
        /// </param>
        /// <returns>
        /// Индекс в списке из элемента графа 
        /// <paramref name="graph"/> под номером 
        /// <paramref name="vertex"/> (с индексом 
        /// <paramref name="vertex"/> - 1).
        /// </returns>
        public static int AddEdgeToGraph(
            List<Pair<int, double>>[] graph,
            int vertex,
            Pair<int, double> edge)
        {
            //Если список пуст, добавляем в конец 
            //и возвращаем индекс 0.
            if (graph[vertex - 1].Count == 0)
            {
                graph[vertex - 1].Add(edge);
                return 0;
            }
            /*Далее к рёбрам применяются операторы сравнения. 
             * Для более подробной информации о сравнении 
             * элементов типа Pair<T, U> обратитесь к 
             * исходному коду этого типа.
             */

            if (edge <= graph[vertex - 1][0])
            {
                graph[vertex - 1].Insert(0, edge);
                return 0;
            }
            if (edge >= graph[vertex - 1][graph[vertex - 1].Count - 1])
            {
                graph[vertex - 1].Add(edge);
                return graph[vertex - 1].Count - 1;
            }
            for (int i = 0; i < graph[vertex - 1].Count - 1; i++)
                if (edge >= graph[vertex - 1][i] &&
                    edge <= graph[vertex - 1][i + 1])
                {
                    graph[vertex - 1].Insert(i + 1, edge);
                    return i + 1;
                }
            //Недостижимо при корректных данных, но компилятор этого 
            //просчитать не может.
            return -1;
        }

        /// <summary>
        /// Вычисляет количество компонент сильной связности 
        /// в графе <paramref name="graph"/>.
        /// </summary>
        /// <param name="graph">
        /// Граф, в котором необхоимо найти количество 
        /// компонент сильной связности.
        /// </param>
        /// <remarks>
        /// Данный метод представляет собой 
        /// <see href="https://e-maxx.ru/algo/strong_connected_components">
        /// алгоритм построения конденсации графа</see>.
        /// </remarks>
        /// <returns>
        /// Количество компонент сильной связности в 
        /// графе <paramref name="graph"/>. 
        /// </returns>
        public static int FindAmountOfStronglyConnectedComponents(
            List<Pair<int, double>>[] graph)
        {
            //Массив посещённых вершин.
            bool[] visited = new bool[graph.Length];
            //Список индексов вершин, расположенных по 
            //порядку топологической сортировки (1-й 
            //шаг алгоритма), т.е. по порядку выхода.
            List<int> order = new List<int>();
            //Выполняем топологическую сортировку.
            for (int i = 0; i < graph.Length; i++)
                if (!visited[i])
                    DFS(graph, i + 1,
                        visited, order);

            //Освежаем массив посещённых вершин.
            for (int i = 0; i < graph.Length; i++)
                visited[i] = false;
            //Получаем транспонированный граф для 2-го 
            //шага алгоритма.
            List<Pair<int, double>>[] transposeGraph =
                GetTransposeGraph(graph);
            //Переменная для количества компонент сильной связности.
            int amountOfComponents = 0;
            for (int i = 0; i < transposeGraph.Length; i++)
                //Проходимся по вершинам транспонированного 
                //графа в порядке топологической сортировки.
                if (!visited[
                    order[transposeGraph.Length - i - 1] - 1])
                {
                    DFS(transposeGraph,
                        order[graph.Length - i - 1], visited);
                    amountOfComponents++;
                }
            //Возвращаем количество компонент сильной связности.
            return amountOfComponents;
        }

        /// <summary>
        /// Алгоритм <see href="https://e-maxx.ru/algo/dfs"> поиска в глубину </see> 
        /// по графу <paramref name="graph"/> из вершины с номером <paramref name="vertex"/>, 
        /// определяющий, какие из вершин графа были посещены (через <paramref name="visited"/>).
        /// </summary>
        /// <param name="graph">
        /// Граф, в котором производится поиск в глубину.
        /// </param>
        /// <param name="vertex">
        /// Номер начальной вершины для поиска.
        /// </param>
        /// <param name="visited">
        /// Массив посещённых вершин (должен быть предварительно проинициализирован).
        /// </param>
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
        /// <summary>
        /// Алгоритм <see href="https://e-maxx.ru/algo/dfs"> поиска в глубину </see> 
        /// по графу <paramref name="graph"/> из вершины с номером <paramref name="vertex"/>, 
        /// определяющий, какие из вершин графа были посещены (через <paramref name="visited"/>) 
        /// и в каком порядке (через <paramref name="order"/>).
        /// </summary>
        /// <param name="graph">
        /// Граф, в котором производится поиск в глубину.
        /// </param>
        /// <param name="vertex">
        /// Номер начальной вершины для поиска.</param>
        /// <param name="visited">
        /// Массив посещённых вершин (должен быть предварительно проинициализирован).
        /// </param>
        /// <param name="order">
        /// Список, определяющий порядок вершин (должен быть предварительно проинициализирован).
        /// </param>
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

        /// <summary>
        /// Создаёт граф, транспонированный (обратный) переданному 
        /// <paramref name="graph"/>.
        /// </summary>
        /// <param name="graph">
        /// Граф, который необходимо транспонировать (обратить). 
        /// </param>
        /// <returns>
        /// Транспонированный (обратный) для <paramref name="graph"/> 
        /// граф.
        /// </returns>
        public static List<Pair<int, double>>[] GetTransposeGraph(
            List<Pair<int, double>>[] graph)
        {
            //Инициализируем обратный граф на то же количество вершин.
            List<Pair<int, double>>[] transposeGraph =
                new List<Pair<int, double>>[graph.Length];
            //Инициализируем списки рёбер для каждой верщины.
            for (int i = 0; i < graph.Length; i++)
                transposeGraph[i] = new List<Pair<int, double>>();
            //Добавляем обратные рёбра в новый массив.
            for (int i = 0; i < graph.Length; i++)
                for (int j = 0; j < graph[i].Count; j++)
                    AddEdgeToGraph(transposeGraph,
                        graph[i][j].First,
                        new Pair<int, double>(
                            i + 1,
                            graph[i][j].Second));
            //Возвращаем транспонированный массив.
            return transposeGraph;
        }
    }//public static class MetricOrientedGraph
}//namespace GraphLib
