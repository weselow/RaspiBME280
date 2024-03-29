﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Device.Gpio;

namespace RaspiTemp.Cpu
{
    internal static class CpuManager
    {
        private static NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private static string temperatureFilePath = "/sys/class/thermal/thermal_zone0/temp";

        public static async Task<bool> RunAsync(CancellationToken token)
        {
            await Task.Run(() => true);

            //проверить наличие файла
            if (!File.Exists(temperatureFilePath))
            {
                Logger.Error("CPU Temperature File was not found: {path}.", temperatureFilePath);
                return false;
            }

            //прочитать критическую температуру из настроек
            var cpuMaxTemp = Convert.ToInt32(Tools.ReadSetting("CpuMaxTemperature") ?? "50");
            var cpuCoolerPin = Convert.ToInt32(Tools.ReadSetting("CpuFanPin") ?? "14");
            var alarmDelaySeconds = Convert.ToInt32(Tools.ReadSetting("AlarmSendEverySecondsDelay") ?? "60");
            bool ifAllow = Tools.ReadSetting("AllowCpuFanController")?.ToLower() == "true";

            //проверяем, а надо ли вообще контролировать вентилятор
            if (!ifAllow)
            {
                Logger.Error("Allow CPU Fan is set to FALSE!");
                return false;
            }

            using var controller = new GpioController();
            controller.OpenPin(cpuCoolerPin, PinMode.Output);
            int counter = 0;
            while (!token.IsCancellationRequested)
            {
                var ct = await GetCpuTemperature();
                controller.Write(cpuCoolerPin, (ct > cpuMaxTemp ? PinValue.High : PinValue.Low));

                if (ct > cpuMaxTemp)
                {
                    if (counter % alarmDelaySeconds == 0) { Logger.Error("CRITICAL CPU TEMPERATURE: {temperature}", ct); }
                    else { Logger.Info("CRITICAL CPU TEMPERATURE: {temperature}", ct); }
                }
                else
                {
                    var msg = $"CPU Temperature: {ct}";
                    Logger.Info(msg);
                }

                Thread.Sleep(1000);
                counter = counter > Int32.MaxValue - 5 ? 0 : counter + 1;
            }

            return true;
        }



        static async Task<int> GetCpuTemperature()
        {
            try
            {
                string temperatureData = await File.ReadAllTextAsync(temperatureFilePath);
                int temperature = int.Parse(temperatureData) / 1000; // Преобразование в градусы Цельсия                
                return temperature;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Ошибка при получении температуры процессора: {message}", ex.Message);
            }

            return -1;
        }
    }
}
