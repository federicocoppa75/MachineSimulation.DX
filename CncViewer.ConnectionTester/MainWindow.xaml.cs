using CncViewer.Connection.Helpers;
using CncViewer.ConnectionTester.ViewModels;
using System.Windows;

namespace CncViewer.ConnectionTester
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainViewModel();
        }
    }
}
