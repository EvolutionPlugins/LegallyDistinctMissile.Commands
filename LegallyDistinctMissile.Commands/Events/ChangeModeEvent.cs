#region

using OpenMod.Core.Eventing;
using OpenMod.Unturned.Users;

#endregion

namespace LegallyDistinctMissile.Commands.Events
{
    public abstract class ChangeModeEvent : Event
    {
        protected ChangeModeEvent(UnturnedUser unturnedUser, bool isActive)
        {
            UnturnedUser = unturnedUser;
            IsActive = isActive;
        }

        public UnturnedUser UnturnedUser { get; }
        public bool IsActive { get; }
    }
}