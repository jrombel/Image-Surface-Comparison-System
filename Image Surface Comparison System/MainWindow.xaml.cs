using System.Windows;
using System.Windows.Controls;
using Image_Surface_Comparison_System.Pages;
using System.IO;

namespace Image_Surface_Comparison_System
{
    public partial class MainWindow : Window
    {
        Page calculation_p;
        Page analysis_p;
        Page help_p;

        public MainWindow()
        {
            InitializeComponent();

            calculation_p = new Calculation();
            analysis_p = new Analysis();
            help_p = new Help();

            main_f.Content = calculation_p;
            Base.path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        }

        private void Calculate_Click(object sender, RoutedEventArgs e)
        {
            main_f.Content = calculation_p;
        }
        private void Analysis_Click(object sender, RoutedEventArgs e)
        {
            main_f.Content = analysis_p;
            Base.Save();
        }
        private void Help_Click(object sender, RoutedEventArgs e)
        {
            main_f.Content = help_p;
        }
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            Base.Save();
        }
    }
}