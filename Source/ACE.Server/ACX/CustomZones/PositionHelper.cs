using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACE.Server.Entity;
using ACE.Entity;
using ACE.Server.Managers;

namespace ACE.Server.ACX.CustomZones
{
    public static class PositionHelper
    {
        //TODO: Decide whether to fetch the Landblock for a position to use it's IsDungeon code vs. implementing that for a Position
        //The Landblock may already have cached the result
        public static bool IsDungeon(this Position target) => LandblockManager.GetLandblock(target.LandblockId, false).IsDungeon;
        public static bool HasDungeon(this Position target) => LandblockManager.GetLandblock(target.LandblockId, false).HasDungeon;
    }
}
