﻿using Discord;
using Discord.Commands;
using NadekoBot.Attributes;
using NadekoBot.Services;
using NadekoBot.Services.Database;
using NadekoBot.Services.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NadekoBot.Modules.Utility
{
    public partial class Utility
    {
        [LocalizedCommand, LocalizedDescription, LocalizedSummary, LocalizedAlias]
        [RequireContext(ContextType.Guild)]
        public async Task ShowQuote(IUserMessage umsg, string keyword)
        {
            var channel = umsg.Channel as ITextChannel;

            if (string.IsNullOrWhiteSpace(keyword))
                return;

            keyword = keyword.ToUpperInvariant();

            Quote quote;
            using (var uow = DbHandler.Instance.GetUnitOfWork())
            {
                quote = await uow.Quotes.GetRandomQuoteByKeywordAsync(channel.Guild.Id, keyword).ConfigureAwait(false);
            }

            if (quote == null)
                return;

            await channel.SendMessageAsync("📣 " + quote.Text);
        }

        [LocalizedCommand, LocalizedDescription, LocalizedSummary, LocalizedAlias]
        [RequireContext(ContextType.Guild)]
        public async Task AddQuote(IUserMessage umsg, string keyword, [Remainder] string text)
        {
            var channel = umsg.Channel as ITextChannel;

            if (string.IsNullOrWhiteSpace(keyword) || string.IsNullOrWhiteSpace(text))
                return;

            keyword = keyword.ToUpperInvariant();

            using (var uow = DbHandler.UnitOfWork())
            {
                uow.Quotes.Add(new Quote
                {
                    AuthorId = umsg.Author.Id,
                    AuthorName = umsg.Author.Username,
                    GuildId = channel.Guild.Id,
                    Keyword = keyword,
                    Text = text,
                });
                await uow.CompleteAsync().ConfigureAwait(false);
                await channel.SendMessageAsync("`Quote added.`").ConfigureAwait(false);
            }
        }

        [LocalizedCommand, LocalizedDescription, LocalizedSummary, LocalizedAlias]
        [RequireContext(ContextType.Guild)]
        public async Task DeleteQuote(IUserMessage umsg, string keyword)
        {
            var channel = umsg.Channel as ITextChannel;

            if (string.IsNullOrWhiteSpace(keyword))
                return;

            keyword = keyword.ToUpperInvariant();

            using (var uow = DbHandler.UnitOfWork())
            {
                var q = await uow.Quotes.GetRandomQuoteByKeywordAsync(channel.Guild.Id, keyword);

                if (q == null)
                {
                    await channel.SendMessageAsync("`No quotes found.`");
                    return;
                }

                uow.Quotes.Remove(q);
                await uow.CompleteAsync();
            }
            await channel.SendMessageAsync("`Deleted a random quote.`");
        }

        [LocalizedCommand, LocalizedDescription, LocalizedSummary, LocalizedAlias]
        [RequireContext(ContextType.Guild)]
        public async Task DelAllQuotes(IUserMessage umsg, string keyword)
        {
            var channel = umsg.Channel as ITextChannel;

            if (string.IsNullOrWhiteSpace(keyword))
                return;

            keyword = keyword.ToUpperInvariant();

            using (var uow = DbHandler.UnitOfWork())
            {
                var quotes = uow.Quotes.GetAllQuotesByKeyword(keyword);

                uow.Quotes.RemoveRange(quotes.ToArray());//wtf?!

                await uow.CompleteAsync();
            }

            await channel.SendMessageAsync($"`Deleted all quotes with '{keyword}' keyword`");
        }
    }
}
