#region

using OpenMod.Unturned.Users;

#endregion

namespace LegallyDistinctMissile.Commands.Events
{
    public class ChangeGodModeEvent : ChangeModeEvent
    {
        public ChangeGodModeEvent(UnturnedUser unturnedUser, bool isActive) : base(unturnedUser, isActive)
        {
        }
    }
}