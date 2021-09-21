using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using Discord;
using ACE.Entity.Enum;
using ACE.Server.Network.GameMessages.Messages;
using ACE.Server.Managers;
using ACE.Server.Entity;
using System.Timers;
using System.Collections.Concurrent;

namespace ACE.Server.ACX.Discord
{
    public class DiscordRelay
    {
        //TurbineChatHandler.cs and GameMessageTurbineChat.cs are what's relevant
        //Todo: Filtering relevant messages.  White/blacklisting.

        //Supply credentials
        private const ulong RELAY_CHANNEL_ID = 877615436286546010;
        private const string BOT_TOKEN = "ODg1OTYzNjgyNDExMDc3NjYz.YTur-g.bM8JBx83QaKJ6f5gs1CZ4NKL3Hk";

        private static DiscordSocketClient discord;
        private static IMessageChannel channel;

        //Outgoing messages
        private static ConcurrentQueue<string> outgoingMessages;
        private static Timer messageTimer;
        private const int MAX_MESSAGE_LENGTH = 10000;
        private const double MESSAGE_INTERVAL = 10000;
        private const string PREFIX = "~";

        //Initialize in Program.cs or on first use?
        public async static void Initialize()
        {
            //Set up outgoing message queue
            outgoingMessages = new ConcurrentQueue<string>();
            messageTimer = new Timer
            {
                AutoReset = true,
                Enabled = false,
                Interval = MESSAGE_INTERVAL,
            };
            messageTimer.Elapsed += SendQueuedMessages;

            discord = new DiscordSocketClient();
            await discord.LoginAsync(TokenType.Bot, BOT_TOKEN);
            await discord.StartAsync();
            discord.Ready += OnReady;
        }

        //Finish initializing when logged in to Discord
        private static async Task OnReady()
        {
            //Grab the channel to be used for relaying messages
            channel = discord.GetChannel(RELAY_CHANNEL_ID) as IMessageChannel;
            if (channel == null)
            {
                //Handle errors starting up
                return;
            }

            //Set up relay
            discord.MessageReceived += OnDiscordChat;

            //Start ACE-->Discord timer
            messageTimer.Enabled = true;

            //Say hi
            QueueMessageForDiscord("Discord bot is online.");
        }

        //Batch messages going to Discord to help with rate limits
        private static void SendQueuedMessages(object sender, ElapsedEventArgs e)
        {
            if (channel is null)
                return;

            var batchedMessage = new StringBuilder();

            while (batchedMessage.Length < MAX_MESSAGE_LENGTH &&
                outgoingMessages.TryDequeue(out string message))
            {
                batchedMessage.AppendLine(message);
            }

            Task.Run(async () =>
            {
                await channel.SendMessageAsync(batchedMessage.ToString());
            });
        }

        //Relay messages from Discord
        private static Task OnDiscordChat(SocketMessage msg)
        {
            //Ignore bot chat and incorrect channels
            if (msg.Author.IsBot || msg.Channel.Id != RELAY_CHANNEL_ID)
                return Task.CompletedTask;

            //Check if the server has disabled general chat
            if (PropertyManager.GetBool("chat_disable_general").Item)
                return Task.CompletedTask;

            //Construct message
            var chatMessage = new GameMessageTurbineChat(
                ChatNetworkBlobType.NETBLOB_EVENT_BINARY,
                ChatNetworkBlobDispatchType.ASYNCMETHOD_SENDTOROOMBYNAME,
                TurbineChatChannel.General,
                PREFIX + msg.Author.Username, //Use prefix to filter out messages the relay is sending
                //"~Discord",
                msg.Content,
                0,
                ChatType.General);
            //var gameMessageTurbineChat = new GameMessageTurbineChat(ChatNetworkBlobType.NETBLOB_EVENT_BINARY, ChatNetworkBlobDispatchType.ASYNCMETHOD_SENDTOROOMBYNAME, adjustedChannelID, session.Player.Name, message, senderID, adjustedchatType);


            //Send a message to any player who is listening to general chat
            foreach (var recipient in PlayerManager.GetAllOnline())
            {
                // handle filters
                if (!recipient.GetCharacterOption(CharacterOption.ListenToGeneralChat))
                    return Task.CompletedTask;

                //Todo: think about how to handle squelches?
                //if (recipient.SquelchManager.Squelches.Contains(session.Player, ChatMessageType.AllChannels))
                //    continue;

                recipient.Session.Network.EnqueueSend(chatMessage);
            }

            return Task.CompletedTask;
        }

        //Called when a GameMessageTurbineChat is created to see if it should be sent to Discord
        public static void RelayIngameChat(string message, string senderName, ChatType chatType, uint channel, uint senderID, ChatNetworkBlobType chatNetworkBlobType, ChatNetworkBlobDispatchType chatNetworkBlobDispatchType)
        {
            if (message is null || senderName is null)
                return;
            if (senderName.StartsWith(PREFIX))
                return;

            if (chatType == ChatType.General || chatType == ChatType.LFG)
                QueueMessageForDiscord($"[{chatType}] {senderName}: {message}");
        }
        public static void QueueMessageForDiscord(string message)
        {
            outgoingMessages.Enqueue(message);
        }
    }
}
