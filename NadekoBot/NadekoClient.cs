using Discord;
using Discord.Commands;
using Discord.WebSocket;
using NadekoBot.Classes;
using NadekoBot.Database;
using NadekoBot.Models.DB;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace NadekoBot
{
    public class NadekoClient
    {
        public static DiscordSocketClient SocketClient { get; private set; }
        public static Stats NadekoStats { get; set; }
        public static CommandService Commands { get; set; }
        public static Models.Credentials Creds { get; set; }
        public static string BotMention { get; set; } = "";
        public static string DataDir { get; set; } = "";
        private static Logger _logger { get; set; }
        public static ADataBase DB { get; set; }
        private static List<Discord.Commands.Module> _externalModules { get; set; } = new List<Discord.Commands.Module>();
        internal static ExternalCommands.ExternalCommands ExternalCommands { get; set; }

        public static void Main(string[] args)
        {
            Init();

            SocketClient = new DiscordSocketClient(new Discord.DiscordSocketConfig()
            {
                AudioMode = Discord.Audio.AudioMode.Disabled,
                MessageCacheSize = 0,
                LogLevel = Discord.LogSeverity.Warning,
            });
            NadekoStats = new Stats(new Logger());
            Commands = new CommandService();
            new NadekoClient().Start().GetAwaiter().GetResult();
        }

        private static void Init()
        {
            _logger = new Logger();
            DataDir = Directory.GetParent(Assembly.GetEntryAssembly().Location).CreateSubdirectory("data").ToString();
            Console.OutputEncoding = Encoding.Unicode;

            InitializeCredentials();
            BotMention = $"<@{Creds.BotId}>";
            DB = new MySQLiteDB();

        }

        public static async Task InitializeExternalCommands()
        {
            string path = Path.Combine(DataDir, "External Commands");
            Directory.CreateDirectory(Path.Combine(path, "References"));
            if (Directory.EnumerateFiles(path).Any())
            {
                _logger.LogDebug("Compiling and installing custom commands");
                ExternalCommands = new NadekoBot.ExternalCommands.ExternalCommands(path);
                var assembly = ExternalCommands.getResultingAssembly();
                if (assembly == null) _logger.LogError("Not loading external commands");
                else
                {
                    if (_externalModules.Any())
                    {
                        foreach (var mod in _externalModules)
                        {
                           await Commands.Unload(mod);
                        }
                        _externalModules.Clear();
                    }
                    var temp = Commands.Modules;
                    await Commands.LoadAssembly(assembly);
                    _externalModules = Commands.Modules.Except(temp).ToList();
                }
                _logger.LogDebug("done installing external commands");
            }
        }

        private async Task Start()
        {
            _logger.LogInformation(NadekoBot.Strings.NadekoClient_Start_ConnectingToDiscord);
            await SocketClient.LoginAsync(Discord.TokenType.Bot, Creds.Token);
            await SocketClient.ConnectAsync();
            _logger.LogInformation(NadekoBot.Strings.NadekoClient_Start_Connected);
            await InstallCommands();
            await InitializeExternalCommands();
            _logger.LogInformation(NadekoBot.Strings.NadekoClient_Start_ReadyToReceiveCommands);

            await Task.Delay(-1);
        }

        private async Task InstallCommands()
        {
            SocketClient.MessageReceived += HandleMessage;
            await Commands.LoadAssembly(Assembly.GetEntryAssembly());
        }

        private  Task HandleMessage(IMessage msg)
        {
            // Internal integer, marks where the command begins
            int argPos = 0;
            if (IsCommand(msg, ref argPos))
            {
                _logger.LogInformation($@"Command: {msg.Content}");
                #pragma warning disable CS4014 //Cause it's what I want :D
                Task.Run(async () => {
                    var result = await Commands.Execute(msg, argPos);
                    if (!result.IsSuccess) _logger.LogError("Execution of {0} failed: {1}", msg, result.ErrorReason);
                });
                
            }
            return Task.FromResult(true);
        }

        private bool IsCommand(IMessage msg, ref int argPos)
        {
            foreach (var prefix in new[] { "!" })
            {
                if (msg.HasStringPrefix(prefix, ref argPos)) return true;
            }
            return false;
        }

        private static void InitializeCredentials()
        {
            try
            {
                File.WriteAllText(Path.Combine(DataDir, "credentials_example.json"), JsonConvert.SerializeObject(new Models.Credentials(), Formatting.Indented));
            }
            catch (Exception e)
            {
                _logger.LogError(Strings.NadekoClient_Main_WritingExampleOfCredentialsFailed0, e.Message);
            }
            try
            {
                Creds = JsonConvert.DeserializeObject<Models.Credentials>(File.ReadAllText(Path.Combine(DataDir, "credentials.json")));
            }
            catch (Exception e)
            {
                _logger.LogCritical(NadekoBot.Strings.NadekoClient_InitializeCredentials_CouldNotInitializeCredentialsFromCredentialsJson0Quitting, e.Message);
                Console.Read();
                Environment.Exit(1);
            }
            if (string.IsNullOrWhiteSpace(Creds.Token))
            {
                _logger.LogCritical(NadekoBot.Strings.NadekoClient_InitializeCredentials_CouldNotReadTokenQuitting);
                Console.Read();
                Environment.Exit(1);
            }

        }

        internal static bool IsOwner(ulong id) => Creds.OwnerIds.Contains(id);
    }
}