using System.ComponentModel;

namespace MachineElements.ViewModels.Enums
{
    public enum ToolHolderType
    {
        Static,         // cambio utensile manuale
        AutoSource,     // porta utensile (posizione magazzino)
        AutoSink        // mandrino con cambio utensile automatico
    }
}
