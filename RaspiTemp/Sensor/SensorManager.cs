using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace RaspiTemp.Sensor
{
    public static class SensorManager
    {
        private static NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static async Task<bool> RunAsync(CancellationToken token)
        {
            await Task.Run(() => true);

            var maxTem = Convert.ToInt32(Tools.ReadSetting("MaxTemperature") ?? "40");
            var alarmDelaySeconds = Convert.ToInt32(Tools.ReadSetting("AlarmSendEverySecondsDelay") ?? "60");
            var messageSecondsDelay = Convert.ToInt32(Tools.ReadSetting("MessageSecondsDelay") ?? "3");
            Logger.Warn("Max Air Temperature to Control: {maxTem}", maxTem);
            Logger.Warn("Alarm sends every {alarmDelaySeconds} seconds.", alarmDelaySeconds);

            //раз в минуту проверяем данные
            //если превысили температуру  - то каждые 10 минут шлем уведомления в телеграмм
            //до тех пор, пока температура не снизится до заданной
            int counter = 0;
            while (!token.IsCancellationRequested)
            {
                var data = SensorReader.Read();
                if (data.Temperature.DegreesCelsius > maxTem && counter % alarmDelaySeconds == 0)
                {
                    //время отправлять сообщение
                    Logger.Error("CRITICAL AIR TEMPERATURE: {temperature}", data.Temperature.DegreesCelsius);
                }
                else if (counter % messageSecondsDelay == 0)
                {
                    var msg = $"{DateTime.Now.ToLongTimeString} - Air Temperature: {data.Temperature.DegreesCelsius:0.#}";
                    Logger.Info(msg);
                }

                Thread.Sleep(1000);
                counter++;
            }

            return true;
        }

        
    }
}
