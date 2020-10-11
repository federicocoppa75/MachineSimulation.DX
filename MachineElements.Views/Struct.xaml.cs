using MachineElements.ViewModels.Interfaces;
using System.Windows;
using System.Windows.Controls;

namespace MachineElements.Views
{
    /// <summary>
    /// Logica di interazione per Struct.xaml
    /// </summary>
    public partial class Struct : UserControl
    {
        private bool _waitForTreeviewSelectionChanged;

        public Struct()
        {
            InitializeComponent();
        }

        private void TreeViewItem_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (_waitForTreeviewSelectionChanged)
            {
                _waitForTreeviewSelectionChanged = false;
            }
            else
            {
                var item = sender as TreeViewItem;
                var dc = item.DataContext as IMachineElementViewModel;

                if ((dc != null) && dc.IsSelected)
                {
                    dc.IsSelected = false;
                }
            }
        }

        private void TreeViewItem_Selected(object sender, RoutedEventArgs e)
        {
            _waitForTreeviewSelectionChanged = true;
        }

    }
}
