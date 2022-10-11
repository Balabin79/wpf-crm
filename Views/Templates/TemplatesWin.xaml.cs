using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Dental.Views.Templates
{
    public partial class TemplatesWin : Window
    {
        public TemplatesWin()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty TitleWinProp = DependencyProperty.Register(
            "TitleWin",
            typeof(string),
            typeof(TemplatesWin)
        );

        public string TitleWin
        {
            get => GetValue(TitleWinProp).ToString();
            set => SetValue(TitleWinProp, value);
        }
    }
}
