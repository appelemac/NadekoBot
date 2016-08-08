using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace NadekoBot.Classes
{
    public class Logger : ILogger
    {
        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotImplementedException();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            throw new NotImplementedException();
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            ConsoleColor fontColor;
            switch (logLevel)
            {
                case LogLevel.Error:
                    fontColor = ConsoleColor.Red;
                    break;
                default:
                    fontColor = ConsoleColor.White;
                    break;
            }
            Console.ForegroundColor = fontColor;
            Console.WriteLine(formatter.Invoke(state, exception));
        }

        public void LogInformation(string message, params object[] reps)
        {
            Log(ConsoleColor.Cyan, string.Format(message, reps));
        }

        public void LogError(string message, params object[] reps)
        {
            Log(ConsoleColor.Red, string.Format(message, reps));
        }

        public void LogWarning(string message, params object[] reps)
        {
            Log(ConsoleColor.Yellow, string.Format(message, reps));
        }

        public void LogCritical(string message, params object[] reps)
        {
            Log(ConsoleColor.DarkRed, string.Format(message, reps));
        }

        public void LogDebug(string message, params object[] reps)
        {
            Log(ConsoleColor.Green, string.Format(message, reps));
        }

        private void Log(ConsoleColor consoleColor, string message)
        {
            Console.ForegroundColor = consoleColor;
            Console.WriteLine(message);
        }
    }
}
