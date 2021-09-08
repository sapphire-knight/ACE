using ACE.Server.WorldObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACE.Server.DuskfallMods
{
    static class RaiseHelpers
    {
        public static bool IsAttribute(this RaiseTarget target) { return target < RaiseTarget.World; }
        public static void SetLevel(this RaiseTarget target, Player player, int level)
        {
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
                    break;
                case RaiseTarget.Defense:
                    break;
                case RaiseTarget.Offense:
                    break;
            }
            return;
        }
        public static int GetLevel(this RaiseTarget target, Player player)
        {
            switch (target)
            {
                case RaiseTarget.Str: return player.RaisedStr;
                case RaiseTarget.End: return player.RaisedEnd;
                case RaiseTarget.Quick: return player.RaisedQuick;
                case RaiseTarget.Coord: return player.RaisedCoord;
                case RaiseTarget.Focus: return player.RaisedFocus;
                case RaiseTarget.Self: return player.RaisedSelf;

                case RaiseTarget.World: return -1;
                case RaiseTarget.Defense: return -1;
                case RaiseTarget.Offense: return -1;
            }
            return -1;
        }
    }


    enum RaiseTarget
    {
        Str = 1, End = 2, Quick = 3, Coord = 4, Focus = 5, Self = 6, //Match ACE.Entity.Enum.Properties.PropertyAttribute to work with casting
        World, Offense, Defense,
        Enlighten
    }

}
