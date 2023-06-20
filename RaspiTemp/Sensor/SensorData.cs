using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaspiTemp.Sensor
{
    internal class SensorData
    {
        public UnitsNet.Temperature Temperature { get; set; }
        public UnitsNet.Pressure Pressure { get; set; }
        public UnitsNet.RelativeHumidity Humidity { get; set; }
        public UnitsNet.Length EstimatedAltitude { get; set; }

    }
}
