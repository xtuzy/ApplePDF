using ApplePDF.Demo.Desktop.Windows.Extension;
using ApplePDF.PdfKit;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Xamarin.Essentials;
using System.Drawing.Imaging;
using System.IO;
using System.Drawing;
using System.Diagnostics;
using SharpConstraintLayout.Wpf;
using PDFiumCore;
using System.Collections.Generic;
using System;

namespace ApplePDF.Demo.Desktop.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            Content = new PdfViewerPage();
        }
    }
}
