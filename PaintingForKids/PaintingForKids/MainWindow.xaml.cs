using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using static System.Net.Mime.MediaTypeNames;

namespace PaintingForKids
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // значения по умолчанию
            this.MyInkCanvas.EditingMode = InkCanvasEditingMode.Ink;
            this.inkRadio.IsChecked = true;
            this.comboColors.SelectedIndex = 0;
            this.MySlider.Value = 1;
            this.StylusCircle.StrokeThickness = 3;
            this.MyInkCanvas.UseCustomCursor = true;
            this.MyInkCanvas.Cursor = MyCursor();

            CurrentStylus = 0;
            CurrentColor = (this.comboColors.SelectedItem as StackPanel).Tag.ToString();
            CurrentSize = 10;
            ChangeStylusColorSize(CurrentStylus, CurrentColor, CurrentSize);
        }

        // перечисления и свойства
        // 
        // выбранный тип пера - круг, прямоугольник, овал
        private enum SelectedStylus
        {
            StylusCircle, StylusRect, StylusEllipse
        }
        private SelectedStylus CurrentStylus { get; set; }
        // выбранный цвет
        private string CurrentColor { get; set; }
        // выбранный рисунок
        private string? CurrentPicture { get; set; }
        // выбранный размер пера
        private int CurrentSize { get; set; }


        private void RadioButtonClicked(object sender, RoutedEventArgs e)
        {
            // В зависимости от того, какая кнопка отправила событие,
            // поместить InkCanvas в нужный режим оперирования.
            switch ((sender as RadioButton)?.Content.ToString())
            {
                // Эти строки должны совпадать со значениями свойства Content 
                // каждого элемента RadioButton.
                case "Ink":
                    this.MyInkCanvas.EditingMode = InkCanvasEditingMode.Ink;
                    break;
                case "Erase":
                    this.MyInkCanvas.EditingMode = InkCanvasEditingMode.EraseByStroke;
                    break;
                case "Select":
                    this.MyInkCanvas.EditingMode = InkCanvasEditingMode.Select;
                    break;
            }
        }

        // изменяется тип пера
        private void btnStylusCircle_Click(object sender, RoutedEventArgs e)
        {
            this.StylusCircle.StrokeThickness = 3;
            this.StylusRect.StrokeThickness = 1;
            this.StylusEllipse.StrokeThickness = 1;
            CurrentStylus = (SelectedStylus)(0);
            ChangeStylusColorSize(CurrentStylus, CurrentColor, CurrentSize);
        }

        private void btnStylusRect_Click(object sender, RoutedEventArgs e)
        {
            this.StylusCircle.StrokeThickness = 1;
            this.StylusRect.StrokeThickness = 3;
            this.StylusEllipse.StrokeThickness = 1;
            CurrentStylus = (SelectedStylus)(1);
            ChangeStylusColorSize(CurrentStylus, CurrentColor, CurrentSize);
        }
        private void btnStylusEllipse_Click(object sender, RoutedEventArgs e)
        {
            this.StylusCircle.StrokeThickness = 1;
            this.StylusRect.StrokeThickness = 1;
            this.StylusEllipse.StrokeThickness = 3;
            CurrentStylus = (SelectedStylus)(2);
            ChangeStylusColorSize(CurrentStylus, CurrentColor, CurrentSize);
        }

        // изменяется цвет
        private void ColorChanged(object sender, SelectionChangedEventArgs e)
        {
            CurrentColor = (this.comboColors.SelectedItem as StackPanel).Tag.ToString();
            ChangeStylusColorSize(CurrentStylus, CurrentColor, CurrentSize);
        }

        // изменяется размер пера
        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (this.MyInkCanvas != null)
            {
                CurrentSize = ((int)(MySlider.Value * 10));
                ChangeStylusColorSize(CurrentStylus, CurrentColor, CurrentSize);
            }
        }

        // выбирается картинка 
        private void PictureChanged(object sender, SelectionChangedEventArgs e)
        {
            CurrentPicture = (this.comboPictures.SelectedItem as StackPanel).Tag.ToString();
        }

        // устанавливается картинка 
        private void AddPicture_Click(object sender, RoutedEventArgs e)
        {
            MyInkCanvas.Children.Clear();
            System.Windows.Controls.Image newImage = new System.Windows.Controls.Image();
            newImage.Source = new BitmapImage(new Uri("/Resources/" + CurrentPicture + ".jpg", UriKind.Relative)); 
            newImage.Width = MyInkCanvas.Width;
            newImage.Height = MyInkCanvas.Height;
            MyInkCanvas.Children.Add(newImage);
        }

        // сохраняется рисунок
        private void SaveData(object sender, RoutedEventArgs e)
        {
            // Сохранить все данные InkCanvas в локальном файле.
            using (FileStream fs = new FileStream("StrokeData.bin", FileMode.Create))
            {
                this.MyInkCanvas.Strokes.Save(fs);
                fs.Close();
            }
        }

        // загружается рисунок
        private void LoadData(object sender, RoutedEventArgs e)
        {
            // Наполнить StrokeCollection из файла.
            using (FileStream fs = new FileStream("StrokeData.bin", FileMode.Open, FileAccess.Read))
            {
                StrokeCollection strokes = new StrokeCollection(fs);
                this.MyInkCanvas.Strokes = strokes;
            }
        }

        // очищается область для рисования
        private void Clear(object sender, RoutedEventArgs e)
        {
            // Очистить все штрихи.
            this.MyInkCanvas.Strokes.Clear();
        }

        // изменяется указатель мыши
        private Cursor MyCursor()
        {
            Cursor cursor = Cursors.Pen;
            return cursor;
        }

        // установка типа пера, цвета, размера
        private void ChangeStylusColorSize(SelectedStylus stylus, string color, int size)
        {
            // Изменить цвет, используемый для визуализации штрихов.
            this.MyInkCanvas.DefaultDrawingAttributes.Color = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(color);
            switch (stylus)
            {
                case SelectedStylus.StylusCircle:
                    {
                        this.MyInkCanvas.DefaultDrawingAttributes.StylusTip = StylusTip.Ellipse;
                        if (size > 0)
                        {
                            this.MyInkCanvas.DefaultDrawingAttributes.Height = size;
                            this.MyInkCanvas.DefaultDrawingAttributes.Width = size;
                        }
                        break;
                    }
                case SelectedStylus.StylusRect:
                    {
                        this.MyInkCanvas.DefaultDrawingAttributes.StylusTip = StylusTip.Rectangle;
                        if (size > 0)
                        {
                            this.MyInkCanvas.DefaultDrawingAttributes.Height = size;
                            this.MyInkCanvas.DefaultDrawingAttributes.Width = size;
                        }
                        break;
                    }
                case SelectedStylus.StylusEllipse:
                    {
                        this.MyInkCanvas.DefaultDrawingAttributes.StylusTip = StylusTip.Ellipse;
                        if (size > 0)
                        {
                            this.MyInkCanvas.DefaultDrawingAttributes.Height = size;
                            this.MyInkCanvas.DefaultDrawingAttributes.Width = size / 5;
                        }
                        break;
                    }
            }

        }

        private void MainWindow1_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            MyInkCanvas.Width = MainWindow1.Width;
        }

    }
}


