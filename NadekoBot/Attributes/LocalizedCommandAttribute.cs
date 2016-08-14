using Discord.Commands;
using System.Runtime.CompilerServices;

namespace NadekoBot.Attributes
{
    public class LocalizedCommandAttribute : CommandAttribute
    {
        public LocalizedCommandAttribute([CallerMemberName] string memberName = "") : base(Strings.ResourceManager.GetString(memberName.ToLowerInvariant() + "_text"))
        {

        }
    }
}