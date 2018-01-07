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

namespace Image_Surface_Comparison_System.Pages
{
    public partial class Analysis : Page
    {
        public Analysis()
        {
            InitializeComponent();

            var r = new Random();
            Values = new ChartValues<ObservableValue>();

            //Lets define a custom mapper, to set fill and stroke
            //according to chart values...
            Mapper = Mappers.Xy<ObservableValue>()
                .X((item, index) => index)
                .Y(item => item.Value)
                .Fill(item => item.Value > 200 ? DangerBrush : null)
                .Stroke(item => item.Value > 200 ? DangerBrush : null);

            Formatter = x => x + " ms";

            DangerBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(238, 83, 80));

            DataContext = this;
        }

        public Func<double, string> Formatter { get; set; }
        public ChartValues<ObservableValue> Values { get; set; }
        public Brush DangerBrush { get; set; }
        public CartesianMapper<ObservableValue> Mapper { get; set; }




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
            Values.Clear();

            string path = Base.path + "\\Data\\" + album_cb.SelectedValue;

            foreach (string photo in Directory.GetFiles(path).Select(System.IO.Path.GetFileName).ToArray())
            {
                using (StreamReader sr = new StreamReader(path + "\\" + photo))
                {
                    sr.ReadLine();
                    sr.ReadLine();
                    Values.Add(new ObservableValue(Int32.Parse(sr.ReadLine())));           
                }
            }
        }

        private void order_cb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
