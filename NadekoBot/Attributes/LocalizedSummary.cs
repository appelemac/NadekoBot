using Discord.Commands;
using System.Runtime.CompilerServices;

namespace NadekoBot.Attributes
{
    public class LocalizedSummaryAttribute : SummaryAttribute
    {
        public LocalizedSummaryAttribute([CallerMemberName] string memberName = "") : base(Strings.ResourceManager.GetString(memberName.ToLowerInvariant() + "_summary"))
        {

        }
    }
}