using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace RaspiTemp
{
    internal static class Tools
    {
        private static NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Получаем значение из настроек.
        /// </summary>
        public static string? ReadSetting(string key)
        {
            string result = string.Empty;
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                result = appSettings[key] ?? string.Empty;               
            }
            catch (ConfigurationErrorsException)
            {
                Logger.Warn("Error reading app settings.");
            }

            return !string.IsNullOrEmpty(result) ? result : null;
        }
    }
}
