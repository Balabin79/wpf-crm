using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Grid;
using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Dental.Views.HandbooksPages
{
    /// <summary>
    /// Логика взаимодействия для Organizations.xaml
    /// </summary>
    public partial class Organizations : Page
    {
        public Organizations()
        {
            InitializeComponent();

        }

        private void _image_Validate(object sender, DevExpress.Xpf.Editors.ValidationEventArgs e)
        {
            return;
            //=((Dental.Infrastructures.Extensions.ImageEditEx)e.Source).ImagePath;
        }

        private void GridColumn_Loaded(object sender, RoutedEventArgs e)
        {
            int x = 0;
        }

        private void PART_Editor_ConvertEditValue(DependencyObject sender, DevExpress.Xpf.Editors.ConvertEditValueEventArgs args)
        {/*
           var img = new BitmapImage(new Uri(ImagePath));

            ImageConverter converter = new ImageConverter();
            ((DevExpress.Xpf.Editors.BaseEdit)sender).EditValue = (byte[])converter.ConvertTo(
                ((DevExpress.Xpf.Editors.ImageEdit)sender).Source, 
                typeof(byte[]));*/

            int x = 0;
        }
    }
}
