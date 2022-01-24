// See https://aka.ms/new-console-template for more information
using RaspiTemp;
using System.Configuration;

NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

Console.WriteLine("Hello, World!");
var maxTem = Convert.ToInt32(ReadSetting("MaxTemperature") ?? "40");
var alarmDelaySeconds = Convert.ToInt32(ReadSetting("AlarmSendEverySecondsDelay") ?? "60");
var messageSecondsDelay = Convert.ToInt32(ReadSetting("MessageSecondsDelay") ?? "3");
Console.WriteLine($"Max Temperature to Control: {maxTem}");
Console.WriteLine($"Alarm sends every {alarmDelaySeconds} seconds.");

//раз в минуту проверяем данные
//если превысили температуру  - то каждые 10 минут шлем уведомления в телеграмм
//до тех пор, пока температура не снизится до заданной
int counter = 0;
while (true)
{
    var data = SensorReader.Read();
    if (data.Temperature.DegreesCelsius > maxTem && counter % alarmDelaySeconds == 0)
    {
        //время отправлять сообщение
        Logger.Info("Критическая температура: {temperature}", data.Temperature.DegreesCelsius);
    }
    else if (counter % messageSecondsDelay == 0)
    {
        Console.WriteLine($"{DateTime.Now.ToLongTimeString} - Temperature: {data.Temperature.DegreesCelsius:0.#}");
    }

    Thread.Sleep(1000);
    counter++;
}


/// <summary>
/// Получаем максимальную температуру из настроек.
/// </summary>
string? ReadSetting(string key)
{
    string result = string.Empty;    
    try
    {
        var appSettings = ConfigurationManager.AppSettings;
        result = appSettings[key] ?? string.Empty;
        Console.WriteLine(result);
    }
    catch (ConfigurationErrorsException)
    {
        Logger.Warn("Error reading app settings");
    }

    return !string.IsNullOrEmpty(result) ? result : null;
}