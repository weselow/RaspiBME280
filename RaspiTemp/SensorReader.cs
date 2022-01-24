using System;
using System.Collections.Generic;
using System.Device.I2c;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Iot.Device.Bmxx80;
using Iot.Device.Bmxx80.PowerMode;

namespace RaspiTemp
{
    /// <summary>
    /// Класс по получению данных с датчика.
    /// </summary>
    /// <remarks>Подробнее: <seealso href="https://docs.microsoft.com/ru-ru/dotnet/iot/tutorials/temp-sensor"/></remarks>
    internal static class SensorReader
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public static SensorData Read()
        {
            /* Для i2cSettings задан новый экземпляр I2cConnectionSettings. Конструктор задает параметру busId значение 1, 
             * а параметру deviceAddress — значение Bme280.DefaultI2cAddress.
             * Важно!
             * Некоторые производители коммутационных плат BME280 используют значение дополнительного адреса. 
             * Для этих устройств используйте Bme280.SecondaryI2cAddress.
             */
            var i2cSettings = new I2cConnectionSettings(1, Bme280.DefaultI2cAddress);
            using I2cDevice i2cDevice = I2cDevice.Create(i2cSettings);
            using var bme280 = new Bme280(i2cDevice);

            //Время, необходимое микросхеме для выполнения измерений с текущими параметрами микросхемы
            int measurementTime = bme280.GetMeasurementDuration();

            //Задает режим питания Bmx280PowerMode.Forced.
            //В этом случае микросхема проводит одно измерение, сохраняет результаты, а затем переходит в спящий режим.
            bme280.SetPowerMode(Bmx280PowerMode.Forced);
            Thread.Sleep(measurementTime);

            //Считывает значения температуры, давления, влажности и высоты над уровнем моря.
            SensorData result = new();
            bme280.TryReadTemperature(out var tempValue);
            result.Temperature = tempValue;
            bme280.TryReadPressure(out var preValue);
            result.Pressure = preValue;
            bme280.TryReadHumidity(out var humValue);
            result.Humidity = humValue;
            bme280.TryReadAltitude(out var altValue);
            result.EstimatedAltitude = altValue;

            //string msg = $"Temperature: {tempValue.DegreesCelsius:0.#}";
            //Logger.Info(msg);
            //Console.WriteLine($"Pressure: {preValue.Hectopascals:#.##} hPa");
            //Console.WriteLine($"Relative humidity: {humValue.Percent:#.##}%");
            //Console.WriteLine($"Estimated altitude: {altValue.Meters:#} m");

            return result;
        }
    }
}
