using System;
using System.Collections.Generic;
using System.Text;

namespace MaterialRemoval.Models
{
    public static class ImplicitFactoryHelper
    {
        static readonly double _tolerance = 0.0000000001;

        public static bool IsZero(double value) => (value <= _tolerance) && (value >= -(_tolerance));

    }
}
