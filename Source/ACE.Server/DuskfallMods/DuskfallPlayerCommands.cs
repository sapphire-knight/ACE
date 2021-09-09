using System;
using ACE.Entity.Enum;
using ACE.Server.Managers;
using ACE.Server.Network;
using ACE.Server.Network.GameMessages.Messages;
using ACE.Server.WorldObjects;
using ACE.Entity.Enum.Properties;
using ACE.Server.DuskfallMods;

namespace ACE.Server.Command.Handlers
{
    public static class DuskfallPlayerCommands
    {
        [CommandHandler("raise", AccessLevel.Player, CommandHandlerFlag.RequiresWorld, "/raise str/end/coord/focus/self, /raise enlighten, /raise offense, and /raise defense..", "/raise <target> <amount>")]
        public static void HandleAttribute(Session session, params string[] parameters)
        {
            Player player = session.Player;
            RaiseTarget target;
            if (parameters.Length < 1 || !Enum.TryParse<RaiseTarget>(parameters[0], true, out target))
            {
                //If a bad target was selected quit and list the valid commands
                session.Network.EnqueueSend(new GameMessageSystemChat($"You must specify what you wish to raise: /raise <{String.Join("|", Enum.GetNames(typeof(RaiseTarget)))}> [amount]", ChatMessageType.Broadcast));
                return;
            }

            int amt = 1;
            if (parameters.Length > 1)
            {
                if (!int.TryParse(parameters[1], out amt))
                {
                    session.Network.EnqueueSend(new GameMessageSystemChat($"You must specify or omit the amount to raise: /raise <{String.Join("|", Enum.GetNames(typeof(RaiseTarget)))}> [amount]", ChatMessageType.Broadcast));
                    return;
                }
            }
            //Check for bad amounts to level
            if (amt < 1 || amt > DuskfallSettings.RAISE_MAX)
            {
                session.Network.EnqueueSend(new GameMessageSystemChat($"Provide an amount from 1-{DuskfallSettings.RAISE_MAX}: /raise <{String.Join("|", Enum.GetNames(typeof(RaiseTarget)))}> [amount]", ChatMessageType.Broadcast));
                return;
            }

            //Get the cost
            var currentLevel = target.GetLevel(player);
            long cost = long.MaxValue;
            if (!target.TryGetCostToLevel(currentLevel, amt, out cost))
            {
                session.Network.EnqueueSend(new GameMessageSystemChat($"Error: Unable to find cost required to raise.  Report to admin.", ChatMessageType.Broadcast));
                return;
            }

            //Acceptable /raise target/amount
            if (target.TryGetAttribute(player, out var attribute))
            {
                //Require max attr first
                if (!attribute.IsMaxRank)
                {
                    session.Network.EnqueueSend(new GameMessageSystemChat($"Your {target} is not max level yet. Please raise {target} until it is maxed out. ", ChatMessageType.Broadcast));
                    return;
                }

                //Halt if there isn't enough xp
                if (session.Player.AvailableExperience < cost)
                {
                    session.Network.EnqueueSend(new GameMessageSystemChat($"You do not have enough available experience to level your {target}{(amt == 1 ? "" : $" {amt} times")}.  {cost:N0} needed.", ChatMessageType.Broadcast));
                    return;
                }

                //Otherwise go ahead raising the attribute
                //attribute.StartingValue += (uint)amt; //Moved to SetLevel
                target.SetLevel(player, currentLevel + (int)amt);
                player.AvailableExperience -= cost;

                //Update the player
                //TODO: Decide if player updates should be done in RaiseTarget.SetLevel
                session.Network.EnqueueSend(new GameMessagePrivateUpdatePropertyInt64(player, PropertyInt64.AvailableExperience, player.AvailableExperience ?? 0));
                session.Network.EnqueueSend(new GameMessagePrivateUpdateAttribute(player, attribute));
                session.Network.EnqueueSend(new GameMessageSystemChat($"Your base {target} is now {attribute.Base}! Spent {cost:N0} xp.", ChatMessageType.Advancement));
                return;
            }

            if (cost > player.AvailableLuminance || !player.SpendLuminance(cost))
            {
                var lumMult = (target == RaiseTarget.World ? DuskfallSettings.RAISE_WORLD_MULT : DuskfallSettings.RAISE_RATING_MULT);
                ChatPacket.SendServerMessage(session, $"Not enough Luminance, you require {lumMult} Luminance per point of {target}.", ChatMessageType.Broadcast);
                return;
            }

            //If successful in spending luminance level the target
            switch (target)
            {
                case RaiseTarget.World:
                //case RaiseTarget.Enlighten: //Leave in if you want an alias
                    ChatPacket.SendServerMessage(session, $"You have raised your World Aug to {player.LumAugAllSkills}! Skills increased by {amt} for {cost:N0} Luminance.", ChatMessageType.Broadcast);
                    session.Network.EnqueueSend(new GameMessagePrivateUpdatePropertyInt(player, PropertyInt.LumAugAllSkills, player.LumAugAllSkills));
                    return;
                case RaiseTarget.Defense:
                    player.LumAugDamageReductionRating += (int)amt;
                    ChatPacket.SendServerMessage(session, $"Your Damage Reduction Rating has increased by {amt} to {player.LumAugDamageReductionRating} for {cost:N0} Luminance.", ChatMessageType.Broadcast);
                    session.Network.EnqueueSend(new GameMessagePrivateUpdatePropertyInt(player, PropertyInt.LumAugDamageReductionRating, player.LumAugDamageReductionRating));
                    return;
                case RaiseTarget.Offense:
                    player.LumAugDamageRating += (int)amt;
                    ChatPacket.SendServerMessage(session, $"Your Damage Rating has increased by {amt} to {player.LumAugDamageRating} for {cost:N0} Luminance.", ChatMessageType.Broadcast);
                    session.Network.EnqueueSend(new GameMessagePrivateUpdatePropertyInt(player, PropertyInt.LumAugDamageRating, player.LumAugDamageRating));
                    return;
            }
        }

        [CommandHandler("vassalxp", AccessLevel.Player, CommandHandlerFlag.RequiresWorld, "Shows full experience from vassals.")]
        public static void HandleShowVassalXp(Session session, params string[] parameters)
        {
            //Needless null checking
            if (session?.Player == null)
                return;

            CommandHandlerHelper.WriteOutputInfo(session, "Experience from vassals:", ChatMessageType.Broadcast);
            foreach (var vassalNode in AllegianceManager.GetAllegianceNode(session?.Player).Vassals.Values)
            {
                var vassal = vassalNode.Player;
                CommandHandlerHelper.WriteOutputInfo(session, $"{vassal.Name,-30}{vassal.AllegianceXPGenerated,-20:N0}", ChatMessageType.Broadcast);
            }
        }
    }
}
