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
    /// Логика взаимодействия для FileGraph.xaml
    /// </summary>
    public partial class FileGraph : Window
    {
        public FileGraph()
        {
            InitializeComponent();
        }

        private void GetGraph_Click(object sender, RoutedEventArgs e)
        {
            FilePathTextBox.IsEnabled = false;
            List<Pair<int, double>>[] graph;
            try
            {
                graph =
                    MetricOrientedGraph.ReadGraphFromFile(
                        FilePathTextBox.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("При попытке получить граф из файла \"" +
                    FilePathTextBox.Text + "\" возникло искючение типа " +
                    ex.GetType() + ": \"" + Environment.NewLine +
                    ex.Message + "\". ");

                FilePathTextBox.IsEnabled = true;
                return;
            }
            (new DrawnGraph(graph)).Show();
            Close();
        }

        private void ExitProgram_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
