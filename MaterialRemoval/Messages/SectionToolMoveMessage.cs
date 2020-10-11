using MaterialRemoval.Models;

namespace MaterialRemoval.Messages
{
    public class SectionToolMoveMessage
    {
        public int XSectionIndex { get; set; }
        public int YSectionIndex { get; set; }
        public ImplicitToolBase Tool { get; set; }
    }
}
