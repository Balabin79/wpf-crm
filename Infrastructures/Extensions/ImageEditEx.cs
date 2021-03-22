using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Dental.Infrastructures.Extensions
{
    public class ImageEditEx : ImageEdit
    {
        public static readonly DependencyProperty ImagePathProperty = DependencyProperty.Register("ImagePath",
            typeof(string), typeof(ImageEditEx), null);


        public static readonly DependencyProperty OpenFileDialogFilterProperty = DependencyProperty.Register("OpenFileDialogFilter",
            typeof(string), typeof(ImageEditEx), null);


        public string ImagePath
        {
            get { return (string)GetValue(ImagePathProperty); }
            set { SetValue(ImagePathProperty, value); }
        }

        public string OpenFileDialogFilter
        {
            get { return (string)GetValue(OpenFileDialogFilterProperty); }
            set { SetValue(OpenFileDialogFilterProperty, value); }
        }

        protected override void LoadCore()
        {
            if (Image == null)
                return;

            ImageSource image = LoadImage();
            if (image != null) EditStrategy.SetImage(image);
        }

        private ImageSource LoadImage()
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = OpenFileDialogFilter ?? EditorLocalizer.GetString(EditorStringId.ImageEdit_OpenFileFilter);
            if (dialog.ShowDialog() == true)
            {
                using (Stream stream = dialog.OpenFile())
                {
                    if (stream is FileStream)
                    {
                        ImagePath = ((FileStream)stream).Name;
                        Source = new BitmapImage(new Uri(ImagePath));                       
                    }                                           
                    var ms = new MemoryStream(stream.GetDataFromStream());
                    return ImageHelper.CreateImageFromStream(ms);
                }
            }
            return null;
        }
    }
}
