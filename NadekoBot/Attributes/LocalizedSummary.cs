using Discord.Commands;
using NadekoBot.Classes;
using System.Runtime.CompilerServices;

namespace NadekoBot.Attributes
{
    public class LocalizedSummaryAttribute : SummaryAttribute
    {
        public LocalizedSummaryAttribute([CallerMemberName] string memberName = "") : base(Localization.LoadString(memberName.ToLowerInvariant() + "_summary"))
        {

        }
    }
}