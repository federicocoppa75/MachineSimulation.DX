using CncViewer.Connection.Helpers;
using CncViewer.Connection.ViewModels.Links;
using System.Windows.Controls;

namespace CncViewer.Connecton.View
{
    /// <summary>
    /// Logica di interazione per LinksConnections.xaml
    /// </summary>
    public partial class LinksConnections : UserControl
    {
        public LinksConnections()
        {
            InitializeComponent();

            DataContext = new LinksConnectionsViewModel();

            VariableReadingEngine.InitializeInstance();
        }
    }
}
