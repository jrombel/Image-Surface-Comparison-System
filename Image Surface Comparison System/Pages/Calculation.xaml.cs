﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Image_Surface_Comparison_System
{
    public partial class Calculation : Page
    {
        Photo photo;
        Photo photoOrginal;
        int selectedTool;
        int selectedMode;
        bool brushDown = false;

        Point startHand;
        Point originHand;

        int brushShape = 0;
        double scale = 1;

        public Calculation()
        {
            InitializeComponent();

            selectedTool = 0;
            selectedMode = 0;

            photoDegreeTolerance_s.Value = 20;
            opacity_s.Value = 1;
            brushSize_s.Value = 10;
        }

        private void LoadImage()
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            dlg.Filter = "Image Files| *.jpg; *.jpeg; *.png;|JPEG Files (*.jpeg,*.jpg)| *.jpg; *.jpeg;|PNG Files (*.png)|*.png;|GIF Files (*.gif)|*.gif;";

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                string filename = dlg.FileName;

                BitmapImage b = new BitmapImage();
                b.BeginInit();
                b.UriSource = new Uri(filename);
                b.EndInit();

                image_img.Source = b;
                imageOrginal_img.Source = b;
                photoOrginal = new Photo((BitmapSource)imageOrginal_img.Source);
                photo = new Photo((BitmapSource)image_img.Source);
            }
        }

        private void Photo_Click(object sender, RoutedEventArgs e)
        {
            LoadImage();
        }

        private void Image_img_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Image image = (Image)sender;

            Photo photo;
            Photo orginal;

            photo = this.photo;
            orginal = photoOrginal;

            if (image.Source.ToString() == "pack://application:,,,/Image Surface Comparison System;component/Resources/loadPhoto_img.png")
            {
                LoadImage();
                return;
            }

            //click coordinates
            double x = Math.Floor(e.GetPosition(image).X * photo.photo.PixelWidth / image.ActualWidth);
            double y = Math.Floor(e.GetPosition(image).Y * photo.photo.PixelHeight / image.ActualHeight);

            //determining the color of the indicated pixel
            int index = photo.GetIndex((int)x, (int)y);
            Color selected = null;

            //operation of the selected tool
            if (selectedTool == 0)
            {
                WandTool wandTool;
                selected = new Color(orginal.GetColor(index));
                wandTool = new WandTool(photo, orginal, selectedMode, (int)x, (int)y, (bool)photoDegreeToleranceAdjacent_cb.IsChecked, selected, (byte)photoDegreeTolerance_s.Value);

                image.Source = wandTool.Wand().photo;
                //Image1Color_l.Content = "Color: " + selected.ToString();
            }
            else if (selectedTool == 1)
            {
                BrushTool brushTool;
                brushTool = new BrushTool(photo, orginal, selectedMode, (int)x, (int)y, Int32.Parse(brushSize_tb.Text), brushShape);
                image.Source = brushTool.Brush().photo;

                if (selectedMode == 0)
                {
                    selectionAddMode_rb.IsChecked = true;
                    selectedMode = 1;
                }

                brushDown = true;
            }
            else if (selectedTool == 2)
            {

            }
            else if (selectedTool == 3)
            {
                image_img.CaptureMouse();
                TranslateTransform tt = new TranslateTransform(image_img.RenderTransform.Value.OffsetX, image_img.RenderTransform.Value.OffsetY);
                startHand = e.GetPosition(this);
                originHand = new Point(tt.X, tt.Y);
            }
            else if (selectedTool == 4)
            {
                if (magnifierZoomInMode.IsChecked == true)
                {
                    image_img.RenderTransform = null;

                    var position = e.MouseDevice.GetPosition(image_img);

                    scale += 0.1;

                    image_img.RenderTransform = new ScaleTransform(scale, scale, position.X, position.Y);
                    imageOrginal_img.RenderTransform = new ScaleTransform(scale, scale, position.X, position.Y);
                    magnifierZoom_l.Content = scale * 100 + "%";
                }
                else
                {
                    image_img.RenderTransform = null;

                    var position = e.MouseDevice.GetPosition(image_img);

                    if (scale - 0.1 >= 1)
                    {
                        scale -= 0.1;

                        image_img.RenderTransform = new ScaleTransform(scale, scale, position.X, position.Y);
                        imageOrginal_img.RenderTransform = new ScaleTransform(scale, scale, position.X, position.Y);
                        magnifierZoom_l.Content = scale * 100 + "%";
                    }
                }
            }
            double allPixels = photo.width * photo.height;
            degree_l.Content = string.Format("{0:0.00}", (((double)(photo.selectedPixelsCount) / allPixels)) * 100) + "% \nSelected: " + photo.selectedPixelsCount + "pixels\nAll: " + allPixels + "pixels";
        }

        private void Image_img_MouseMove(object sender, MouseEventArgs e)
        {
            if (selectedTool == 1 && brushDown == true)
            {
                Image image = (Image)sender;

                Photo photo;
                Photo orginal;

                photo = this.photo;
                orginal = photoOrginal;

                double x = Math.Floor(e.GetPosition(image).X * photo.photo.PixelWidth / image.ActualWidth);
                double y = Math.Floor(e.GetPosition(image).Y * photo.photo.PixelHeight / image.ActualHeight);

                if (Mouse.LeftButton == MouseButtonState.Pressed)
                {
                    BrushTool brushTool;
                    brushTool = new BrushTool(photo, orginal, selectedMode, (int)x, (int)y, Int32.Parse(brushSize_tb.Text), brushShape);
                    image.Source = brushTool.Brush().photo;
                }
                else
                {
                    brushDown = false;
                }
            }
            else if (selectedTool == 3)
            {
                if (image_img.IsMouseCaptured)
                {
                    Vector v = startHand - e.GetPosition(this);
                    image_img.RenderTransform = new ScaleTransform(scale, scale, originHand.X - v.X, originHand.Y - v.Y);
                    imageOrginal_img.RenderTransform = new ScaleTransform(scale, scale, originHand.X - v.X, originHand.Y - v.Y);
                }
            }

            if (photo != null)
            {
                double allPixels = photo.width * photo.height;
                degree_l.Content = string.Format("{0:0.00}", (((double)(photo.selectedPixelsCount) / allPixels)) * 100) + "% \nSelected: " + photo.selectedPixelsCount + "pixels\nAll: " + allPixels + "pixels";
            }
        }

        private void image_img_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (selectedTool == 3)
            {
                image_img.ReleaseMouseCapture();
            }
        }

        private void Image_img_MouseEnter(object sender, MouseEventArgs e)
        {

            if (image_img.Source.ToString() != "pack://application:,,,/Image Surface Comparison System;component/Resources/loadPhoto_img.png")
            {
                if (selectedTool == 1)
                    Mouse.OverrideCursor = CreateCursor(Int32.Parse(brushSize_tb.Text) * scale * (image_img.ActualHeight / photo.photo.PixelHeight), Int32.Parse(brushSize_tb.Text) * scale * (image_img.ActualWidth / photo.photo.PixelWidth));
            }
            else
            {
                this.Cursor = Cursors.Hand;
            }
        }

        private void Image_img_MouseLeave(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = null;
            this.Cursor = null;
        }

        private void image_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            image_img.RenderTransform = null;

            var position = e.MouseDevice.GetPosition(image_img);

            if (e.Delta > 0)
                scale += 0.1;
            else
            {
                if (scale - 0.1 >= 1)
                    scale -= 0.1;
            }

            image_img.RenderTransform = new ScaleTransform(scale, scale, position.X, position.Y);
            imageOrginal_img.RenderTransform = new ScaleTransform(scale, scale, position.X, position.Y);
            magnifierZoom_l.Content = scale * 100 + "%";
            if (selectedTool == 1)
                Mouse.OverrideCursor = CreateCursor(Int32.Parse(brushSize_tb.Text) * scale * (image_img.ActualHeight / photo.photo.PixelHeight), Int32.Parse(brushSize_tb.Text) * scale * (image_img.ActualWidth / photo.photo.PixelWidth));
        }

        Cursor CreateCursor(double rx, double ry)
        {
            DrawingVisual vis = new DrawingVisual();
            DrawingContext dc = vis.RenderOpen();

            SolidColorBrush brush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, Base.selectedColor.red, Base.selectedColor.green, Base.selectedColor.blue));

            if (squareShape_rb.IsChecked == true)
                dc.DrawRectangle(brush, new Pen(Brushes.Black, 1), new Rect(0, 0, rx, ry));
            else
                dc.DrawEllipse(brush, new Pen(Brushes.Black, 1), new Point(rx / 2, ry / 2), rx / 2, ry / 2);
            dc.Close();

            RenderTargetBitmap rtb = new RenderTargetBitmap(512, 512, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(vis);

            MemoryStream ms1 = new MemoryStream();

            PngBitmapEncoder penc = new PngBitmapEncoder();
            penc.Frames.Add(BitmapFrame.Create(rtb));
            penc.Save(ms1);

            byte[] pngBytes = ms1.ToArray();
            int size = pngBytes.GetLength(0);

            MemoryStream ms = new MemoryStream();
            ms.Write(BitConverter.GetBytes((Int16)0), 0, 2);
            ms.Write(BitConverter.GetBytes((Int16)2), 0, 2);
            ms.Write(BitConverter.GetBytes((Int16)1), 0, 2);
            ms.WriteByte(32);
            ms.WriteByte(32);
            ms.WriteByte(0);
            ms.WriteByte(0);
            ms.Write(BitConverter.GetBytes((Int16)(rx / 2.0)), 0, 2);
            ms.Write(BitConverter.GetBytes((Int16)(ry / 2.0)), 0, 2);
            ms.Write(BitConverter.GetBytes(size), 0, 4);
            ms.Write(BitConverter.GetBytes((Int32)22), 0, 4);
            ms.Write(pngBytes, 0, size);
            ms.Seek(0, SeekOrigin.Begin);

            return new Cursor(ms);
        }

        private void selectionToolChange_rb_Click(object sender, RoutedEventArgs e)
        {
            if (selectionWandTool_rb.IsChecked == true)
                selectedTool = 0;
            else if (selectionBrushTool_rb.IsChecked == true)
                selectedTool = 1;
            else if (selectionPolygonTool_rb.IsChecked == true)
                selectedTool = 2;
            else if (handTool_rb.IsChecked == true)
                selectedTool = 3;
            else if (magnifierTool_rb.IsChecked == true)
                selectedTool = 4;
        }

        private void selectionModeChange_rb_Click(object sender, RoutedEventArgs e)
        {
            if (selectionNewMode_rb.IsChecked == true)
                selectedMode = 0;
            else if (selectionAddMode_rb.IsChecked == true)
                selectedMode = 1;
            else if (selectionSubtractMode_rb.IsChecked == true)
                selectedMode = 2;
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
            Base.photos = new List<String>();

            foreach (var photo in Directory.GetFiles(Base.path + "\\" + album_cb.SelectedValue).Select(Path.GetFileName).ToArray())
            {
                Base.photos.Add(photo);
            }

            if (Base.photos.Count > 0)
                photo_cb.SelectedItem = Base.photos[0];
            else
            {
                photo_cb.SelectedItem = null;
                photoOrginal = null;
                photo = null;
            }
            photo_cb.ItemsSource = Base.photos;
        }

        private void photo_cb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            String photoCounter;
            if (photo_cb.SelectedValue != null)
            {
                string filename = Base.path + "\\" + album_cb.SelectedValue + "\\" + photo_cb.SelectedValue;

                BitmapImage b = new BitmapImage();
                b.BeginInit();
                b.UriSource = new Uri(filename);
                b.EndInit();

                image_img.Source = b;
                imageOrginal_img.Source = b;
                photoOrginal = new Photo((BitmapSource)imageOrginal_img.Source);
                photo = new Photo((BitmapSource)image_img.Source);
                photoCounter = (photo_cb.SelectedIndex + 1) + " / " + photo_cb.Items.Count;
            }
            else
            {
                image_img.Source = null;
                imageOrginal_img.Source = null;
                photoCounter = "0 / 0";
            }

            photo_tb.Text = photoCounter;
        }

        private void PreviousPhoto_Click(object sender, RoutedEventArgs e)
        {
            if (photo_cb.SelectedIndex - 1 >= 0)
                photo_cb.SelectedIndex--;
        }

        private void NextPhoto_Click(object sender, RoutedEventArgs e)
        {
            if (photo_cb.SelectedIndex + 1 < photo_cb.Items.Count)
                photo_cb.SelectedIndex++;
        }

        private void brushShape_rb_Click(object sender, RoutedEventArgs e)
        {
            if (circleShape_rb.IsChecked == true)
                brushShape = 0;
            else
                brushShape = 1;
        }
    }
}