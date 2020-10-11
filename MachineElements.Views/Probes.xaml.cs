﻿using MachineElements.ViewModels.Probing;
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
    /// Logica di interazione per Probes.xaml
    /// </summary>
    public partial class Probes : UserControl
    {
        public Probes()
        {
            InitializeComponent();

            DataContext = new ProbesViewModel();
        }
    }
}
