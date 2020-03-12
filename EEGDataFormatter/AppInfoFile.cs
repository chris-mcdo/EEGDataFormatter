using System;
using System.Collections.Generic;
using System.Text;

namespace EEGDataFormatter
{
    class AppInfoFile
    {
        public double DisplayWidth { get; set; }
        public double DisplayHeight { get; set; }
        public double AppHeight { get; set; }
        public double AppWidth { get; set; }
        public double ContentHeight { get; set; }
        public double ContentWidth { get; set; }
        public long SystemStartTime { get; set; }
        public long SystemEndTime { get; set; }
    }
}
