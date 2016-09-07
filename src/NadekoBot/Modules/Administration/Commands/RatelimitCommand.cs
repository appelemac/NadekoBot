using Discord;
using Discord.Commands;
using Discord.WebSocket;
using NadekoBot.Attributes;
using NadekoBot.Extensions;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace NadekoBot.Modules.Administration
{
    public partial class Administration
    {
        [Group]
        public class RatelimitCommand
        {
            public static ConcurrentDictionary<ulong, Ratelimiter> RatelimitingChannels = new ConcurrentDictionary<ulong, Ratelimiter>();

            private DiscordSocketClient _client { get; }

            public class Ratelimiter
            {
                public class RatelimitedUser
                {
                    public ulong UserId { get; set; }
                    public int MessageCount { get; set; } = 0;
                }

                public ulong ChannelId { get; set; }

                public int MaxMessages { get; set; }
                public int PerSeconds { get; set; }

                public CancellationTokenSource cancelSource { get; set; } = new CancellationTokenSource();

                public ConcurrentDictionary<ulong, RatelimitedUser> Users { get; set; } = new ConcurrentDictionary<ulong, RatelimitedUser>();

                public bool CheckUserRatelimit(ulong id)
                {
                    RatelimitedUser usr = Users.GetOrAdd(id, (key) => new RatelimitedUser() { UserId = id });
                    if (usr.MessageCount == MaxMessages)
                    {
                        return true;
                    }
                    else
                    {
                        usr.MessageCount++;
                        var t = Task.Run(async () => {
                            try
                            {
                                await Task.Delay(PerSeconds * 1000, cancelSource.Token);
                            }
                            catch (OperationCanceledException) { }
                            usr.MessageCount--;
                        });
                        return false;
                    }

                }
            }

            public RatelimitCommand()
            {
                this._client = NadekoBot.Client;

               _client.MessageReceived += (umsg) =>
                {
                    var t = Task.Run(async () =>
                    {
                        var usrMsg = umsg as IUserMessage;
                        var channel = usrMsg.Channel as ITextChannel;

                        if (channel == null || await usrMsg.IsAuthor())
                            return;
                        Ratelimiter limiter;
                        if (!RatelimitingChannels.TryGetValue(channel.Id, out limiter))
                            return;

                        if (limiter.CheckUserRatelimit(usrMsg.Author.Id))
                            await usrMsg.DeleteAsync();
                    });
                    return Task.CompletedTask;
                };
            }

            [LocalizedCommand, LocalizedDescription, LocalizedSummary, LocalizedAlias]
            [RequireContext(ContextType.Guild)]
            public async Task Slowmode(IUserMessage umsg, int msg = 1, int perSec = 5)
            {
                var channel = (ITextChannel)umsg.Channel;

                Ratelimiter throwaway;
                if (RatelimitingChannels.TryRemove(channel.Id, out throwaway))
                {
                    throwaway.cancelSource.Cancel();
                    await channel.SendMessageAsync("`Slow mode disabled.`").ConfigureAwait(false);
                    return;
                }

                if (msg < 1 || perSec < 1)
                {
                    await channel.SendMessageAsync("`Invalid parameters.`");
                    return;
                }

                if (RatelimitingChannels.TryAdd(channel.Id,throwaway = new Ratelimiter() {
                    ChannelId = channel.Id,
                    MaxMessages = msg,
                    PerSeconds = perSec,
                }))
                {
                    await channel.SendMessageAsync("`Slow mode initiated.` " +
                                                $"Users can't send more than {throwaway.MaxMessages} message(s) every {throwaway.PerSeconds} second(s).")
                                                .ConfigureAwait(false);
                }
            }
        }
    }
}