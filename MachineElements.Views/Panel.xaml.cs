using MachineElements.ViewModels.Panel;
using System.Windows.Controls;

namespace MachineElements.Views
{
    /// <summary>
    /// Logica di interazione per Panel.xaml
    /// </summary>
    public partial class Panel : UserControl
    {
        public Panel()
        {
            InitializeComponent();

            DataContext = new PanelHoldersManagerViewModel();
        }
    }
}
