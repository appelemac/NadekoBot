using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NadekoBot.Classes
{
    public class Localization
    {
        public static string LoadString(string key)
        {
            return Strings.ResourceManager.GetString(key) ?? string.Concat(key.TakeWhile(c=>c!= '_'));
        }
    }
}
