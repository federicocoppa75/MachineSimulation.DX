using MaterialRemoval.Models;

namespace MaterialRemoval.Messages
{
    public class SectionRoutToolMoveMessage
    {
        public int XSectionIndex { get; set; }
        public int YSectionIndex { get; set; }
        public ImplicitRouting Rout { get; set; }
    }
}
