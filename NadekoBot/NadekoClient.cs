using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NadekoBot
{
    public class NadekoClient
    {
        public static DiscordSocketClient SocketClient { get; private set; }
        public static CommandService Commands { get; set; }
        public static Models.Credentials Creds { get; set; }
        public static string BotMention { get; set; } = "";
        public static string DataDir { get; set; } = "";

        public static void Main(string[] args)
        {
            DataDir = Directory.GetParent(Assembly.GetEntryAssembly().Location).CreateSubdirectory("data").ToString();
            Console.OutputEncoding = Encoding.Unicode;
            InitializeCredentials();
            SocketClient = new DiscordSocketClient(new Discord.DiscordSocketConfig()
            {
                AudioMode = Discord.Audio.AudioMode.Disabled,
                MessageCacheSize = 0,
                LogLevel = Discord.LogSeverity.Verbose,
            });
            Commands = new CommandService();
            new NadekoClient().Start().GetAwaiter().GetResult();
        }

        private async Task Start()
        {
            await SocketClient.LoginAsync(Discord.TokenType.Bot, Creds.Token);
            await SocketClient.ConnectAsync();
            await InstallCommands();
            await Task.Delay(-1);
        }

        private async Task InstallCommands()
        {
            SocketClient.MessageReceived += HandleMessage;
            await Commands.LoadAssembly(Assembly.GetEntryAssembly());
        }

        private async Task HandleMessage(IMessage msg)
        {
            // Internal integer, marks where the command begins
            int argPos = 0;
            if (IsCommand(msg, ref argPos))
            {
                Console.WriteLine($@"Command: {msg.Content}");
                var result = await Commands.Execute(msg, argPos);
                if (!result.IsSuccess)
                {
                    Console.WriteLine($"Command {msg.Content} failed: {result.ErrorReason}");
                }
            }
        }

        private bool IsCommand(IMessage msg, ref int argPos)
        {
            foreach (var prefix in new[] { "nadeko!" })
            {
                if (msg.HasStringPrefix(prefix, ref argPos)) return true;
            }
            return false;
        }

        private static void InitializeCredentials()
        {
            try
            {
                File.WriteAllText(Path.Combine(DataDir,"credentials_example.json"), JsonConvert.SerializeObject(new Models.Credentials(), Formatting.Indented));
            }
            catch (Exception e)
            {
                Console.WriteLine(Strings.NadekoClient_Main_WritingExampleOfCredentialsFailed0, e.Message);
            }
            try
            {
                Creds = JsonConvert.DeserializeObject<Models.Credentials>(File.ReadAllText(Path.Combine(DataDir,"credentials.json")));
            }
            catch (Exception e)
            {
                Console.WriteLine(NadekoBot.Strings.NadekoClient_InitializeCredentials_CouldNotInitializeCredentialsFromCredentialsJson0Quitting, e.Message);
                Console.Read();
                Environment.Exit(1);
            }
            if (string.IsNullOrWhiteSpace(Creds.Token))
            {
                Console.WriteLine(NadekoBot.Strings.NadekoClient_InitializeCredentials_CouldNotReadTokenQuitting);
                Console.Read();
                Environment.Exit(1);
            }
            BotMention = $"<@{Creds.BotId}>";
        }
    }
}