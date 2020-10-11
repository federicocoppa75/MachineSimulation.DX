using MachineElements.ViewModels.ToolChange;
using System.Windows.Controls;

namespace MachineElements.Views
{
    /// <summary>
    /// Logica di interazione per Toolchange.xaml
    /// </summary>
    public partial class Toolchange : UserControl
    {
        public Toolchange()
        {
            InitializeComponent();

            DataContext = new ToolChangeViewModel();
        }
    }
}
