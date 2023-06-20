// See https://aka.ms/new-console-template for more information
using RaspiTemp.Sensor;
using RaspiTemp.Cpu;

NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

Logger.Info("Hello, World!");

CancellationTokenSource cts = new CancellationTokenSource();

var sensorTask = SensorManager.RunAsync(cts.Token);
var cputask = CpuManager.RunAsync(cts.Token);

Console.CancelKeyPress += (sender, e) =>
{
    e.Cancel = true; // Отменяем стандартное поведение завершения программы
    Console.WriteLine("... Starting finishing program ...");
    cts.Cancel();
    Task.WaitAll(sensorTask, cputask);
    Environment.Exit(0); // Завершаем программу явно
};

Task.WaitAll(sensorTask, cputask);
Console.WriteLine("Program Finished!");
NLog.LogManager.Shutdown();