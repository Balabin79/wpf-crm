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

        private void PART_Editor_ConvertEditValue(DependencyObject sender, DevExpress.Xpf.Editors.ConvertEditValueEventArgs args)
        {
            if (args.ImageSource != null)
            {
                if (args.ImageSource is BitmapImage) return;
                    var ImageData = File.ReadAllBytes(((System.Windows.Media.Imaging.BitmapImage)args.ImageSource).UriSource.AbsolutePath);
                var stream = new MemoryStream(ImageData);
                /*var imageSource = new BitmapImage();
                imageSource.BeginInit();
                imageSource.StreamSource = stream;
                imageSource.EndInit();*/

                args.EditValue = ((MemoryStream)stream).ToArray();
                args.Handled = true;
                return;
                int x = 0;
                /*WriteableBitmap wb = new WriteableBitmap((BitmapSource)args.ImageSource);
                ImageTools.ExtendedImage ei = ImageTools.ImageExtensions.ToImage(wb);
                Stream stream = ImageTools.ImageExtensions.ToStream(ei);
                args.EditValue = ((MemoryStream)stream).ToArray();
                args.Handled = true;*/
            }
        
        }
    }
}
