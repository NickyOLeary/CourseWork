using System;
using GraphLib;
using System.Collections.Generic;
using System.Windows;

namespace SimulationWPF
{
    /// <summary>
    /// Логика взаимодействия для FileGraph.xaml
    /// </summary>
    public partial class FileGraph : Window
    {
        public FileGraph()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Кнопка для попытки считывания графа из файла (путь взят 
        /// из <see cref="FilePathTextBox"/>) для перехода в 
        /// форму рисования.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GetGraph_Click(object sender, RoutedEventArgs e)
        {
            //Блокируем запись в FilePathTextBox на время чтения графа.
            FilePathTextBox.IsEnabled = false;
            List<Pair<int, double>>[] graph;
            try
            {
                graph = MetricOrientedGraph.ReadGraphFromFile(
                        FilePathTextBox.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("При попытке получить граф из файла \"" +
                    FilePathTextBox.Text + "\" возникло искючение типа " +
                    ex.GetType() + ": " + Environment.NewLine + "\"" +
                    ex.Message + "\". ");
                //Разблокируем, так как придётся опять читать.
                FilePathTextBox.IsEnabled = true;
                return;
            }
            //Переходим в форму рисования графа.
            (new DrawnGraph(graph)).Show();
            Close();
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
