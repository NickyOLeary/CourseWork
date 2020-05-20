using System;
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

        }

        private void ExitProgram_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
