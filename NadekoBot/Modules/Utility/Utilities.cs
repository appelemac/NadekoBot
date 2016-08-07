using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace NadekoBot.Modules.Utility
{
    [Module]
    public class Utilities
    {
        [Command("stats")]
        public static async Task StatsCommand(IMessage msg)
        {
            await msg.Channel.SendMessageAsync($@"Cool stuff:
```xl
Servers: {(await NadekoClient.SocketClient.GetGuildSummariesAsync()).Count}

```
");
        }
    }
}