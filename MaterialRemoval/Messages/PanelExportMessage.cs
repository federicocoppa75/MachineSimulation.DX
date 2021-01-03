using HelixToolkit.Wpf.SharpDX;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaterialRemoval.Messages
{
    public class PanelExportMessage
    {
        public Action<Geometry3D> AddSectionGeometry { get; set; }
    }
}
