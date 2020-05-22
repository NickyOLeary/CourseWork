using System.Windows;

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

        /// <summary>
        /// Кнопка для выхода из программы.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitProgram_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Кнопка для перехода в форму считывания графа из файла.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FileChoice_Click(object sender, RoutedEventArgs e)
        {
            (new FileGraph()).Show();
            Close();
        }

        /// <summary>
        /// Кнопка для перехода в форму рисования графа.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DrawChoice_Click(object sender, RoutedEventArgs e)
        {
            (new DrawnGraph()).Show();
            Close();
        }
    }
}
