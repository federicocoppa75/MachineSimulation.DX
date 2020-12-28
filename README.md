# MachineSimulation.DX
This solution includes projects for visualizing CNC machine movements similar to those of the [**MachineEditor**](https://github.com/federicocoppa75/MachineEditor) project (the same files are used to describe the machine), but the display is done via SharpDx [**HelixToolkit.Wpf.SharpDX**](https://www.nuget.org/packages/HelixToolkit.Wpf.SharpDX/2.13.1), this allows an increase in [performance](https://github.com/helix-toolkit/helix-toolkit/wiki/WPF-vs-WPF.SharpDX).
### Externl Dipendencies
This solution depends on some modules generated by the [**MachineEditor**](https://github.com/federicocoppa75/MachineEditor) solution:
* MachineModels.dll
* MachineModels.IO.dll
* MachineSteps.Models.dll
* MachineSteps.Plugins.IsoConverterBase.dll
* MachineSteps.Plugins.IsoIstructions.dll
* MachineSteps.Plugins.IsoParser.dll

These modules are downloaded from nuget repository (try with "Restore nuget packages" command).


## CncViewer
Application to view the machine movements connected to the CNC.
## CncViewer.ConfigEditor
Editor to establish the relationships between CNC variables and machine model links.

<!-- ## CncViewer.Connection -->
<!-- ## CncViewer.Connection.Bridge -->

## CncViewer.ConnectionTester
Test application for reading CNC variables.

<!-- ## CncViewer.Connecton.View
## CncViewer.Models
## MachineElement.Model.IO
## MachineElements.Models
## MachineElements.ViewModels
## MachineElements.ViewModels.Interfaces
## MachineElements.Views
## MachineSteps.Models.IO
## MachineSteps.ViewModels
## MachineSteps.Views -->

## MachineViewer
Application to view the movements expressed by the ISO file.

[![](http://img.youtube.com/vi/bw8Z2V1w3zQ/0.jpg)](http://www.youtube.com/watch?v=bw8Z2V1w3zQ "MachineView")
<!-- ## MachineViewer.Helpers
## MaterialRemoval
## Tooling.Models
## Tooling.Models.IO
## Tools.Models -->
