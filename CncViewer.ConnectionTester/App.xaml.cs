using CncViewer.Connection.Interfaces;
using CncViewer.ConnectionTester.Implementation;
using GalaSoft.MvvmLight.Ioc;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace CncViewer.ConnectionTester
{
    /// <summary>
    /// Logica di interazione per App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            SimpleIoc.Default.Register<IVariableValueChangedObserver<bool>, BoolVariableValueChangedObserver>();
            SimpleIoc.Default.Register<IVariableValueChangedObserver<double>, DoubleVariableValueChangedObserver>();
        }
    }
}
