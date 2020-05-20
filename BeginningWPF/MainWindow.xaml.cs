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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SimulationWPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ExitProgram_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void FileChoice_Click(object sender, RoutedEventArgs e)
        {
            (new FileGraph()).Show();
            Close();
        }

        private void DrawChoice_Click(object sender, RoutedEventArgs e)
        {
            (new DrawnGraph()).Show();
            Close();
        }
    }
}
