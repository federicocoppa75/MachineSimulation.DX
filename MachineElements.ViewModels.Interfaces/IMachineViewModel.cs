using MachineElements.ViewModels.Interfaces.Enums;
using Color = System.Windows.Media.Color;

namespace MachineElements.ViewModels.Interfaces
{
    public interface IMachineViewModel
    {
        LightType LightType { get; set; }
        bool ShowFrameDetails { get; set; }
        bool ShowTriangleCountInfo { get; set; }
        bool ShowFrameRate { get; set; }
        bool ShowCameraInfo { get; set; }
        bool EnableSelectionByView { get; set; }
        bool AddProbePoint { get; set; }
        IStepExecutionInfoProvider StepExecutionInfoProvider { get; set; }
        Color BackgroundStartColor { get; set; }
        Color BackgroundStopColor { get; set; }
    }
}
