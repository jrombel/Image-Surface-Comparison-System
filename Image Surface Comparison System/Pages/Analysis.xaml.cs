using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;

namespace Image_Surface_Comparison_System.Pages
{
    public partial class Analysis : Page
    {
        public Analysis()
        {
            InitializeComponent();
        }

        private void album_cb_DropDownOpened(object sender, EventArgs e)
        {
            Base.albums = new List<String>();

            foreach (var dir in Directory.GetDirectories(Base.path))
            {
                var dirName = new DirectoryInfo(dir).Name;
                Base.albums.Add(dirName);
            }

            album_cb.ItemsSource = Base.albums;
        }

        private void album_cb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void order_cb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
