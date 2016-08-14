using Discord.Commands;
using NadekoBot.Classes;
using System.Runtime.CompilerServices;

namespace NadekoBot.Attributes
{
    public class LocalizedDescriptionAttribute : DescriptionAttribute
    {
        public LocalizedDescriptionAttribute([CallerMemberName] string memberName = "") : base(Localization.LoadString(memberName.ToLowerInvariant() + "_description"))
        {

        }
    }
}