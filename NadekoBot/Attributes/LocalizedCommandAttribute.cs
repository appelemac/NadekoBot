using Discord.Commands;
using NadekoBot.Classes;
using System.Runtime.CompilerServices;

namespace NadekoBot.Attributes
{
    public class LocalizedCommandAttribute : CommandAttribute
    {
        public LocalizedCommandAttribute([CallerMemberName] string memberName = "") : base(Localization.LoadString(memberName.ToLowerInvariant() + "_text"))
        {

        }
    }
}