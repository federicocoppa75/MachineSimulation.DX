using GalaSoft.MvvmLight;
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
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;
using Material = HelixToolkit.Wpf.SharpDX.Material;

namespace MachineElements.Views.PropertyGrid.PropertyEditors
{
    /// <summary>
    /// Logica di interazione per MaterialPicker.xaml
    /// </summary>
    public partial class MaterialPicker : UserControl//, ITypeEditor
    {
        public Material Value
        {
            get { return (Material)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(Material), typeof(MaterialPicker), new PropertyMetadata(null, (d, e) =>
            {
                //var mp = d as MaterialPicker;

                //if(e.NewValue != null)
                //{
                //    var materials = GetMaterials();
                //    var name = (e.NewValue as Material).Name;
                //    var sm = materials.Where((m) => string.Compare(name, m.Name) == 0).FirstOrDefault();

                //    if(sm != null)
                //    {
                //        mp.combo.SelectedItem = sm;
                //    }
                //    else
                //    {
                //        mp.combo.SelectedItem = null;
                //    }
                //}
                //else
                //{
                //    mp.combo.SelectedItem = null;
                //}

            }));


        public MaterialPicker()
        {
            InitializeComponent();

            combo.SelectionChanged += OnComboSelectionChanged;

            combo.ItemsSource = GetMaterials();
        }

        private static List<Material> GetMaterials() => HelixToolkit.Wpf.SharpDX.PhongMaterials.Materials.Cast<Material>().ToList();


        private void OnComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

    }
}
