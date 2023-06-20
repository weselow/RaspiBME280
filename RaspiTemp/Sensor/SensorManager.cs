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
            bool ifUsePrimaryAddress = true;
            while (!token.IsCancellationRequested)
            {
                SensorData data;
                try
                {
                    data = SensorReader.Read(ifUsePrimaryAddress);                    
                }
                catch (Exception e1)
                {
                    try
                    {
                        ifUsePrimaryAddress = !ifUsePrimaryAddress;
                        data = SensorReader.Read(ifUsePrimaryAddress);
                    }
                    catch (Exception e2)
                    {
                        Logger.Error("Ошибка при получении данных по первичному или вторичному адресу.");
                        Console.WriteLine("=== Ошибка е1 ===");
                        Console.WriteLine(e1);
                        Console.WriteLine("=== ===");
                        Console.WriteLine("== Ошибка е2:");
                        Console.WriteLine(e2);
                        Console.WriteLine("=== ===");

                        return false;
                    }                    
                }



                Console.WriteLine($"Air Temp: {data.Temperature.DegreesCelsius}.");
                if (data.Temperature.DegreesCelsius > maxTem && counter % alarmDelaySeconds == 0)
                {
                    //время отправлять сообщение
                    Logger.Error("CRITICAL AIR TEMPERATURE: {temperature}", data.Temperature.DegreesCelsius);
                }
                else if (counter % messageSecondsDelay == 0)
                {
                    var msg = $"Air Temperature: {data.Temperature.DegreesCelsius}";
                    Logger.Info(msg);
                }

                Thread.Sleep(1000);
                counter++;
            }

            return true;
        }

        
    }
}
