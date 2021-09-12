using System;
using ACE.Entity.Enum;
using ACE.Server.Managers;
using ACE.Server.Network;
using ACE.Server.Network.GameMessages.Messages;
using ACE.Server.WorldObjects;
using ACE.Entity.Enum.Properties;
using ACE.Server.DuskfallMods;
using System.Collections.Generic;

namespace ACE.Server.Command.Handlers
{
    public static class DuskfallAdminCommands
    {
        [CommandHandler("raiserefundto", AccessLevel.Admin, CommandHandlerFlag.None, 1, "Refunds costs associated with /raise.", "/raiserefund [*|name|id]")]
        public static void HandleRaiseRefundTo(Session session, params string[] parameters)
        {
            //Todo: Handle offline players by adding properties directly to the helper?
            //Refund all players
            if (parameters[0] == "*")
            {
                //PlayerManager.GetAllPlayers().ForEach(p => DuskfallRaise.RaiseRefundToPlayer((Player)p));
                PlayerManager.GetAllOnline().ForEach(p => DuskfallRaise.RaiseRefundToPlayer(p));
                return;
            }

            //Refund by name/ID
            //var player = PlayerManager.FindByName(parameters[0]) as Player;
            var player = PlayerManager.GetOnlinePlayer(parameters[0]);
            if (player == null)
            {
                ChatPacket.SendServerMessage(session, $"No player {parameters[0]} found.", ChatMessageType.Broadcast);
                return;
            }

            DuskfallRaise.RaiseRefundToPlayer(player);
        }
    }
}
