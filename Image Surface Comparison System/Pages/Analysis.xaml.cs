using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;
using LiveCharts;
using LiveCharts.Defaults;
using System.Windows.Media;
using System.Windows;
using System.Linq;
using LiveCharts.Wpf;

namespace Image_Surface_Comparison_System.Pages
{
    public partial class Analysis : Page
    {
        ChartValues<ObservableValue> ValuesTmp = new ChartValues<ObservableValue>();
        List<String> LabelsTmp = new List<String>();

        public SeriesCollection seriesCollection { get; set; }
        public LineSeries lineSeries { get; set; }
        public string[] Labels { get; set; }

        public Analysis()
        {
            InitializeComponent();

            lineSeries = new LineSeries
            {
                Values = ValuesTmp,
                DataLabels = true,
                LabelPoint = point => Math.Round(point.Y, 2) + "%",
                LineSmoothness = 0,
                Foreground = Brushes.White
            };

            seriesCollection = new SeriesCollection
            {
                lineSeries
            };
            
            DataContext = this;
        }

        private void album_cb_DropDownOpened(object sender, EventArgs e)
        {
            Base.albums = new List<String>();

            foreach (var dir in Directory.GetDirectories(Base.path + "\\Photos"))
            {
                var dirName = new DirectoryInfo(dir).Name;
                Base.albums.Add(dirName);
            }

            album_cb.ItemsSource = Base.albums;
        }

        private void PrintClick(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();

            if (printDialog.ShowDialog() == true)
            {
                chart.Background = Brushes.White;
                chart.Foreground = Brushes.Black;
                chartAxisX.Foreground = Brushes.Black;
                chartAxisY.Foreground = Brushes.Black;
                lineSeries.Foreground = Brushes.Black;
                chartName_l.Visibility = Visibility.Visible;
                chart.Margin = new Thickness(30);

                printDialog.PrintVisual(chart_grid, "");

                chart.Background = null;
                chart.Foreground = Brushes.White;
                chartAxisX.Foreground = Brushes.White;
                chartAxisY.Foreground = Brushes.White;
                lineSeries.Foreground = Brushes.White;
                chartName_l.Visibility = Visibility.Collapsed;
                chart.Margin = new Thickness(0);
            }
        }

        private void album_cb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DrawGraph();
        }

        private void UpdateClick(object sender, RoutedEventArgs e)
        {
            DrawGraph();
        }

        private void DrawGraph()
        {
            ValuesTmp.Clear();
            LabelsTmp.Clear();

            string path = Base.path + "\\Data\\" + album_cb.SelectedValue;

            if (Directory.Exists(path))
            {
                foreach (string photo in Directory.GetFiles(path).Select(Path.GetFileName).ToArray())
                {
                    using (StreamReader sr = new StreamReader(path + "\\" + photo))
                    {
                        string label = sr.ReadLine();
                        sr.ReadLine();
                        //if (percentages_cbi.IsSelected == true)
                        ValuesTmp.Add(new ObservableValue(((Double.Parse(sr.ReadLine())) / (Double.Parse(sr.ReadLine()))) * 100));
                        //else if (pixels_cbi.IsSelected == true)
                        //    ValuesTmp.Add(new ObservableValue(Int32.Parse(sr.ReadLine())));

                        LabelsTmp.Add(label);
                    }
                }
                chartAxisX.Labels = LabelsTmp.ToArray();
            }
            else
                MessageBox.Show("No areas selected!");
        }
    }
}