using MachineElements.ViewModels.Colliders;

namespace MachineElements.ViewModels.Messages.Panel
{
    public class UnhookPanelMessage
    {
        /// <summary>
        /// elemento macchina a cui agganciare il pannello.
        /// </summary>
        public IPanelHooker Hooker { get; set; }
    }
}
