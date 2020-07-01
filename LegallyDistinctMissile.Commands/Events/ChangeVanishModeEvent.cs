#region

using OpenMod.Unturned.Users;

#endregion

namespace LegallyDistinctMissile.Commands.Events
{
    public class ChangeVanishModeEvent : ChangeModeEvent
    {
        public ChangeVanishModeEvent(UnturnedUser unturnedUser, bool isActive) : base(unturnedUser, isActive)
        {
        }
    }
}