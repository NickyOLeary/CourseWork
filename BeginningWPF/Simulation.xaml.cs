using System;
using GraphLib;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Threading;

namespace SimulationWPF
{
    /// <summary>
    /// Логика взаимодействия для Simulation.xaml
    /// </summary>
    public partial class Simulation : Window
    {
        /// <summary>
        /// Метрический неориентированный граф.
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
        /// Список эпсилон-окрестностей.
        /// </summary>
        List<EpsilonNeighbourhood> neighs;
        /// <summary>
        /// Величина эпсилонокрестности.
        /// </summary>
        double epsilon;
        /// <summary>
        /// Таймер для смещения всех эпсилон окрестностей.
        /// </summary>
        DispatcherTimer tmr;
        /// <summary>
        /// Время, выводимое пользователю.
        /// </summary>
        double timeForUser = 0;

        /// <summary>
        /// Инициализирует поля входными параметрами.
        /// </summary>
        /// <param name="_graph">
        /// Метрический оиентированный граф.
        /// </param>
        /// <param name="_drawnVertices">
        /// Массив графических вершин.
        /// </param>
        /// <param name="_drawnEdges">
        /// Массив списков графических рёбер.
        /// </param>
        /// <param name="_epsilon">
        /// Величина эпсилон-окрестности. 
        /// </param>
        /// <param name="_startVertex">
        /// Номер начальной вершины.
        /// </param>
        public Simulation(List<Pair<int, double>>[] _graph,
            Vertex[] _drawnVertices, List<Edge>[] _drawnEdges,
            double _epsilon, int _startVertex)
        {
            //Запоминаем переданные напрямую параметры.
            InitializeComponent();
            graph = _graph;
            drawnVertices = _drawnVertices;
            drawnEdges = _drawnEdges;
            epsilon = _epsilon;
            neighs = new List<EpsilonNeighbourhood>();

            //Добавляем вершины в SimulationCanvas. 
            for (int i = 0; i < drawnVertices.Length; i++)
            {
                SimulationCanvas.Children.Add(drawnVertices[i].Ring);
                SimulationCanvas.Children.Add(drawnVertices[i].RingContent);
            }
            //Добавляем рёбра в SimulationCanvas. 
            for (int i = 0; i < drawnEdges.Length; i++)
                for (int j = 0; j < drawnEdges[i].Count; j++)
                {
                    drawnEdges[i][j].Weight.IsEnabled = false;
                    SimulationCanvas.Children.Add(drawnEdges[i][j].DrawnEdge);
                    SimulationCanvas.Children.Add(drawnEdges[i][j].Weight);
                    SimulationCanvas.Children.Add(drawnEdges[i][j].Arrow);
                }
            //Добавляем первые эпсилон-окрестности в SimulationCanvas.
            for (int i = 0; i < graph[_startVertex - 1].Count; i++)
            {
                neighs.Add(new EpsilonNeighbourhood(
                    drawnEdges[_startVertex - 1][i].First,
                    drawnEdges[_startVertex - 1][i].Middle,
                    drawnEdges[_startVertex - 1][i].Final,
                    _startVertex - 1, i, graph[_startVertex - 1][i].Second,
                    epsilon, 2 * epsilon, 2 * epsilon / 3));
                SimulationCanvas.Children.Add(neighs[i].Neighbourhood);
            }

            //Отключаем возможность редактировать текст в 
            //информационном блоке текста.
            InfoTextBox.IsEnabled = false;

            //Включаем таймер, при каждом тике которого 
            //происходит смещение всех эпсилон-окрестностей.
            tmr = new DispatcherTimer();
            tmr.Interval = new TimeSpan(1);
            tmr.Tick += TmrTick;
        }

        /// <summary>
        /// Кнопка, останавливающая и возобновляющая моделирования.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetSimulationBtn_Click(object sender, RoutedEventArgs e)
        {
            if (tmr.IsEnabled)
                tmr.Stop();
            else
                tmr.Start();
        }

        /// <summary>
        /// Вызывает при каждом тике таймера <see cref="tmr"/>
        /// для смещения всех эпсилон-окрестностей на шаг.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void TmrTick(object sender, EventArgs e)
        {
            /*Выключаем таймер, чтобы в процессе асинхронного смещения 
            всех окрестностей данный метод не был вызван ещё раз, то 
            привело бы к накладыванию добавления элементов в 
            SimulationCanvas и к некорректному вычислению времени 
            насыщения.*/
            tmr.Stop();
            //Удаляем все элементы из SimulationCanvas.
            //В противном случае затеряются те эпсилон-окресности, 
            //которые необходимо удалить.
            for (int i = 0; i < neighs.Count; i++)
                SimulationCanvas.Children.Remove(neighs[i].Neighbourhood);
            try
            {
                //Выводим текущее значение времени.
                timeForUser += checked(2 * epsilon / 3);
                InfoTextBox.Text = $"Время: {timeForUser}";
                //Вызываем смещение всех эпсилон-окрестностей в другом потоке, чтобы 
                //основной поток не зависал и была возможность остановить моделирование 
                //или выйти из программы.
                neighs = await Task.Run(() =>
                    Application.Current.Dispatcher.Invoke(MoveAllNeighs));
            }
            catch (Exception ex)
            {
                //Не хватило ресурсов для дальнейшего моделирования. 
                tmr.Stop();
                MessageBox.Show("Дальнейшее моделирование невозможно " +
                    "по следующей причине : " + Environment.NewLine +
                    ex.Message + Environment.NewLine +
                    "Программа закроется через 10 секунд. ");
                Thread.Sleep(10000);
                Close();
            }
            //Добавляем все элементы в SimulationCanvas.
            for (int i = 0; i < neighs.Count; i++)
                SimulationCanvas.Children.Add(neighs[i].Neighbourhood);
            //Запускаем заново таймер.
            tmr.Start();
        }

        /// <summary>
        /// Метод смещения всех эпсилон-окрестностей на шаг.
        /// </summary>
        /// <returns></returns>
        private List<EpsilonNeighbourhood> MoveAllNeighs()
        {
            //Заводим список новых эпсилон-окрестностей.
            List<EpsilonNeighbourhood> newNeighs =
                new List<EpsilonNeighbourhood>();
            for (int i = 0; i < neighs.Count; i++)
            {
                //Смещаем очередную эпсилон-окрестность 
                //и получаем число, определяющее дальнейшие действия.
                double newEps = neighs[i].MoveNeighbourhood();
                if (newEps >= 0)
                {
                    //Добавляем текущую окрестность и все, 
                    //выходящие из вершины, в которую 
                    //текущая пришла.
                    newNeighs.Add(neighs[i]);
                    int vertexIndex = graph[neighs[i].EdgeIndexI]
                        [neighs[i].EdgeIndexJ].First - 1;
                    for (int j = 0; j < drawnEdges[vertexIndex].Count; j++)
                        newNeighs.Add(new EpsilonNeighbourhood(
                            drawnEdges[vertexIndex][j].First,
                            drawnEdges[vertexIndex][j].Middle,
                            drawnEdges[vertexIndex][j].Final,
                            vertexIndex, j, graph[vertexIndex][j].Second,
                            newEps, 2 * epsilon, 2 * epsilon / 3));
                }
                else if (newEps == -2)
                    //Добавляем только текущую.
                    newNeighs.Add(neighs[i]);
                //Если вернулась (-1), то вершину не нужно обавлять.
            }
            return newNeighs;
        }

        /// <summary>
        /// Кнопка для выхода из программы.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitProgram_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
