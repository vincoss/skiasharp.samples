using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkiaSharpSamples.SkiaSharpHelpers
{
    class TouchManipulationInfo
    {
        public SKPoint PreviousPoint { set; get; }

        public SKPoint NewPoint { set; get; }
    }
}
