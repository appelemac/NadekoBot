using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NadekoBot.Classes
{
    public class Stats
    {
        public int ServerCount { get; set; }
        private Logger _logger;

        public Stats(Logger logger)
        {
            _logger = logger;
            NadekoClient.SocketClient.Log += SocketClient_Log;
            
        }

        private async Task SocketClient_Log(Discord.LogMessage arg)
        {
            switch (arg.Severity)
            {
                case Discord.LogSeverity.Critical:
                    _logger.LogCritical(arg.Message);
                    break;
                case Discord.LogSeverity.Error:
                    _logger.LogError(arg.Message);
                    break;
                case Discord.LogSeverity.Warning:
                    _logger.LogWarning(arg.Message);
                    break;
                case Discord.LogSeverity.Info:
                    _logger.LogInformation(arg.Message);
                    break;
                default:
                    _logger.LogDebug(arg.Message);
                    break;
            }
            await Task.Delay(0);
        }
    }
}
