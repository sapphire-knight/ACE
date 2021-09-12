using ACE.Entity.Enum.Properties;
using ACE.Server.Entity;
using ACE.Server.WorldObjects;
using ACE.Server.WorldObjects.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACE.Server.DuskfallMods
{
    static class RaiseTargetHelpers
    {
        //TODO: Decide if this should update player of lum/exp and the raised property
        public static void SetLevel(this RaiseTarget target, Player player, int level)
        {
            //If it's an attribute being changed, make sure to update the starting value
            if (target.TryGetAttribute(player, out CreatureAttribute attribute))
            {
                //Find the change in current and desired level
                var levelChange = level - GetLevel(target, player);
                attribute.StartingValue += (uint)levelChange;   //Tested to work with negatives
            }            

            //Set the appropriate RaisedAttr or rating to desired level
            switch (target)
            {
                case RaiseTarget.Str:
                    player.RaisedStr = level;
                    break;
                case RaiseTarget.End:
                    player.RaisedEnd = level;
                    break;
                case RaiseTarget.Quick:
                    player.RaisedQuick = level;
                    break;
                case RaiseTarget.Coord:
                    player.RaisedCoord = level;
                    break;
                case RaiseTarget.Focus:
                    player.RaisedFocus = level;
                    break;
                case RaiseTarget.Self:
                    player.RaisedSelf = level;
                    break;
                case RaiseTarget.World:
                    player.LumAugAllSkills = level;
                    break;
                case RaiseTarget.Defense:
                    player.LumAugDamageReductionRating = level;
                    break;
                case RaiseTarget.Offense:
                    player.LumAugDamageRating = level;
                    break;
            }
            return;
        }
        public static int GetLevel(this RaiseTarget target, Player player)
        {
            switch (target)
            {
                //Attributes
                case RaiseTarget.Str: return player.RaisedStr;
                case RaiseTarget.End: return player.RaisedEnd;
                case RaiseTarget.Quick: return player.RaisedQuick;
                case RaiseTarget.Coord: return player.RaisedCoord;
                case RaiseTarget.Focus: return player.RaisedFocus;
                case RaiseTarget.Self: return player.RaisedSelf;
                //Ratings
                case RaiseTarget.World: return player.LumAugAllSkills;
                case RaiseTarget.Defense: return player.LumAugDamageReductionRating;
                case RaiseTarget.Offense: return player.LumAugDamageRating;
            }
            return -1;
        }
        public static int StartingLevel(this RaiseTarget target)
        {
            switch (target)
            {
                //Attributes
                case RaiseTarget t when t.IsAttribute(): return 1;
                //Ratings return the normal max.
                ////Comment out to allow leveling down to 0 which would let a player go through the normal process to net a little Lum
                case RaiseTarget.World: return 10;  //Max World 
                case RaiseTarget.Defense: return 5;
                case RaiseTarget.Offense: return 5;
                default: return 0;
            }
        }
        public static bool TryGetCostToLevel(this RaiseTarget target, int startLevel, int numLevels, out long cost)
        {
            cost = uint.MaxValue;
            //This may be too restrictive but it guarantees you are /raising some amount from a valid starting point
            if (startLevel < target.StartingLevel() || numLevels < 1)
                return false;

            try
            {
                switch (target)
                {
                    case RaiseTarget t when t.IsAttribute():
                        var avgLevel = (2 * startLevel + numLevels) / 2.0;  //Could use a decimal, but being off a very small amount should be fine
                        long avgCost = (long)(DuskfallSettings.RAISE_ATTR_MULT * avgLevel / (DuskfallSettings.RAISE_ATTR_MULT_DECAY - DuskfallSettings.RAISE_ATTR_LVL_DECAY * avgLevel));
                        cost = checked(avgCost * numLevels);
                        return true;
                    case RaiseTarget.Offense:
                    case RaiseTarget.Defense:
                        cost = checked(numLevels * DuskfallSettings.RAISE_RATING_MULT);
                        return true;
                    case RaiseTarget.World:
                        cost = checked(numLevels * DuskfallSettings.RAISE_WORLD_MULT);
                        return true;
                }
            }
            catch (OverflowException ex) { }
            return false;
        }
        private static bool IsAttribute(this RaiseTarget target) { return target < RaiseTarget.World; }
        public static bool TryGetAttribute(this RaiseTarget target, Player player, out CreatureAttribute? attribute)
        {
            attribute = null;
            if (!target.IsAttribute())
                return false;

            //If the target is an attribute set it and succeed
            attribute = player.Attributes[(PropertyAttribute)target];  //TODO: Requires the RaiseTarget enum to line up with the PropertyAttribute-- probably should do this a better way
            return true;
        }
    }
}
