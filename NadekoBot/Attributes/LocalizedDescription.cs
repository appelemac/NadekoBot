using Discord.Commands;
using System.Runtime.CompilerServices;

namespace NadekoBot.Attributes
{
    public class LocalizedDescriptionAttribute : DescriptionAttribute
    {
        public LocalizedDescriptionAttribute([CallerMemberName] string memberName = "") : base(Strings.ResourceManager.GetString(memberName.ToLowerInvariant() + "_description"))
        {

        }
    }
}