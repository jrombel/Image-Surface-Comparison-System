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
        Page aboutSystem_p;

        public MainWindow()
        {
            InitializeComponent();

            calculation_p = new Calculation();
            analysis_p = new Analysis();
            aboutSystem_p = new AboutSystem();

            main_f.Content = calculation_p;
            Base.path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\Photos";
        }

        private void Calculate_Click(object sender, RoutedEventArgs e)
        {
            main_f.Content = calculation_p;
        }
        private void Analysis_Click(object sender, RoutedEventArgs e)
        {
            main_f.Content = analysis_p;
        }
        private void AboutSystem_Click(object sender, RoutedEventArgs e)
        {
            main_f.Content = aboutSystem_p;
        }
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}