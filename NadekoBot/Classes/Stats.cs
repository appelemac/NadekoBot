using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;


namespace NadekoBot.Classes
{
    public class Stats
    {
        public string BotVersion = $"{typeof(NadekoClient).GetTypeInfo().Assembly.GetName().Name} v{typeof(NadekoClient).GetTypeInfo().Assembly.GetName().Version}";
        private int commandsRan = 0;
        private string statsCache = "";
        private readonly Stopwatch statsStopwatch = new Stopwatch();

        public int ServerCount { get; private set; } = 0;
        public int TextChannelsCount { get; private set; } = 0;
        public int VoiceChannelsCount { get; private set; } = 0;

        private static ulong messageCounter = 0;
        public static ulong MessageCounter => messageCounter;


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

        public override string ToString()
        {
            return string.Format(@"
`Author: Kwoth` `Library: Discord.Net`
`Bot Version: {0}`
`Owners' Ids:` {1}
`Uptime: {2}`
`Servers: {3}` | `TextChannels: {4}` | `VoiceChannels {5}`
`Commands ran this seesion: {6}`
`Message queue size: {7}`
`Greeted {8} times.` | `Playing {9} songs, {10} queued.`
`Messages: {11} ({12}/sec)` `Heap: {13}`
");
        }

    }
}
