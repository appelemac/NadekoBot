﻿using Discord;
using Discord.Commands;
using NadekoBot.Attributes;
using NadekoBot.Classes;
using NadekoBot.Services;
using NadekoBot.Services.Database.Models;
using NLog;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace NadekoBot.Modules.Administration
{
    public partial class Administration
    {
        [Group]
        public class ServerGreetCommands
        {
            public static long Greeted = 0;
            private Logger _log;

            public ServerGreetCommands()
            {
                NadekoBot.Client.UserJoined += UserJoined;
                NadekoBot.Client.UserLeft += UserLeft;
                _log = LogManager.GetCurrentClassLogger();
            }

            private Task UserLeft(IGuildUser user)
            {
                var leftTask = Task.Run(async () =>
                {
                    GuildConfig conf;
                    using (var uow = DbHandler.UnitOfWork())
                    {
                        conf = uow.GuildConfigs.For(user.Guild.Id);
                    }

                    if (!conf.SendChannelByeMessage) return;
                    var channel = (await user.Guild.GetTextChannelsAsync()).SingleOrDefault(c => c.Id == conf.ByeMessageChannelId);

                    if (channel == null) //maybe warn the server owner that the channel is missing
                        return;

                    var msg = conf.ChannelByeMessageText.Replace("%user%", "**" + user.Username + "**");
                    if (string.IsNullOrWhiteSpace(msg))
                        return;

                    var toDelete = await channel.SendMessageAsync(msg).ConfigureAwait(false);
                    if (conf.AutoDeleteByeMessages)
                    {
                        var t = Task.Run(async () =>
                        {
                            await Task.Delay(conf.AutoDeleteGreetMessagesTimer * 1000).ConfigureAwait(false); // 5 minutes
                            await toDelete.DeleteAsync().ConfigureAwait(false);
                        });
                    }
                });
                return Task.CompletedTask;
            }

            private Task UserJoined(IGuildUser user)
            {
                var joinedTask = Task.Run(async () =>
                {
                    GuildConfig conf;
                    using (var uow = DbHandler.UnitOfWork())
                    {
                        conf = uow.GuildConfigs.For(user.Guild.Id);
                    }

                    if (conf.SendChannelGreetMessage)
                    {
                        var channel = (await user.Guild.GetTextChannelsAsync()).SingleOrDefault(c => c.Id == conf.GreetMessageChannelId);
                        if (channel != null) //maybe warn the server owner that the channel is missing
                        {
                            var msg = conf.ChannelGreetMessageText.Replace("%user%", user.Username).Replace("%server%", user.Guild.Name);
                            if (!string.IsNullOrWhiteSpace(msg))
                            {
                                var toDelete = await channel.SendMessageAsync(msg).ConfigureAwait(false);
                                if (conf.AutoDeleteGreetMessages)
                                {
                                    var t = Task.Run(async () =>
                                    {
                                        await Task.Delay(conf.AutoDeleteGreetMessagesTimer * 1000).ConfigureAwait(false); // 5 minutes
                                        await toDelete.DeleteAsync().ConfigureAwait(false);
                                    });
                                }
                            }
                        }
                    }

                    if (conf.SendDmGreetMessage)
                    {
                        var channel = await user.CreateDMChannelAsync();

                        if (channel != null)
                        {
                            var msg = conf.DmGreetMessageText.Replace("%user%", user.Username).Replace("%server%", user.Guild.Name);
                            if (!string.IsNullOrWhiteSpace(msg))
                            {
                                await channel.SendMessageAsync(msg).ConfigureAwait(false);
                            }
                        }
                    }
                });
                return Task.CompletedTask;
            }

            [LocalizedCommand, LocalizedDescription, LocalizedSummary]
            [RequireContext(ContextType.Guild)]
            [RequirePermission(GuildPermission.ManageGuild)]
            public async Task GreetDel(IMessage imsg)
            {
                var channel = (ITextChannel)imsg.Channel;

                GuildConfig conf;
                using (var uow = DbHandler.UnitOfWork())
                {
                    conf = uow.GuildConfigs.For(channel.Guild.Id);
                    conf.AutoDeleteGreetMessages = !conf.AutoDeleteGreetMessages;
                    uow.GuildConfigs.Update(conf);
                    await uow.CompleteAsync();
                }

                if (conf.AutoDeleteGreetMessages)
                    await channel.SendMessageAsync("`Automatic deletion of greet messages has been enabled.`").ConfigureAwait(false);
                else
                    await channel.SendMessageAsync("`Automatic deletion of greet messages has been disabled.`").ConfigureAwait(false);
            }

            [LocalizedCommand, LocalizedDescription, LocalizedSummary]
            [RequireContext(ContextType.Guild)]
            [RequirePermission(GuildPermission.ManageGuild)]
            public async Task Greet(IMessage imsg)
            {
                var channel = (ITextChannel)imsg.Channel;

                GuildConfig conf;
                using (var uow = DbHandler.UnitOfWork())
                {
                    conf = uow.GuildConfigs.For(channel.Guild.Id);
                    conf.SendChannelGreetMessage = !conf.SendChannelGreetMessage;
                    conf.GreetMessageChannelId = channel.Id;
                    uow.GuildConfigs.Update(conf);
                    await uow.CompleteAsync();
                }

                if (conf.SendChannelGreetMessage)
                    await channel.SendMessageAsync("Greet announcements enabled on this channel.").ConfigureAwait(false);
                else
                    await channel.SendMessageAsync("Greet announcements disabled.").ConfigureAwait(false);
            }
            
            [LocalizedCommand, LocalizedDescription, LocalizedSummary]
            [RequireContext(ContextType.Guild)]
            [RequirePermission(GuildPermission.ManageGuild)]
            public async Task GreetMsg(IMessage imsg, [Remainder] string text)
            {
                var channel = (ITextChannel)imsg.Channel;

                GuildConfig conf;
                using (var uow = DbHandler.UnitOfWork())
                {
                    conf = uow.GuildConfigs.For(channel.Guild.Id);
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        conf.ChannelGreetMessageText = text;
                        uow.GuildConfigs.Update(conf);
                        await uow.CompleteAsync();
                    }
                }

                if (string.IsNullOrWhiteSpace(text))
                {
                    await channel.SendMessageAsync("`Current greet message:` " + conf.ChannelGreetMessageText);
                    return;
                }
                await channel.SendMessageAsync("New greet message set.").ConfigureAwait(false);
                if (!conf.SendChannelGreetMessage)
                    await channel.SendMessageAsync("Enable greet messsages by typing `.greet`").ConfigureAwait(false);
            }

            [LocalizedCommand, LocalizedDescription, LocalizedSummary]
            [RequireContext(ContextType.Guild)]
            [RequirePermission(GuildPermission.ManageGuild)]
            public async Task GreetDm(IMessage imsg)
            {
                var channel = (ITextChannel)imsg.Channel;

                GuildConfig conf;
                using (var uow = DbHandler.UnitOfWork())
                {
                    conf = uow.GuildConfigs.For(channel.Guild.Id);
                    conf.SendDmGreetMessage = !conf.SendDmGreetMessage;
                    uow.GuildConfigs.Update(conf);
                    await uow.CompleteAsync();
                }

                if (conf.SendDmGreetMessage)
                    await channel.SendMessageAsync("Greet announcements enabled on this channel.").ConfigureAwait(false);
                else
                    await channel.SendMessageAsync("Greet announcements disabled.").ConfigureAwait(false);
            }
            
            [LocalizedCommand, LocalizedDescription, LocalizedSummary]
            [RequireContext(ContextType.Guild)]
            [RequirePermission(GuildPermission.ManageGuild)]
            public async Task GreetDmMsg(IMessage imsg, [Remainder] string text)
            {
                var channel = (ITextChannel)imsg.Channel;

                GuildConfig conf;
                using (var uow = DbHandler.UnitOfWork())
                {
                    conf = uow.GuildConfigs.For(channel.Guild.Id);
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        conf.DmGreetMessageText = text;
                        uow.GuildConfigs.Update(conf);
                        await uow.CompleteAsync();
                    }
                }

                if (string.IsNullOrWhiteSpace(text))
                {
                    await channel.SendMessageAsync("`Current DM greet message:` " + conf.DmGreetMessageText);
                    return;
                }
                await channel.SendMessageAsync("New DM greet message set.").ConfigureAwait(false);
                if (!conf.SendDmGreetMessage)
                    await channel.SendMessageAsync("Enable DM greet messsages by typing `.greetdm`").ConfigureAwait(false);
            }

            [LocalizedCommand, LocalizedDescription, LocalizedSummary]
            [RequireContext(ContextType.Guild)]
            [RequirePermission(GuildPermission.ManageGuild)]
            public async Task Bye(IMessage imsg)
            {
                var channel = (ITextChannel)imsg.Channel;

                GuildConfig conf;
                using (var uow = DbHandler.UnitOfWork())
                {
                    conf = uow.GuildConfigs.For(channel.Guild.Id);
                    conf.SendChannelByeMessage = !conf.SendChannelByeMessage;
                    conf.ByeMessageChannelId = channel.Id;
                    uow.GuildConfigs.Update(conf);
                    await uow.CompleteAsync();
                }

                if (conf.SendChannelByeMessage)
                    await channel.SendMessageAsync("Bye announcements enabled on this channel.").ConfigureAwait(false);
                else
                    await channel.SendMessageAsync("Bye announcements disabled.").ConfigureAwait(false);
            }

            [LocalizedCommand, LocalizedDescription, LocalizedSummary]
            [RequireContext(ContextType.Guild)]
            [RequirePermission(GuildPermission.ManageGuild)]
            public async Task ByeMsg(IMessage imsg, [Remainder] string text)
            {
                var channel = (ITextChannel)imsg.Channel;

                GuildConfig conf;
                using (var uow = DbHandler.UnitOfWork())
                {
                    conf = uow.GuildConfigs.For(channel.Guild.Id);
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        conf.ChannelByeMessageText = text;
                        uow.GuildConfigs.Update(conf);
                        await uow.CompleteAsync();
                    }
                }

                if (string.IsNullOrWhiteSpace(text))
                {
                    await channel.SendMessageAsync("`Current bye message:` " + conf.ChannelGreetMessageText);
                    return;
                }
                await channel.SendMessageAsync("New bye message set.").ConfigureAwait(false);
                if (!conf.SendChannelByeMessage)
                    await channel.SendMessageAsync("Enable bye messsages by typing `.bye`").ConfigureAwait(false);
            }

            [LocalizedCommand, LocalizedDescription, LocalizedSummary]
            [RequireContext(ContextType.Guild)]
            [RequirePermission(GuildPermission.ManageGuild)]
            public async Task ByeDel(IMessage imsg)
            {
                var channel = (ITextChannel)imsg.Channel;

                GuildConfig conf;
                using (var uow = DbHandler.UnitOfWork())
                {
                    conf = uow.GuildConfigs.For(channel.Guild.Id);
                    conf.AutoDeleteByeMessages = !conf.AutoDeleteByeMessages;
                    uow.GuildConfigs.Update(conf);
                    await uow.CompleteAsync();
                }

                if (conf.AutoDeleteByeMessages)
                    await channel.SendMessageAsync("`Automatic deletion of bye messages has been enabled.`").ConfigureAwait(false);
                else
                    await channel.SendMessageAsync("`Automatic deletion of bye messages has been disabled.`").ConfigureAwait(false);
            }

        }
    }
}