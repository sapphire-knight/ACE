using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACE.Server.DuskfallMods
{
    public static class DuskfallSettings
    {
        //RAISE_ATTR_MULT * L / (RAISE_ATTR_MULT_DECAY - RAISE_ATTR_LVL_DECAY * L), where L = current amount raised
        public const double RAISE_ATTR_MULT = 3292201940D;
        public const double RAISE_ATTR_MULT_DECAY = 7.995D;
        public const double RAISE_ATTR_LVL_DECAY = 0.001D;
        public const long RAISE_RATING_MULT = 15000000;
        public const long RAISE_WORLD_MULT = 5000000;
        public const uint RAISE_MAX = 100;
    }
}
