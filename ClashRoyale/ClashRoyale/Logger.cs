using System;
using System.IO;
using NLog;
using SharpRaven.Data;

namespace ClashRoyale
{
    public class Logger
    {
        private static NLog.Logger _logger;

        public Logger()
        {
            if (!Directory.Exists("Logs"))
                Directory.CreateDirectory("Logs");

            _logger = LogManager.GetCurrentClassLogger();
        }

        public static void Log(object message, Type type, ErrorLevel logType = ErrorLevel.Info)
        {
            switch (logType)
            {
                case ErrorLevel.Info:
                {
                    _logger.Info(message);

                    Console.WriteLine($"[{logType.ToString()}] {message}");
                    break;
                }

                case ErrorLevel.Warning:
                {
                    _logger.Warn(message);

                    if ( /*Configuration.Debug*/true)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkMagenta;
                        Console.WriteLine($"[{logType.ToString()}] {message}");
                        Console.ResetColor();
                    }

                    Resources.Sentry.Report(message.ToString(), type, logType);
                    break;
                }

                case ErrorLevel.Error:
                {
                    _logger.Error(message);

                    if ( /*Configuration.Debug*/true)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"[{logType.ToString()}] {message}");
                        Console.ResetColor();
                    }

                    Resources.Sentry.Report(message.ToString(), type, logType);
                    break;
                }

                case ErrorLevel.Debug:
                {
                    if ( /*Configuration.Debug*/true)
                    {
                        _logger.Debug(message);

                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine($"[{logType.ToString()}] {message}");
                        Console.ResetColor();
                    }

                    break;
                }
            }
        }
    }
}