using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NadekoBot.Classes
{
    public class Prompt
    {
        public delegate void OnResponse(object source, MessageEventArgs e);

        private ITextChannel channel;
        private IEnumerable<IUser> targets;
        private string[] _possibilities;
        public Prompt(ITextChannel channel, IEnumerable<IUser> targets, params string[] possible_responses)
        {
            this.channel = channel;
            this.targets = targets;
            _possibilities = possible_responses;
            NadekoClient.SocketClient.MessageReceived += HandleMessage;
        }
        public async Task HandleMessage(IMessage msg)
        {
            if (msg.Channel.Id != channel.Id) return;

        }
    }

    public class MessageEventArgs : EventArgs
    {
        private IMessage EventMessage;
        public MessageEventArgs(IMessage msg)
        {
            EventMessage = msg;
        }

    }
}
