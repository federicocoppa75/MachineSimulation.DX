using CncViewer.Connection.Enums;

namespace CncViewer.Connection.ViewModels.Links
{
    public class TwoPosLinkViewModel : LinkViewModel<bool>
    {
        public override LinkType Type => LinkType.TwoPos;

        public TwoPosLinkViewModel(int id, string variable) : base(id, variable)
        {
        }
    }
}
