using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EEGDataFormatter
{
    class InputDataModel
    {
        public int Battery { get; set; }
        [JsonConverter(typeof(SensorsConverter))] public List<Sensor> Sensors { get; set; }
        public String SystemMillisecond { get; set; }
    }
}