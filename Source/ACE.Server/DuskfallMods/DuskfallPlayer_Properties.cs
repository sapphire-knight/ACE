using ACE.Common;
using ACE.Entity;
using ACE.Entity.Enum;
using ACE.Entity.Enum.Properties;
using ACE.Server.Network.GameMessages;
using ACE.Server.Network.GameMessages.Messages;

namespace ACE.Server.WorldObjects
{
    partial class Player
    {
        public long LastRaisedRefundTimestamp
        {
            get => GetProperty(PropertyInt64.LastRefundTimestamp) ?? 1;
            set { if (value <= 0) RemoveProperty(PropertyInt64.LastRefundTimestamp); else SetProperty(PropertyInt64.LastRefundTimestamp, value); }
        }

        public int RaisedStr
        {
            get => GetProperty(PropertyInt.RaisedStr) ?? 1;
            set { if (value == 0) RemoveProperty(PropertyInt.RaisedStr); else SetProperty(PropertyInt.RaisedStr, value); }
        }

        public int RaisedEnd
        {
            get => GetProperty(PropertyInt.RaisedEnd) ?? 1;
            set { if (value == 0) RemoveProperty(PropertyInt.RaisedEnd); else SetProperty(PropertyInt.RaisedEnd, value); }
        }

        public int RaisedCoord
        {
            get => GetProperty(PropertyInt.RaisedCoord) ?? 1;
            set { if (value == 0) RemoveProperty(PropertyInt.RaisedCoord); else SetProperty(PropertyInt.RaisedCoord, value); }
        }

        public int RaisedQuick
        {
            get => GetProperty(PropertyInt.RaisedQuick) ?? 1;
            set { if (value == 0) RemoveProperty(PropertyInt.RaisedQuick); else SetProperty(PropertyInt.RaisedQuick, value); }
        }

        public int RaisedFocus
        {
            get => GetProperty(PropertyInt.RaisedFocus) ?? 1;
            set { if (value == 0) RemoveProperty(PropertyInt.RaisedFocus); else SetProperty(PropertyInt.RaisedFocus, value); }
        }

        public int RaisedSelf
        {
            get => GetProperty(PropertyInt.RaisedSelf) ?? 1;
            set { if (value == 0) RemoveProperty(PropertyInt.RaisedSelf); else SetProperty(PropertyInt.RaisedSelf, value); }
        }

        public long? TotalXpBeyond
        {
            get => GetProperty(PropertyInt64.TotalXpBeyond);
            set { if (!value.HasValue) RemoveProperty(PropertyInt64.TotalXpBeyond); else SetProperty(PropertyInt64.TotalXpBeyond, value.Value); }
        }
        public int? LastLevel
        {
            get => GetProperty(PropertyInt.LastLevel);
            set { if (!value.HasValue) RemoveProperty(PropertyInt.LastLevel); else SetProperty(PropertyInt.LastLevel, value.Value); }
        }
    }
}
