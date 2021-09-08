using System;
using System.Collections.Generic;

using log4net;

using ACE.Common;
using ACE.Database;
using ACE.Entity;
using ACE.Entity.Enum;
using ACE.Server;
using ACE.Server.Command;
using ACE.Server.Entity;
using ACE.Server.Entity.Actions;
using ACE.Server.Managers;
using ACE.Server.Network;
using ACE.Server.Network.GameEvent.Events;
using ACE.Server.Network.GameMessages.Messages;
using ACE.Server.WorldObjects;
using ACE.Entity.Enum.Properties;

namespace ACE.Server.Command.Handlers
{
    public static class DuskfallCommands
    {
        [CommandHandler("raise", AccessLevel.Player, CommandHandlerFlag.RequiresWorld, "/raise str/end/coord/focus/self, /raise enlighten, /raise offense, and /raise defense..","/raise <target> <amount>")]
        public static void HandleAttribute(Session session, params string[] parameters)
        {
            Player player = session.Player;
            var amt = 1;
            if (parameters.Length > 1)
                int.TryParse(parameters[1], out amt);

            if (parameters[0].Equals("str", StringComparison.OrdinalIgnoreCase))
            {
                var str = session.Player.Strength;

                if (!str.IsMaxRank)
                {
                    session.Network.EnqueueSend(new GameMessageSystemChat($"Your Strength is not maxed. ", ChatMessageType.Broadcast));
                    return;
                }

                double strcost = 0;
                var multiamount = 0UL;
                var strCostEthereal = session.Player.RaisedStr;

                if (amt > 1)
                {
                    for (var i = 0; i < amt; i++)
                    {
                        strcost = (ulong)Math.Round(10UL * (ulong)strCostEthereal / (7.995D - (0.001D * strCostEthereal)) * 329220194D);
                        multiamount += (ulong)strcost;
                        strCostEthereal++;
                    }
                }
                else
                    strcost = (ulong)Math.Round(10UL * (ulong)session.Player.RaisedStr / (7.995D - (0.001D * session.Player.RaisedStr)) * 329220194D);

                if (session.Player.AvailableExperience < (long?)multiamount && amt > 1)
                {
                    session.Network.EnqueueSend(new GameMessageSystemChat($"You do not have enough experience to increase you Strength {amt} times. ", ChatMessageType.Broadcast));
                    return;
                }
                else if (session.Player.AvailableExperience < strcost)
                {
                    session.Network.EnqueueSend(new GameMessageSystemChat($"You do not have enough experience to increase your Strength. ", ChatMessageType.Broadcast));
                    return;
                }

                if (amt > 1)
                {
                    session.Player.RaisedStr += amt;
                    str.StartingValue += (uint)amt;
                    session.Player.AvailableExperience = session.Player.AvailableExperience - (long?)multiamount;
                }
                else
                {
                    session.Player.RaisedStr++;
                    str.StartingValue += 1;
                    session.Player.AvailableExperience = session.Player.AvailableExperience - (long?)strcost;
                }


                session.Network.EnqueueSend(new GameMessagePrivateUpdatePropertyInt64(session.Player, PropertyInt64.AvailableExperience, session.Player.AvailableExperience ?? 0));
                session.Network.EnqueueSend(new GameMessagePrivateUpdateAttribute(session.Player, session.Player.Strength));

                if (amt > 1)
                    session.Network.EnqueueSend(new GameMessageSystemChat($"Your base Strength is now {str.Base}! Spent {multiamount:N0} xp.", ChatMessageType.Advancement));
                else
                    session.Network.EnqueueSend(new GameMessageSystemChat($"Your base Strength is now {str.Base}! Spent {strcost:N0} xp.", ChatMessageType.Advancement));
            }

            if (parameters[0].Equals("end", StringComparison.OrdinalIgnoreCase))
            {
                var end = session.Player.Endurance;

                if (!end.IsMaxRank)
                {
                    session.Network.EnqueueSend(new GameMessageSystemChat($"Your Endurance is not maxed. ", ChatMessageType.Broadcast));
                    return;
                }

                double endcost = 0;
                var multiamount = 0UL;
                var endCostEthereal = session.Player.RaisedEnd;

                if (amt > 1)
                {
                    for (var i = 0; i < amt; i++)
                    {
                        endcost = (ulong)Math.Round(10UL * (ulong)endCostEthereal / (7.995D - (0.001D * endCostEthereal)) * 329220194D);
                        multiamount += (ulong)endcost;
                        endCostEthereal++;
                    }
                }
                else
                    endcost = (ulong)Math.Round(10UL * (ulong)session.Player.RaisedEnd / (7.995D - (0.001D * session.Player.RaisedEnd)) * 329220194D);

                if (session.Player.AvailableExperience < (long?)multiamount && amt > 1)
                {
                    session.Network.EnqueueSend(new GameMessageSystemChat($"You do not have enough experience to increase your Endurance {amt} times. ", ChatMessageType.Broadcast));
                    return;
                }
                else if (session.Player.AvailableExperience < endcost)
                {
                    session.Network.EnqueueSend(new GameMessageSystemChat($"You do not have enough experience to increase your Endurance. ", ChatMessageType.Broadcast));
                    return;
                }

                if (amt > 1)
                {
                    session.Player.RaisedEnd += amt;
                    end.StartingValue += (uint)amt;
                    session.Player.AvailableExperience = session.Player.AvailableExperience - (long?)multiamount;
                }
                else
                {
                    session.Player.RaisedEnd++;
                    end.StartingValue += 1;
                    session.Player.AvailableExperience = session.Player.AvailableExperience - (long?)endcost;
                }


                session.Network.EnqueueSend(new GameMessagePrivateUpdatePropertyInt64(session.Player, PropertyInt64.AvailableExperience, session.Player.AvailableExperience ?? 0));
                session.Network.EnqueueSend(new GameMessagePrivateUpdateAttribute(session.Player, session.Player.Endurance));

                if (amt > 1)
                    session.Network.EnqueueSend(new GameMessageSystemChat($"Your base Endurance is now {end.Base}! Spent {multiamount:N0} xp.", ChatMessageType.Advancement));
                else
                    session.Network.EnqueueSend(new GameMessageSystemChat($"Your base Endurance is now {end.Base}! Spent {endcost:N0} xp.", ChatMessageType.Advancement));
            }

            if (parameters[0].Equals("coord", StringComparison.OrdinalIgnoreCase))
            {
                var coord = session.Player.Coordination;

                if (!coord.IsMaxRank)
                {
                    session.Network.EnqueueSend(new GameMessageSystemChat($"Your Coordination is not maxed.", ChatMessageType.Broadcast));
                    return;
                }

                double coordcost = 0;
                var multiamount = 0UL;
                var coordCostEthereal = session.Player.RaisedCoord;

                if (amt > 1)
                {
                    for (var i = 0; i < amt; i++)
                    {
                        coordcost = (ulong)Math.Round(10UL * (ulong)coordCostEthereal / (7.995D - (0.001D * coordCostEthereal)) * 329220194D);
                        multiamount += (ulong)coordcost;
                        coordCostEthereal++;
                    }
                }
                else
                    coordcost = (ulong)Math.Round(10UL * (ulong)session.Player.RaisedCoord / (7.995D - (0.001D * session.Player.RaisedCoord)) * 329220194D);

                if (session.Player.AvailableExperience < (long?)multiamount && amt > 1)
                {
                    session.Network.EnqueueSend(new GameMessageSystemChat($"You do not have enough experience to increase your Coordination {amt} times. ", ChatMessageType.Broadcast));
                    return;
                }
                else if (session.Player.AvailableExperience < coordcost)
                {
                    session.Network.EnqueueSend(new GameMessageSystemChat($"You do not have enough experience to increase your Coordination. ", ChatMessageType.Broadcast));
                    return;
                }

                if (amt > 1)
                {
                    session.Player.RaisedCoord += amt;
                    coord.StartingValue += (uint)amt;
                    session.Player.AvailableExperience = session.Player.AvailableExperience - (long?)multiamount;
                }
                else
                {
                    session.Player.RaisedCoord++;
                    coord.StartingValue += 1;
                    session.Player.AvailableExperience = session.Player.AvailableExperience - (long?)coordcost;
                }


                session.Network.EnqueueSend(new GameMessagePrivateUpdatePropertyInt64(session.Player, PropertyInt64.AvailableExperience, session.Player.AvailableExperience ?? 0));
                session.Network.EnqueueSend(new GameMessagePrivateUpdateAttribute(session.Player, session.Player.Coordination));

                if (amt > 1)
                    session.Network.EnqueueSend(new GameMessageSystemChat($"Your base Coordination is now {coord.Base}! Spent {multiamount:N0} xp.", ChatMessageType.Advancement));
                else
                    session.Network.EnqueueSend(new GameMessageSystemChat($"Your base Coordination is now {coord.Base}! Spent {coordcost:N0} xp.", ChatMessageType.Advancement));
            }

            if (parameters[0].Equals("quick", StringComparison.OrdinalIgnoreCase))
            {
                var quick = session.Player.Quickness;

                if (!quick.IsMaxRank)
                {
                    session.Network.EnqueueSend(new GameMessageSystemChat($"Your Quickness is not maxed.", ChatMessageType.Broadcast));
                    return;
                }

                double quickcost = 0;
                var multiamount = 0UL;
                var quickCostEthereal = session.Player.RaisedQuick;

                if (amt > 1)
                {
                    for (var i = 0; i < amt; i++)
                    {
                        quickcost = (ulong)Math.Round(10UL * (ulong)quickCostEthereal / (7.995D - (0.001D * quickCostEthereal)) * 329220194D);
                        multiamount += (ulong)quickcost;
                        quickCostEthereal++;
                    }
                }
                else
                    quickcost = (ulong)Math.Round(10UL * (ulong)session.Player.RaisedQuick / (7.995D - (0.001D * session.Player.RaisedQuick)) * 329220194D);

                if (session.Player.AvailableExperience < (long?)multiamount && amt > 1)
                {
                    session.Network.EnqueueSend(new GameMessageSystemChat($"You do not have enough experience to increase your Quickness {amt} times. ", ChatMessageType.Broadcast));
                    return;
                }
                else if (session.Player.AvailableExperience < quickcost)
                {
                    session.Network.EnqueueSend(new GameMessageSystemChat($"You do not have enough experience to increase your Quickness. ", ChatMessageType.Broadcast));
                    return;
                }

                if (amt > 1)
                {
                    session.Player.RaisedQuick += amt;
                    quick.StartingValue += (uint)amt;
                    session.Player.AvailableExperience = session.Player.AvailableExperience - (long?)multiamount;
                }
                else
                {
                    session.Player.RaisedQuick++;
                    quick.StartingValue += 1;
                    session.Player.AvailableExperience = session.Player.AvailableExperience - (long?)quickcost;
                }


                session.Network.EnqueueSend(new GameMessagePrivateUpdatePropertyInt64(session.Player, PropertyInt64.AvailableExperience, session.Player.AvailableExperience ?? 0));
                session.Network.EnqueueSend(new GameMessagePrivateUpdateAttribute(session.Player, session.Player.Quickness));

                if (amt > 1)
                    session.Network.EnqueueSend(new GameMessageSystemChat($"Your base Quickness is now {quick.Base}! Spent {multiamount:N0} xp.", ChatMessageType.Advancement));
                else
                    session.Network.EnqueueSend(new GameMessageSystemChat($"Your base Quickness is now {quick.Base}! Spent {quickcost:N0} xp.", ChatMessageType.Advancement));
            }

            if (parameters[0].Equals("focus", StringComparison.OrdinalIgnoreCase))
            {
                var focus = session.Player.Focus;

                if (!focus.IsMaxRank)
                {
                    session.Network.EnqueueSend(new GameMessageSystemChat($"Your Focus is not maxed.", ChatMessageType.Broadcast));
                    return;
                }

                double focuscost = 0;
                var multiamount = 0UL;
                var focusCostEthereal = session.Player.RaisedFocus;

                if (amt > 1)
                {
                    for (var i = 0; i < amt; i++)
                    {
                        focuscost = (ulong)Math.Round(10UL * (ulong)focusCostEthereal / (7.995D - (0.001D * focusCostEthereal)) * 329220194D);
                        multiamount += (ulong)focuscost;
                        focusCostEthereal++;
                    }
                }
                else
                    focuscost = (ulong)Math.Round(10UL * (ulong)session.Player.RaisedFocus / (7.995D - (0.001D * session.Player.RaisedFocus)) * 329220194D);

                if (session.Player.AvailableExperience < (long?)multiamount && amt > 1)
                {
                    session.Network.EnqueueSend(new GameMessageSystemChat($"You do not have enough experience to increase your Focus {amt} times. ", ChatMessageType.Broadcast));
                    return;
                }
                else if (session.Player.AvailableExperience < focuscost)
                {
                    session.Network.EnqueueSend(new GameMessageSystemChat($"You do not have enough experience to increase your Focus. ", ChatMessageType.Broadcast));
                    return;
                }

                if (amt > 1)
                {
                    session.Player.RaisedFocus += amt;
                    focus.StartingValue += (uint)amt;
                    session.Player.AvailableExperience = session.Player.AvailableExperience - (long?)multiamount;
                }
                else
                {
                    session.Player.RaisedFocus++;
                    focus.StartingValue += 1;
                    session.Player.AvailableExperience = session.Player.AvailableExperience - (long?)focuscost;
                }


                session.Network.EnqueueSend(new GameMessagePrivateUpdatePropertyInt64(session.Player, PropertyInt64.AvailableExperience, session.Player.AvailableExperience ?? 0));
                session.Network.EnqueueSend(new GameMessagePrivateUpdateAttribute(session.Player, session.Player.Focus));

                if (amt > 1)
                    session.Network.EnqueueSend(new GameMessageSystemChat($"Your base Focus is now {focus.Base}! Spent {multiamount:N0} xp.", ChatMessageType.Advancement));
                else
                    session.Network.EnqueueSend(new GameMessageSystemChat($"Your base Focus is now {focus.Base}! Spent {focuscost:N0} xp.", ChatMessageType.Advancement));
            }


            if (parameters[0].Equals("self", StringComparison.OrdinalIgnoreCase))
            {
                var self = session.Player.Self;

                if (!self.IsMaxRank)
                {
                    session.Network.EnqueueSend(new GameMessageSystemChat($"Your Self is not maxed. ", ChatMessageType.Broadcast));
                    return;
                }

                double selfcost = 0;
                var multiamount = 0UL;
                var selfCostEthereal = session.Player.RaisedSelf;

                if (amt > 1)
                {
                    for (var i = 0; i < amt; i++)
                    {
                        selfcost = (ulong)Math.Round(10UL * (ulong)selfCostEthereal / (7.995D - (0.001D * selfCostEthereal)) * 329220194D);
                        multiamount += (ulong)selfcost;
                        selfCostEthereal++;
                    }
                }
                else
                    selfcost = (ulong)Math.Round(10UL * (ulong)session.Player.RaisedSelf / (7.995D - (0.001D * session.Player.RaisedSelf)) * 329220194D);

                if (session.Player.AvailableExperience < (long?)multiamount && amt > 1)
                {
                    session.Network.EnqueueSend(new GameMessageSystemChat($"You do not have enough experience to increase your Self {amt} times. ", ChatMessageType.Broadcast));
                    return;
                }
                else if (session.Player.AvailableExperience < selfcost)
                {
                    session.Network.EnqueueSend(new GameMessageSystemChat($"You do not have enough experience to increase your Self. ", ChatMessageType.Broadcast));
                    return;
                }

                if (amt > 1)
                {
                    session.Player.RaisedSelf += amt;
                    self.StartingValue += (uint)amt;
                    session.Player.AvailableExperience = session.Player.AvailableExperience - (long?)multiamount;
                }
                else
                {
                    session.Player.RaisedSelf++;
                    self.StartingValue += 1;
                    session.Player.AvailableExperience = session.Player.AvailableExperience - (long?)selfcost;
                }


                session.Network.EnqueueSend(new GameMessagePrivateUpdatePropertyInt64(session.Player, PropertyInt64.AvailableExperience, session.Player.AvailableExperience ?? 0));
                session.Network.EnqueueSend(new GameMessagePrivateUpdateAttribute(session.Player, session.Player.Self));

                if (amt > 1)
                    session.Network.EnqueueSend(new GameMessageSystemChat($"Your base Self is now {self.Base}! Spent {multiamount:N0} xp.", ChatMessageType.Advancement));
                else
                    session.Network.EnqueueSend(new GameMessageSystemChat($"Your base Self is now {self.Base}! Spent {selfcost:N0} xp.", ChatMessageType.Advancement));
            }

            if (parameters[0].Equals("str", StringComparison.OrdinalIgnoreCase) && parameters[0].Equals("end", StringComparison.OrdinalIgnoreCase) && parameters[0].Equals("coord", StringComparison.OrdinalIgnoreCase) &&
                parameters[0].Equals("quick", StringComparison.OrdinalIgnoreCase) && parameters[0].Equals("focus", StringComparison.OrdinalIgnoreCase) && parameters[0].Equals("self", StringComparison.OrdinalIgnoreCase))
            {

                session.Network.EnqueueSend(new GameMessageSystemChat($"State which attribute to raise. ex. /raise Str or /raise End.", ChatMessageType.Broadcast));

            }



            int result = 1;
            int _luminanceRating = 0;

            if (parameters[0].ToLowerInvariant().Equals("offense"))
            {
                for (int j = 0; j < result; j++)
                {
                    _luminanceRating = player.LumAugDamageRating;


                    if (15000000L > player.AvailableLuminance || !player.SpendLuminance(15000000L))
                    {
                        player.Session.Network.EnqueueSend(new GameMessagePrivateUpdatePropertyInt(player, PropertyInt.LumAugDamageRating, _luminanceRating));
                        ChatPacket.SendServerMessage(session, string.Format("Your Damage Rating has increased by {0}.", j), ChatMessageType.Broadcast);
                        ChatPacket.SendServerMessage(session, "Not enough Luminance, you require 15,000,000 Luminance per point.", ChatMessageType.Broadcast);
                        return;
                    }
                    player.LumAugDamageRating++;
                }
                player.Session.Network.EnqueueSend(new GameMessagePrivateUpdatePropertyInt(player, PropertyInt.LumAugDamageRating, player.LumAugDamageRating));
                ChatPacket.SendServerMessage(session, string.Format("Your Damage Rating has increased by {0}.", result), ChatMessageType.Broadcast);
                return;
            }
            if (parameters[0].ToLowerInvariant().Equals("defense"))
            {
                for (int san = 0; san < result; san++)
                {
                    _luminanceRating = player.LumAugDamageReductionRating;

                    if (15000000L > player.AvailableLuminance || !player.SpendLuminance(15000000L))
                    {
                        player.Session.Network.EnqueueSend(new GameMessagePrivateUpdatePropertyInt(player, PropertyInt.LumAugDamageReductionRating, _luminanceRating));
                        ChatPacket.SendServerMessage(session, string.Format("Your Damage Reduction Rating has increased by {0}.", san), ChatMessageType.Broadcast);
                        ChatPacket.SendServerMessage(session, "Not enough Luminance, you require 15,000,000 Luminance per point.", ChatMessageType.Broadcast);
                        return;
                    }
                    player.LumAugDamageReductionRating++;
                }
                player.Session.Network.EnqueueSend(new GameMessagePrivateUpdatePropertyInt(player, PropertyInt.LumAugDamageReductionRating, player.LumAugDamageReductionRating));
                ChatPacket.SendServerMessage(session, string.Format("Your Damage Reduction Rating has increased by {0}.", result), ChatMessageType.Broadcast);
                return;
            }
            if (parameters[0].ToLowerInvariant().Equals("world"))
            {
                for (int san = 0; san < result; san++)
                {
                    _luminanceRating = player.LumAugAllSkills;

                    if (5000000L > player.AvailableLuminance || !player.SpendLuminance(5000000L))
                    {
                        player.Session.Network.EnqueueSend(new GameMessagePrivateUpdatePropertyInt(player, PropertyInt.LumAugAllSkills, _luminanceRating));
                        ChatPacket.SendServerMessage(session, string.Format("You have raised your World Aug! Skills have increased by {0}.", san), ChatMessageType.Broadcast);
                        ChatPacket.SendServerMessage(session, "Not enough Luminance, you require 5,000,000 Luminance per point.", ChatMessageType.Broadcast);
                        return;
                    }
                    player.LumAugAllSkills++;
                }
                player.Session.Network.EnqueueSend(new GameMessagePrivateUpdatePropertyInt(player, PropertyInt.LumAugAllSkills, player.LumAugAllSkills));
                ChatPacket.SendServerMessage(session, string.Format("You have raised your World Aug! Skills have increased by {0}.", result), ChatMessageType.Broadcast);
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
