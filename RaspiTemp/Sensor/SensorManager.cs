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

            var maxTem = Convert.ToInt32(Tools.ReadSetting("MaxTemperature") ?? "30");
            var alarmDelaySeconds = Convert.ToInt32(Tools.ReadSetting("AlarmSendEverySecondsDelay") ?? "60");
            var messageSecondsDelay = Convert.ToInt32(Tools.ReadSetting("MessageSecondsDelay") ?? "3");
            bool ifAllowAirController = Tools.ReadSetting("AllowAirTempController")?.ToLower() == "true";

            if (!ifAllowAirController)
            {
                Logger.Error("Air Controller is set to FALSE!");
                return false;
            }


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
                        Logger.Error("Ошибка при получении данных по первичному или вторичному адресу I2C устройства.");
                        Console.WriteLine("=== Ошибка е1 ===");
                        Console.WriteLine(e1);
                        Console.WriteLine("=== ===");
                        Console.WriteLine("== Ошибка е2:");
                        Console.WriteLine(e2);
                        Console.WriteLine("=== ===");

                        return false;
                    }
                }

                if (data.Temperature.DegreesCelsius > maxTem)
                {
                    if (counter % alarmDelaySeconds == 0)
                    {
                        Logger.Error("CRITICAL AIR TEMPERATURE: {temperature}", Convert.ToInt32(data.Temperature.DegreesCelsius));
                    }
                    else 
                    {
                        Logger.Info("CRITICAL AIR TEMPERATURE: {temperature}", Convert.ToInt32(data.Temperature.DegreesCelsius));
                    }
                    
                }
                else
                {
                    var msg = $"Air Temperature: {Convert.ToInt32(data.Temperature.DegreesCelsius)}";
                    Logger.Info(msg);
                }

                Thread.Sleep(messageSecondsDelay * 1000);
                counter = counter > Int32.MaxValue - 5 ? 0 : counter + 1;
            }

            return true;
        }


    }
}
