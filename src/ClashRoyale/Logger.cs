using System;
using System.IO;
using NLog;
using SharpRaven.Data;

namespace ClashRoyale
{
    public class Logger
    {
#if DEBUG
        private static readonly object ConsoleSync = new object();
#endif

        private static NLog.Logger _logger;

        public Logger()
        {
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
#if DEBUG
                    lock (ConsoleSync)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkMagenta;
                        Console.WriteLine($"[{logType.ToString()}] {message}");
                        Console.ResetColor();
                    }

                    Resources.Sentry.Report(message.ToString(), type, logType);
#endif
                    break;
                }

                case ErrorLevel.Error:
                {
                    _logger.Error(message);
#if DEBUG

                    lock (ConsoleSync)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"[{logType.ToString()}] {message}");
                        Console.ResetColor();
                    }

                    Resources.Sentry.Report(message.ToString(), type, logType);
#endif
                    break;
                }

                case ErrorLevel.Debug:
                {
#if DEBUG
                    _logger.Debug(message);

                    lock (ConsoleSync)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                        Console.WriteLine($"[{logType.ToString()}] {message}");
                        Console.ResetColor();
                    }
#endif
                    break;
                }
            }
        }
    }
}