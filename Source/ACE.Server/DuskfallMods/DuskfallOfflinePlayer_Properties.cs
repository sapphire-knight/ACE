using System;
using System.Threading;

using ACE.Database;
using ACE.Database.Models.Auth;
using ACE.Entity;
using ACE.Entity.Enum.Properties;
using ACE.Entity.Models;
using ACE.Server.WorldObjects;

namespace ACE.Server.Entity
{
    //Currently just supports needed Properties for offline players, copied in from DuskfallPlayer_Properties.
    //TODO: Figure out a better way to implement this
    public partial class OfflinePlayer : IPlayer
    {
        /// <summary>
        /// Check upon login if a refund should be issued
        /// </summary>
        public bool RaiseRefundOnLogin
        {
            get => GetProperty(PropertyBool.RaiseRefundOnLogin) ?? false;
            set => SetProperty(PropertyBool.RaiseRefundOnLogin, value); 
        }

    }
}
