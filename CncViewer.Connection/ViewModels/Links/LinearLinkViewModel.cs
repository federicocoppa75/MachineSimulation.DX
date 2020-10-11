using CncViewer.Connection.Enums;

namespace CncViewer.Connection.ViewModels.Links
{
    public class LinearLinkViewModel : LinkViewModel<double>
    {

        public override LinkType Type => LinkType.Linear;

        public LinearLinkViewModel(int id, string variable) : base(id, variable)
        {
        }
    }
}
