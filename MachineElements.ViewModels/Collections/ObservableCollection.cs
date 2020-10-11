
using PropertyChangedEventArgs = System.ComponentModel.PropertyChangedEventArgs;
using NotifyCollectionChangedEventArgs = System.Collections.Specialized.NotifyCollectionChangedEventArgs;
using INotifierController = MachineElements.ViewModels.Interfaces.Collections.INotifierController;

namespace MachineElements.ViewModels.Collections
{
    public class ObservableCollection<T> : System.Collections.ObjectModel.ObservableCollection<T>, INotifierController
    {
        public bool EnableNotify { get; set; } = true;

        public ObservableCollection() : base()
        {
        }

        public ObservableCollection(System.Collections.Generic.IEnumerable<T> collection) : base(collection)
        {
        }

        public ObservableCollection(System.Collections.Generic.List<T> list) : base (list)
        {
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if(EnableNotify) base.OnCollectionChanged(e);
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if(EnableNotify) base.OnPropertyChanged(e);
        }
    }
}
