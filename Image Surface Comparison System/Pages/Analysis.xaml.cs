using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Configurations;
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

        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> Formatter { get; set; }

        public Analysis()
        {
            InitializeComponent();

            SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Values = ValuesTmp,
                    DataLabels = true,
                    LabelPoint = point => Math.Round(point.Y, 2) + "%",
                    LineSmoothness = 0,
                    Foreground = Brushes.White
                }
            };

            Formatter = value => value + ".00K items";
            DataContext = this;
        }

        private void Chart_OnDataClick(object sender, ChartPoint point)
        {
            MessageBox.Show("You clicked " + point.X + ", " + point.Y);
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
                        sr.ReadLine();
                        sr.ReadLine();
                        //if (percentages_cbi.IsSelected == true)
                        ValuesTmp.Add(new ObservableValue(((Double.Parse(sr.ReadLine())) / (Double.Parse(sr.ReadLine()))) * 100));
                        //else if (pixels_cbi.IsSelected == true)
                        //    ValuesTmp.Add(new ObservableValue(Int32.Parse(sr.ReadLine())));

                        LabelsTmp.Add(photo);
                    }
                }
                label_a.Labels = LabelsTmp.ToArray();
            }
            else
                MessageBox.Show("No areas selected!");
        }
    }
}