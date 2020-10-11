using GalaSoft.MvvmLight.Threading;
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
using MColor = System.Windows.Media.Color;
using DColor = System.Drawing.Color;
using System.ComponentModel;

namespace CncViewer
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

            UpdateFromSettings();
        }

        private void UpdateFromSettings()
        {
            var vm = DataContext as MainViewModel;

            vm.IsPanelHolderVisible = Properties.Settings.Default.ShowPanelHolder;
            vm.IsCollidersVisible = Properties.Settings.Default.ShowCollider;
            vm.IsFPSActive = Properties.Settings.Default.ShowFPS;
            vm.IsTriangleNumberActive = Properties.Settings.Default.ShowTrianglesNumber;
            vm.IsFrameDetailsActive = Properties.Settings.Default.ShowFrameDetails;
            vm.IsEnabledSelectionByView = Properties.Settings.Default.SelectByView;
            vm.DynamicTransition = Properties.Settings.Default.DynamicTransition;
            vm.AutoStepOver = Properties.Settings.Default.AutoStepOver;
            vm.MaterialRemoval = Properties.Settings.Default.MaterialRemoval;
            vm.BackgroundStartColor = Convert(Properties.Settings.Default.BackgroundStartColor);
            vm.BackgroundStopColor = Convert(Properties.Settings.Default.BackgroundStopColor);
            vm.LoadLastEnvironmentAtStartup = Properties.Settings.Default.LoadLastEnvironmentAtStartup;
            vm.LastEnvironmentAtStartup = Properties.Settings.Default.LastEnvironmentAtStartup;
            vm.IsCameraInfoActive = Properties.Settings.Default.ShowCameraInfo;

            LightTypeFromSettingsToViewModel();

            if (vm.LoadLastEnvironmentAtStartup)
            {
                vm.OpenLastEnvironment();
            }
        }

        private void SaveToSettings()
        {
            var vm = DataContext as MainViewModel;

            Properties.Settings.Default.ShowPanelHolder = vm.IsPanelHolderVisible;
            Properties.Settings.Default.ShowCollider = vm.IsCollidersVisible;
            Properties.Settings.Default.ShowFPS = vm.IsFPSActive;
            Properties.Settings.Default.ShowTrianglesNumber = vm.IsTriangleNumberActive;
            Properties.Settings.Default.ShowFrameDetails = vm.IsFrameDetailsActive;
            Properties.Settings.Default.SelectByView = vm.IsEnabledSelectionByView;
            Properties.Settings.Default.DynamicTransition = vm.DynamicTransition;
            Properties.Settings.Default.AutoStepOver = vm.AutoStepOver;
            Properties.Settings.Default.MaterialRemoval = vm.MaterialRemoval;
            Properties.Settings.Default.BackgroundStartColor = Convert(vm.BackgroundStartColor);
            Properties.Settings.Default.BackgroundStopColor = Convert(vm.BackgroundStopColor);
            Properties.Settings.Default.LoadLastEnvironmentAtStartup = vm.LoadLastEnvironmentAtStartup;
            Properties.Settings.Default.LastEnvironmentAtStartup = vm.LastEnvironmentAtStartup;
            Properties.Settings.Default.ShowCameraInfo = vm.IsCameraInfoActive;

            LightTypeFromViewModelToSettings();

        }

        private void LightTypeFromSettingsToViewModel()
        {
            var vm = DataContext as MainViewModel;

            switch (Properties.Settings.Default.LightType)
            {
                case 0: vm.IsDefaultLights = true; break;
                case 1: vm.IsDefaultLights2 = true; break;
                case 2: vm.IsSunLight = true; break;
                case 3: vm.IsSpotHeadLight = true; break;
                default: break;
            }
        }

        private void LightTypeFromViewModelToSettings()
        {
            var vm = DataContext as MainViewModel;

            if (vm.IsDefaultLights) Properties.Settings.Default.LightType = 0;
            else if (vm.IsDefaultLights2) Properties.Settings.Default.LightType = 1;
            else if (vm.IsSunLight) Properties.Settings.Default.LightType = 2;
            else if (vm.IsSpotHeadLight) Properties.Settings.Default.LightType = 3;
        }


        private static MColor Convert(DColor color)
        {
            return new MColor()
            {
                A = color.A,
                B = color.B,
                G = color.G,
                R = color.R,
            };
        }

        private static DColor Convert(MColor color) => DColor.FromArgb(color.A, color.R, color.G, color.B);

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            SaveToSettings();
            Properties.Settings.Default.Save();
        }
    }
}
