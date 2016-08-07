using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NadekoBot
{
    public class NadekoClient
    {
        public static Discord.API.DiscordSocketApiClient SocketClient { get; private set; }
        public static Models.Credentials Creds { get; set; }
        public static string BotMention { get; set; } = "";


        public static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.Unicode;
            InitializeCredentials();
            

        }

        private static void InitializeCredentials()
        {
            try
            {
                File.WriteAllText("credentials_example.json", JsonConvert.SerializeObject(new Models.Credentials(), Formatting.Indented));
            }
            catch (Exception e)
            {
                Console.WriteLine(Strings.NadekoClient_Main_WritingExampleOfCredentialsFailed0, e.Message);
            }
            try
            {
                Creds = JsonConvert.DeserializeObject<Models.Credentials>(File.ReadAllText("credentials.json"))
            } catch (Exception e)
            {
                Console.WriteLine("Could not initialize Credentials from credentials.json: {0}, quitting...", e.Message);
                Console.Read();
                Environment.Exit(1);
            }
        }
    }
}
