using MachineElements.ViewModels.Inserters;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MachineElements.Views
{
    /// <summary>
    /// Logica di interazione per Injectors.xaml
    /// </summary>
    public partial class Injectors : UserControl
    {
        public Injectors()
        {
            InitializeComponent();

            DataContext = new InjectorManagersViewModel();
        }
    }
}
