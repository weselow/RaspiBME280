# RaspiBME280
Rasperberri Temperature Controller with Telegram Notifications.

## Схема подключения датчика

![Sensor connection](https://docs.microsoft.com/ru-ru/dotnet/iot/media/rpi-bmp280_i2c-thumb.png)

Ниже приведен перечень подключений от устройства Raspberry Pi к плате BME280:
- 3,3V к VIN или 3V3 (показано красным цветом)
- GND (заземление) к GND (черный)
- SDA (GPIO 2) к SDI или SDA (синий)
- SCL (GPIO 3) к SCK или SCL (оранжевый)

*См.[источник](https://docs.microsoft.com/ru-ru/dotnet/iot/tutorials/temp-sensor)*

## Настройки:
В файл `NLog.config` добавить значения:
 - botToken: `botToken="xxx"`
 - chatId: `chatId="xxx"`
 
 Для логгирования в Seq можно указать:
 - `serverUrl="xxx"` 
 - `apiKey="xxx"`.
