using MachineSteps.Models;
using MachineSteps.Plugins.IsoParser;
using System;

namespace MachineElement.Model.IO
{
    public static class IsoStepsLoader
    {
        public static MachineStepsDocument LoadAndParse(string fileName, bool traceOut = false, Func<int, Tuple<double, double>> getLinkLimits = null)
        {
            return IsoParser.Parse(fileName, traceOut, getLinkLimits);
        }
    }
}
