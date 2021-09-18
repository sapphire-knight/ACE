using ACE.Common;
using ACE.Entity;
using ACE.Entity.Enum;
using ACE.Entity.Enum.Properties;
using ACE.Server.Command.Handlers;
using ACE.Server.ACX;
using ACE.Server.Network;
using ACE.Server.Network.GameMessages;
using ACE.Server.Network.GameMessages.Messages;
using ACE.Server.ACX.CustomZones;

namespace ACE.Server.WorldObjects
{
    partial class Player
    {
        public void HandleOnPortalBehavior()
        {
            if (RaiseRefundOnLogin)
            {
                ChatPacket.SendServerMessage(this.Session, $"A request to refund resources from /raise was sent while you were logged off.", ChatMessageType.Broadcast);
                ACXRaise.RaiseRefundToPlayer(this);
                RaiseRefundOnLogin = false;
            }

            //if (this.Location.IsDungeon())
            //{
            //    ChatPacket.SendServerMessage(this.Session, $"God on dungeon test.", ChatMessageType.Broadcast);
            //    ACXAdminCommands.GodMode(this.Session);
            //}
            //else
            //{
            //    ChatPacket.SendServerMessage(this.Session, $"Ungod on exit dungeon test.", ChatMessageType.Broadcast);
            //    ACXAdminCommands.UngodMode(this.Session);

            //}
        }
    }
}
