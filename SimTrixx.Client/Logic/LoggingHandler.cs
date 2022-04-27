using System;
using Serilog;
using SimTrixx.Common.Enums;

namespace SimTrixx.Client.Logic
{
    public class LoggingHandler
    {
        public LoggingHandler(string currentDomainBaseDirectory)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File($@"{currentDomainBaseDirectory}Logs\Log-{DateTime.Now.Month}-{DateTime.Now.Day}-{DateTime.Now.Year}.txt")
                .CreateLogger();
        }

        public void LogIt(LogType severity, string message)
        {
            switch (severity)
            {
                case LogType.Information:
                    Log.Logger.Information(message);
                    break;
                case LogType.Error:
                    Log.Logger.Error(message);
                    break;
                case LogType.Warning:
                    Log.Logger.Warning(message);
                    break;
                default:
                    Log.Logger.Information(message);
                    break;
            }
        }
    }
}
