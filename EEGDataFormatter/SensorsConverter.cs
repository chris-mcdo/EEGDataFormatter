using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EEGDataFormatter
{
    class SensorsConverter : JsonConverter
    {

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(List<Sensor>);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            // deserialize as object

            var sensors = serializer.Deserialize<JObject>(reader);
            var sensorList = new List<Sensor>();

            // create an array out of the properties
            foreach (JProperty jSensor in sensors.Properties())
            {
                if (ValidateJSensor(jSensor)){
                    var sensor = jSensor.Value.ToObject<Sensor>();
                    sensor.Name = jSensor.Name;
                    sensorList.Add(sensor);
                }
            }

            return sensorList;
        }

        public bool ValidateJSensor(JProperty jSensor)
        {
            return !(jSensor.Name.Equals("Z") || jSensor.Name.Equals("Unknown"));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }


    }
}
