using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Image_Surface_Comparison_System
{
    public partial class Calculation : Page
    {
        //brush
        bool brushDown = false;
        int brushShape = 0;

        //poygon
        List<Point> points;
        Line lineTmp;
        bool selectedPolygonState = true;

        //hand & magnifier
        Point deltaHand;
        Point originHand;
        double scale = 1;
        double renderCenterX;
        double renderCenterY;
        bool displacement = false;

        public Calculation()
        {
            InitializeComponent();

            Base.selectedTool = 0;
            Base.selectedMode = 0;

            photoDegreeTolerance_s.Value = 20;
            opacity_s.Value = 1;
            brushSize_s.Value = 10;

            points = new List<Point>();

            lineTmp = new Line();
            lineTmp.Margin = new Thickness(0, 0, 0, 0);
            lineTmp.Stroke = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, Base.selectedColor.red, Base.selectedColor.green, Base.selectedColor.blue));
            lineTmp.StrokeThickness = 0.5;
            canvas_c.Children.Add(lineTmp);
        }

        private void Image_img_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            if (image_img.Source.ToString() == "pack://application:,,,/Image Surface Comparison System;component/Resources/loadPhoto_img.png")
            {
                //LoadImage();
                return;
            }

            double x = Math.Floor(e.GetPosition(image_img).X * Base.photo.photo.PixelWidth / image_img.ActualWidth);
            double y = Math.Floor(e.GetPosition(image_img).Y * Base.photo.photo.PixelHeight / image_img.ActualHeight);

            int index = Base.photo.GetIndex((int)x, (int)y);
            Color selected = null;

            if (Base.selectedTool == 0)
            {
                selected = new Color(Base.photoOrginal.GetColor(index));
                if (autoOtherPhotos_ch.IsChecked == false)
                {
                    WandTool.Wand((int)x, (int)y, selected, (byte)photoDegreeTolerance_s.Value);
                }
                else
                {
                    for (int i = 0; i < photo_cb.Items.Count; i++)
                    {
                        photo_cb.SelectedIndex = i;
                        string filename = Base.path + "\\Photos\\" + album_cb.SelectedValue + "\\" + photo_cb.SelectedValue;
                        Base.lastFilename = album_cb.SelectedValue + "\\" + photo_cb.SelectedValue;

                        BitmapImage b = new BitmapImage();
                        b.BeginInit();
                        b.UriSource = new Uri(filename);
                        b.EndInit();

                        Base.photo = new Photo((BitmapSource)b);
                        Base.photoOrginal = new Photo((BitmapSource)b);

                        if (Color.Difference(selected, Base.photoOrginal.GetColor(Base.photoOrginal.GetIndex((int)x, (int)y))) < (byte)photoDegreeTolerance_s.Value)
                        {
                            WandTool.Wand((int)x, (int)y, selected, (byte)photoDegreeTolerance_s.Value);
                        }
                        else
                        {
                            int width = (int)(Base.photoOrginal.width * 0.1);
                            int height = (int)(Base.photoOrginal.height * 0.1);
                            int limit;
                            if (width > height)
                                limit = width;
                            else
                                limit = height;

                            int xx = (int)x, yy = (int)y;
                            for (int step = 1; step < limit; step++)
                            {
                                int startX, startY, endX, endY;
                                bool end = false;
                                int count;
                                if (x - step >= 0)
                                    startX = (int)x - step;
                                else
                                    startX = 0;

                                if (y - step >= 0)
                                    startY = (int)y - step;
                                else
                                    startY = 0;

                                if (x + step < Base.photo.width)
                                    endX = (int)x + step;
                                else
                                    endX = Base.photo.width - 1;

                                if (y + step < Base.photo.height)
                                    endY = (int)y + step;
                                else
                                    endY = Base.photo.height - 1;

                                count = 0;
                                for (int xxx = startX; xxx < endX; xxx++)
                                {
                                    if (Color.Difference(selected, Base.photoOrginal.GetColor(Base.photoOrginal.GetIndex(xxx, startY))) < (byte)photoDegreeTolerance_s.Value)
                                    {
                                        count++;
                                        if (count > 5)
                                        {
                                            xx = xxx;
                                            yy = startY;
                                            end = true;
                                        }
                                    }
                                }
                                count = 0;
                                if (end == false)
                                {
                                    for (int xxx = startX; xxx < endX; xxx++)
                                    {
                                        if (Color.Difference(selected, Base.photoOrginal.GetColor(Base.photoOrginal.GetIndex(xxx, endY))) < (byte)photoDegreeTolerance_s.Value)
                                        {
                                            count++;
                                            if (count > 5)
                                            {
                                                xx = xxx;
                                                yy = endY;
                                                end = true;
                                            }
                                        }
                                    }
                                }
                                count = 0;
                                if (end == false)
                                {
                                    for (int yyy = startY + 1; yyy < endY - 1; yyy++)
                                    {
                                        if (Color.Difference(selected, Base.photoOrginal.GetColor(Base.photoOrginal.GetIndex(startX, yyy))) < (byte)photoDegreeTolerance_s.Value)
                                        {
                                            count++;
                                            if (count > 5)
                                            {
                                                xx = startX;
                                                yy = yyy;
                                                end = true;
                                            }
                                        }
                                    }
                                }
                                count = 0;
                                if (end == false)
                                {
                                    for (int yyy = startY + 1; yyy < endY - 1; yyy++)
                                    {
                                        if (Color.Difference(selected, Base.photoOrginal.GetColor(Base.photoOrginal.GetIndex(endX, yyy))) < (byte)photoDegreeTolerance_s.Value)
                                        {
                                            count++;
                                            if (count > 5)
                                            {
                                                xx = endX;
                                                yy = yyy;
                                                end = true;
                                            }
                                        }
                                    }
                                }
                                if (end == true)
                                    break;
                            }
                            WandTool.Wand((int)xx, (int)yy, selected, (byte)photoDegreeTolerance_s.Value);
                        }

                        image_img.Source = Base.photo.photo;
                        imageOrginal_img.Source = Base.photoOrginal.photo;
                    }
                }

                if (Base.selectedMode == 0)
                {
                    selectionAddMode_rb.IsChecked = true;
                    Base.selectedMode = 1;
                }
            }
            else if (Base.selectedTool == 1)
            {
                BrushTool.Brush((int)x, (int)y, Int32.Parse(brushSize_tb.Text), brushShape);

                if (Base.selectedMode == 0)
                {
                    selectionAddMode_rb.IsChecked = true;
                    Base.selectedMode = 1;
                }

                brushDown = true;
            }
            else if (Base.selectedTool == 2 && selectedPolygonState == true)
            {
                AddPoint(e.GetPosition(canvas_c).X, e.GetPosition(canvas_c).Y);
            }
            else if (Base.selectedTool == 3)
            {
                image_img.CaptureMouse();
                originHand = e.GetPosition(image_img);
            }
            else if (Base.selectedTool == 4)
            {
                image_img.RenderTransform = null;
                var position = e.MouseDevice.GetPosition(image_img);

                if (magnifierZoomInMode.IsChecked == true)
                {
                    scale += 0.1;
                }
                else
                {
                    if (scale - 0.1 >= 1)
                        scale -= 0.1;
                }

                renderCenterX = position.X;
                renderCenterY = position.Y;

                image_img.RenderTransform = new ScaleTransform(scale, scale, renderCenterX, renderCenterY);
                imageOrginal_img.RenderTransform = new ScaleTransform(scale, scale, renderCenterX, renderCenterY);
                canvas_c.RenderTransform = new ScaleTransform(scale, scale, renderCenterX, renderCenterY);
                magnifierZoom_l.Content = scale * 100 + "%";
            }
            double allPixels = Base.photo.width * Base.photo.height;
            degree_l.Content = string.Format("{0:0.00}", (((double)(Base.photo.selectedPixelsCount) / allPixels)) * 100) + "% \nSelected: " + Base.photo.selectedPixelsCount + "pixels\nAll: " + allPixels + "pixels";
        }

        private void Image_img_MouseMove(object sender, MouseEventArgs e)
        {
            if (Base.selectedTool == 1 && brushDown == true)
            {
                double x = Math.Floor(e.GetPosition(image_img).X * Base.photo.photo.PixelWidth / image_img.ActualWidth);
                double y = Math.Floor(e.GetPosition(image_img).Y * Base.photo.photo.PixelHeight / image_img.ActualHeight);

                if (Mouse.LeftButton == MouseButtonState.Pressed)
                    BrushTool.Brush((int)x, (int)y, Int32.Parse(brushSize_tb.Text), brushShape);
                else
                    brushDown = false;
            }
            else if (Base.selectedTool == 2)
            {
                if (selectedPolygonState == true && points.Count > 0)
                {
                    lineTmp.X2 = e.GetPosition(canvas_c).X;
                    lineTmp.Y2 = e.GetPosition(canvas_c).Y + 1;
                }
            }

            else if (Base.selectedTool == 3)
            {
                if (image_img.IsMouseCaptured)
                {
                    deltaHand = e.GetPosition(image_img);

                    renderCenterX = renderCenterX + (originHand.X - deltaHand.X);
                    renderCenterY = renderCenterY + (originHand.Y - deltaHand.Y);

                    image_img.RenderTransform = new ScaleTransform(scale, scale, renderCenterX, renderCenterY);
                    imageOrginal_img.RenderTransform = new ScaleTransform(scale, scale, renderCenterX, renderCenterY);
                    canvas_c.RenderTransform = new ScaleTransform(scale, scale, renderCenterX, renderCenterY);
                }
            }

            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                if (displacement == false)
                {
                    originHand = e.GetPosition(image_img);
                    displacement = true;
                }
                else
                {
                    deltaHand = e.GetPosition(image_img);

                    renderCenterX = renderCenterX + (originHand.X - deltaHand.X);
                    renderCenterY = renderCenterY + (originHand.Y - deltaHand.Y);

                    image_img.RenderTransform = new ScaleTransform(scale, scale, renderCenterX, renderCenterY);
                    imageOrginal_img.RenderTransform = new ScaleTransform(scale, scale, renderCenterX, renderCenterY);
                    canvas_c.RenderTransform = new ScaleTransform(scale, scale, renderCenterX, renderCenterY);
                }
            }
            if (e.MiddleButton == MouseButtonState.Released)
            {
                displacement = false;
            }

            if (Base.photo != null)
            {
                double allPixels = Base.photo.width * Base.photo.height;
                degree_l.Content = string.Format("{0:0.00}", (((double)(Base.photo.selectedPixelsCount) / allPixels)) * 100) + "% \nSelected: " + Base.photo.selectedPixelsCount + "pixels\nAll: " + allPixels + "pixels";
            }
        }

        private void image_img_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (Base.selectedTool == 3)
            {
                image_img.ReleaseMouseCapture();
            }
        }

        private void Image_img_MouseEnter(object sender, MouseEventArgs e)
        {

            if (image_img.Source.ToString() != "pack://application:,,,/Image Surface Comparison System;component/Resources/loadPhoto_img.png")
            {
                if (Base.selectedTool == 1)
                    Mouse.OverrideCursor = CreateCursor(Int32.Parse(brushSize_tb.Text) * scale * (image_img.ActualHeight / Base.photo.photo.PixelHeight), Int32.Parse(brushSize_tb.Text) * scale * (image_img.ActualWidth / Base.photo.photo.PixelWidth));
                else if (Base.selectedTool == 2)
                    Cursor = Cursors.Cross;
                else if (Base.selectedTool == 3)
                    Cursor = ((TextBlock)this.Resources["CursorGrab"]).Cursor;
                else if (Base.selectedTool == 4)
                    Cursor = ((TextBlock)this.Resources["CursorMagnify"]).Cursor;
            }
            //else
            //{
            //    this.Cursor = Cursors.Hand;
            //}
        }

        private void Image_img_MouseLeave(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = null;
            Cursor = null;
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

            renderCenterX = position.X;
            renderCenterY = position.Y;

            image_img.RenderTransform = new ScaleTransform(scale, scale, renderCenterX, renderCenterY);
            imageOrginal_img.RenderTransform = new ScaleTransform(scale, scale, renderCenterX, renderCenterY);
            canvas_c.RenderTransform = new ScaleTransform(scale, scale, renderCenterX, renderCenterY);
            magnifierZoom_l.Content = scale * 100 + "%";

            if (Base.selectedTool == 1)
                Mouse.OverrideCursor = CreateCursor(Int32.Parse(brushSize_tb.Text) * scale * (image_img.ActualHeight / Base.photo.photo.PixelHeight), Int32.Parse(brushSize_tb.Text) * scale * (image_img.ActualWidth / Base.photo.photo.PixelWidth));
        }

        private void canvas_c_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (Base.selectedTool == 2 && selectedPolygonState == true)
            {
                var mouseWasDownOn = e.Source as FrameworkElement;
                if (mouseWasDownOn != null && mouseWasDownOn.ToString() != "System.Windows.Shapes.Ellipse")
                {
                    if (points.Count > 1)
                    {
                        if (e.GetPosition(canvas_c).X > (points[0].X - 2) && e.GetPosition(canvas_c).X < (points[0].X + 2) &&
                            e.GetPosition(canvas_c).Y > (points[0].Y - 2) && e.GetPosition(canvas_c).Y < (points[0].Y + 2))
                        {
                            lineTmp.X2 = points[0].X;
                            lineTmp.Y2 = points[0].Y;

                            selectedPolygonState = false;

                            selectPolygon_b.IsEnabled = true;

                            return;
                        }
                    }
                    AddPoint(e.GetPosition(canvas_c).X, e.GetPosition(canvas_c).Y);
                }
                else if (mouseWasDownOn != null && mouseWasDownOn.ToString() == "System.Windows.Shapes.Ellipse")
                {
                    if (points.Count > 1)
                    {
                        if (e.GetPosition(canvas_c).X > (points[0].X - 2) && e.GetPosition(canvas_c).X < (points[0].X + 2) &&
                            e.GetPosition(canvas_c).Y > (points[0].Y - 2) && e.GetPosition(canvas_c).Y < (points[0].Y + 2))
                        {
                            lineTmp.X2 = points[0].X;
                            lineTmp.Y2 = points[0].Y;

                            selectedPolygonState = false;

                            selectPolygon_b.IsEnabled = true;

                            return;
                        }
                    }
                }
            }
        }

        private void canvas_c_MouseMove(object sender, MouseEventArgs e)
        {
            if (Base.selectedTool == 2)
            {
                if (selectedPolygonState == true && points.Count > 0)
                {
                    lineTmp.X2 = e.GetPosition(canvas_c).X;
                    lineTmp.Y2 = e.GetPosition(canvas_c).Y;
                }
            }
        }

        private void selectionToolChange_rb_Click(object sender, RoutedEventArgs e)
        {
            if (selectionWandTool_rb.IsChecked == true)
                Base.selectedTool = 0;
            else if (selectionBrushTool_rb.IsChecked == true)
                Base.selectedTool = 1;
            else if (selectionPolygonTool_rb.IsChecked == true)
                Base.selectedTool = 2;
            else if (handTool_rb.IsChecked == true)
                Base.selectedTool = 3;
            else if (magnifierTool_rb.IsChecked == true)
                Base.selectedTool = 4;
        }

        private void selectionModeChange_rb_Click(object sender, RoutedEventArgs e)
        {
            if (selectionNewMode_rb.IsChecked == true)
                Base.selectedMode = 0;
            else if (selectionAddMode_rb.IsChecked == true)
                Base.selectedMode = 1;
            else if (selectionSubtractMode_rb.IsChecked == true)
                Base.selectedMode = 2;
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
            Base.photos = new List<String>();

            foreach (var photo in Directory.GetFiles(Base.path + "\\Photos\\" + album_cb.SelectedValue).Select(System.IO.Path.GetFileName).ToArray())
            {
                Base.photos.Add(photo);
            }

            if (Base.photos.Count > 0)
                photo_cb.SelectedItem = Base.photos[0];
            else
            {
                photo_cb.SelectedItem = null;
                Base.photoOrginal = null;
                Base.photo = null;
            }
            photo_cb.ItemsSource = Base.photos;
        }

        private void photo_cb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Base.photo != null)
                Base.Save();

            String photoCounter;
            if (photo_cb.SelectedValue != null)
            {
                string filename = Base.path + "\\Photos\\" + album_cb.SelectedValue + "\\" + photo_cb.SelectedValue;
                Base.lastFilename = album_cb.SelectedValue + "\\" + photo_cb.SelectedValue;

                BitmapImage b = new BitmapImage();
                b.BeginInit();
                b.UriSource = new Uri(filename);
                b.EndInit();

                image_img.Source = b;
                imageOrginal_img.Source = b;
                Base.photoOrginal = new Photo((BitmapSource)imageOrginal_img.Source);
                Base.photo = new Photo((BitmapSource)image_img.Source);
                photoCounter = (photo_cb.SelectedIndex + 1) + " / " + photo_cb.Items.Count;
            }
            else
            {
                image_img.Source = null;
                imageOrginal_img.Source = null;
                photoCounter = "0 / 0";
            }

            string[] words = Base.lastFilename.Split('\\');
            string folder = words[0];
            string file = (words[1].Split('.'))[0] + ".txt";
            string fullPath = Base.path + "\\Data\\" + folder + "\\" + file;
            if (File.Exists(fullPath))
            {
                using (StreamReader sr = new StreamReader(fullPath))
                {
                    string line;
                    line = sr.ReadLine();
                    line = sr.ReadLine();
                    line = sr.ReadLine();
                    Base.photo.selectedPixelsCount = Int32.Parse(sr.ReadLine());
                    line = sr.ReadLine();

                    for (int y = 0; y < Base.photo.height; y++)
                    {
                        line = sr.ReadLine();
                        string[] parts = line.Split(' ');
                        for (int x = 0; x < Base.photo.width; x++)
                        {
                            if (parts[x] == "0")
                                Base.photo.selectedPixels[x, y] = false;
                            else
                            {
                                Base.photo.selectedPixels[x, y] = true;
                                int index;
                                index = Base.photo.GetIndex(x, y);
                                Base.photo.pixelData[index] = Color.ToUint(Base.selectedColor);
                            }
                        }
                    }
                }
            }
            image_img.Source = Base.photo.photo;

            Base.photo.photo.WritePixels(new Int32Rect(0, 0, (int)Base.photo.width, (int)Base.photo.height), Base.photo.pixelData, Base.photo.widthInByte, 0);
            photo_tb.Text = photoCounter;

            double allPixels = Base.photo.width * Base.photo.height;
            degree_l.Content = string.Format("{0:0.00}", (((double)(Base.photo.selectedPixelsCount) / allPixels)) * 100) + "% \nSelected: " + Base.photo.selectedPixelsCount + "pixels\nAll: " + allPixels + "pixels";
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

        private void AddPoint(double x, double y)
        {
            Ellipse pointStart = new Ellipse();
            pointStart = new Ellipse();
            pointStart.Height = 2;
            pointStart.Width = 2;
            pointStart.Margin = new Thickness(x - 1, y - 1, 0, 0);
            pointStart.Stroke = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, Base.selectedColor.red, Base.selectedColor.green, Base.selectedColor.blue));
            canvas_c.Children.Add(pointStart);

            Point point = new Point(x, y);
            points.Add(point);

            lineTmp.X1 = point.X;
            lineTmp.Y1 = point.Y;

            if (points.Count > 1)
            {
                Line line = new Line();
                line.Margin = new Thickness(0, 0, 0, 0);
                line.X1 = point.X;
                line.Y1 = point.Y;
                line.X2 = points[points.Count - 2].X;
                line.Y2 = points[points.Count - 2].Y;
                line.Stroke = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, Base.selectedColor.red, Base.selectedColor.green, Base.selectedColor.blue));
                line.StrokeThickness = 0.5;
                canvas_c.Children.Add(line);
            }
        }

        private void selectPolygon_b_Click(object sender, RoutedEventArgs e)
        {
            PolygonTool.Polygon(points, image_img.ActualWidth, image_img.ActualHeight);

            if (Base.selectedMode == 0)
            {
                selectionAddMode_rb.IsChecked = true;
                Base.selectedMode = 1;
            }

            clearPolygon_b_Click(sender, e);
        }

        private void clearPolygon_b_Click(object sender, RoutedEventArgs e)
        {
            selectedPolygonState = true;
            canvas_c.Children.Clear();
            points.Clear();

            canvas_c.Children.Add(lineTmp);
            lineTmp.X1 = 0;
            lineTmp.Y1 = 0;
            lineTmp.X2 = 0;
            lineTmp.Y2 = 0;

            selectPolygon_b.IsEnabled = false;
        }

        private void UndoTool_Click(object sender, RoutedEventArgs e)
        {
            if (photo_cb.SelectedValue != null)
            {
                string filename = Base.path + "\\Photos\\" + album_cb.SelectedValue + "\\" + photo_cb.SelectedValue;
                Base.lastFilename = album_cb.SelectedValue + "\\" + photo_cb.SelectedValue;

                BitmapImage b = new BitmapImage();
                b.BeginInit();
                b.UriSource = new Uri(filename);
                b.EndInit();

                int selectedPixelsCountTmp = Base.photo.selectedPixelsCount;
                bool[,] selectedPixelsTmp = (bool[,])Base.photo.selectedPixels.Clone();
                image_img.Source = b;
                imageOrginal_img.Source = b;
                Base.photoOrginal = new Photo((BitmapSource)imageOrginal_img.Source);
                Base.photo = new Photo((BitmapSource)image_img.Source);
                Base.photo.selectedPixels = (bool[,])selectedPixelsTmp.Clone();
                Base.photo.selectedPixelsCount = selectedPixelsCountTmp;
                ReloadPhoto();
            }
        }

        private void ClearSelection_Click(object sender, RoutedEventArgs e)
        {
            if (Base.photo != null)
            {
                Base.photo.pixelData = (uint[])Base.photoOrginal.pixelData.Clone();
                Array.Clear(Base.photo.selectedPixels, 0, Base.photo.selectedPixels.Length);
                Base.photo.selectedPixelsCount = 0;

                ReloadPhoto();
            }
        }

        private void selectedColorPicker_cp_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
        {
            Base.selectedColor = new Color(selectedColorPicker_cp.SelectedColor.Value.R, selectedColorPicker_cp.SelectedColor.Value.G, selectedColorPicker_cp.SelectedColor.Value.B);
            lineTmp.Stroke = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, Base.selectedColor.red, Base.selectedColor.green, Base.selectedColor.blue));
            ReloadPhoto();
        }

        private void ReloadPhoto()
        {
            if (Base.photo != null)
            {
                for (int y = 0; y < Base.photo.height; y++)
                {
                    for (int x = 0; x < Base.photo.width; x++)
                    {
                        if (Base.photo.selectedPixels[x, y] == true)
                        {
                            int index;
                            index = Base.photo.GetIndex(x, y);
                            Base.photo.pixelData[index] = Color.ToUint(Base.selectedColor);
                        }
                    }
                }

                Base.photo.photo.WritePixels(new Int32Rect(0, 0, (int)Base.photo.width, (int)Base.photo.height), Base.photo.pixelData, Base.photo.widthInByte, 0);
                image_img.Source = Base.photo.photo;
            }
        }

        private void photoProcessing_btn(object sender, RoutedEventArgs e)
        {
            if (Base.photo != null)
            {
                if (photoProcessing_cb.SelectedIndex == 0) //Filtering
                {
                    if (filteringPhotoProcessing.SelectedIndex == 0)
                        PhotoProcessing.Smoothing();
                    else if (filteringPhotoProcessing.SelectedIndex == 1)
                        PhotoProcessing.Median();
                    else if (filteringPhotoProcessing.SelectedIndex == 2)
                        PhotoProcessing.EdgeDetect();
                    else if (filteringPhotoProcessing.SelectedIndex == 3)
                        PhotoProcessing.HighPassSharpening();
                    else if (filteringPhotoProcessing.SelectedIndex == 4)
                        PhotoProcessing.GaussianBlur();

                    Base.photoOrginal.Clone(Base.photo);
                }
                else if (photoProcessing_cb.SelectedIndex == 1) //Binaryzation
                {
                    PhotoProcessing.Binaryzation((int)manuallyValue_s.Value);
                }

                ReloadPhoto();
            }
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

        //private void LoadImage()
        //{
        //    Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

        //    dlg.Filter = "Image Files| *.jpg; *.jpeg; *.png;|JPEG Files (*.jpeg,*.jpg)| *.jpg; *.jpeg;|PNG Files (*.png)|*.png;|GIF Files (*.gif)|*.gif;";

        //    Nullable<bool> result = dlg.ShowDialog();

        //    if (result == true)
        //    {
        //        string filename = dlg.FileName;

        //        BitmapImage b = new BitmapImage();
        //        b.BeginInit();
        //        b.UriSource = new Uri(filename);
        //        b.EndInit();

        //        image_img.Source = b;
        //        imageOrginal_img.Source = b;
        //        photoOrginal = new Photo((BitmapSource)imageOrginal_img.Source);
        //        photo = new Photo((BitmapSource)image_img.Source);
        //    }
        //}
    }
}