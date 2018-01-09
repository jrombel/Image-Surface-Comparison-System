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
        Photo photo;
        Photo photoOrginal;
        int selectedTool;
        int selectedMode;
        bool brushDown = false;

        Point deltaHand;
        Point originHand;

        int brushShape = 0;
        double scale = 1;

        List<Point> points;
        Line lineTmp;
        bool selectedPolygonState = true;

        Photo undoPhoto;

        double renderCenterX;
        double renderCenterY;

        bool displacement = false;

        public Calculation()
        {
            InitializeComponent();

            selectedTool = 0;
            selectedMode = 0;

            photoDegreeTolerance_s.Value = 20;
            opacity_s.Value = 1;
            brushSize_s.Value = 10;

            points = new List<Point>();

            lineTmp = new Line();
            lineTmp.Margin = new Thickness(0, 0, 0, 0);
            lineTmp.Stroke = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, Base.selectedColor.red, Base.selectedColor.green, Base.selectedColor.blue));
            lineTmp.StrokeThickness = 0.5;
            canvas_c.Children.Add(lineTmp);

            undoPhoto = new Photo();
        }

        private void Photo_Click(object sender, RoutedEventArgs e)
        {
            //LoadImage();
        }

        private void Image_img_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            if (image_img.Source.ToString() == "pack://application:,,,/Image Surface Comparison System;component/Resources/loadPhoto_img.png")
            {
                //LoadImage();
                return;
            }

            double x = Math.Floor(e.GetPosition(image_img).X * photo.photo.PixelWidth / image_img.ActualWidth);
            double y = Math.Floor(e.GetPosition(image_img).Y * photo.photo.PixelHeight / image_img.ActualHeight);

            int index = photo.GetIndex((int)x, (int)y);
            Color selected = null;

            if (selectedTool == 0)
            {
                WandTool wandTool;
                selected = new Color(photoOrginal.GetColor(index));

                wandTool = new WandTool(photo, photoOrginal, selectedMode, (int)x, (int)y, (bool)photoDegreeToleranceAdjacent_cb.IsChecked, selected, (byte)photoDegreeTolerance_s.Value);
                image_img.Source = wandTool.Wand().photo;
                Base.Save(photo, lastFilename);

                if (selectedMode == 0)
                {
                    selectionAddMode_rb.IsChecked = true;
                    selectedMode = 1;
                }
            }
            else if (selectedTool == 1)
            {
                BrushTool brushTool;

                brushTool = new BrushTool(photo, photoOrginal, selectedMode, (int)x, (int)y, Int32.Parse(brushSize_tb.Text), brushShape);
                image_img.Source = brushTool.Brush().photo;

                Base.Save(photo, lastFilename);

                if (selectedMode == 0)
                {
                    selectionAddMode_rb.IsChecked = true;
                    selectedMode = 1;
                }

                brushDown = true;
            }
            else if (selectedTool == 2 && selectedPolygonState == true)
            {
                AddPoint(e.GetPosition(canvas_c).X, e.GetPosition(canvas_c).Y);
            }
            else if (selectedTool == 3)
            {
                image_img.CaptureMouse();
                originHand = e.GetPosition(image_img);

            }
            else if (selectedTool == 4)
            {
                if (magnifierZoomInMode.IsChecked == true)
                {
                    image_img.RenderTransform = null;

                    var position = e.MouseDevice.GetPosition(image_img);

                    scale += 0.1;

                    renderCenterX = position.X;
                    renderCenterY = position.Y;

                    image_img.RenderTransform = new ScaleTransform(scale, scale, renderCenterX, renderCenterY);
                    imageOrginal_img.RenderTransform = new ScaleTransform(scale, scale, renderCenterX, renderCenterY);
                    canvas_c.RenderTransform = new ScaleTransform(scale, scale, renderCenterX, renderCenterY);
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
                        canvas_c.RenderTransform = new ScaleTransform(scale, scale, position.X, position.Y);
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

                double x = Math.Floor(e.GetPosition(image_img).X * photo.photo.PixelWidth / image_img.ActualWidth);
                double y = Math.Floor(e.GetPosition(image_img).Y * photo.photo.PixelHeight / image_img.ActualHeight);

                if (Mouse.LeftButton == MouseButtonState.Pressed)
                {
                    BrushTool brushTool;
                    brushTool = new BrushTool(photo, photoOrginal, selectedMode, (int)x, (int)y, Int32.Parse(brushSize_tb.Text), brushShape);
                    image_img.Source = brushTool.Brush().photo;
                }
                else
                {
                    brushDown = false;
                }
            }
            else if (selectedTool == 2)
            {
                if (selectedPolygonState == true && points.Count > 0)
                {
                    lineTmp.X2 = e.GetPosition(canvas_c).X;
                    lineTmp.Y2 = e.GetPosition(canvas_c).Y;
                }
            }

            else if (selectedTool == 3)
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

            if (photo != null)
            {
                double allPixels = photo.width * photo.height;
                degree_l.Content = string.Format("{0:0.00}", (((double)(photo.selectedPixelsCount) / allPixels)) * 100) + "% \nSelected: " + photo.selectedPixelsCount + "pixels\nAll: " + allPixels + "pixels";
            }
        }

        private void canvas_c_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (selectedTool == 2 && selectedPolygonState == true)
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
                else if (selectedTool == 3)
                    Cursor = ((TextBlock)this.Resources["CursorGrab"]).Cursor;
                else if (selectedTool == 4)
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

            renderCenterX = position.X;
            renderCenterY = position.Y;

            image_img.RenderTransform = new ScaleTransform(scale, scale, renderCenterX, renderCenterY);
            imageOrginal_img.RenderTransform = new ScaleTransform(scale, scale, renderCenterX, renderCenterY);
            canvas_c.RenderTransform = new ScaleTransform(scale, scale, renderCenterX, renderCenterY);
            magnifierZoom_l.Content = scale * 100 + "%";
            if (selectedTool == 1)
                Mouse.OverrideCursor = CreateCursor(Int32.Parse(brushSize_tb.Text) * scale * (image_img.ActualHeight / photo.photo.PixelHeight), Int32.Parse(brushSize_tb.Text) * scale * (image_img.ActualWidth / photo.photo.PixelWidth));
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
                photoOrginal = null;
                photo = null;
            }
            photo_cb.ItemsSource = Base.photos;
        }

        string lastFilename = "";

        private void photo_cb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lastFilename != "" && photo != null)
                Base.Save(photo, lastFilename);

            String photoCounter;
            if (photo_cb.SelectedValue != null)
            {
                string filename = Base.path + "\\Photos\\" + album_cb.SelectedValue + "\\" + photo_cb.SelectedValue;
                lastFilename = album_cb.SelectedValue + "\\" + photo_cb.SelectedValue;

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

            string[] words = lastFilename.Split('\\');
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
                    photo.selectedPixelsCount = Int32.Parse(sr.ReadLine());
                    line = sr.ReadLine();

                    for (int y = 0; y < photo.height; y++)
                    {
                        line = sr.ReadLine();
                        string[] parts = line.Split(' ');
                        for (int x = 0; x < photo.width; x++)
                        {
                            if (parts[x] == "0")
                                photo.selectedPixels[x, y] = false;
                            else
                            {
                                photo.selectedPixels[x, y] = true;
                                int index;
                                index = photo.GetIndex(x, y);
                                photo.pixelData[index] = Color.ToUint(Base.selectedColor);
                            }
                        }
                    }
                    image_img.Source = photo.photo;

                }
            }
            photo.photo.WritePixels(new Int32Rect(0, 0, (int)photo.width, (int)photo.height), photo.pixelData, photo.widthInByte, 0);
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

        private void canvas_c_MouseMove(object sender, MouseEventArgs e)
        {
            if (selectedTool == 2)
            {
                if (selectedPolygonState == true && points.Count > 0)
                {
                    lineTmp.X2 = e.GetPosition(canvas_c).X;
                    lineTmp.Y2 = e.GetPosition(canvas_c).Y;
                }
            }
        }

        private void selectPolygon_b_Click(object sender, RoutedEventArgs e)
        {
            PolygonTool brushTool;

            brushTool = new PolygonTool(photo, photoOrginal, selectedMode, points, image_img.ActualWidth, image_img.ActualHeight);
            image_img.Source = brushTool.Polygon().photo;

            Base.Save(photo, lastFilename);

            if (selectedMode == 0)
            {
                selectionAddMode_rb.IsChecked = true;
                selectedMode = 1;
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
            if (undoPhoto != null && photo != null)
            {
                photo.Clone(undoPhoto);
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
            for (int y = 0; y < photo.height; y++)
            {
                for (int x = 0; x < photo.width; x++)
                {
                    if (photo.selectedPixels[x, y] == true)
                    {
                        int index;
                        index = photo.GetIndex(x, y);
                        photo.pixelData[index] = Color.ToUint(Base.selectedColor);
                    }
                }
            }

            photo.photo.WritePixels(new Int32Rect(0, 0, (int)photo.width, (int)photo.height), photo.pixelData, photo.widthInByte, 0);
            image_img.Source = photo.photo;
        }

        private void photoProcessing_btn(object sender, RoutedEventArgs e)
        {
            if (photo != null)
            {
                undoPhoto.Clone(photo);

                if (photoProcessing_cb.SelectedIndex == 0) //Filtering
                {

                    if (filteringPhotoProcessing.SelectedIndex == 0)
                        photo = PhotoProcessing.Smoothing(photoOrginal);
                    else if (filteringPhotoProcessing.SelectedIndex == 1)
                        photo = PhotoProcessing.Median(photoOrginal);
                    else if (filteringPhotoProcessing.SelectedIndex == 2)
                        photo = PhotoProcessing.EdgeDetect(photoOrginal);
                    else if (filteringPhotoProcessing.SelectedIndex == 3)
                        photo = PhotoProcessing.HighPassSharpening(photoOrginal);
                    else if (filteringPhotoProcessing.SelectedIndex == 4)
                        photo = PhotoProcessing.GaussianBlur(photoOrginal);

                }
                else if (photoProcessing_cb.SelectedIndex == 1) //Binaryzation
                {
                    photo = PhotoProcessing.Binaryzation(photoOrginal, (int)manuallyValue_s.Value);
                }
                //else if (photoProcessing_cb.SelectedIndex == 2) //Morphology
                //{
                //    if (morphologyPhotoProcessing.SelectedIndex == 0)
                //        photo = PhotoProcessing.Dilation(photoOrginal);
                //    else if (morphologyPhotoProcessing.SelectedIndex == 1)
                //        photo = PhotoProcessing.Erosion(photoOrginal);
                //    else if (morphologyPhotoProcessing.SelectedIndex == 2)
                //        photo = PhotoProcessing.Opening(photoOrginal);
                //    else if (morphologyPhotoProcessing.SelectedIndex == 3)
                //        photo = PhotoProcessing.Closing(photoOrginal);
                //}

                //photoOrginal = photo;
                ReloadPhoto();
                //wyświetlanie obrazu w image_img
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