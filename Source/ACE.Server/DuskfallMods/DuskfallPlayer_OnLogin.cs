using ACE.Common;
using ACE.Entity;
using ACE.Entity.Enum;
using ACE.Entity.Enum.Properties;
using ACE.Server.DuskfallMods;
using ACE.Server.Network;
using ACE.Server.Network.GameMessages;
using ACE.Server.Network.GameMessages.Messages;

namespace ACE.Server.WorldObjects
{
    partial class Player
    {
        public void HandleOnLoginBehavior()
        {
            if (RaiseRefundOnLogin)
            {
                ChatPacket.SendServerMessage(this.Session, $"A request to refund resources from /raise was sent while you were logged off.", ChatMessageType.Broadcast);
                DuskfallRaise.RaiseRefundToPlayer(this);
                RaiseRefundOnLogin = false;
            }
        }
    }
}
