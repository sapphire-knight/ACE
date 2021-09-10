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
        [CommandHandler("raiserefund", AccessLevel.Admin, CommandHandlerFlag.RequiresWorld, 0, "Refunds costs associated with /raise.", "/raiserefund [name|ID] [Raise target]")]
        public static void HandleRaiseRefund(Session session, params string[] parameters)
        {
            var player = session.Player;

            player.MaximumLuminance = long.MaxValue / 2;
            foreach (var target in Enum.GetValues<RaiseTarget>())
            {
                var level = target.GetLevel(player);
                var startLevel = target.StartingLevel();
                var timesLeveled = level - startLevel;

                //Check if anything invested
                if(timesLeveled < 1)
                {
                    continue;
                }

                //Get resources spent
                if (!target.TryGetCostToLevel(startLevel, timesLeveled, out long cost))
                {
                    ChatPacket.SendServerMessage(session, $"Failed to get cost for {timesLeveled} levels of {target}.", ChatMessageType.Broadcast);
                    continue;
                }

                //Refund the resource and update the player
                if (target.TryGetAttribute(player, out var attribute))
                {
                    player.AvailableExperience += cost;
                    ChatPacket.SendServerMessage(session, $"Refunding {timesLeveled} levels of {target} for {cost:N0} xp.", ChatMessageType.Broadcast);
                    session.Network.EnqueueSend(new GameMessagePrivateUpdateAttribute(player, attribute));
                }
                else
                {
                    player.AvailableLuminance += cost;
                    ChatPacket.SendServerMessage(session, $"Refunding {timesLeveled} levels of {target} for {cost:N0} lum.", ChatMessageType.Broadcast);
                }
                //Finally, set the level to what it should be
                target.SetLevel(player, startLevel);
            }
            //Send player their updated ratings
            session.Network.EnqueueSend(new GameMessagePrivateUpdatePropertyInt(player, PropertyInt.LumAugAllSkills, player.LumAugAllSkills));
            session.Network.EnqueueSend(new GameMessagePrivateUpdatePropertyInt(player, PropertyInt.LumAugDamageReductionRating, player.LumAugDamageReductionRating));
            session.Network.EnqueueSend(new GameMessagePrivateUpdatePropertyInt(player, PropertyInt.LumAugDamageRating, player.LumAugDamageRating));
            //Send updated resources
            session.Network.EnqueueSend(new GameMessagePrivateUpdatePropertyInt64(player, PropertyInt64.AvailableLuminance, player.AvailableLuminance ?? 0));
            session.Network.EnqueueSend(new GameMessagePrivateUpdatePropertyInt64(player, PropertyInt64.AvailableExperience, player.AvailableExperience ?? 0));

            //Todo: enable for server / player name
            //if (parameters?.Length > 0)
            //{
            //    List<CommandParameterHelpers.ACECommandParameter> aceParams = new List<CommandParameterHelpers.ACECommandParameter>()
            //    {
            //        new CommandParameterHelpers.ACECommandParameter() {
            //            Type = CommandParameterHelpers.ACECommandParameterType.OnlinePlayerNameOrIid,
            //            Required = false,
            //            DefaultValue = session.Player
            //        },
            //        new CommandParameterHelpers.ACECommandParameter()
            //        {
            //            Type = CommandParameterHelpers.ACECommandParameterType.PositiveLong,
            //            Required = true,
            //            ErrorMessage = "You must specify the amount of xp."
            //        }
            //    };
            //    if (CommandParameterHelpers.ResolveACEParameters(session, parameters, aceParams))
            //    {
            //        try
            //        {
            //            var amount = aceParams[1].AsLong;
            //            aceParams[0].AsPlayer.GrantXP(amount, XpType.Admin, ShareType.None);

            //            session.Network.EnqueueSend(new GameMessageSystemChat($"{amount:N0} experience granted.", ChatMessageType.Advancement));

            //            PlayerManager.BroadcastToAuditChannel(session.Player, $"{session.Player.Name} granted {amount:N0} experience to {aceParams[0].AsPlayer.Name}.");

            //            return;
            //        }
            //        catch
            //        {
            //            //overflow
            //        }
            //    }
            //}

            //ChatPacket.SendServerMessage(session, "Usage: /grantxp [name] 1234 (max 999999999999)", ChatMessageType.Broadcast);
        }
    }
}
