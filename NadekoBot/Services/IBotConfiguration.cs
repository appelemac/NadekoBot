using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NadekoBot.Services
{
    public interface IBotConfiguration
    {
        bool DontJoinServers { get; set; }
        bool ForwardMessages { get; set; }
        bool ForwardToAllOwners { get; set; }

        bool RotatePlayingStatus { get; set; }
        List<string> PlayingStatuses { get; set; }

        ulong BufferSize { get; set; }
        List<string> RaceAnimals { get; set; }
        string RemindMessageFormat { get; set; }


        HashSet<ulong> BlacklistedServers { get; set; }
        HashSet<ulong> BlacklistedChannels { get; set; }
        HashSet<ulong> BlacklistedUsers { get; set; }

        List<string> EightBallResponses { get; set; }
        Currency Currency { get; set; }
        string ExternalCommandsFolder { get; set; }
        IModulePrefixes ModulePrefixes { get; set; }
    }

    public class Currency
    {
        public string Sign { get; set; }
        public string Name { get; set; }
        public string PluralName { get; set; }
    }


    public interface IModulePrefixes
    {
        string Administration { get; set; }
        string Searches { get; set; }
        string NSFW { get; set; }
        string Conversations { get; set; }
        string ClashOfClans { get; set; }
        string Help { get; set; }
        string Music { get; set; }
        string Trello { get; set; }
        string Games { get; set; }
        string Gambling { get; set; }
        string Permissions { get; set; }
        string Programming { get; set; }
        string Pokemon { get; set; }
        string Utility { get; set; }
    }

}